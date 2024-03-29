﻿using System;
using System.Threading.Tasks;
using EasyNow.Collection;
using EasyNow.Dto;
using EasyNow.Dto.Query;

namespace EasyNow.Service
{
    public abstract class BaseRepositoryService<T>:IRepositoryService<T>
    {
        public abstract Task<TResult> AddAsync<TResult>(TResult model);

        public abstract Task<TResult[]> AddRangeAsync<TResult>(TResult[] models);

        public abstract Task<TResult[]> QueryAllAsync<TResult>(QueryAllDto query);

        public abstract Task<PagedList<TResult>> QueryAsync<TResult>(QueryDto query);

        public abstract Task<TResult> GetAsync<TResult>(Guid id);

        public abstract Task<TResult> UpdateAsync<TResult>(TResult model)where TResult:IIdKeyDto;

        public abstract Task<bool> DeleteAsync(Guid[] ids);
    }
}