using System;

using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

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
    public interface IMailTemplate : IMailTemplateAddresses
    {
        [NotNullValidator]
        [ImmutableFieldId("35ac3650-afe9-4acf-9e98-621b576312d4")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        string Key { get; set; }

        [ImmutableFieldId("07330426-8fbf-40b2-9894-6c3a45abb172")]
        [StoreFieldType(PhysicalStoreFieldType.String, 512, IsNullable = true)]
        string ModelType { get; set; }

        [ImmutableFieldId("c7a4ef88-3c73-4cca-89af-67455bf8e7d9")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        [Obsolete("Use IMailTemplateContent.Subject")]
        string Subject { get; set; }

        [ImmutableFieldId("c52dda12-1ae2-4f32-bfc2-bd971c3cfb9b")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString, IsNullable = true)]
        [Obsolete("Use IMailTemplateContent.Body")]
        string Body { get; set; }

        [ImmutableFieldId("44991190-8622-468b-a01b-f37418d78da3")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [DefaultFieldBoolValue(false)]
        bool EncryptMessage { get; set; }

        [ImmutableFieldId("0880b90e-5774-4332-9034-e93400610511")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256, IsNullable = true)]
        string EncryptPassword { get; set; }
    }
}
