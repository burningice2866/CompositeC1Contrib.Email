using System;
using System.Linq;
using System.Threading;

using Composite.C1Console.Events;
using Composite.Core;
using Composite.Core.Threading;
using Composite.Data;

using CompositeC1Contrib.Email.Data;
using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    public class MailWorker
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
        private static readonly MailWorker Instance = new MailWorker();

        private volatile bool _running;
        private volatile bool _processQueuesNow;
        private readonly Thread _thread;

        private MailWorker()
        {
            var threadStart = new ThreadStart(Run);

            _thread = new Thread(threadStart);
        }

        public static void Initialize()
        {
            GlobalEventSystemFacade.SubscribeToPrepareForShutDownEvent(PrepareForShutDown);

            Instance._running = true;

            Instance._thread.Start();
        }

        public static void ProcessQueuesNow()
        {
            Instance._processQueuesNow = true;
        }

        private static void PrepareForShutDown(PrepareForShutDownEventArgs e)
        {
            Instance._running = false;
        }

        private void Run()
        {
            var ticker = 60;

            try
            {
                using (ThreadDataManager.EnsureInitialize())
                {
                    while (_running)
                    {
                        try
                        {
                            if (!_processQueuesNow && ticker != 60)
                            {
                                continue;
                            }

                            _processQueuesNow = false;

                            SendPendingMessages();
                        }
                        catch (Exception ex)
                        {
                            Log.LogWarning("Unhandled error when sending pending messages, sleep for 1 minute", ex);
                        }
                        finally
                        {
                            if (ticker == 60)
                            {
                                ticker = 0;
                            }

                            ticker = ticker + 1;

                            Thread.Sleep(OneSecond);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogCritical("Unhandled error in ThreadDataManager, worker is stopping", ex);
            }
        }

        private void SendPendingMessages()
        {
            using (var data = new DataConnection())
            {
                var queues = MailQueuesFacade.GetMailQueues().Where(q => !q.Paused);
                foreach (var queue in queues)
                {
                    if (!_running)
                    {
                        return;
                    }

                    var queueLocal = queue;

                    var messages = data.Get<IQueuedMailMessage>().Where(m => m.QueueId == queueLocal.Id).ToList();
                    if (!messages.Any())
                    {
                        continue;
                    }

                    foreach (var message in messages)
                    {
                        if (!_running)
                        {
                            return;
                        }

                        try
                        {
                            MailsFacade.SendQueuedMessage(message, data, queue);
                        }
                        catch (Exception exc)
                        {
                            data.LogErrorEvent(exc, message);
                        }
                    }
                }
            }
        }
    }
}
