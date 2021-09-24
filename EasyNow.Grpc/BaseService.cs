using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Autofac;
using EasyNow.Collection;
using EasyNow.Dto;
using EasyNow.Dto.Query;
using EasyNow.Service;
using JetBrains.Annotations;

namespace EasyNow.Grpc
{
    public abstract class BaseService<TEntity, TResult>:IBaseService<TEntity, TResult> where TResult : IIdKeyDto
    {
        public ILifetimeScope LifetimeScope { get; set; }
        protected IRepositoryService<TEntity> RepositoryService => LifetimeScope.Resolve<IRepositoryService<TEntity>>();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        public virtual Task<TResult> Add([NotNull] TResult model)
        {
            return RepositoryService.AddAsync(model);
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [OperationContract]
        public virtual Task<TResult[]> AddRange([NotNull] TResult[] models)
        {
            return RepositoryService.AddRangeAsync(models);
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [OperationContract]
        public virtual Task<TResult[]> QueryAll([NotNull] QueryAllDto query)
        {
            return RepositoryService.QueryAllAsync<TResult>(query);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [OperationContract]
        public virtual Task<PagedList<TResult>> Query([NotNull] QueryDto query)
        {
            return RepositoryService.QueryAsync<TResult>(query);
        }

        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [OperationContract]
        public virtual Task<TResult> Get(GrpcReq<Guid> req)
        {
            return RepositoryService.GetAsync<TResult>(req.Data);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        public virtual Task<TResult> Update([NotNull] TResult model)
        {
            return RepositoryService.UpdateAsync(model);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [OperationContract]
        public virtual Task<bool> Delete([NotNull] GrpcReq<Guid[]> req)
        {
            return RepositoryService.DeleteAsync(req.Data);
        }
    }
}