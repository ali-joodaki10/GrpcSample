using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcServer.Interceptors
{
    public class ExceptionInterceptor:Interceptor
    {
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            return base.AsyncClientStreamingCall(context, continuation);
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                ///همه متدهای یونری پروژه از این مسیر رد میشوند
                return await continuation(request, context);

            }
            catch (Exception ex)
            {
                Guid exId = Guid.NewGuid();
                //log exe
                //save log for dbLog

                Metadata trailers = new Metadata();

                trailers.Add("ExceptionId", exId.ToString());

                throw new RpcException(new Status(StatusCode.Internal,"خطا رخ داده است."), trailers);
            }
        }
    }
}
