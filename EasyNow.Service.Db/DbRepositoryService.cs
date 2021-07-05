using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Principal;
using System.Threading.Tasks;
using Autofac;
using EasyNow.Dal;
using EasyNow.Dal.Extensions;
using EasyNow.Dto;
using EasyNow.Dto.Exceptions;
using EasyNow.Dto.Query;
using EasyNow.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace EasyNow.Service.Db
{
    public class DbRepositoryService<T,TUser>:BaseRepositoryService<T> where T:class,IIdKeyEntity
    {
        public ILifetimeScope LifetimeScope { get; set; }
        private readonly DbContext _context;

        private static Type ListType = typeof(List<>);

        public DbRepositoryService(DbContext context)
        {
            _context = context;
        }

        private TUser CurrentUser => LifetimeScope.Resolve<IUserResolver<TUser>>()
            .GetUserIdentity(LifetimeScope.Resolve<IPrincipal>().Identity.Name);

        private DbSet<T> DbSet => _context.Set<T>();

        public override async Task<TResult> AddAsync<TResult>(TResult model)
        {
            var entity = model.To<T>();
            if (entity.Id == default)
            {
                entity.Id=Guid.NewGuid();
            }
            if (entity is IAuditEntity<TUser> auditEntity)
            {
                var utcNow = DateTime.UtcNow;
                auditEntity.Creator = CurrentUser;
                auditEntity.Updater = CurrentUser;
                auditEntity.CreateTime = utcNow;
                auditEntity.UpdateTime = utcNow;
            }
            await this.DbSet.AddAsync(entity);
            await this._context.SaveChangesAsync();
            return entity.To<TResult>();
        }

        public override Task<PagedList<TResult>> QueryAsync<TResult>(QueryDto query)
        {
            var q = this.DbSet.AsNoTracking();
            var properties = typeof(T).GetProperties();
            var propertyDic = properties.ToDictionary(e => e.Name, e => e.PropertyType);

            if (query.Conditions != null && query.Conditions.Any())
            {
                var queryList = new List<string>();
                var paramList = new List<object>();
                
                foreach (var condition in query.Conditions)
                {
                    var prop = propertyDic.FirstOrDefault(e =>
                        e.Key.Equals(condition.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (!string.IsNullOrEmpty(prop.Key))
                    {
                        switch (condition.Operator)
                        {
                            case EOperator.Eq:
                                queryList.Add($"{prop.Key} = @{paramList.Count}");
                                paramList.Add(condition.Value);
                                break;
                            case EOperator.Neq:
                                queryList.Add($"{prop.Key} != @{paramList.Count}");
                                paramList.Add(condition.Value);
                                break;
                            // todo 暂时无法实现真正的like,只能用contains替代
                            case EOperator.Like:
                                queryList.Add($"{prop.Key}.Contains(@{paramList.Count})");
                                paramList.Add(condition.Value);
                                break;
                            case EOperator.In:
                                queryList.Add($"@{paramList.Count}.Contains({prop.Key})");
                                var list=(IList)Activator.CreateInstance(ListType.MakeGenericType(prop.Value));
                                (condition.Value as IEnumerable<object>)
                                    .Select(e => Convert.ChangeType(e, prop.Value)).ToArray().Foreach(e =>
                                    {
                                        list.Add(e);
                                    });
                                paramList.Add(list);
                                break;
                            case EOperator.Nin:
                                queryList.Add($"!(@{paramList.Count}.Contains({prop.Key}))");
                                var list1=(IList)Activator.CreateInstance(ListType.MakeGenericType(prop.Value));
                                (condition.Value as IEnumerable<object>)
                                    .Select(e => Convert.ChangeType(e, prop.Value)).ToArray().Foreach(e =>
                                    {
                                        list1.Add(e);
                                    });
                                paramList.Add(list1);
                                break;
                            case EOperator.Gt:
                                queryList.Add($"{prop.Key} > @{paramList.Count}");
                                paramList.Add(condition.Value);
                                break;
                            case EOperator.Gte:
                                queryList.Add($"{prop.Key} >= @{paramList.Count}");
                                paramList.Add(condition.Value);
                                break;
                            case EOperator.Lt:
                                queryList.Add($"{prop.Key} < @{paramList.Count}");
                                paramList.Add(condition.Value);
                                break;
                            case EOperator.Lte:
                                queryList.Add($"{prop.Key} <= @{paramList.Count}");
                                paramList.Add(condition.Value);
                                break;
                            case EOperator.Null:
                                queryList.Add($"{prop.Key} == null");
                                break;
                            case EOperator.NNull:
                                queryList.Add($"{prop.Key} != null");
                                break;
                        }
                        
                    }
                }

                if (queryList.Any())
                {
                    q = q.Where(queryList.Join(" and "), paramList.ToArray());
                }
            }

            var orderStr = string.Empty;
            if (query.Orders !=null&& query.Orders.Any())
            {
                var orderList = new List<string>();
                foreach (var condition in query.Orders)
                {
                    var prop = propertyDic.FirstOrDefault(e =>
                        e.Key.Equals(condition.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (!string.IsNullOrEmpty(prop.Key))
                    {
                        orderList.Add($"{prop.Key} {condition.Order.ToString().ToLower()}");
                    }
                }

                if (orderList.Any())
                {
                    orderStr = orderList.Join(",");
                }
            }

            // 此处如果没有传排序规则，则默认用Id升序
            if (string.IsNullOrEmpty(orderStr))
            {
                orderStr = "Id asc";
            }
            return q.OrderBy(orderStr).ToPagedListAsync<T, TResult>(query);
        }

        public override async Task<TResult> GetAsync<TResult>(Guid id)
        {
            var result =await this.DbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (result == null)
            {
                return default;
            }

            return result.To<TResult>();
        }

        public override async Task<TResult> UpdateAsync<TResult>(TResult model)
        {
            var entity = await this.DbSet.FirstOrDefaultAsync(e => e.Id == model.Id);
            if (entity == null)
            {
                throw new MessageException("未找到数据");
            }

            entity.CopyFrom(model);
            if (entity is IAuditEntity<TUser> auditEntity)
            {
                auditEntity.Updater = CurrentUser;
                auditEntity.UpdateTime = DateTime.UtcNow;
            }
            await this._context.SaveChangesAsync();
            return entity.To<TResult>();
        }

        public override async Task<bool> DeleteAsync(Guid[] ids)
        {
            if (typeof(T).IsAssignableTo<ISoftDeleteEntity>())
            {
                return (await this.DbSet.Where(e => ids.Contains(e.Id))
                    .UpdateAsync(e => new {IsDeleted = true}
                    )) == ids.Length;
            }
            return (await this.DbSet.Where(e => ids.Contains(e.Id)).DeleteAsync()) == ids.Length;
        }
    }
}