﻿namespace SDL.ECommerce.UnitTests.Dxa.Controllers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using FakeHttpContext;

    using NSubstitute;

    using Ploeh.AutoFixture;

    using Sdl.Web.Common.Configuration;
    using Sdl.Web.Common.Interfaces;
    using Sdl.Web.Common.Models;

    using SDL.ECommerce.Api;
    using SDL.ECommerce.DXA.Controllers;
    using SDL.ECommerce.DXA.Servants;

    using Xunit;

    public class ProductPageController_Test : Test<ProductPageController>
    {
        private readonly Localization _localization;

        private readonly Url _url;

        public ProductPageController_Test()
        {
            _localization = Fixture.Create<Localization>();

            _url = Fixture.Create<Url>();
        }

        public class WhenCallingProductPageWithValidUrlWithoutQueryString : MultipleAssertTest<ProductPageController_Test>
        {
            private readonly PageModel _pageModel;

            private readonly IDictionary _httpContextItems;

            private readonly ViewResult _result;

            private readonly IProduct _product;

            public WhenCallingProductPageWithValidUrlWithoutQueryString()
            {
                _pageModel = Fixture.Create<PageModel>();

                Fixture.Freeze<IPageModelServant>()
                       .ResolveTemplatePage(Arg.Any<IEnumerable<string>>(), Arg.Any<IContentProvider>())
                       .Returns(_pageModel);

                _product = Fixture.Create<IProduct>();

                _product.Name.Returns(Fixture.Create<string>());

                Fixture.Freeze<IECommerceClient>()
                       .DetailService.GetDetail(Parent._url.OneLevel.Replace("/", string.Empty))
                       .Returns(_product);
                
                using (new FakeHttpContext())
                {
                    HttpContext.Current.Items.Add("Localization", Parent._localization);

                    ProductPageController controller;

                    using (new DependencyTestProvider(Fixture))
                    {
                        controller = Parent.SUT.Value;
                    }

                    controller.Request.QueryString.Returns(new NameValueCollection());
                        
                    _result = controller.ProductPage(Parent._url.OneLevel) as ViewResult;

                    _httpContextItems = HttpContext.Current.Items;
                }
            }

            [Fact]
            public void TheResolvedTemplatePageShouldBeReturned()
            {
                Assert.Equal(_pageModel, _result.Model);
            }

            [Fact]
            public void TemplateTitleSholdBeModelName()
            {
                Assert.Equal(_product.Name, ((PageModel)_result.Model).Title);
            }
            
            [Fact]
            public void TheProductShouldBeSetInTheContext()
            {
                Assert.Equal(_product, _httpContextItems["ECOM-Product"]);
            }

            [Fact]
            public void TheUrlPrrefixShouldBeSetInTheContext()
            {
                Assert.Equal($"{Parent._localization.Path}/c", _httpContextItems["ECOM-UrlPrefix"]);
            }

            [Fact]
            public void ControllerRouteValueShouldBePage()
            {
                Assert.Equal("Page", Parent.SUT.Value.RouteData.Values["Controller"]);
            }
        }

        public class WhenCallingProductPageWithOneLevelUrl : MultipleAssertTest<ProductPageController_Test>
        {
            public WhenCallingProductPageWithOneLevelUrl()
            {
                Fixture.Freeze<IPageModelServant>()
                       .ResolveTemplatePage(Arg.Any<IEnumerable<string>>(), Arg.Any<IContentProvider>())
                       .Returns(Fixture.Create<PageModel>());

                using (new FakeHttpContext())
                {
                    HttpContext.Current.Items.Add("Localization", Parent._localization);

                    ProductPageController controller;

                    using (new DependencyTestProvider(Fixture))
                    {
                        controller = Parent.SUT.Value;
                    }

                    controller.Request.QueryString.Returns(new NameValueCollection());

                    controller.ProductPage(Parent._url.OneLevel);
                }
            }

            [Fact]
            public void ThenACallToPathServantWithEmptySeoIdShouldBeMade()
            {
                Fixture.GetStub<IPathServant>()
                       .Received(1)
                       .GetSearchPath(null, Arg.Any<IProduct>());
            }
        }

        public class WhenCallingProductPageWithTwoLevelUrl : MultipleAssertTest<ProductPageController_Test>
        {
            private readonly string[] _urlParts;

            public WhenCallingProductPageWithTwoLevelUrl()
            {
                _urlParts = Parent._url.Parts(Parent._url.TwoLevels);

                Fixture.Freeze<IPageModelServant>()
                       .ResolveTemplatePage(Arg.Any<IEnumerable<string>>(), Arg.Any<IContentProvider>())
                       .Returns(Fixture.Create<PageModel>());

                using (new FakeHttpContext())
                {
                    HttpContext.Current.Items.Add("Localization", Parent._localization);

                    ProductPageController controller;

                    using (new DependencyTestProvider(Fixture))
                    {
                        controller = Parent.SUT.Value;
                    }

                    controller.Request.QueryString.Returns(new NameValueCollection());

                    controller.ProductPage(Parent._url.TwoLevels);
                }
            }

            [Fact]
            public void ThenACallToPathServantWithFirstUrlPartShouldBeMade()
            {
                Fixture.GetStub<IPathServant>()
                       .Received(1)
                       .GetSearchPath(_urlParts[0], Arg.Any<IProduct>());
            }

            [Fact]
            public void ThenACallToGetProductDetailWithTheSecondUrlPartShouldBeMade()
            {
                Fixture.GetStub<IECommerceClient>()
                       .Received(1)
                       .DetailService.GetDetail(_urlParts[1]);
            }
        }

        public class WhenCallingProductPageWithEmptyUrl : MultipleAssertTest<ProductPageController_Test>
        {
            private readonly ActionResult _result;

            private readonly PageModel _errorModel;

            public WhenCallingProductPageWithEmptyUrl()
            {
                Fixture.Freeze<IPageModelServant>()
                       .ResolveTemplatePage(Arg.Any<IEnumerable<string>>(), Arg.Any<IContentProvider>())
                       .Returns(Fixture.Create<PageModel>());

                _errorModel = Fixture.Create<PageModel>();

                Fixture.Freeze<IPageModelServant>()
                       .GetNotFoundPageModel(Arg.Any<IContentProvider>())
                       .Returns(_errorModel);

                ProductPageController controller;

                using (new DependencyTestProvider(Fixture))
                {
                    controller = Parent.SUT.Value;
                }

                controller.Request.QueryString.Returns(new NameValueCollection());

                _result = controller.ProductPage(string.Empty);
            }

            [Fact]
            public void TheStatusCodeIs404()
            {
                Assert.Equal(404, Parent.SUT.Value.Response.StatusCode);
            }

            [Fact]
            public void ControllerRouteValueShouldBePage()
            {
                Assert.Equal("Page", Parent.SUT.Value.RouteData.Values["Controller"]);
            }

            [Fact]
            public void TheResultIsOfTypeViewResult()
            {
                Assert.IsType<ViewResult>(_result);
            }
            
            [Fact]
            public void TheErrorMethodIsReturned()
            {
                Assert.Equal(_errorModel, ((ViewResult)_result).Model);
            }
        }

        public class WhenCallingProductWithQueryStringParameters : MultipleAssertTest<ProductPageController_Test>
        {
            private readonly KeyValuePair<string, string>[] _queryStringParameters;

            private IDictionary<string, string>_attributesUsed;

            public WhenCallingProductWithQueryStringParameters()
            {
                Fixture.Freeze<IPageModelServant>()
                       .ResolveTemplatePage(Arg.Any<IEnumerable<string>>(), Arg.Any<IContentProvider>())
                       .Returns(Fixture.Create<PageModel>());

                Fixture.Freeze<IECommerceClient>()
                       .DetailService.GetDetail(Arg.Any<string>(), Arg.Do<IDictionary<string, string>>(dictionary => _attributesUsed = dictionary));

                _queryStringParameters = Fixture.CreateMany<KeyValuePair<string, string>>(2)
                                .ToArray();

                var collection = new NameValueCollection();

                foreach (var queryStringParameter in _queryStringParameters)
                {
                    collection.Add(queryStringParameter.Key, queryStringParameter.Value);
                }

                using (new FakeHttpContext())
                {
                    HttpContext.Current.Items.Add("Localization", Parent._localization);
                    
                    ProductPageController controller;

                    using (new DependencyTestProvider(Fixture))
                    {
                        controller = Parent.SUT.Value;
                    }

                    controller.Request.QueryString.Returns(collection);

                    controller.ProductPage(Parent._url.OneLevel);
                }
            }

            [Fact]
            public void TheQueryStringParametersIsPassedWhenGettingAProduct()
            {
                Assert.Equal(_queryStringParameters, _attributesUsed);
            }
        }
    }
}