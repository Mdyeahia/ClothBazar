﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClothBazar.Entities;
using ClothBazar.Services;
using ClothBazar.Web.ViewModels;

namespace ClothBazar.Web.Controllers
{
    public class ProductController : Controller
    {

        //ProductsService productsService=new ProductsService();
       // CategoriesService categoryService = new CategoriesService();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProductTable(string search, int? pageNo)
        {
            ProductSearchViewModel model = new ProductSearchViewModel();

            model.SearchTerm = search;
            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            var pageSize = ConfigurationsService.Instance.PageSize();

            var totalRecords = ProductsService.Instance.GetAllProductCount(search);
            model.Products = ProductsService.Instance.GetProducts(search, pageNo.Value,pageSize);

            //int pageSize = int.Parse(ConfigurationsService.Instance.GetConfig("ListingPageSize").Value);

            if (model.Products!=null)
            {
                model.Pager = new Pager(totalRecords, pageNo, pageSize);
                return PartialView(model);
            }

            return HttpNotFound();
        }
        [HttpGet]
        public ActionResult Create()
        {
            NewProductViewModel model=new NewProductViewModel();
            model.AvailableCategories = CategoriesService.Instance.GetAllCategories();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Create(NewProductViewModel model)
        {
            //CategoriesService categoriesService=new CategoriesService();

            var newProduct=new Product();
            newProduct.Name = model.Name;
            newProduct.Description = model.Description;
            newProduct.Price = model.Price;
            //newProduct.CategoryID = model.CategoryID;
            newProduct.ImageURL = model.ImageURL;
            newProduct.Category = CategoriesService.Instance.GetCategory(model.CategoryID);

            ProductsService.Instance.SaveProduct(newProduct);
            return RedirectToAction("ProductTable");
        }
        [HttpGet]
        public ActionResult Edit(int ID)
        {
            EditProductViewModel model = new EditProductViewModel();
            var product = ProductsService.Instance.GetProduct(ID);

            model.ID = product.ID;
            model.Name = product.Name;
            model.Description = product.Description;
            model.Price = product.Price;
            model.ImageURL = product.ImageURL;
            model.CategoryID = product.Category != null ? product.Category.ID:0;

            model.AvailableCategories = CategoriesService.Instance.GetAllCategories();

            return PartialView(model);
        }
        [HttpPost]
        public ActionResult Edit(EditProductViewModel model)
        {
            //var existingProduct = productsService.GetProduct(model.ID);
            var existingProduct = ProductsService.Instance.GetProduct(model.ID);
            existingProduct.Name = model.Name;
            existingProduct.Description = model.Description;
            existingProduct.Price = model.Price;
            existingProduct.ImageURL = model.ImageURL;
            existingProduct.Category = null;// CategoriesService.Instance.GetCategory(model.CategoryID);
            existingProduct.CategoryID = model.CategoryID;

            if (!string.IsNullOrEmpty(model.ImageURL))
            {
                existingProduct.ImageURL = model.ImageURL;
            }

            //productsService.UpdateProduct(existingProduct);

            ProductsService.Instance.UpdateProduct(existingProduct);
            return RedirectToAction("ProductTable");
        }
        [HttpPost]
        public ActionResult Delete(int ID)
        {
            ProductsService.Instance.DeleteProduct(ID);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Details(int ID)
        {
            ProductViewModel model = new ProductViewModel();
            model.Product = ProductsService.Instance.GetProduct(ID); 

            if (model.Product==null) { return HttpNotFound(); }

            return View(model);
        }
    
    }
}