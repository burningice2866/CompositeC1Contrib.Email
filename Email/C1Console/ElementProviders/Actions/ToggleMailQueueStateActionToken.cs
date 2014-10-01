using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders.Actions
{
    [ActionExecutor(typeof(ToggleMailQueueStateActionExecutor))]
    public class ToggleMailQueueStateActionToken : ActionToken
    {
        private Guid _queueId;
        public Guid QueueId
        {
            get { return _queueId; }
        }

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return new[] { PermissionType.Edit, PermissionType.Administrate }; }
        }

        public override bool IgnoreEntityTokenLocking
        {
            get { return false; }
        }

        public ToggleMailQueueStateActionToken(Guid queueId)
        {
            _queueId = queueId;
        }

        public override string Serialize()
        {
            return _queueId.ToString();
        }

        public static ActionToken Deserialize(string serialiedWorkflowActionToken)
        {
            return new ToggleMailQueueStateActionToken(Guid.Parse(serialiedWorkflowActionToken));
        }
    }

    public class ToggleMailQueueStateActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var queueId = ((ToggleMailQueueStateActionToken)actionToken).QueueId;

            using (var data = new DataConnection(PublicationScope.Unpublished))
            {
                var queue = data.Get<IMailQueue>().Single(q => q.Id == queueId);

                queue.Paused = !queue.Paused;

                data.Update(queue);

                if (!queue.Paused)
                {
                    MailWorker.ProcessQueuesNow();
                }
            }

            EntityTokenCacheFacade.ClearCache();

            var treeRefresher = new ParentTreeRefresher(flowControllerServicesContainer);
            treeRefresher.PostRefreshMesseges(entityToken);

            return null;
        }
    }
}
