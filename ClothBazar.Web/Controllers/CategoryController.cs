using ClothBazar.Entities;
using ClothBazar.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClothBazar.Web.ViewModels;

namespace ClothBazar.Web.Controllers
{
    public class CategoryController : Controller
    {
        //CategoriesService categoriesService= new CategoriesService();

        public ActionResult Index()
        {
            

            return View();
        }
        public ActionResult CategoryTable(string search)
        {
            CategorySearchViewModel model=new CategorySearchViewModel();

            model.Categories = CategoriesService.Instance.GetCategories();
            if (!string.IsNullOrEmpty(search))
            {
                
                model.Categories = model.Categories.Where(p => p.Name != null && p.Name.ToLower().Contains(search.ToLower()))
                    .ToList();
            }

            return PartialView("CategoryTable",model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            NewCategoryViewModels model=new NewCategoryViewModels();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Create(NewCategoryViewModels model)
        {


            //categoryService.SaveCategory(category);
            var newCategory=new Category();
            newCategory.Name = model.Name;
            newCategory.Description = model.Description;
            newCategory.ImageURL = model.ImageURL;
            newCategory.isFeatured = model.isFeatured;

            CategoriesService.Instance.SaveCategory(newCategory);

            return RedirectToAction("CategoryTable");
        }
        [HttpGet]
        public ActionResult Edit(int ID)
        {
            EditCategoryViewModel model=new EditCategoryViewModel();
            var category = CategoriesService.Instance.GetCategory(ID);

            model.ID = category.ID;
            model.Name = category.Name;
            model.Description = category.Description;
            model.ImageURL = category.ImageURL;
            model.isFeatured = category.isFeatured;

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Edit(EditCategoryViewModel model)
        {
            var existingCategory = CategoriesService.Instance.GetCategory(model.ID);
            existingCategory.Name = model.Name;
            existingCategory.Description = model.Description;
            existingCategory.ImageURL = model.ImageURL;
            existingCategory.isFeatured = model.isFeatured;

            CategoriesService.Instance.UpdateCategory(existingCategory);
            return RedirectToAction("CategoryTable");
        }
        [HttpGet]
        public ActionResult Delete(int ID)
        {
            var category = CategoriesService.Instance.GetCategory(ID);
            return View(category);
        }

        [HttpPost]
        public ActionResult Delete(Category category)
        {

            CategoriesService.Instance.DeleteCategory(category.ID);
            return RedirectToAction("Index");
        }
    }
}