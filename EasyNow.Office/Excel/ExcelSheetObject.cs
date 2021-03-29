using System.Collections.Generic;
using EasyNow.Utility.Extensions;
using NPOI.SS.UserModel;

namespace EasyNow.Office.Excel
{
    /// <summary>
    /// Excel的Sheet对象
    /// </summary>
    public class ExcelSheetObject
    {
        /// <summary>
        /// The _npoi cell style dictionary.
        /// </summary>
        private readonly Dictionary<string, ICellStyle> _npoiCellStyleDictionary = new Dictionary<string, ICellStyle>();

        /// <summary>
        /// 列宽数组
        /// </summary>
        public double[] ColumnWidthArray { get; set; }

        /// <summary>
        /// 行对象集合
        /// </summary>
        public IList<ExcelRowObject> ExcelRows { get; set; }

        /// <summary>
        /// 是否显示网格线
        /// </summary>
        public bool GridlinesVisible { get; set; }

        /// <summary>
        /// 列分组集合
        /// </summary>
        public IList<ExcelGroupObject> GroupColumns { get; set; }

        /// <summary>
        /// 行分组集合
        /// </summary>
        public IList<ExcelGroupObject> GroupRows { get; set; }

        /// <summary>
        /// 单元格合并list
        /// </summary>
        public IList<ExcelMerge> Merges { get; set; }

        /// <summary>
        /// Sheet名称
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public ExcelImageObject[] Images { get; set; }

        /// <summary>
        /// 用于解决导出excel时cellstyle声明过多问题
        /// </summary>
        /// <param name="workbook">
        /// </param>
        /// <param name="cellStyleObject">
        /// </param>
        /// <returns>
        /// The <see cref="ICellStyle"/>.
        /// </returns>
        internal ICellStyle GetCellStyle(IWorkbook workbook, ExcelCellStyleObject cellStyleObject)
        {
            var json = cellStyleObject.ToJson();
            if (this._npoiCellStyleDictionary.ContainsKey(json))
            {
                return this._npoiCellStyleDictionary[json];
            }

            var cellStyle = cellStyleObject.GetNpoiCellStyle(workbook);
            this._npoiCellStyleDictionary.Add(json, cellStyle);
            return cellStyle;
        }
    }
}
