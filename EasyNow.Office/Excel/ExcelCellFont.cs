using System.Drawing;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace EasyNow.Office.Excel
{
    /// <summary>
    /// Excel单元格字体
    /// </summary>
    public class ExcelCellFont
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelCellFont"/> class.
        /// </summary>
        public ExcelCellFont()
        {
            this.Size = 10;
        }

        /// <summary>
        /// 加粗
        /// </summary>
        public bool Bold { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The get npoi font.
        /// </summary>
        /// <param name="workbook">
        /// The workbook.
        /// </param>
        /// <returns>
        /// The <see cref="IFont"/>.
        /// </returns>
        internal IFont GetNpoiFont(IWorkbook workbook)
        {
            var font = workbook.CreateFont();
            if (!string.IsNullOrEmpty(this.Name))
            {
                font.FontName = this.Name;
            }

            font.FontHeightInPoints = (short)this.Size;
            font.Boldweight = (short)(this.Bold ? FontBoldWeight.Bold : FontBoldWeight.None);
            if (this.Color.HasValue)
            {
                font.Color = this.GetXLColour(this.Color.Value);
            }

            return font;
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
                color = XlPalette.AddColor(systemColour.R, systemColour.G, systemColour.B);
            }

            return color.Indexed;
        }
    }
}