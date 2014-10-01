using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using Composite.Data;

namespace CompositeC1Contrib.Email.Data.Types
{
    [Title("Queued mail message")]
    [LabelPropertyName("Subject")]
    [ImmutableTypeId("b37fa3e8-95c0-45fe-9957-c8af313234ef")]
    public interface IQueuedMailMessage : IMailMessage
    {
        [NotNullValidator]
        [ImmutableFieldId("6a7e3f0b-c93e-4eb5-82d4-d4612e5f7e4d")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        string SerializedMessage { get; set; }
    }
}
