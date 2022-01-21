using System;
using System.Threading.Tasks;
using EasyNow.Collection;
using EasyNow.Dto;
using EasyNow.Dto.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace EasyNow.Grpc.Interceptors
{
    public class ExceptionInterceptor:Interceptor
    {
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger<ExceptionInterceptor> Logger { get; set; }

        private static readonly Type GrpcResultType = typeof(GrpcResult);
        private static readonly Type GrpcResultGenericType = typeof(GrpcResult<>);

        /// <inheritdoc />
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await base.UnaryServerHandler(request, context, continuation);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception e)
            {
                var resp = OnException<TResponse>(e);
                return resp;
            }
        }

        /// <inheritdoc />
        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context,
            ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await base.ClientStreamingServerHandler(requestStream, context, continuation);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception e)
            {
                var resp = OnException<TResponse>(e);
                return resp;
            }
        }

        /// <inheritdoc />
        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                await base.ServerStreamingServerHandler(request, responseStream, context, continuation);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception e)
            {
                var resp = OnException<TResponse>(e);
                await responseStream.WriteAsync(resp);
            }
        }

        /// <inheritdoc />
        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                await base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception e)
            {
                var resp = OnException<TResponse>(e);
                await responseStream.WriteAsync(resp);
            }
        }

        private TResponse OnException<TResponse>(Exception e) where TResponse:class
        {
            var msgException = e as MessageException;
            if (typeof(TResponse) == GrpcResultType)
            {
                return new GrpcResult
                {
                    Code = -1,
                    Msg = msgException!=null?msgException.Message:"执行失败"
                } as TResponse;
            }

            if (typeof(TResponse).GetGenericTypeDefinition() == GrpcResultGenericType &&
                Activator.CreateInstance<TResponse>() is GrpcResult grpcResult)
            {
                grpcResult.Code = -1;
                grpcResult.Msg = msgException != null ? msgException.Message : "执行失败";
                return grpcResult as TResponse;
            }

            if (msgException == null)
            {
                Logger.LogError(e,"gRPC执行异常");
            }

            return Activator.CreateInstance<TResponse>();
        }
    }
}
