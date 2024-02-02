using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer.Protos;
using GrpcServer.Repository;

namespace GrpcServer.Services
{
    public class ProductGrpcService : ProductService.ProductServiceBase
    {
        private readonly IProductRepository productRepository;
        public ProductGrpcService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public override Task<CollectionSample> CollectionMethod(CollectionSample request, ServerCallContext context)
        {
            request.MyDictionary.Add(1, "first value");
            request.MyList.Add(1);
            request.MyList.Add(2);
            request.MyList.Add(3);

            var list = request.MyList.ToList();

            return base.CollectionMethod(request, context);
        }
        public override async Task AddNewProduct(IAsyncStreamReader<AddNewProductRequest> requestStream, IServerStreamWriter<ProductReply> responseStream, ServerCallContext context)
        {

            await foreach (var item in requestStream.ReadAllAsync())
            {
                var result = productRepository.Add(item.Name, item.Price);

                await responseStream.WriteAsync(new ProductReply
                {
                    Id = result.Id,
                    Name = result.Name,
                    Price = result.Price,
                });

            }

        }

        public override async Task<Empty> DeleteProduct(IAsyncStreamReader<ProductByIdRequest> requestStream, ServerCallContext context)
        {
            await foreach (var item in requestStream.ReadAllAsync())
            {
                productRepository.Remove(item.Id);

            }
            return new Empty();
        }

        public override async Task<ProductReply> GetProductById(ProductByIdRequest request, ServerCallContext context)
        {
            var product = productRepository.GetProductById(request.Id);
            return new ProductReply { Id = product.Id, Name = product.Name ,Price=product.Price};
        }

        public override async Task GetProducts(Empty request, IServerStreamWriter<ProductReply> responseStream, ServerCallContext context)
        {
            foreach (var item in productRepository.GetProducts())
            {
               await responseStream.WriteAsync(new ProductReply
                {
                    Id = item.Id,
                    Name= item.Name,
                    Price = item.Price,
                });
            }
        }

        public override async Task<ProductReply> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            var updateProduct = productRepository.Update(new Product
            {
                Id = request.Id,
                Name = request.Name,
                Price = request.Price,
            });

            return new ProductReply
            {
                Id = updateProduct.Id,
                Name = updateProduct.Name,
                Price = updateProduct.Price,
            };
        }
    }
}
