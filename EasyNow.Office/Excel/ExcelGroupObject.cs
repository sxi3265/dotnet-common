namespace EasyNow.Office.Excel
{
    /// <summary>
    /// 分组对象
    /// </summary>
    public class ExcelGroupObject
    {
        /// <summary>
        /// 第一个索引位置
        /// </summary>
        public int FirstIndex { get; set; }

        /// <summary>
        /// 分组后是否隐藏
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// 最后一个索引位置
        /// </summary>
        public int LastIndex { get; set; }
    }
}