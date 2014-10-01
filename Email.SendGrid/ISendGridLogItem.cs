using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.SendGrid
{
    [Title("SendGrid log item")]
    [ImmutableTypeId("e872157e-e0df-450c-9e0a-80c827ecc7bf")]
    public interface ISendGridLogItem : ILogItem
    {
        [StoreFieldType(PhysicalStoreFieldType.String, 256)]
        [ImmutableFieldId("c2009cef-1a9e-4bc7-bd3d-2d058410ed07")]
        string Email { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 128, IsNullable = true)]
        [ImmutableFieldId("93e216d5-2607-43a6-91fe-88c139f34d91")]
        string Template { get; set; }
    }
}
