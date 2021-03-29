using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using EasyNow.Office.Excel;

namespace EasyNow.Office
{
    public interface IExcelUtil
    {
        /// <summary>
        /// 根据模板填充行数据
        /// </summary>
        /// <param name="templateStream"></param>
        /// <param name="data"></param>
        /// <param name="placeHolderTemplate"></param>
        /// <returns></returns>
        Stream FillRowDataWithTemplate(Stream templateStream, TemplateExcelCellObject[][] data,string placeHolderTemplate=@"^\$\{code}$");

        /// <summary>
        /// 根据模板填充列数据
        /// </summary>
        /// <param name="templateStream"></param>
        /// <param name="data"></param>
        /// <param name="placeHolderTemplate"></param>
        /// <returns></returns>
        Stream FillColumnDataWithTemplate(Stream templateStream, Dictionary<string, ExcelCellObject[]> data,
            string placeHolderTemplate=@"^\$\{code}$");

        DataTable ExportDataTableFromStream(Stream stream, int firstRow, int firstColumn, bool exportColumnName);

        DataTable ExportDataTableFromFile(string filepath, int firstRow, int firstColumn, bool exportColumnName);

        void ExportExcel<T>(ICollection<T> results, string filePath, bool autoSizeColumn) where T : class;

        void ExportExcel<T>(ICollection<T> results, Stream stream, bool autoSizeColumn) where T : class;

        void ExportExcel(ExcelSheetObject sheetObject, ExcelSaveFormat excelSaveFormat, Stream stream);

        void ExportPDF<T>(ICollection<T> dbResults, Stream stream) where T : class;

        DateTime GetDate(double date);

        IList<T> ImportExcelFromFiles<T>(string filepath);

        IList<T> ImportExcelFromFiles<T>(Stream stream);

        IList<T> ImportCsv<T>(string filepath);
        
        IList<T> ImportCsv<T>(string filepath, Encoding encoding);

        IList<T> ImportCsv<T>(Stream stream);
        
        IList<T> ImportCsv<T>(Stream stream, Encoding encoding);

        string[][] ImportCsv(Stream stream, Encoding encoding);

        DataTable ExportDataTableFromCsvStream(Stream stream, int firstRow, int firstColumn,
            bool exportColumnName, Encoding encoding);
    }
}