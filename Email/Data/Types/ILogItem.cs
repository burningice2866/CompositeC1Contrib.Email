using System;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace CompositeC1Contrib.Email.Data.Types
{
    [AutoUpdateble]
    [KeyPropertyName("Id")]
    [LabelPropertyName("Id")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [DataScope(DataScopeIdentifier.PublicName)]
    public interface ILogItem : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("bde8386d-1159-47a3-9126-44908506ef94")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("56ed96e1-05b5-46e5-9dbf-691dacb6ca6c")]
        Guid MailMessageId { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.DateTime)]
        [ImmutableFieldId("63cbce22-fa93-4e60-b056-da3d6392e4b9")]
        DateTime Timestamp { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 32)]
        [ImmutableFieldId("c35f6aa4-55fc-44bf-bd5d-1f51d6ddd1e4")]
        string EventName { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.LargeString, IsNullable = true)]
        [ImmutableFieldId("b41f5811-3df6-4457-b5ad-4c6ee1028b3e")]
        string EventData { get; set; }
    }
}
