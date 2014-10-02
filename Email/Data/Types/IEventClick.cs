using Composite.Data;

namespace CompositeC1Contrib.Email.Data.Types
{
    [Title("Click log item")]
    [ImmutableTypeId("608c7507-8142-4bc7-acf8-5f2edeacc162")]
    public interface IEventClick : IEvent
    {
        [StoreFieldType(PhysicalStoreFieldType.String, 254)]
        [ImmutableFieldId("c4090067-1f3d-4bf1-8ac2-b4ac084d469e")]
        string Email { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [ImmutableFieldId("c65ed7c1-5bb5-40ca-9004-55d20edc5a14")]
        string Url { get; set; }
    }
}