using System.Threading.Tasks;

namespace EasyNow.Collection
{
    /// <summary>
    /// GrpcResult扩展方法
    /// </summary>
    public static class GrpcResultExtensions
    {
        /// <summary>
        /// ToGrpcResult
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GrpcResult<T> ToGrpcResult<T>(this T data)
        {
            return new GrpcResult<T>
            {
                Code = true,
                Data = data
            };
        }

        /// <summary>
        /// ToGrpcResult
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task<GrpcResult<T>> ToGrpcResult<T>(this Task<T> task)
        {
            return new GrpcResult<T>
            {
                Code = true,
                Data = await task
            };
        }

        /// <summary>
        /// 获取GrpcResult的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grpcResultTask"></param>
        /// <returns></returns>
        public static async Task<T> GetData<T>(this Task<GrpcResult<T>> grpcResultTask)
        {
            return (await grpcResultTask).Data;
        }
    }
}