using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Helpers
{
    public static class Extensions
    {
        public static IDisposable BeginCollectionItem<TModel>(this HtmlHelper<TModel> html, string collectionName)
        {
            string itemIndex = Guid.NewGuid().ToString();
            string collectionItemName = String.Format("{0}[{1}]", collectionName, itemIndex);

            TagBuilder indexField = new TagBuilder("input");
            indexField.MergeAttributes(new Dictionary<string, string>() {
                { "name", String.Format("{0}.Index", collectionName) },
                { "value", itemIndex },
                { "type", "hidden" },
                { "autocomplete", "off" }
            });

            html.ViewContext.Writer.WriteLine(indexField.ToString());
            return new CollectionItemNamePrefixScope(html.ViewData.TemplateInfo, collectionItemName);
        }

        private class CollectionItemNamePrefixScope : IDisposable
        {
            private readonly TemplateInfo _templateInfo;
            private readonly string _previousPrefix;

            public CollectionItemNamePrefixScope(TemplateInfo templateInfo, string collectionItemName)
            {
                this._templateInfo = templateInfo;

                _previousPrefix = templateInfo.HtmlFieldPrefix;
                templateInfo.HtmlFieldPrefix = collectionItemName;
            }

            public void Dispose()
            {
                _templateInfo.HtmlFieldPrefix = _previousPrefix;
            }
        }
    }
}
