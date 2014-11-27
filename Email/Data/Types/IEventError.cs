using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using Composite.Data;

namespace CompositeC1Contrib.Email.Data.Types
{
    [Title("Error log item")]
    [ImmutableTypeId("cdc5723f-2233-4075-8c50-ace5194e484f")]
    public interface IEventError : IEvent
    {
        [NotNullValidator]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [ImmutableFieldId("54f7540f-5475-4450-948d-6e29ae653303")]
        string Error { get; set; }
    }
}