using ClothBazar.Database;
using ClothBazar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ClothBazar.Services
{
    public class ProductsService
    {
        #region Singleton

        public static ProductsService Instance
        {
            get
            {
                if (instance == null) instance = new ProductsService();
                {
                    return instance;
                }
            }
        }
        private static ProductsService instance { get; set; }

        private ProductsService()
        {

        }

        #endregion


        public Product GetProduct(int ID)
        {
            using (var context = new CBContext())
            {
                return context.Products.Where(x => x.ID == ID).Include(x => x.Category).FirstOrDefault();
            }
        }

        public int GetAllProductCount(string search)
        {
            using (var context = new CBContext())
            {
                if (!string.IsNullOrEmpty(search))
                {
                    return context.Products.Count(p => p.Name != null
                                                       && p.Name.ToLower()
                                                           .Contains(search.ToLower()));
                }


                return context.Products.Count();

            }
        }

        public List<Product> GetProducts(List<int> IDs)
        {
            using (var context = new CBContext())
            {
                return context.Products.Where(p => IDs.Contains(p.ID)).ToList();
            }
        }
        public List<Product> GetLatestProducts(int numberOfProducts)
        {
            using (var context = new CBContext())
            {
                return context.Products.OrderByDescending(p => p.ID).Take(numberOfProducts).Include(p => p.Category)
                    .ToList();
            }
        }
        public List<Product> GetLProductsbyCategory(int categoryID,int pageSize)
        {
            using (var context = new CBContext())
            {
                return context.Products.Where(p=>p.Category.ID==categoryID)
                    .OrderByDescending(p=>p.ID)
                    .Take(pageSize)
                    .Include(p=>p.Category)
                    .ToList();
            }
        }
        public List<Product> GetProducts(int pageNo,int pageSize)
        {
            using (var context = new CBContext())
            {
                return context.Products.OrderByDescending(p => p.ID)
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .Include(p => p.Category).ToList();
            }
        }
        public List<Product> GetProducts(string search,int pageNo)
        {
            int pageSize = int.Parse(ConfigurationsService.Instance.GetConfig("ListingPageSize").Value);
            using (var context = new CBContext())
            {
                if (!string.IsNullOrEmpty(search))
                {
                    return context.Products
                        .OrderBy(p => p.ID)
                        .Skip((pageNo - 1) * pageSize)
                        .Take(pageSize)
                        .Include(p => p.Category)
                        .ToList();
                }
                else
                {
                    return context.Products
                        .OrderBy(c => c.ID)
                        .Skip((pageNo - 1) * pageSize)
                        .Take(pageSize)
                        .Include(c => c.Category)
                        .ToList();
                }
                

                //return context.Products.Include(c => c.Category).ToList();
            }
        }
        public void SaveProduct(Product product)
        {
            using (var context=new CBContext())
            {
                context.Entry(product.Category).State = System.Data.Entity.EntityState.Unchanged;
                context.Products.Add(product);
                context.SaveChanges();
            }
        }
        
        public void UpdateProduct(Product product)
        {
            using (var context = new CBContext())
            {
                context.Entry(product).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        public void DeleteProduct(int ID)
        {
            using (var context = new CBContext())
            {
               // context.Entry(category).State = System.Data.Entity.EntityState.Modified;
               var product = context.Products.Find(ID);
               context.Products.Remove(product);
                context.SaveChanges();
            }
        }

        public int GetMaximumPrice()
        {
            using (var context = new CBContext())
            {
                return (int) (context.Products.Max(p => p.Price));
            }
        }

        public List<Product> SearchProducts(string searchTerm, int? maximumPrice, int? minimumPrice, int? categoryId, int? sortBy)
        {
            using (var context = new CBContext())
            {
                var products = context.Products.ToList();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    products = products.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
                }

                if (maximumPrice.HasValue)
                {
                    products = products.Where(p => p.Price >= minimumPrice.Value).ToList();
                }

                if (minimumPrice.HasValue)
                {
                    products = products.Where(p => p.Price <= maximumPrice.Value).ToList();
                }

                if (categoryId.HasValue)
                {
                    products = products.Where(p => p.Category.ID == categoryId.Value).ToList();
                }

                if (sortBy.HasValue)
                {
                    switch (sortBy.Value)
                    {
                        case 2:
                            products = products.OrderByDescending(x => x.ID).ToList();
                            break;
                        case 3:
                            products = products.OrderBy(x => x.Price).ToList();
                            break;
                        default:
                            products = products.OrderByDescending(x => x.Price).ToList();
                            break;
                    }
                }

                return products;
            }
        }
    }
}
