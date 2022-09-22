using System;
using System.Threading.Tasks;
using EasyNow.Collection;
using EasyNow.Dto;
using EasyNow.Dto.Query;
using JetBrains.Annotations;

namespace EasyNow.Service
{
    public interface IRepositoryService<T>
    {
        Task<TResult> AddAsync<TResult>([System.Diagnostics.CodeAnalysis.NotNull]TResult model);
        Task<TResult[]> AddRangeAsync<TResult>([System.Diagnostics.CodeAnalysis.NotNull]TResult[] models);

        Task<TResult[]> QueryAllAsync<TResult>(QueryAllDto query);

        Task<PagedList<TResult>> QueryAsync<TResult>([System.Diagnostics.CodeAnalysis.NotNull] QueryDto query);

        Task<TResult> GetAsync<TResult>(Guid id);

        Task<TResult> UpdateAsync<TResult>([System.Diagnostics.CodeAnalysis.NotNull]TResult model)where TResult:IIdKeyDto;

        Task<bool> DeleteAsync([System.Diagnostics.CodeAnalysis.NotNull]Guid[] ids);
    }
}