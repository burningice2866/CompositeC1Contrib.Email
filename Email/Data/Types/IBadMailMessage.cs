using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using Composite.Data;

namespace CompositeC1Contrib.Email.Data.Types
{
    [Title("Bad mail message")]
    [LabelPropertyName("Subject")]
    [ImmutableTypeId("2fcbc3e9-555b-43fc-978b-daecc4da5b33")]
    public interface IBadMailMessage : IMailMessage
    {
        [NotNullValidator]
        [ImmutableFieldId("4603c5c0-140e-4b43-be83-b366149f4f20")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        string SerializedMessage { get; set; }
    }
}
