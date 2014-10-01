using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Data;

using CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens;
using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders.Actions
{
    [ActionExecutor(typeof(DeleteMailQueueActionExecutor))]
    public class DeleteMailQueueActionToken : ActionToken
    {
        static private readonly IEnumerable<PermissionType> _permissionTypes = new[] { PermissionType.Administrate };

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return _permissionTypes; }
        }

        public override string Serialize()
        {
            return "DeleteMailQueueActionToken";
        }

        public static ActionToken Deserialize(string serializedData)
        {
            return new DeleteMailQueueActionToken();
        }
    }

    public class DeleteMailQueueActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var dataToken = (DataEntityToken)entityToken;
            var queue = (IMailQueue)dataToken.Data;

            using (var data = new DataConnection())
            {
                var messages = data.Get<IQueuedMailMessage>().Where(m => m.QueueId == queue.Id).AsEnumerable();
                data.Delete(messages);

                data.Delete(queue);
            }

            var treeRefresher = new SpecificTreeRefresher(flowControllerServicesContainer);
            treeRefresher.PostRefreshMesseges(new MailQueuesEntityToken());

            return null;
        }
    }
}
