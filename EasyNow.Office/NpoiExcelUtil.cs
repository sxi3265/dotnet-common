using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EasyNow.Office.Csv;
using EasyNow.Office.Excel;
using EasyNow.Utility.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace EasyNow.Office
{
    internal class NpoiExcelUtil : IExcelUtil
    {
        private readonly IServiceProvider _serviceProvider;

        public NpoiExcelUtil(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private IWorkbook GetWorkbook(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                try
                {
                    stream.Position = 0;
                    stream.CopyTo(ms);
                    stream.Position = 0;
                    return new HSSFWorkbook(stream);
                }
                catch
                {
                    ms.Position = 0;
                    return new XSSFWorkbook(ms);
                }
            }
        }

        public Stream FillRowDataWithTemplate(Stream templateStream, TemplateExcelCellObject[][] data, string placeHolderTemplate=@"^\$\{code}$")
        {
            var dic = new Dictionary<string,ExcelCellObject[]>();
            for (var i = 0; i < data.Length; i++)
            {
                for (var j = 0; j < data[i].Length; j++)
                {
                    var cellObject = data[i][j];
                    if (!dic.ContainsKey(cellObject.Code))
                    {
                        dic.Add(cellObject.Code,new ExcelCellObject[data.Length]);
                    }

                    var cells = dic[cellObject.Code];
                    cells[i] = cellObject;
                }
            }

            return FillColumnDataWithTemplate(templateStream,dic,placeHolderTemplate);
        }

        public Stream FillColumnDataWithTemplate(Stream templateStream, Dictionary<string, ExcelCellObject[]> data,
            string placeHolderTemplate=@"^\$\{code}$")
        {
            var regex = new Regex(placeHolderTemplate.Replace("code","(?<code>.*?)"));
            var workbook = GetWorkbook(templateStream);
            var sheet = workbook.GetSheetAt(0);
            var indexDic = new Dictionary<string,Tuple<int,int,ICellStyle>>();

            // 查找占位符位置
            for (var i = 0; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                for (var j = 0; j < row.LastCellNum; j++)
                {
                    var cell = row.GetCell(j);
                    if (cell != null)
                    {
                        var cellValue = cell.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(cellValue) &&!indexDic.ContainsKey(cellValue))
                        {
                            var match = regex.Match(cellValue);
                            if (match.Success)
                            {
                                indexDic.Add(match.Groups["code"].Value,new Tuple<int, int,ICellStyle>(i,j,cell.CellStyle));
                                cell.SetCellValue(string.Empty);
                            }
                        }
                    }
                }
            }

            var sheetObject=new ExcelSheetObject();
            foreach (var key in indexDic.Keys)
            {
                var index = indexDic[key];
                var merged=false;
                CellRangeAddress merge=null;
                for(var n = 0; n < sheet.NumMergedRegions; n++)
                {
                    merge = sheet.GetMergedRegion(n);
                    if (merge.FirstRow == index.Item1 && merge.FirstColumn == index.Item2 && merge.LastRow == merge.FirstRow)
                    {
                        merged=true;
                        break;
                    }
                }

                if (!data.ContainsKey(key))
                {
                    continue;
                }
                var cells = data[key];
                for (var i = 0; i < cells.Count(); i++)
                {
                    if (cells[i] == null)
                    {
                        // 如果为空表示不做填充
                        continue;
                    }
                    var rowNumber = index.Item1 + i;
                    var row = sheet.GetRow(rowNumber) ?? sheet.CreateRow(rowNumber);

                    var cell = row.GetCell(index.Item2) ?? row.CreateCell(index.Item2);
                    
                    
                        cell.SetCellValue(cells[i].Content??string.Empty);
                        if (cells[i].Style != null)
                        {
                            cell.CellStyle = sheetObject.GetCellStyle(workbook, cells[i].Style);
                        }
                        else
                        {
                            // 复制单元格样式
                            cell.CellStyle=index.Item3;
                        }
                    
                    // 如果模板中的单元格有合并，此处也复制合并
                    if (merged)
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(rowNumber,rowNumber,index.Item2,merge.LastColumn));
                        for(var n = index.Item2; n <= merge.LastColumn; n++)
                        {
                            var cell1 = row.GetCell(n)??row.CreateCell(n);
                            if(cells[i] != null)
                            {
                                if(cells[i].Style != null)
                                {
                                    cell1.CellStyle=sheetObject.GetCellStyle(workbook,cells[i].Style);
                                }
                                else
                                {
                                    // 复制单元格样式
                                    var rowTmp = sheet.GetRow(index.Item1);
                                    var cellTmp = rowTmp.GetCell(n);
                                    cell1.CellStyle = cellTmp != null ? cellTmp.CellStyle : index.Item3;
                                }
                            }
                        }
                    }
                }
                
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public DataTable ExportDataTableFromStream(Stream stream, int firstRow, int firstColumn, bool exportColumnName)
        {
            var workbook = GetWorkbook(stream);

            var sheet = workbook.GetSheetAt(0);

            var dataTable = new DataTable();
            var trow = sheet.GetRow(0);
            for (var j = firstColumn; j < trow.LastCellNum; j++)
            {
                var cell = trow.GetCell(j);
                if (exportColumnName && cell != null)
                {
                    dataTable.Columns.Add(this.GetCellValue(cell));
                }
                else
                {
                    dataTable.Columns.Add(new DataColumn());
                }
            }

            if (exportColumnName)
            {
                firstRow += 1;
            }

            for (var i = firstRow; i <= sheet.LastRowNum; i++)
            {
                var dataRow = dataTable.NewRow();
                var row = sheet.GetRow(i);
                if (row != null)
                {
                    for (var j = firstColumn; j < dataTable.Columns.Count; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell != null)
                        {
                            dataRow[j] = this.GetCellValue(cell);
                        }
                    }
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        public DataTable ExportDataTableFromFile(string filepath, int firstRow, int firstColumn, bool exportColumnName)
        {
            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                return this.ExportDataTableFromStream(fs, firstRow, firstColumn, exportColumnName);
            }
        }

        public void ExportExcel<T>(ICollection<T> results, string filePath, bool autoSizeColumn) where T : class
        {
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                ExportExcel(results, fs, autoSizeColumn);
            }
        }

        public void ExportExcel<T>(ICollection<T> results, Stream stream, bool autoSizeColumn) where T : class
        {
            var book = this.CreateWorkbook(results, autoSizeColumn);
            book.Write(stream);
        }

        public void ExportExcel(ExcelSheetObject sheetObject, ExcelSaveFormat excelSaveFormat, Stream stream)
        {
            if (excelSaveFormat == ExcelSaveFormat.Csv)
            {
                var sb = new StringBuilder();
                if (sheetObject.ExcelRows != null)
                {
                    foreach (var row in sheetObject.ExcelRows)
                    {
                        if (row.ExcelCells != null)
                        {
                            foreach (var cell in row.ExcelCells)
                            {
                                var content = cell.Content??string.Empty;
                                
                                // 如果存在逗号，在整段内容用双引号包起来
                                if (content.Contains(","))
                                {
                                    // 如果还存在双引号，则在该双引号之前再加一个双引号
                                    if (content.Contains("\""))
                                    {
                                        content = content.Replace("\"", "\"\"");
                                    }
                                    
                                    content = $"\"{content}\"";
                                }

                                sb.Append(content);
                                sb.Append(",");
                            }
                        }

                        sb.Append("\n");
                    }
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                stream.Write(bytes, 0, bytes.Length);
            }
            else
            {
                var workbook = excelSaveFormat == ExcelSaveFormat.Excel97To2003
                                         ? (IWorkbook)new HSSFWorkbook()
                                         : new XSSFWorkbook();
                var sheet = !string.IsNullOrEmpty(sheetObject.SheetName) ? workbook.CreateSheet(sheetObject.SheetName) : workbook.CreateSheet();

                // 显示网格线
                sheet.DisplayGridlines = sheetObject.GridlinesVisible;

                if (sheetObject.GroupRows != null)
                {
                    foreach (var row in sheetObject.GroupRows)
                    {
                        sheet.GroupRow(row.FirstIndex, row.LastIndex);
                        sheet.SetRowGroupCollapsed(row.FirstIndex, row.IsHidden);
                    }
                }

                if (sheetObject.GroupColumns != null)
                {
                    foreach (var column in sheetObject.GroupColumns)
                    {
                        sheet.GroupColumn(column.FirstIndex, column.LastIndex);
                        sheet.SetColumnGroupCollapsed(column.FirstIndex, column.IsHidden);
                    }
                }

                if (sheetObject.ExcelRows != null)
                {
                    for (var i = 0; i < sheetObject.ExcelRows.Count; i++)
                    {
                        var row = sheetObject.ExcelRows[i];
                        var sheetRow = sheet.CreateRow(i);

                        // 设置行高
                        if (row.RowHeight.HasValue)
                        {
                            sheetRow.Height = (short)(row.RowHeight.Value * 20);
                        }

                        if (row.ExcelCells == null) continue;
                        for (var j = 0; j < row.ExcelCells.Count; j++)
                        {
                            var cell = row.ExcelCells[j];
                            var sheetCell = sheetRow.CreateCell(j);
                            sheetCell.SetCellValue(cell.Content);
                            if (cell.Style != null)
                            {
                                sheetCell.CellStyle = sheetObject.GetCellStyle(workbook, cell.Style);
                            }
                        }
                    }
                }

                sheetObject.Merges?.ToList()
                    .ForEach(
                        e =>
                        {
                            sheet.AddMergedRegion(
                                new CellRangeAddress(
                                    e.FirstRow,
                                    e.FirstRow + e.TotalRows - 1,
                                    e.FirstColumn,
                                    e.FirstColumn + e.TotalColumns - 1));
                        });

                if (sheetObject.ColumnWidthArray != null)
                {
                    for (var i = 0; i < sheetObject.ColumnWidthArray.GetLength(0); i++)
                    {
                        sheet.SetColumnWidth(i, (int)sheetObject.ColumnWidthArray[i] * 256);
                    }
                }

                if (sheetObject.Images != null && sheetObject.Images.Any())
                {
                    var patriarch = sheet.CreateDrawingPatriarch();
                    foreach (var image in sheetObject.Images)
                    {
                        if (!image.Anchors.Any())
                        {
                            continue;
                        }
                        var picIndex = workbook.AddPicture(image.Data, image.GetPictureType());
                        image.Anchors.Foreach(e =>
                        {
                            patriarch.CreatePicture(
                                patriarch.CreateAnchor(e.LeftX, e.TopY, e.RightX, e.BottomY, e.LeftTopCell.X,
                                    e.LeftTopCell.Y, e.RightBottomCell.X, e.RightBottomCell.Y), picIndex);
                        });
                    }
                }

                workbook.Write(stream);
            }

            stream.Position = 0;
            stream.Flush();
        }

        public void ExportPDF<T>(ICollection<T> dbResults, Stream stream) where T : class
        {
            throw new NotImplementedException();
        }

        public DateTime GetDate(double date)
        {
            return DateUtil.GetJavaDate(date);
        }

        public IList<T> ImportExcelFromFiles<T>(string filepath)
        {
            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                return this.ImportExcel<T>(fs);
            }
        }

        public IList<T> ImportExcelFromFiles<T>(Stream stream)
        {
            return this.ImportExcel<T>(stream);
        }

        public IList<T> ImportCsv<T>(string filepath)
        {
            return ImportCsv<T>(filepath, Encoding.UTF8);
        }

        public IList<T> ImportCsv<T>(string filepath, Encoding encoding)
        {
            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                return this.ImportCsv<T>(fs,encoding);
            }
        }

        public IList<T> ImportCsv<T>(Stream stream)
        {
            return ImportCsv<T>(stream, Encoding.UTF8);
        }

        public DataTable ExportDataTableFromCsvStream(Stream stream, int firstRow, int firstColumn,
            bool exportColumnName, Encoding encoding)
        {
            var dataTable = new DataTable();
            var arr = ImportCsv(stream, encoding);
            // 只有列头或者没有任何行，说明无数据
            if (arr.Length < 2)
            {
                return dataTable;
            }
            var nameDic = new Dictionary<string,int>();
            foreach (var s in arr[0])
            {
                if (nameDic.ContainsKey(s))
                {
                    nameDic[s] += 1;
                    dataTable.Columns.Add($"{s}{nameDic[s]}");
                }
                else
                {
                    dataTable.Columns.Add(s);
                    nameDic.Add(s,0);
                }
            }

            if (exportColumnName)
            {
                firstRow += 1;
            }

            for (var i = firstRow; i < arr.Length; i++)
            {
                var dr = dataTable.NewRow();
                for (var j = firstColumn; j < dataTable.Columns.Count; j++)
                {
                    dr[j] = arr[i][j];
                }
                dataTable.Rows.Add(dr);
            }

            return dataTable;
        }

        public IList<T> ImportCsv<T>(Stream stream, Encoding encoding)
        {
            var result = new List<T>();
            var arr = ImportCsv(stream, encoding);
            
            // 只有列头或者没有任何行，说明无数据
            if (arr.Length < 2)
            {
                return result;
            }

            var type = typeof(T);
            var localizer = _serviceProvider.GetService<IStringLocalizerFactory>()?.Create(type);

            var properties =
                type.GetProperties()
                    .Select(
                        p =>
                            new
                            {
                                PropertyName = p.Name,
                                Name = p.GetCustomAttributes(typeof(DisplayAttribute), false)
                                    .Cast<DisplayAttribute>()
                                    .Select(e =>
                                        localizer != null && !string.IsNullOrEmpty(e.Name) && e.ResourceType == null
                                            ? localizer[e.Name]
                                            : e.GetName())
                                    .FirstOrDefault(),
                                Property = p
                            })
                    .ToArray();

            // 找出列头对应的属性
            var propArr = new PropertyInfo[arr[0].Length];
            for (var i = 0; i < arr[0].Length; i++)
            {
                var pi = properties.Where(e => e.Name == arr[0][i]).Select(e=>e.Property).FirstOrDefault();
                if (pi == null)
                {
                    pi=properties.Where(e => e.PropertyName == arr[0][i]).Select(e=>e.Property).FirstOrDefault();
                }

                if (pi != null)
                {
                    propArr[i] = pi;
                }
            }

            // 构造对象，并给属性赋值
            for (var i = 1; i < arr.Length; i++)
            {
                var item = Activator.CreateInstance<T>();
                for (var j = 0; j < propArr.Length; j++)
                {
                    var pi = propArr[j];
                    if (pi == null) continue;
                    var propType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                    pi.SetValue(item,Convert.ChangeType(arr[i][j], propType));
                }
                result.Add(item);
            }

            return result;
        }

        public string[][] ImportCsv(Stream stream, Encoding encoding)
        {
            var result = new List<string[]>();
            using (var csvReader = new CsvTextFieldParser(stream,encoding))
            {
                if (csvReader.EndOfData)
                {
                    return result.ToArray();
                }
                csvReader.Delimiters=new [] { "," };
                csvReader.HasFieldsEnclosedInQuotes = true;
                do
                {
                    result.Add(csvReader.ReadFields());
                } while (!csvReader.EndOfData);
            }

            return result.ToArray();
        }

        private string GetCellValue(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                // 公式
                case CellType.Formula:
                    return cell.NumericCellValue.ToString();
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    return cell.NumericCellValue.ToString();
                default:
                    return string.Empty;
            }
        }

        private HSSFWorkbook CreateWorkbook<T>(ICollection<T> results, bool autoSizeColumn) where T : class
        {
            var book = new HSSFWorkbook();
            var sheet = book.CreateSheet("Data");

            var type = typeof(T);
            var localizer = _serviceProvider.GetService<IStringLocalizerFactory>()?.Create(type);

            var properties =
                type.GetProperties()
                    .Where(p => p.IsDefined(typeof(DisplayAttribute), false))
                    .OrderBy(p =>
                        (p.GetCustomAttributes<ShowOrderAttribute>(false).FirstOrDefault() ??
                         new ShowOrderAttribute(int.MaxValue)).Order).ThenBy(p => p.Name)
                    .Select(
                        p =>
                        {
                            var attr = p.GetCustomAttributes(typeof(DisplayAttribute), false)
                                .Cast<DisplayAttribute>()
                                .Single();
                            return new
                            {
                                PropertyName = p.Name,
                                Name = localizer != null && !string.IsNullOrEmpty(attr.Name) &&
                                       attr.ResourceType == null
                                    ? localizer[attr.Name]
                                    : attr.GetName(),
                                Property = p
                            };
                        })
                    .ToArray();
            var tableHeadName = new List<string>();
            var row = sheet.CreateRow(0);
            for (var i = 0; i < properties.Length; i++)
            {
                var cc = properties[i];
                tableHeadName.Add(!string.IsNullOrEmpty(cc.Name)
                    ? Convert.ToString(cc.Name)
                    : Convert.ToString(cc.PropertyName));

                var cell = row.CreateCell(i);
                cell.SetCellValue(tableHeadName[i]);
            }

            var data = results.ToArray();
            for (var i = 0; i < data.Length; i++)
            {
                row = sheet.CreateRow(i + 1);
                for (var j = 0; j < properties.Count(); j++)
                {
                    var cell = row.CreateCell(j);
                    var obj = properties[j].Property.GetValue(data[i]);
                    if (obj == null)
                    {
                        continue;
                    }
                    var propertyType = properties[j].Property.PropertyType;
                    if (propertyType.IsNullableType())
                    {
                        propertyType = propertyType.GetTypeOfNullable();
                    }

                    cell.SetCellValue(propertyType == typeof(DateTime)
                        ? ((DateTime) obj).ToString("yyyy-MM-dd HH:mm:ss")
                        : obj.ToString());
                }
            }

            if (!autoSizeColumn) return book;
            {
                // Auto-fit all the columns
                for (var i = 0; i < tableHeadName.Count; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
            }

            return book;
        }

        private IList<T> ImportExcel<T>(Stream stream)
        {
            var dataTable = this.ExportDataTableFromStream(stream, 0, 0, true);
            return this.TableToList<T>(dataTable);
        }

        private IList<T> TableToList<T>(DataTable table)
        {
            var result = new List<T>();
            
            var type = typeof(T);
            var localizer = _serviceProvider.GetService<IStringLocalizerFactory>()?.Create(type);
            var properties = type.GetProperties();
            foreach (DataRow row in table.Rows)
            {
                var item = Activator.CreateInstance<T>();
                foreach (DataColumn column in table.Columns)
                {
                    var query =properties.Where(p => p.IsDefined(typeof(DisplayAttribute), false))
                            .Select(
                                p =>
                                {
                                    var attr = p.GetCustomAttributes(typeof(DisplayAttribute), false)
                                        .Cast<DisplayAttribute>()
                                        .Single();
                                    return new
                                    {
                                        PropertyName = p.Name,
                                        Name = localizer != null && !string.IsNullOrEmpty(attr.Name) &&
                                               attr.ResourceType == null
                                            ? localizer[attr.Name]
                                            : attr.GetName()
                                    };
                                })
                            .Where(c => c.Name == column.ColumnName);

                    var pi = typeof(T).GetProperty(query.Any() ? query.FirstOrDefault().PropertyName : column.ColumnName);

                    if (pi != null && row[column] != DBNull.Value)
                    {
                        var propType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                        pi.SetValue(item, Convert.ChangeType(row[column], propType), new object[0]);
                    }
                }

                result.Add(item);
            }

            return result;
        }
    }
}