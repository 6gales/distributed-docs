using System.Collections.Generic;
using System.Linq;

namespace DistributedDocs.Client
{
    namespace Data
    {
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Url
            {
                get
                {
                    return "products/" + Id;
                }
            }
        }

        public static class Products
        {
            private static readonly List<Product> products = new List<Product>
            {
                new Product {Id = 1, Name = "Test"},
                new Product {Id = 2, Name = "Test2"},
                new Product {Id = 3, Name = "Test3"},
                new Product {Id = 4, Name = "Test4"}
            };

            public static Product GetProduct(int id) 
            {
                return products.SingleOrDefault(p => p.Id == id);
            }
            public static List<Product> GetProducts(int? page, int? pageSize) 
            {
                if (page.HasValue && pageSize.HasValue) {
                    var filtered = products.Skip((page.Value-1) * pageSize.Value).Take(pageSize.Value);
                    return filtered.ToList();
                }
                return products;
            }
        }
    }
}