
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc_Client.Interceptors;
using GrpcServer.Protos;
using Microsoft.Extensions.Logging;
using static GrpcServer.Protos.ProductService;

ILoggerFactory logger = LoggerFactory.Create(c =>
{
    c.AddConsole();
    c.SetMinimumLevel(LogLevel.Debug);
});

var handler = new SocketsHttpHandler
{
    /// باعث میشود کانکشن ما هر 5 ثانیه پینگ میگیرد تا برای درخواست های بعدی از بین نرود
    KeepAlivePingDelay= TimeSpan.FromSeconds(5),

    ///بعد از 7 دقیقه اگر کاری نکنیم درخواست از بین میرود تا سربار نداشته باشد
    PooledConnectionIdleTimeout= TimeSpan.FromMinutes(7),

    ///بعد از 11 دقیقه درخواست ما کاملا از بین میرود
    KeepAlivePingTimeout= TimeSpan.FromMinutes(11),

    ///در اچ تی تی پی 2 ما بیش از 100 درخواست نمیتوانیم ارسال کنیم و اگر بیشتر شد صف تشکیل میدهد. راه حل استفاده از چندین درخواست است
    EnableMultipleHttp2Connections= true,

};

GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:7058/",new GrpcChannelOptions
{
    LoggerFactory=logger,

    //پیشفرض 4 مگابایت میباشد
    MaxReceiveMessageSize=11_000_00,

    MaxSendMessageSize=12_000_000,

    HttpHandler=handler
    
});

ProductServiceClient client = new ProductServiceClient(channel.Intercept(new ExceptionInterceptor()));

//----unary-----
GetById(client);

//----stream----
//--client-stream--
//await Remove(client);


//--bidirectional--
//using var addProduct = client.AddNewProduct();
//List<AddNewProductRequest> addProducts = new List<AddNewProductRequest>()
//{
//    new AddNewProductRequest{Name="del" , Price=1000},
//    new AddNewProductRequest{Name="asus" , Price=2000},
//    new AddNewProductRequest{Name="hp" , Price=3000},
//};

//foreach (var item in addProducts)
//{
//    await addProduct.RequestStream.WriteAsync(item);
//}
//await addProduct.RequestStream.CompleteAsync();

//List<ProductReply> productReplies = new List<ProductReply>();
//await foreach (var item in addProduct.ResponseStream.ReadAllAsync())
//{
//    productReplies.Add(item);
//}



//--server-stream--
//using var getProducts = client.GetProducts(new Empty());
//List<ProductReply> products = new List<ProductReply>();

//await foreach (var product in getProducts.ResponseStream.ReadAllAsync())
//{
//    products.Add(product);
//}

Console.WriteLine("hellooooo....");

static async Task<AsyncClientStreamingCall<ProductByIdRequest,Empty>> Remove(ProductServiceClient client)
{
    // از یوزینگ استفاده میکنیم برای استفاده بهتر از حافظه
    using var deleteProduct = client.DeleteProduct();
    List<ProductByIdRequest> products = new List<ProductByIdRequest>()
{
    new ProductByIdRequest(){Id=1},
    new ProductByIdRequest(){Id=2},
};
    foreach (var item in products)
    {
        await deleteProduct.RequestStream.WriteAsync(item);
    }

    //اگر کامپیلیت را قرار ندهیم در سرور درون حلقه منتظر دیتای جدید است و سربار دارد
    await deleteProduct.RequestStream.CompleteAsync();

    return deleteProduct;
}

static async void GetById(ProductServiceClient client)
{

    try
    {
        /// روش 1
        var product = client.GetProductById(new GrpcServer.Protos.ProductByIdRequest
        {
            Id = 1
        });

        ///روش 2
        //var result = client.GetProductByIdAsync(new GrpcServer.Protos.ProductByIdRequest
        //{
        //    Id = 0
        //});
        //var p = await result.ResponseAsync;

        //مواردی که در هدر است و برای کلاینت ارسال میشود مثل پیام ها
        //var trailers = result.GetTrailers();
    }
    catch(RpcException rex)
    {
       var errorTrailers= rex.Trailers;

        throw;
    }
    catch (Exception)
    {

        throw;
    }

}