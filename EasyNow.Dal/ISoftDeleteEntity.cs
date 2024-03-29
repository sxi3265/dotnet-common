﻿namespace EasyNow.Dal
{
    /// <summary>
    /// 软删实体
    /// </summary>
    public interface ISoftDeleteEntity : IEntity
    {
        /// <summary>
        /// 已删除
        /// </summary>
        bool IsDeleted{ get; set; }
    }
}