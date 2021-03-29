namespace EasyNow.Office.Excel
{
    /// <summary>
    /// Excel单元格对象
    /// </summary>
    public class ExcelCellObject
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 单元格样式
        /// </summary>
        public ExcelCellStyleObject Style { get; set; }
    }
}