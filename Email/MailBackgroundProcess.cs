using System;
using System.Linq;

using Composite.Core;
using Composite.Core.Threading;
using Composite.Data;

using CompositeC1Contrib.Email.Data;
using CompositeC1Contrib.Email.Data.Types;

using Hangfire.Server;

namespace CompositeC1Contrib.Email
{
    public class MailBackgroundProcess : IBackgroundProcess
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
        private const int NumberOfRecordsInSinglePass = 10;

        private static volatile bool _processQueuesNow;

        public static void ProcessQueuesNow()
        {
            _processQueuesNow = true;
        }

        public void Execute(BackgroundProcessContext context)
        {
            var ticker = 60;

            try
            {
                using (ThreadDataManager.EnsureInitialize())
                {
                    while (!context.IsShutdownRequested)
                    {
                        try
                        {
                            if (!_processQueuesNow && ticker != 60)
                            {
                                continue;
                            }

                            _processQueuesNow = false;

                            SendPendingMessages(context);
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

                            context.CancellationToken.WaitHandle.WaitOne(OneSecond);
                            context.CancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Log.LogCritical("Unhandled error in ThreadDataManager, worker is stopping", ex);
            }
        }

        private static void SendPendingMessages(BackgroundProcessContext context)
        {
            using (var data = new DataConnection())
            {
                var queues = MailQueuesFacade.GetMailQueues().Where(q => !q.Paused);
                foreach (var queue in queues)
                {
                    if (context.IsShutdownRequested)
                    {
                        return;
                    }

                    var queueLocal = queue;

                    var messages = data.Get<IQueuedMailMessage>()
                        .Where(m => m.QueueId == queueLocal.Id)
                        .Take(NumberOfRecordsInSinglePass).ToList();

                    foreach (var message in messages)
                    {
                        if (context.IsShutdownRequested)
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

                            var count = data.GetEventCount<IEventError>(message, TimeSpan.FromMinutes(15));
                            if (count >= 3)
                            {
                                MailsFacade.MoveQueuedMessageToBadFolder(message, data);
                            }
                        }
                    }
                }
            }
        }

    }
}
