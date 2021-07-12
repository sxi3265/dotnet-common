using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Principal;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        public override Task<TResult[]> QueryAllAsync<TResult>(QueryAllDto query)
        {
            var q = this.DbSet.AsNoTracking();
            var properties = typeof(T).GetProperties();
            var propertyDic = properties.ToDictionary(e => e.Name, e => e.PropertyType);

            var expressionResult = BuildExpression(query.Expression, propertyDic);
            if (!string.IsNullOrEmpty(expressionResult.expression))
            {
                var paramList = new List<object>();
                var expression = expressionResult.expression;

                expressionResult.paramDic.Foreach(e =>
                {
                    expression=expression.Replace($"@{e.Key:D}", $"@{paramList.Count}");
                    paramList.Add(e.Value);
                });

                q = q.Where(expression, paramList.ToArray());
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

            return q.OrderBy(orderStr).ProjectTo<TResult>(LifetimeScope.Resolve<IMapper>().ConfigurationProvider)
                .ToArrayAsync();
        }

        protected virtual (string expression,Dictionary<Guid,object> paramDic) BuildExpression(Expression expression,Dictionary<string,Type> propertyDic)
        {
            if (expression == null)
            {
                return (null, null);
            }
            var queryList = new List<string>();
            var paramDic = new Dictionary<Guid,object>();
            if (expression.Conditions != null)
            {
                foreach (var condition in expression.Conditions)
                {
                    var prop = propertyDic.FirstOrDefault(e =>
                        e.Key.Equals(condition.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (!string.IsNullOrEmpty(prop.Key))
                    {
                        Guid paramKey;
                        switch (condition.Operator)
                        {
                            case EOperator.Eq:
                                paramKey = Guid.NewGuid();
                                queryList.Add($"{prop.Key} = @{paramKey:D}");
                                paramDic.Add(paramKey, condition.Value);
                                break;
                            case EOperator.Neq:
                                paramKey = Guid.NewGuid();
                                queryList.Add($"{prop.Key} != @{paramKey:D}");
                                paramDic.Add(paramKey, condition.Value);
                                break;
                            // todo 暂时无法实现真正的like,只能用contains替代
                            case EOperator.Like:
                                paramKey = Guid.NewGuid();
                                queryList.Add($"{prop.Key}.Contains(@{paramKey:D})");
                                paramDic.Add(paramKey, condition.Value);
                                break;
                            case EOperator.In:
                                paramKey = Guid.NewGuid();
                                queryList.Add($"@{paramKey:D}.Contains({prop.Key})");
                                var list = (IList) Activator.CreateInstance(ListType.MakeGenericType(prop.Value));
                                (condition.Value as IEnumerable<object>)
                                    .Select(e => Convert.ChangeType(e, prop.Value)).ToArray().Foreach(e =>
                                    {
                                        list.Add(e);
                                    });
                                paramDic.Add(paramKey, list);
                                break;
                            case EOperator.Nin:
                                paramKey = Guid.NewGuid();
                                queryList.Add($"!(@{paramKey:D}.Contains({prop.Key}))");
                                var list1 = (IList) Activator.CreateInstance(ListType.MakeGenericType(prop.Value));
                                (condition.Value as IEnumerable<object>)
                                    .Select(e => Convert.ChangeType(e, prop.Value)).ToArray().Foreach(e =>
                                    {
                                        list1.Add(e);
                                    });
                                paramDic.Add(paramKey, list1);
                                break;
                            case EOperator.Gt:
                                paramKey = Guid.NewGuid();
                                queryList.Add($"{prop.Key} > @{paramKey:D}");
                                paramDic.Add(paramKey, condition.Value);
                                break;
                            case EOperator.Gte:
                                paramKey = Guid.NewGuid();
                                queryList.Add($"{prop.Key} >= @{paramKey:D}");
                                paramDic.Add(paramKey, condition.Value);
                                break;
                            case EOperator.Lt:
                                paramKey = Guid.NewGuid();
                                queryList.Add($"{prop.Key} < @{paramKey:D}");
                                paramDic.Add(paramKey, condition.Value);
                                break;
                            case EOperator.Lte:
                                paramKey = Guid.NewGuid();
                                queryList.Add($"{prop.Key} <= @{paramKey:D}");
                                paramDic.Add(paramKey, condition.Value);
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
            }

            if (expression.Expressions != null)
            {
                foreach (var item in expression.Expressions)
                {
                    var result = BuildExpression(item, propertyDic);
                    if (string.IsNullOrEmpty(result.expression))
                    {
                        continue;
                    }
                    queryList.Add(result.expression);
                    result.paramDic.Foreach(e =>
                    {
                        paramDic.Add(e.Key,e.Value);
                    });
                }
            }

            if (queryList.Any())
            {
                return ($"({queryList.Join($" {expression.Operator.ToString().ToLower()} ")})", paramDic);
            }

            return (null, null);
        }

        public override Task<PagedList<TResult>> QueryAsync<TResult>(QueryDto query)
        {
            var q = this.DbSet.AsNoTracking();
            var properties = typeof(T).GetProperties();
            var propertyDic = properties.ToDictionary(e => e.Name, e => e.PropertyType);

            var expressionResult = BuildExpression(query.Expression, propertyDic);
            if (!string.IsNullOrEmpty(expressionResult.expression))
            {
                var paramList = new List<object>();
                var expression = expressionResult.expression;

                expressionResult.paramDic.Foreach(e =>
                {
                    expression=expression.Replace($"@{e.Key:D}", $"@{paramList.Count}");
                    paramList.Add(e.Value);
                });

                q = q.Where(expression, paramList.ToArray());
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