﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Shared.Services;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Pricing;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Controllers
{
    public class MobileProductController : ContentController<MobileProduct>
    {
        private readonly IPromotionService _promotionService;
        private readonly IContentLoader _contentLoader;
        private readonly IPriceService _priceService;
        private readonly ICurrentMarket _currentMarket;
        private readonly ICurrencyService _currencyservice;
        private readonly IRelationRepository _relationRepository;
        private readonly AppContextFacade _appContext;
        private readonly UrlResolver _urlResolver;
        private readonly FilterPublished _filterPublished;
        private readonly CultureInfo _preferredCulture;
        private readonly bool _isInEditMode;

        public MobileProductController(
            IPromotionService promotionService,
            IContentLoader contentLoader,
            IPriceService priceService,
            ICurrentMarket currentMarket,
            CurrencyService currencyservice,
            IRelationRepository relationRepository,
            AppContextFacade appContext,
            UrlResolver urlResolver,
            FilterPublished filterPublished,
            Func<CultureInfo> preferredCulture,
            Func<bool> isInEditMode)
        {
            _promotionService = promotionService;
            _contentLoader = contentLoader;
            _priceService = priceService;
            _currentMarket = currentMarket;
            _currencyservice = currencyservice;
            _relationRepository = relationRepository;
            _appContext = appContext;
            _urlResolver = urlResolver;
            _preferredCulture = preferredCulture();
            _isInEditMode = isInEditMode();
            _filterPublished = filterPublished;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index(MobileProduct currentContent, string variationCode = "", bool quickview = false)
        {
            var variantData = currentContent.GetVariants(_relationRepository);
            var testData = _contentLoader.GetItems(variantData, _preferredCulture).ToList();
            var testData2 = testData.Cast<MobileVariant>().Where(v => v.IsAvailableInCurrentMarket(_currentMarket) && !_filterPublished.ShouldFilter(v));
            var testResult = testData2.ToList();
            
            var variations = GetVariations(currentContent);
            if (_isInEditMode && !variations.Any())
            {
                var productWithoutVariation = new MobileProductViewModel
                {
                    Product = currentContent,
                    Images = currentContent.GetAssets<IContentImage>(_contentLoader, _urlResolver)
                };
                return Request.IsAjaxRequest() ? PartialView("ProductWithoutVariation", productWithoutVariation) : (ActionResult)View("ProductWithoutVariation", productWithoutVariation);
            }
            MobileVariant variation;
            if (!TryGetMobileVariant(variations, variationCode, out variation))
            {
                return HttpNotFound();
            }

            var market = _currentMarket.GetCurrentMarket();
            var currency = _currencyservice.GetCurrentCurrency();

            var defaultPrice = GetDefaultPrice(variation, market, currency);
            var discountPrice = GetDiscountPrice(defaultPrice, market, currency);

            var viewModel = new MobileProductViewModel
            {
                Product = currentContent,
                Variation = variation,
                OriginalPrice = defaultPrice != null ? defaultPrice.UnitPrice : new Money(0, currency),
                Price = discountPrice,
                Colors = variations
                    .Where(x => x.ScreenSize != null && x.ScreenSize == variation.ScreenSize)
                    .Select(x => new SelectListItem
                    {
                        Selected = false,
                        Text = x.Color,
                        Value = x.Color
                    })
                    .ToList(),
                Sizes = variations
                    .Where(x => x.Color != null && x.Color == variation.Color)
                    .Select(x => new SelectListItem
                    {
                        Selected = false,
                        Text = x.ScreenSize,
                        Value = x.ScreenSize
                    })
                    .ToList(),
                Color = variation.Color,
                Size = variation.ScreenSize,
                Images = variation.GetAssets<IContentImage>(_contentLoader, _urlResolver),
                IsAvailable = defaultPrice != null
            };

            if (quickview)
            {
                return PartialView("Quickview", viewModel);
            }

            return Request.IsAjaxRequest() ? PartialView(viewModel) : (ActionResult)View(viewModel);
        }

        [HttpPost]
        public ActionResult SelectVariant(MobileProduct currentContent, string color, string size, bool quickview = false)
        {
            var variations = GetVariations(currentContent);

            MobileVariant variation;
            if (!TryGetMobileVariantByColorAndSize(variations, color, size, out variation))
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index", new { variationCode = variation.Code, quickview });
        }

        private IEnumerable<MobileVariant> GetVariations(MobileProduct currentContent)
        {
            return _contentLoader
                .GetItems(currentContent.GetVariants(_relationRepository), _preferredCulture)
                .Cast<MobileVariant>()
                .Where(v => v.IsAvailableInCurrentMarket(_currentMarket) && !_filterPublished.ShouldFilter(v));
        }

        private static bool TryGetMobileVariant(IEnumerable<MobileVariant> variations, string variationCode, out MobileVariant variation)
        {
            variation = !string.IsNullOrEmpty(variationCode) ?
                variations.FirstOrDefault(x => x.Code == variationCode) :
                variations.FirstOrDefault();

            return variation != null;
        }

        private static bool TryGetMobileVariantByColorAndSize(IEnumerable<MobileVariant> variations, string color, string size, out MobileVariant variation)
        {
            variation = variations.FirstOrDefault(x =>
                x.Color.Equals(color, StringComparison.OrdinalIgnoreCase) &&
                x.ScreenSize.Equals(size, StringComparison.OrdinalIgnoreCase));

            return variation != null;
        }

        private IPriceValue GetDefaultPrice(MobileVariant variation, IMarket market, Currency currency)
        {
            return _priceService.GetDefaultPrice(
                market.MarketId,
                DateTime.Now,
                new CatalogKey(_appContext.ApplicationId, variation.Code),
                currency);
        }

        private Money? GetDiscountPrice(IPriceValue defaultPrice, IMarket market, Currency currency)
        {
            if (defaultPrice == null)
            {
                return null;
            }

            return _promotionService.GetDiscountPrice(defaultPrice.CatalogKey, market.MarketId, currency).UnitPrice;
        }
    }
}