using System;

using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.ProcessControlled;

namespace CompositeC1Contrib.Email.Data.Types
{
    [AutoUpdateble]
    [KeyPropertyName("Id")]
    [LabelPropertyName("TemplateKey")]
    [Title("Mail template content")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("1cc71d9d-a405-4b7e-9d5b-57cbcd5e95ec")]
    [DataScope(DataScopeIdentifier.PublicName)]
    public interface IMailTemplateContent : ILocalizedControlled, IMailTemplateAddresses
    {
        [ImmutableFieldId("c6d08645-54c4-4cda-b0cd-f054be9a5fda")]
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        Guid Id { get; set; }

        [ImmutableFieldId("ddcca13c-f862-4124-be7c-423dac25a4d7")]
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [ForeignKey(typeof(IMailTemplate), "Key", AllowCascadeDeletes = true)]
        string TemplateKey { get; set; }

        [NotNullValidator]
        [ImmutableFieldId("c7a4ef88-3c73-4cca-89af-67455bf8e7d9")]
        [StoreFieldType(PhysicalStoreFieldType.String, 256)]
        string Subject { get; set; }

        [NotNullValidator]
        [ImmutableFieldId("c52dda12-1ae2-4f32-bfc2-bd971c3cfb9b")]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        string Body { get; set; }
    }
}
