using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc_Client.Interceptors
{
    internal class ExceptionInterceptor: Interceptor
    {
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                ///همه متدهای یونری پروژه از این مسیر رد میشوند
                return continuation(request, context);

            }
            catch (RpcException ex)
            {
                //log exe
                //save log for dbLog

                var errorTrailers = ex.Trailers;
                throw;

            }
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                ///همه متدهای یونری پروژه از این مسیر رد میشوند
                return continuation(request, context);

            }
            catch (RpcException ex)
            {
                //log exe
                //save log for dbLog

                var errorTrailers = ex.Trailers;
                throw;

            }
        }
    }
}
