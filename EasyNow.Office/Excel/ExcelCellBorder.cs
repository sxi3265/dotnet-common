namespace EasyNow.Office.Excel
{
    /// <summary>
    /// Excel单元格边框
    /// </summary>
    public class ExcelCellBorder
    {
        /// <summary>
        /// The _excel cell border collection.
        /// </summary>
        private ExcelCellBorderCollection _excelCellBorderCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelCellBorder"/> class.
        /// </summary>
        /// <param name="excelCellBorderCollection">
        /// The excel cell border collection.
        /// </param>
        /// <param name="excelBorderType">
        /// The excel border type.
        /// </param>
        public ExcelCellBorder(ExcelCellBorderCollection excelCellBorderCollection, ExcelBorderType excelBorderType)
        {
            this._excelCellBorderCollection = excelCellBorderCollection;
            this.ExcelBorderType = excelBorderType;
        }

        /// <summary>
        /// Gets or sets the excel border type.
        /// </summary>
        public ExcelBorderType ExcelBorderType { get; set; }

        /// <summary>
        /// 线条样式
        /// </summary>
        public ExcelCellBorderType LineStyle { get; set; }
    }
}