namespace EasyNow.Office.Excel
{
    /// <summary>
    /// 单元格合并对象
    /// </summary>
    public class ExcelMerge
    {
        /// <summary>
        /// 合并的开始列(从0开始)
        /// </summary>
        public int FirstColumn { get; set; }

        /// <summary>
        /// 合并的开始行(从0开始)
        /// </summary>
        public int FirstRow { get; set; }

        /// <summary>
        /// 需要合并的列数
        /// </summary>
        public int TotalColumns { get; set; }

        /// <summary>
        /// 需要合并的行数
        /// </summary>
        public int TotalRows { get; set; }
    }
}