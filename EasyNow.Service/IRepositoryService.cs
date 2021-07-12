using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EasyNow.Dto;
using EasyNow.Dto.Query;

namespace EasyNow.Service
{
    public interface IRepositoryService<T>
    {
        Task<TResult> AddAsync<TResult>([NotNull]TResult model);

        Task<TResult[]> QueryAllAsync<TResult>(QueryAllDto query);

        Task<PagedList<TResult>> QueryAsync<TResult>([NotNull] QueryDto query);

        Task<TResult> GetAsync<TResult>(Guid id);

        Task<TResult> UpdateAsync<TResult>([NotNull]TResult model)where TResult:IIdKeyDto;

        Task<bool> DeleteAsync([NotNull]Guid[] ids);
    }
}