using System.Collections.Generic;

namespace EasyNow.Office.Excel
{
    /// <summary>
    /// Excel行对象
    /// </summary>
    public class ExcelRowObject
    {
        /// <summary>
        /// 该行的单元格集合
        /// </summary>
        public IList<ExcelCellObject> ExcelCells { get; set; }

        /// <summary>
        /// 行高
        /// </summary>
        public double? RowHeight { get; set; }
    }
}