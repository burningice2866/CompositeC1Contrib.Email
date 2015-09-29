using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.GoogleAnalytics
{
    [AutoUpdateble]
    [DataScope(DataScopeIdentifier.PublicName)]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    [ImmutableTypeId("f851e780-c966-413d-b6b2-fb3a601a727c")]
    [Title("Google Analytics mail template settings")]
    [LabelPropertyName("MailTemplateKey")]
    [KeyPropertyName("MailTemplateKey")]
    public interface IGoogleAnalyticsMailTemplateSettings : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [ImmutableFieldId("8ae82184-b92a-427c-b1c0-269a7e37e8e7")]
        [ForeignKey(typeof(IMailTemplate), "Key", AllowCascadeDeletes = true)]
        string MailTemplateKey { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [ImmutableFieldId("bf2c9373-9ea5-436b-83b1-056444599292")]
        bool Enabled { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [ImmutableFieldId("85e3bc85-8a91-40a0-b906-1edc1053d34e")]
        string Source { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 128)]
        [ImmutableFieldId("f0e493e7-6aa2-4edf-adc6-66812a2a3e8f")]
        string Campaign { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [ImmutableFieldId("f24c62ba-9464-4375-9f77-7f6854975ac0")]
        bool TrackOpens { get; set; }
    }
}
