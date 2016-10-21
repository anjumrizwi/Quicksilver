using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Models
{
    [CatalogContentType(GUID = "56844129-fcca-4a61-9141-05438087f624",
        MetaClassName = "MobileNode",
        DisplayName = "Mobile Node",
        Description = "Display mobile products.")]
    [AvailableContentTypes(Include = new[] { typeof(MobileProduct), typeof(MobileVariant), typeof(NodeContent) })]
    public class MobileNode : NodeContent
    {

    }
}