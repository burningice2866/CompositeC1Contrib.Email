﻿using System.Collections.Generic;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders.Actions
{
    [ActionExecutor(typeof(DeleteMailTemplateActionExecutor))]
    public class DeleteMailTemplateActionToken : ActionToken
    {
        private static readonly IEnumerable<PermissionType> _permissionTypes = new[] { PermissionType.Administrate };

        public override IEnumerable<PermissionType> PermissionTypes => _permissionTypes;

        public override string Serialize()
        {
            return "DeleteMailTemplateActionToken";
        }

        public static ActionToken Deserialize(string serializedData)
        {
            return new DeleteMailQueueActionToken();
        }
    }

    public class DeleteMailTemplateActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var dataToken = (DataEntityToken)entityToken;
            var template = (IMailTemplate)dataToken.Data;

            using (var data = new DataConnection())
            {
                data.Delete(template);
            }

            new ParentTreeRefresher(flowControllerServicesContainer).PostRefreshMessages(entityToken);

            return null;
        }
    }
}
