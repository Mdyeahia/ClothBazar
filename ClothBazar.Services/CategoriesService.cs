﻿using ClothBazar.Database;
using ClothBazar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace ClothBazar.Services
{
    public class CategoriesService
    {
        #region Singleton
        public static CategoriesService Instance
        {
            get
            {
                if (instance == null) instance = new CategoriesService();

                return instance;
            }
        }
        private static CategoriesService instance { get; set; }
        private CategoriesService()
        {
        }
        #endregion
        public Category GetCategory(int ID)
        {
            using (var context = new CBContext())
            {
                return context.Categories.Find(ID);
            }
        }
        public List<Category> GetCategories()
        {
            using (var context = new CBContext())
            {
                return context.Categories.Include(p => p.Products).ToList();
            }
        }
        public List<Category> GetFeaturedCategories()
        {
            using (var context = new CBContext())
            {
                return context.Categories.Where(c => c.isFeatured && c.ImageURL !=null).ToList();
            }
        }
        public void SaveCategory(Category category)
        {
            using (var context=new CBContext())
            {
                context.Categories.Add(category);
                context.SaveChanges();
            }
        }
        
        public void UpdateCategory(Category category)
        {
            using (var context = new CBContext())
            {
                context.Entry(category).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        public void DeleteCategory(int ID)
        {
            using (var context = new CBContext())
            {
               // context.Entry(category).State = System.Data.Entity.EntityState.Modified;
               var category = context.Categories.Find(ID);
               context.Categories.Remove(category);
                context.SaveChanges();
            }
        }
    }
}
