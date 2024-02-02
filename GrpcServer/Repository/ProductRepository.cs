namespace GrpcServer.Repository
{
    public interface IProductRepository
    {
        Product Add(string name, int price);
        List<Product> GetProducts();
        Product Update(Product product);
        void Remove(int id);
        Product GetProductById(int id);
    }
    public class ProductRepository : IProductRepository
    {
        List<Product> products = new List<Product>()
        {
            new Product
            {
                Id=1,
                Price=10,
                Name="Lg x587"
            },
             new Product
            {
                Id=2,
                Price=20,
                Name="Samsong x587"
            }
        };
        public Product Add(string name, int price)
        {
            Random rd = new Random();
            int id=rd.Next();

            products.Add(new Product
            {
                Id = id,
                Name = name,
                Price = price
            });

            return products.Where(p => p.Id == id).FirstOrDefault();
        }

        public Product GetProductById(int id)
        {
            return products.Where(p => p.Id == id).FirstOrDefault();
        }

        public List<Product> GetProducts()
        {
            return products;
        }

        public void Remove(int id)
        {
            var product = products.Where(p => p.Id == id).FirstOrDefault();
            products.Remove(product);   
        }

        public Product Update(Product product)
        {
            var productUpdates = products.Where(p => p.Id == product.Id).FirstOrDefault();
            productUpdates.Name=product.Name;
            productUpdates.Price=product.Price;
            return productUpdates;
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}
