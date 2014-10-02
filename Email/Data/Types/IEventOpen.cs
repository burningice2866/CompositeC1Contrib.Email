using Composite.Data;

namespace CompositeC1Contrib.Email.Data.Types
{
    [Title("Open log item")]
    [ImmutableTypeId("cd56a23a-24f5-49de-bb33-b5165243a3e9")]
    public interface IEventOpen : IEvent
    {
        [StoreFieldType(PhysicalStoreFieldType.String, 254)]
        [ImmutableFieldId("cd91f81d-5ef9-48d7-b0a6-e217e3ef772f")]
        string Email { get; set; }
    }
}