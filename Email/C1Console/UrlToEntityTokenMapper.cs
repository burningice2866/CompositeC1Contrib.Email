using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.WebClient;
using Composite.Data;
using CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens;
using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.C1Console
{
    public class UrlToEntityTokenMapper : IUrlToEntityTokenMapper
    {
        public BrowserViewSettings TryGetBrowserViewSettings(EntityToken entityToken, bool showPublishedView)
        {
            return new BrowserViewSettings
            {
                Url = TryGetUrl(entityToken),
                ToolingOn = false
            };
        }

        public EntityToken TryGetEntityToken(string url)
        {
            return null;
        }

        public string TryGetUrl(EntityToken entityToken)
        {
            var folderToken = entityToken as QueueFolderEntityToken;
            if (folderToken != null)
            {
                var url = $"InstalledPackages/CompositeC1Contrib.Email/log.aspx?view={folderToken.Type}&queue={folderToken.Source}";

                return UrlUtils.ResolveAdminUrl(url);
            }

            var dataToken = entityToken as DataEntityToken;
            if (dataToken?.InterfaceType == typeof(IMailTemplate))
            {
                var template = (IMailTemplate)dataToken.Data;

                var url = $"InstalledPackages/CompositeC1Contrib.Email/log.aspx?view=Sent&template={template.Key}";

                return UrlUtils.ResolveAdminUrl(url);
            }

            return null;
        }
    }
}
