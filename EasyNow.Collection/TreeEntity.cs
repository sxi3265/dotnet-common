using System.Collections.Generic;
using ProtoBuf;

namespace EasyNow.Collection
{
    /// <summary>
    /// 树形实体
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    [ProtoContract]
    public class TreeEntity<TKey,TEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [ProtoMember(1,Name = "id")]
        public TKey Id { get; set; }

        /// <summary>
        /// 父Id
        /// </summary>
        [ProtoMember(2,Name = "parentId")]
        public TKey? ParentId { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        [ProtoMember(3,Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [ProtoMember(4,Name = "state")]
        public string State { get; set; }

        /// <summary>
        /// 是否勾选
        /// </summary>
        [ProtoMember(5,Name = "checked")]
        public bool Checked { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        [ProtoMember(6,Name = "children")]
        public IEnumerable<TreeEntity<TKey, TEntity>> Children { get; set; }

        /// <summary>
        /// 实体
        /// </summary>
        [ProtoMember(7,Name = "entity")]
        public TEntity Entity { get; set; }

        /// <summary>
        /// 深度
        /// </summary>
        [ProtoMember(8,Name = "depth")]
        public int Depth { get; set; }
    }
}