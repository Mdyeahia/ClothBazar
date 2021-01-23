using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClothBazar.Services;
using ClothBazar.Entities.Code;
using ClothBazar.Web.ViewModels;

namespace ClothBazar.Web.Controllers
{
    public class ShopController : Controller
    {
        
        //ProductsService productsService = new ProductsService();
        // GET: Shop
        public ActionResult Index(string searchTerm,int? maximumPrice,int?minimumPrice,int? categoryID,int? sortBy)
        {
            ShopViewModel model = new ShopViewModel();
            model.FeaturedCategories = CategoriesService.Instance.GetFeaturedCategories();
            model.MaximumPrice = ProductsService.Instance.GetMaximumPrice();
            model.Products =ProductsService.Instance.SearchProducts(searchTerm, maximumPrice, minimumPrice, categoryID, sortBy);
            model.SortBy = sortBy;


            return View(model);
        }

        public ActionResult Checkout()
        {
            CheckoutViewModel model = new CheckoutViewModel();

             var CartProductsCookie = Request.Cookies["CartProducts"];

            if (CartProductsCookie!=null)
            {
                //var productIDs = CartProductsCookie.Value;
                //var ids = productIDs.Split('-');
                //List<int> pIDs = ids.Select(x => int.Parse(x)).ToList();

                model.CartProductIDs = CartProductsCookie.Value.Split('-').Select(x => int.Parse(x)).ToList();

              model.CartProducts = ProductsService.Instance.GetProducts(model.CartProductIDs);
            }

            return View(model);
        }
    }
}