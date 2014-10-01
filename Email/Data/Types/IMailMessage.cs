using System;

using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace CompositeC1Contrib.Email.Data.Types
{
    [AutoUpdateble]
    [KeyPropertyName("Id")]
    [DataScope(DataScopeIdentifier.PublicName)]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    public interface IMailMessage : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("4160a882-75ae-4043-b174-3be23717f4fc")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("dbf76bbd-94cd-4af9-88b1-7f200891b350")]
        [ForeignKey(typeof(IMailQueue), "Id", AllowCascadeDeletes = true)]
        Guid QueueId { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 128, IsNullable = true)]
        [ImmutableFieldId("ab5ecc63-bd38-432c-9b86-e77b2aa380b4")]
        [ForeignKey(typeof(IMailTemplate), "Key", AllowCascadeDeletes = true, NullReferenceValue = null)]
        string MailTemplateKey { get; set; }

        [ImmutableFieldId("d5d239cd-1642-4c00-a861-5482e8ca0299")]
        [StoreFieldType(PhysicalStoreFieldType.DateTime)]
        DateTime TimeStamp { get; set; }

        [NotNullValidator]
        [ImmutableFieldId("ef5f2674-33e4-4e36-9c4d-cd5db16b8e04")]
        [StoreFieldType(PhysicalStoreFieldType.String, 512)]
        string Subject { get; set; }
    }
}
