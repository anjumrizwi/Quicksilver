using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Commerce.SpecializedProperties;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Models
{
    [CatalogContentType(GUID = "3dcb1b5c-5988-4d1b-bc30-718953fc13b8",
        MetaClassName = "MobileProduct",
        DisplayName = "Mobile Product", Description = "Display Mobile Product")]
    public class MobileProduct : ProductContent
    {
        [Searchable]
        [CultureSpecific]
        [Tokenize]
        [IncludeInDefaultSearch]
        [BackingType(typeof(PropertyString))]
        [Display(Name = "Brand", Order = 1)]
        public virtual string Brand { get; set; }

        [Searchable]
        [CultureSpecific]
        [Tokenize]
        [IncludeInDefaultSearch]
        [Display(Name = "Description", Order = 2)]
        public virtual XhtmlString Description { get; set; }

        [Searchable]
        [CultureSpecific]
        [Tokenize]
        [IncludeInDefaultSearch]
        [Display(Name = "Long Description", Order = 3)]
        public virtual XhtmlString LongDescription { get; set; }

        [CultureSpecific]
        [Display(Name = "Product Teaser", Order = 4)]
        public virtual XhtmlString ProductTeaser { get; set; }

        [Searchable]
        [IncludeInDefaultSearch]
        [BackingType(typeof(PropertyDictionaryMultiple))]
        [Display(Name = "Available Sizes", Order = 5)]
        public virtual ItemCollection<string> AvailableSizes { get; set; }

        [Searchable]
        [IncludeInDefaultSearch]
        [BackingType(typeof(PropertyDictionaryMultiple))]
        [Display(Name = "Available Colors", Order = 6)]
        public virtual ItemCollection<string> AvailableColors { get; set; }
    }
}