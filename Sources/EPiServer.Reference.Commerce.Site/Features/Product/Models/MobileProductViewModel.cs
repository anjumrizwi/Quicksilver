using System.Collections.Generic;
using System.Web.Mvc;
using Mediachase.Commerce;
using Mediachase.Commerce.Pricing;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Models
{
    public class MobileProductViewModel
    {
        public MobileProduct Product { get; set; }
        public Money? Price { get; set; }
        public Money OriginalPrice { get; set; }
        public MobileVariant Variation { get; set; }
        public IList<SelectListItem> Colors { get; set; }
        public IList<SelectListItem> Sizes { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public IList<string> Images { get; set; }
        public bool IsAvailable { get; set; }
    }
}