using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Shell;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Descriptors
{
    [UIDescriptorRegistration]
    public class MobileProductUIDescriptor : UIDescriptor<MobileProduct>
    {
        public MobileProductUIDescriptor()
            : base(ContentTypeCssClassNames.Container)
        {
            DefaultView = CmsViewNames.OnPageEditView;
        }
    }
}