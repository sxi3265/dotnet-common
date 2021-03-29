using System;

namespace EasyNow.Dal
{
    public class BaseEntity:IEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid Creator { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public Guid Updater { get; set; }
    }
}
