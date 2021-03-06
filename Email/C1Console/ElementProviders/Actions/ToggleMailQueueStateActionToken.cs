﻿using System;
using System.Collections.Generic;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;

using CompositeC1Contrib.Email.Data;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders.Actions
{
    [ActionExecutor(typeof(ToggleMailQueueStateActionExecutor))]
    public class ToggleMailQueueStateActionToken : ActionToken
    {
        private Guid _queueId;
        public Guid QueueId => _queueId;

        public override IEnumerable<PermissionType> PermissionTypes => new[] { PermissionType.Edit, PermissionType.Administrate };

        public override bool IgnoreEntityTokenLocking => false;

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
            var queue = MailQueuesFacade.GetMailQueue(queueId);

            queue.Paused = !queue.Paused;

            queue.Save();

            if (!queue.Paused)
            {
                MailBackgroundProcess.ProcessQueuesNow();
            }

            EntityTokenCacheFacade.ClearCache();

            var treeRefresher = new ParentTreeRefresher(flowControllerServicesContainer);
            treeRefresher.PostRefreshMessages(entityToken);

            return null;
        }
    }
}
