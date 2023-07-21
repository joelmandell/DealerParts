using Umbraco.Cms.Core.Models.PublishedContent;

namespace DealerParts.Custom.Extensions
{
    public static class PublishedContentExtensions
    {
        public static bool HideWhenSignedIn(this IPublishedContent content)
        {
            return true;
        }
    }
}
