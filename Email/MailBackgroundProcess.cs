using System;
using System.Linq;
using System.Threading;

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

        private static readonly TimeSpan DelayBetweenPasses = TimeSpan.FromSeconds(1);
        private const int NumberOfRecordsInSinglePass = 10;

        private static volatile bool _processQueuesNow;

        public static void ProcessQueuesNow()
        {
            _processQueuesNow = true;
        }

        public void Execute(BackgroundProcessContext context)
        {
            var ticker = 60;

            using (ThreadDataManager.EnsureInitialize())
            {
                while (!context.IsShutdownRequested)
                {
                    if (_processQueuesNow || ticker == 60)
                    {
                        try
                        {
                            SendPendingMessages(context.CancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Log.LogWarning("Unhandled error when sending pending messages, sleep for 1 minute", ex);
                        }

                        ticker = 0;
                    }

                    ticker++;

                    context.CancellationToken.WaitHandle.WaitOne(OneSecond);
                    context.CancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        private static void SendPendingMessages(CancellationToken cancellationToken)
        {
            using (var data = new DataConnection())
            {
                var queues = MailQueuesFacade.GetMailQueues().Where(q => !q.Paused).ToList();
                foreach (var queue in queues)
                {
                    int removedCount;

                    do
                    {
                        var messages = data.Get<IQueuedMailMessage>()
                            .Where(m => m.QueueId == queue.Id)
                            .Take(NumberOfRecordsInSinglePass).ToList();

                        removedCount = messages.Count;

                        foreach (var message in messages)
                        {
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

                            cancellationToken.WaitHandle.WaitOne(DelayBetweenPasses);
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    } while (removedCount != 0);

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
