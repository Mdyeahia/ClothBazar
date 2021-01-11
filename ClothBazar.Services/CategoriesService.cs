using ClothBazar.Database;
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

        public int GetCategoriesCount(string search)
        {
            using (var context=new CBContext())
            {

                if (!string.IsNullOrEmpty(search))
                {
                    return context.Categories.Count(category => category.Name != null
                                                                && category.Name.ToLower()
                                                                    .Contains(search.ToLower()));
                }
                else
                {
                    return context.Categories.Count();
                }
            }
        }
        public List<Category> GetAllCategories()
        {
            using (var context = new CBContext())
            {
                return context.Categories.ToList();
            }
        }
        public List<Category> GetCategories(string search,int  pageNo)
        {
            int pageSize = int.Parse(ConfigurationsService.Instance.GetConfig("ListingPageSize").Value);
            using (var context = new CBContext())
            {
                if (!string.IsNullOrEmpty(search))
                {
                    return context.Categories.Where(c => c.Name != null && c.Name.ToLower().Contains(search.ToLower()))
                            .OrderBy(c => c.ID)
                            .Skip((pageNo - 1) * pageSize)
                            .Take(pageSize)
                            .Include(c => c.Products)
                            .ToList();
                }
                else
                {
                    return context.Categories
                        .OrderBy(c => c.ID)
                        .Skip((pageNo - 1) * pageSize)
                        .Take(pageSize)
                        .Include(c => c.Products)
                        .ToList();
                }
                
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
