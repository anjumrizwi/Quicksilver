using System.Linq;
using System.Web.Mvc;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Site.Features.Search.Models;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Controllers
{
    public class CategoryController : ContentController<MobileNode>
    {
        private readonly SearchViewModelFactory _viewModelFactory;

        public CategoryController(SearchViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index(MobileNode currentContent, FilterOptionFormModel formModel)
        {
            var model = _viewModelFactory.Create(currentContent, formModel);
            return Request.IsAjaxRequest() ? PartialView(model) : (ActionResult)View(model);
        }
    }
}