using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClothBazar.Entities;
using ClothBazar.Services;
using ClothBazar.Entities.Code;
using ClothBazar.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ClothBazar.Web.Controllers
{
    public class ShopController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //ProductsService productsService = new ProductsService();
        // GET: Shop
        public ActionResult Index(string searchTerm,int? maximumPrice,int?minimumPrice,int? categoryID,int? sortBy,int? pageNo)
        {
            var pageSize = ConfigurationsService.Instance.ShopPageSize();

            ShopViewModel model = new ShopViewModel();

            model.SearchTerm = searchTerm;
            model.FeaturedCategories = CategoriesService.Instance.GetFeaturedCategories();
            model.MaximumPrice = ProductsService.Instance.GetMaximumPrice();
            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;
            model.CategoryID = categoryID;
            model.SortBy = sortBy;
            int totalCount =ProductsService.Instance.SearchProductsCount(searchTerm, maximumPrice, minimumPrice, categoryID, sortBy);
            model.Products = ProductsService.Instance.SearchProducts(searchTerm, maximumPrice, minimumPrice, categoryID, sortBy, pageNo.Value, pageSize);
            model.Pager = new Pager(totalCount, pageNo, pageSize);

            return View(model);
        }
        public ActionResult FilterProducts(string searchTerm, int? maximumPrice, int? minimumPrice, int? categoryID, int? sortBy,int? pageNo)
        {
            var pageSize = ConfigurationsService.Instance.ShopPageSize();
            FilterProductsViewModel model = new FilterProductsViewModel();

            model.SearchTerm = searchTerm;
            model.CategoryID = categoryID;
            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;
            model.SortBy = sortBy;
            int totalCount = ProductsService.Instance.SearchProductsCount(searchTerm, maximumPrice, minimumPrice, categoryID, sortBy);
            model.Products = ProductsService.Instance.SearchProducts(searchTerm, maximumPrice, minimumPrice, categoryID, sortBy,pageNo.Value,pageSize);

            model.Pager = new Pager(totalCount, pageNo, pageSize);

            return PartialView(model);
        }

        [Authorize]
        public ActionResult Checkout()
        {
            CheckoutViewModel model = new CheckoutViewModel();

             var CartProductsCookie = Request.Cookies["CartProducts"];

            if (CartProductsCookie!=null&& !string.IsNullOrEmpty(CartProductsCookie.Value))
            {
                //var productIDs = CartProductsCookie.Value;
                //var ids = productIDs.Split('-');
                //List<int> pIDs = ids.Select(x => int.Parse(x)).ToList();

              model.CartProductIDs = CartProductsCookie.Value.Split('-').Select(x => int.Parse(x)).ToList();

              model.CartProducts = ProductsService.Instance.GetProducts(model.CartProductIDs);

              model.User = UserManager.FindById(User.Identity.GetUserId());
            }

            return View(model);
        }

        public JsonResult PlaceOrder(string productIDs)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (!string.IsNullOrEmpty(productIDs))
            {
                var productQuantities = productIDs.Split('-').Select(x => int.Parse(x)).ToList();

                var broughtProducts = ProductsService.Instance.GetProducts(productQuantities.Distinct().ToList());

                Order newOrder = new Order();
                newOrder.UserID = User.Identity.GetUserId();
                newOrder.OrderedAt=DateTime.Now;
                newOrder.Status = "Pending";
                newOrder.TotalAmount =
                    broughtProducts.Sum(p => p.Price * productQuantities.Count(productID => productID == p.ID));

                newOrder.OrderItems = new List<OrderItem>();
                newOrder.OrderItems.AddRange(broughtProducts.Select(p=>new OrderItem()
                {
                    ProductID = p.ID,
                    Quantity =productQuantities.Count(productID=>productID==p.ID)
                }));
                var rowsEffected = ShopService.Instance.SaveOrder(newOrder);
                result.Data = new {Success = true, Rows = rowsEffected};

            }
            else
            {
                result.Data = new {Success = false};
            }

            return result;
        }
    }
}