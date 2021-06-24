using System;
using System.Threading.Tasks;
using Autofac;
using EasyNow.Dto;
using EasyNow.Dto.Query;
using EasyNow.Service;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace EasyNow.AspNetCore
{
    [ApiController,Route("api/[controller]/[action]")]
    public abstract class BaseDbApiController<T,TResult>:ControllerBase where TResult:IIdKeyDto
    {
        public ILifetimeScope LifetimeScope { get; set; }
        protected IRepositoryService<T> RepositoryService => LifetimeScope.Resolve<IRepositoryService<T>>();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<TResult> Add([NotNull,FromBody] TResult model)
        {
            return RepositoryService.AddAsync(model);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<PagedList<TResult>> Query([NotNull,FromBody] QueryDto query)
        {
            return RepositoryService.QueryAsync<TResult>(query);
        }

        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual Task<TResult> Get(Guid id)
        {
            return RepositoryService.GetAsync<TResult>(id);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<TResult> Update([NotNull,FromBody] TResult model)
        {
            return RepositoryService.UpdateAsync(model);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Task<bool> Delete([NotNull,FromBody] Guid[] ids)
        {
            return RepositoryService.DeleteAsync(ids);
        }
    }
}