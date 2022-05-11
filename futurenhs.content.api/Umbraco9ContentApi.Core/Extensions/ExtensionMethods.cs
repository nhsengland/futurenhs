using Umbraco.Cms.Core;
using Umbraco9ContentApi.Core.Models.Content;
using static Umbraco.Cms.Core.Constants;

namespace Umbraco9ContentApi.Core.Extensions
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the udi of the content model.
        /// </summary>
        /// <param name="contentModel">The content model.</param>
        /// <returns></returns>
        public static string GetUdi(this ContentModel contentModel)
        {
            return Udi.Create(UdiEntityType.Document, contentModel.Item.Id).ToString();
        }
    }
}
