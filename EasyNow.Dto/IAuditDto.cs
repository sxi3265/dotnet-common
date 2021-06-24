using System;

namespace EasyNow.Dto
{
    /// <summary>
    /// 审计
    /// </summary>
    public interface IAuditDto<T>:IDto
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        T Creator { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        DateTime UpdateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        T Updater { get; set; }
    }
}