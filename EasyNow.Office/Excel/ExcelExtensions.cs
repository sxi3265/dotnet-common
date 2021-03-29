using NPOI.SS.UserModel;

namespace EasyNow.Office.Excel
{
    /// <summary>
    /// The excel extensions.
    /// </summary>
    internal static class ExcelExtensions
    {
        /// <summary>
        /// The get npoi border style.
        /// </summary>
        /// <param name="excelCellBorderType">
        /// The excel cell border type.
        /// </param>
        /// <returns>
        /// The <see cref="BorderStyle"/>.
        /// </returns>
        internal static BorderStyle GetNpoiBorderStyle(this ExcelCellBorderType excelCellBorderType)
        {
            switch (excelCellBorderType)
            {
                case ExcelCellBorderType.None:
                    return BorderStyle.None;
                case ExcelCellBorderType.Thin:
                    return BorderStyle.Thin;
                case ExcelCellBorderType.Medium:
                    return BorderStyle.Medium;
                case ExcelCellBorderType.Dashed:
                    return BorderStyle.Dashed;
                case ExcelCellBorderType.Dotted:
                    return BorderStyle.Dotted;
                case ExcelCellBorderType.Thick:
                    return BorderStyle.Thick;
                case ExcelCellBorderType.Double:
                    return BorderStyle.Double;
                case ExcelCellBorderType.Hair:
                    return BorderStyle.Hair;
                case ExcelCellBorderType.MediumDashed:
                    return BorderStyle.MediumDashed;
                case ExcelCellBorderType.DashDot:
                    return BorderStyle.DashDot;
                case ExcelCellBorderType.MediumDashDot:
                    return BorderStyle.MediumDashDot;
                case ExcelCellBorderType.DashDotDot:
                    return BorderStyle.DashDotDot;
                case ExcelCellBorderType.MediumDashDotDot:
                    return BorderStyle.MediumDashDotDot;
                case ExcelCellBorderType.SlantedDashDot:
                    return BorderStyle.SlantedDashDot;
            }

            return BorderStyle.None;
        }

        /// <summary>
        /// The get npoi horizontal alignment.
        /// </summary>
        /// <param name="excelCellTextAlignmentType">
        /// The excel cell text alignment type.
        /// </param>
        /// <returns>
        /// The <see cref="HorizontalAlignment"/>.
        /// </returns>
        internal static HorizontalAlignment GetNpoiHorizontalAlignment(
            this ExcelCellTextAlignmentType excelCellTextAlignmentType)
        {
            switch (excelCellTextAlignmentType)
            {
                case ExcelCellTextAlignmentType.Center:
                    return HorizontalAlignment.Center;
                case ExcelCellTextAlignmentType.CenterAcross:
                    return HorizontalAlignment.CenterSelection;
                case ExcelCellTextAlignmentType.Distributed:
                    return HorizontalAlignment.Distributed;
                case ExcelCellTextAlignmentType.Fill:
                    return HorizontalAlignment.Fill;
                case ExcelCellTextAlignmentType.General:
                    return HorizontalAlignment.General;
                case ExcelCellTextAlignmentType.Justify:
                    return HorizontalAlignment.Justify;
                case ExcelCellTextAlignmentType.Left:
                    return HorizontalAlignment.Left;
                case ExcelCellTextAlignmentType.Right:
                    return HorizontalAlignment.Right;
            }

            return HorizontalAlignment.General;
        }

        /// <summary>
        /// The get npoi vertical alignment.
        /// </summary>
        /// <param name="excelCellTextVerticalAlignmentType">
        /// The excel cell text vertical alignment type.
        /// </param>
        /// <returns>
        /// The <see cref="VerticalAlignment"/>.
        /// </returns>
        internal static VerticalAlignment GetNpoiVerticalAlignment(
            this ExcelCellTextVerticalAlignmentType excelCellTextVerticalAlignmentType)
        {
            switch (excelCellTextVerticalAlignmentType)
            {
                case ExcelCellTextVerticalAlignmentType.Top:
                    return VerticalAlignment.Top;
                case ExcelCellTextVerticalAlignmentType.Bottom:
                    return VerticalAlignment.Bottom;
                case ExcelCellTextVerticalAlignmentType.Center:
                    return VerticalAlignment.Center;
                case ExcelCellTextVerticalAlignmentType.Distributed:
                    return VerticalAlignment.Distributed;
                case ExcelCellTextVerticalAlignmentType.Justify:
                    return VerticalAlignment.Justify;
            }

            return VerticalAlignment.Center;
        }
    }
}