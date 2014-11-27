using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using Composite.Data;

namespace CompositeC1Contrib.Email.Data.Types
{
    [Title("Basic log item")]
    [ImmutableTypeId("2a9be918-a7c5-4863-a52e-13881158aa2a")]
    public interface IEventBasic : IEvent
    {
        [NotNullValidator]
        [StoreFieldType(PhysicalStoreFieldType.String, 32)]
        [ImmutableFieldId("c35f6aa4-55fc-44bf-bd5d-1f51d6ddd1e4")]
        string Event { get; set; }
    }
}