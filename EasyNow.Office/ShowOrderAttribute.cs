using System;

namespace EasyNow.Office
{
    public class ShowOrderAttribute : Attribute
    {
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        public ShowOrderAttribute(int order)
        {
            Order = order;
        }

        public ShowOrderAttribute() { }
    }
}