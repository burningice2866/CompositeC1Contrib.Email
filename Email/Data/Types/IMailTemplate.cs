using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace CompositeC1Contrib.Email.Data.Types
{
    [AutoUpdateble]
    [KeyPropertyName("Key")]
    [LabelPropertyName("Key")]
    [Title("Mail template")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("b1861a88-4f03-4386-be5c-cd8f976498e7")]
    [DataScope(DataScopeIdentifier.PublicName)]
    public interface IMailTemplate : IData
    {
        [ImmutableFieldId("35ac3650-afe9-4acf-9e98-621b576312d4")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        string Key { get; set; }

        [ImmutableFieldId("07330426-8fbf-40b2-9894-6c3a45abb172")]
        [StoreFieldType(PhysicalStoreFieldType.String, 512, IsNullable = true)]
        string ModelType { get; set; }

        [ImmutableFieldId("f6315390-0595-4b20-8b89-d8459daa4707")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        string From { get; set; }

        [ImmutableFieldId("4081688c-a8e7-4339-9bcc-9e0a0cf643e8")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        string To { get; set; }

        [ImmutableFieldId(" f963d29e-e308-4f68-9dd3-c5499802cb04")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        string Cc { get; set; }

        [ImmutableFieldId("a44cd1c4-9f77-4859-80c3-553696fca462")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        string Bcc { get; set; }

        [ImmutableFieldId("c7a4ef88-3c73-4cca-89af-67455bf8e7d9")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256)]
        string Subject { get; set; }

        [ImmutableFieldId("c52dda12-1ae2-4f32-bfc2-bd971c3cfb9b")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        string Body { get; set; }

        [ImmutableFieldId("44991190-8622-468b-a01b-f37418d78da3")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        bool EncryptMessage { get; set; }

        [ImmutableFieldId("0880b90e-5774-4332-9034-e93400610511")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256)]
        string EncryptPassword { get; set; }
    }
}
