using Composite.Data;

namespace CompositeC1Contrib.Email.Data.Types
{
    public interface IMailTemplateAddresses : IData
    {
        [ImmutableFieldId("f6315390-0595-4b20-8b89-d8459daa4707")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        string From { get; set; }

        [ImmutableFieldId("4081688c-a8e7-4339-9bcc-9e0a0cf643e8")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        string To { get; set; }

        [ImmutableFieldId("f963d29e-e308-4f68-9dd3-c5499802cb04")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        string Cc { get; set; }

        [ImmutableFieldId("a44cd1c4-9f77-4859-80c3-553696fca462")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        string Bcc { get; set; }
    }
}
