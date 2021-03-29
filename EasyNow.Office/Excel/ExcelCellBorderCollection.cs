using NPOI.SS.UserModel;

namespace EasyNow.Office.Excel
{
    /// <summary>
    /// The excel cell border collection.
    /// </summary>
    public class ExcelCellBorderCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelCellBorderCollection"/> class.
        /// </summary>
        public ExcelCellBorderCollection()
        {
            this.BorderList = new[]
            {
                new ExcelCellBorder(this, ExcelBorderType.BottomBorder),
                new ExcelCellBorder(this, ExcelBorderType.DiagonalDown),
                new ExcelCellBorder(this, ExcelBorderType.DiagonalUp),
                new ExcelCellBorder(this, ExcelBorderType.LeftBorder),
                new ExcelCellBorder(this, ExcelBorderType.RightBorder),
                new ExcelCellBorder(this, ExcelBorderType.TopBorder), null, null
            };
        }

        /// <summary>
        /// Gets or sets the border list.
        /// </summary>
        public ExcelCellBorder[] BorderList { get; set; }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="excelBorderType">
        /// The excel border type.
        /// </param>
        /// <returns>
        /// The <see cref="ExcelCellBorder"/>.
        /// </returns>
        public ExcelCellBorder this[ExcelBorderType excelBorderType]
        {
            get
            {
                switch (excelBorderType)
                {
                    case ExcelBorderType.LeftBorder:
                        return this.BorderList[3];
                    case ExcelBorderType.RightBorder:
                        return this.BorderList[4];
                    case ExcelBorderType.TopBorder:
                        return this.BorderList[5];
                    case ExcelBorderType.BottomBorder:
                        return this.BorderList[0];
                    case ExcelBorderType.DiagonalDown:
                        return this.BorderList[1];
                    case ExcelBorderType.DiagonalUp:
                        return this.BorderList[2];
                    case ExcelBorderType.Horizontal:
                        return this.BorderList[6]
                               ?? (this.BorderList[6] = new ExcelCellBorder(this, ExcelBorderType.Horizontal));
                    case ExcelBorderType.Vertical:
                        return this.BorderList[7]
                               ?? (this.BorderList[7] = new ExcelCellBorder(this, ExcelBorderType.Vertical));
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// The transform font npoi border.
        /// </summary>
        /// <param name="style">
        /// The style.
        /// </param>
        internal void TransformFontNpoiBorder(ICellStyle style)
        {
            style.BorderBottom = this.BorderList[0].LineStyle.GetNpoiBorderStyle();
            style.BorderLeft = this.BorderList[3].LineStyle.GetNpoiBorderStyle();
            style.BorderRight = this.BorderList[4].LineStyle.GetNpoiBorderStyle();
            style.BorderTop = this.BorderList[5].LineStyle.GetNpoiBorderStyle();
        }
    }
}