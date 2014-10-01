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
    [ImmutableTypeId("c189773a-fc71-40d8-9cc3-21ff94fc7f0d")]
    [Title("Mail queue")]
    [LabelPropertyName("Name")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    public interface IMailQueue : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("eef26b7d-2c10-4bf2-9120-1b6b67b6033c")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net/ns/function/1.0\" />")]
        Guid Id { get; set; }

        [NotNullValidator]
        [StoreFieldType(PhysicalStoreFieldType.String, 255)]
        [ImmutableFieldId("4b25cdc8-4830-4c6a-bcbe-2b0a694a6371")]
        string Name { get; set; }

        [ImmutableFieldId("aa7a5c40-772e-4c0b-93d1-15bd5d1a78ef")]
        [StoreFieldType(PhysicalStoreFieldType.String, 64)]
        [DefaultFieldStringValue("Network")]
        string DeliveryMethod { get; set; }

        [ImmutableFieldId("c0645d03-aa61-46eb-9c41-3cb736aee1c8")]
        [StoreFieldType(PhysicalStoreFieldType.String, 255, IsNullable = true)]
        string From { get; set; }

        [ImmutableFieldId("c5c0f9da-6dd4-4efd-883d-d028be03fd47")]
        [StoreFieldType(PhysicalStoreFieldType.String, 512, IsNullable = true)]
        string PickupDirectoryLocation { get; set; }

        [ImmutableFieldId("7971d16d-5619-495b-aa64-2bd93ca3f68e")]
        [StoreFieldType(PhysicalStoreFieldType.String, 512, IsNullable = true)]
        string Host { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Integer)]
        [ImmutableFieldId("627a6c79-3126-4aa0-9b45-e59bad7f3d4a")]
        [DefaultFieldIntValue(25)]
        int Port { get; set; }

        [ImmutableFieldId("2ab9f411-4c64-46b6-831c-0674407e725a")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [DefaultFieldBoolValue(false)]
        bool DefaultCredentials { get; set; }

        [ImmutableFieldId("137e911b-a897-4866-becf-39d7a3d9f8e3")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [DefaultFieldBoolValue(false)]
        bool EnableSsl { get; set; }

        [ImmutableFieldId("2a361bc5-e5b5-4222-8730-c852d38aa8c5")]
        [StoreFieldType(PhysicalStoreFieldType.String, 512, IsNullable = true)]
        string UserName { get; set; }

        [ImmutableFieldId("442167f5-cf63-4cdf-bcb6-8d00c64cb4ac")]
        [StoreFieldType(PhysicalStoreFieldType.String, 512, IsNullable = true)]
        string Password { get; set; }

        [ImmutableFieldId("8deb7553-4c93-4845-8e66-ca7f43626cea")]
        [StoreFieldType(PhysicalStoreFieldType.String, 512, IsNullable = true)]
        string TargetName { get; set; }

        [ImmutableFieldId("724c1395-2e10-4a82-9bd1-52faf5641993")]
        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [DefaultFieldBoolValue(true)]
        bool Paused { get; set; }
    }
}
