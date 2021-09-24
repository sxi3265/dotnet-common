using System;
using System.ServiceModel;
using System.Threading.Tasks;
using EasyNow.Collection;
using EasyNow.Dto;
using EasyNow.Dto.Query;
using JetBrains.Annotations;

namespace EasyNow.Grpc
{
    public interface IBaseService<TResult>where TResult : IIdKeyDto
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Task<GrpcResult<TResult>> Add([NotNull] TResult model);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [OperationContract]
        Task<GrpcResult<TResult[]>> AddRange([NotNull] TResult[] models);

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [OperationContract]
        Task<GrpcResult<TResult[]>> QueryAll([NotNull] QueryAllDto query);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [OperationContract]
        Task<GrpcResult<PagedList<TResult>>> Query([NotNull] QueryDto query);

        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [OperationContract]
        Task<GrpcResult<TResult>> Get(GrpcReq<Guid> req);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Task<GrpcResult<TResult>> Update([NotNull] TResult model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [OperationContract]
        Task<GrpcResult<bool>> Delete([NotNull] GrpcReq<Guid[]> req);
    }
}