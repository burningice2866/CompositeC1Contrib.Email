using Composite.C1Console.Elements;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders
{
    public interface IElementActionProvider
    {
        bool IsProviderFor(EntityToken entityToken);
        void AddActions(Element element);
    }
}
