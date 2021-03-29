using System.Drawing;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace EasyNow.Office.Excel
{
    /// <summary>
    /// Excel单元格样式对象
    /// </summary>
    public class ExcelCellStyleObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelCellStyleObject"/> class.
        /// </summary>
        public ExcelCellStyleObject()
        {
            this.Font = new ExcelCellFont();
            this.HorizontalAlignment = ExcelCellTextAlignmentType.General;
            this.VerticalAlignment = ExcelCellTextVerticalAlignmentType.Center;
            this.Borders = new ExcelCellBorderCollection();
        }

        /// <summary>
        /// 背景色
        /// </summary>
        public Color? BackgroundColor { get; set; }

        /// <summary>
        /// 边框
        /// </summary>
        public ExcelCellBorderCollection Borders { get; private set; }

        /// <summary>
        /// 字体
        /// </summary>
        public ExcelCellFont Font { get; private set; }

        /// <summary>
        /// 水平对齐
        /// </summary>
        public ExcelCellTextAlignmentType HorizontalAlignment { get; set; }

        /// <summary>
        /// 文本自动换行
        /// </summary>
        public bool TextWrapped { get; set; }

        /// <summary>
        /// 垂直对齐
        /// </summary>
        public ExcelCellTextVerticalAlignmentType VerticalAlignment { get; set; }

        /// <summary>
        /// 数据格式
        /// </summary>
        public string DataFormat { get; set; }

        /// <summary>
        /// The get npoi cell style.
        /// </summary>
        /// <param name="workbook">
        /// The workbook.
        /// </param>
        /// <returns>
        /// The <see cref="ICellStyle"/>.
        /// </returns>
        internal ICellStyle GetNpoiCellStyle(IWorkbook workbook)
        {
            var style = workbook.CreateCellStyle();
            style.Alignment = this.HorizontalAlignment.GetNpoiHorizontalAlignment();
            style.VerticalAlignment = this.VerticalAlignment.GetNpoiVerticalAlignment();
            style.SetFont(this.Font.GetNpoiFont(workbook));
            style.WrapText = this.TextWrapped;
            this.Borders.TransformFontNpoiBorder(style);
            if (this.BackgroundColor.HasValue)
            {
                style.FillForegroundColor = this.GetXLColour(this.BackgroundColor.Value);
                style.FillPattern = FillPattern.SolidForeground;
            }
            if (!string.IsNullOrEmpty(this.DataFormat))
            {
                style.DataFormat = HSSFDataFormat.GetBuiltinFormat(this.DataFormat);
            }

            return style;
        }

        /// <summary>
        /// The get xl colour.
        /// </summary>
        /// <param name="systemColour">
        /// The system colour.
        /// </param>
        /// <returns>
        /// The <see cref="short"/>.
        /// </returns>
        private short GetXLColour(Color systemColour)
        {
            HSSFPalette XlPalette = new HSSFWorkbook().GetCustomPalette();
            var color = XlPalette.FindColor(systemColour.R, systemColour.G, systemColour.B);
            if (color == null)
            {
                try
                {
                    color = XlPalette.AddColor(systemColour.R, systemColour.G, systemColour.B);
                }
                catch
                {
                    color = XlPalette.FindSimilarColor(systemColour.R, systemColour.G, systemColour.B);
                }
            }

            return color.Indexed;
        }
    }
}