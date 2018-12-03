using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using System.Data;
using System.Collections;
using NPOI.SS.UserModel;


namespace Engine.Infrastructure.Utils
{
    public class ExcelHelper
    {

        /// <summary>
        /// 将DataTable数据转化到内存流
        /// </summary>
        /// <param name="listDataTable"></param>
        /// <param name="widths"></param>
        /// <returns></returns>
        public static MemoryStream Export(List<DataTable> listDataTable, int[] widths)
        {
            //先创建一个流
            MemoryStream ms = new MemoryStream();
            if (listDataTable != null && listDataTable.Count != 0)
            {
                    //新建一个excel
                    HSSFWorkbook workbook = new HSSFWorkbook();
                    //excel样式
                    HSSFCellStyle style = (HSSFCellStyle)workbook.CreateCellStyle();
                    foreach (DataTable dt in listDataTable)
                    {
                        //创建一个sheet
                        ISheet sheet = workbook.CreateSheet(dt.TableName);
                        //给指定sheet的内容设置每列宽度（index从0开始，width1000相当于excel设置的列宽3.81）
                        for (int i = 0; i < widths.Length; i++)
                        {
                            sheet.SetColumnWidth(i, widths[i]);
                        }
                        //在sheet里创建行
                        NPOI.SS.UserModel.IRow row1 = sheet.CreateRow(0);
                        //第一行，列名
                        for (var i = 0; i < dt.Columns.Count; i++)
                        {
                            row1.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                        }
                        for (var r = 0; r < dt.Rows.Count; r++)
                        {
                            var row_r = sheet.CreateRow(r + 1);
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                row_r.CreateCell(i).SetCellValue(dt.Rows[r][i].ToString());
                            }
                        }
                    }
                    //写入流
                    workbook.Write(ms);
                    ms.Flush();
                    return ms;
            }
            return null;
        }

        /// <summary>
        /// 导出列名
        /// </summary>
        private static System.Collections.SortedList _listColumnsName;
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="filePath"></param>
        public static void ExportExcel(DataTable dtSource, string filePath)
        {
            _listColumnsName = new SortedList(new NoSort());
            for (int i = 0; i < dtSource.Columns.Count; i++)
            {
                _listColumnsName.Add(dtSource.Columns[i].ColumnName, dtSource.Columns[i].ColumnName);
            }

            HSSFWorkbook excelWorkbook = CreateExcelFile();
            InsertRow(dtSource, excelWorkbook);
            SaveExcelFile(excelWorkbook, filePath);
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="filePath"></param>
        public static void ExportExcel(DataTable dtSource, Stream excelStream)
        {
            _listColumnsName = new SortedList(new NoSort());
            for (int i = 0; i < dtSource.Columns.Count; i++)
            {
                _listColumnsName.Add(dtSource.Columns[i].ColumnName, dtSource.Columns[i].ColumnName);
            }

            HSSFWorkbook excelWorkbook = CreateExcelFile();
            InsertRow(dtSource, excelWorkbook);
            SaveExcelFile(excelWorkbook, excelStream);
        }
        /// <summary>
        /// 保存Excel文件
        /// </summary>
        /// <param name="excelWorkBook"></param>
        /// <param name="filePath"></param>
        protected static void SaveExcelFile(HSSFWorkbook excelWorkBook, string filePath)
        {
            FileStream file = null;
            try
            {
                string fullPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(fullPath)) { Directory.CreateDirectory(fullPath); }
                file = new FileStream(filePath, FileMode.Create);
                excelWorkBook.Write(file);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }
        /// <summary>
        /// 保存Excel文件
        /// </summary>
        /// <param name="excelWorkBook"></param>
        /// <param name="filePath"></param>
        protected static void SaveExcelFile(HSSFWorkbook excelWorkBook, Stream excelStream)
        {
            try
            {
                excelWorkBook.Write(excelStream);
            }
            finally
            {

            }
        }
        /// <summary>
        /// 创建Excel文件
        /// </summary>
        /// <param name="filePath"></param>
        protected static HSSFWorkbook CreateExcelFile()
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            return hssfworkbook;
        }
        /// <summary>
        /// 创建excel表头
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="excelSheet"></param>
        protected static void CreateHeader(NPOI.SS.UserModel.ISheet excelSheet, HSSFWorkbook excelWorkbook)
        {
            int cellIndex = 0;
            NPOI.SS.UserModel.IRow row1 = excelSheet.CreateRow(0);

            NPOI.SS.UserModel.ICellStyle cellStyle = excelWorkbook.CreateCellStyle();
            cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
            //cellStyle.FillPattern = HSSFCellStyle.SQUARES;
            cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            //循环导出列
            foreach (System.Collections.DictionaryEntry de in _listColumnsName)
            {
                NPOI.SS.UserModel.ICell cell = row1.CreateCell(cellIndex);
                cell.CellStyle = cellStyle;
                string cellName = de.Value.ToString().Trim();
                cell.SetCellValue(cellName);
                excelSheet.SetColumnWidth(cellIndex, 15 * 256);
                cellIndex++;
            }
        }
        /// <summary>
        /// 插入数据行
        /// </summary>
        protected static void InsertRow(DataTable dtSource, HSSFWorkbook excelWorkbook)
        {
            int rowCount = 0;
            int sheetCount = 1;
            NPOI.SS.UserModel.ISheet newsheet = null;

            //循环数据源导出数据集
            newsheet = excelWorkbook.CreateSheet("Sheet" + sheetCount);
            CreateHeader(newsheet, excelWorkbook);

            #region 样式声明
            //格式化显示
            NPOI.SS.UserModel.ICellStyle cellStyle_DateTime = excelWorkbook.CreateCellStyle();
            NPOI.SS.UserModel.IDataFormat format = excelWorkbook.CreateDataFormat();
            cellStyle_DateTime.DataFormat = format.GetFormat("yyyy-mm-dd hh:mm:ss");
            #endregion

            foreach (DataRow dr in dtSource.Rows)
            {
                rowCount++;
                //超出10000条数据 创建新的工作簿
                if (rowCount == 5000)
                {
                    rowCount = 1;
                    sheetCount++;
                    newsheet = excelWorkbook.CreateSheet("Sheet" + sheetCount);
                    CreateHeader(newsheet, excelWorkbook);
                }

                NPOI.SS.UserModel.IRow newRow = newsheet.CreateRow(rowCount);
                InsertCell(dtSource, dr, newRow, newsheet, excelWorkbook, cellStyle_DateTime);
            }
        }
        /// <summary>
        /// 导出数据行
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="drSource"></param>
        /// <param name="currentExcelRow"></param>
        /// <param name="excelSheet"></param>
        /// <param name="excelWorkBook"></param>
        protected static void InsertCell(DataTable dtSource, DataRow drSource, NPOI.SS.UserModel.IRow currentExcelRow, NPOI.SS.UserModel.ISheet excelSheet, HSSFWorkbook excelWorkBook, NPOI.SS.UserModel.ICellStyle cellStyle_DateTime)
        {
            for (int cellIndex = 0; cellIndex < _listColumnsName.Count; cellIndex++)
            {
                //列名称
                string columnsName = _listColumnsName.GetKey(cellIndex).ToString();
                NPOI.SS.UserModel.ICell newCell = null;
                System.Type rowType = drSource[columnsName].GetType();
                string drValue = drSource[columnsName].ToString().Trim();
                switch (rowType.ToString())
                {
                    case "System.String"://字符串类型
                        drValue = drValue.Replace("&", "&");
                        drValue = drValue.Replace(">", ">");
                        drValue = drValue.Replace("<", "<");
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(drValue);
                        break;
                    case "System.DateTime"://日期类型
                        DateTime dateV;
                        DateTime.TryParse(drValue, out dateV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(dateV);

                        //格式化显示
                        newCell.CellStyle = cellStyle_DateTime;

                        break;
                    case "System.Boolean"://布尔型
                        bool boolV = false;
                        bool.TryParse(drValue, out boolV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(boolV);
                        break;
                    case "System.Int16"://整型
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Byte":
                        int intV = 0;
                        int.TryParse(drValue, out intV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(intV.ToString());
                        break;
                    case "System.Decimal"://浮点型
                    case "System.Double":
                        double doubV = 0;
                        double.TryParse(drValue, out doubV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(doubV);
                        break;
                    case "System.DBNull"://空值处理
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue("");
                        break;
                    case "System.Guid"://空值处理
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(drValue);
                        break;
                    default:
                        throw (new Exception(rowType.ToString() + "：类型数据无法处理!"));
                }
            }
        }

        #region 与导入相关
        /// <summary>
        /// 检测上传的文件是否存在指定的列
        /// </summary>
        /// <param name="dt">上传的Excel转化为DataTable后的数据</param>
        /// <param name="importColumnNames">需要检查的列</param>
        /// <returns></returns>
        private static bool CheckColumnIsAccess(DataTable dt, string[] importColumnNames)
        {
            int length = importColumnNames.Length;
            int currLength = 0;
            foreach (string colN in importColumnNames)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    if (column.ColumnName.Trim() == colN)
                    {
                        currLength++;
                        break;
                    }
                }
            }
            return length == currLength;
        }

        /// <summary>
        /// 获取需导入的数据（注：此方法会自动删除fileFullName指向的文件）
        /// </summary>
        /// <param name="fileFullName"></param>
        /// <param name="importMaxRowCount"></param>
        /// <param name="importColumnNames"></param>
        /// <param name="errorMsg"></param>
        /// <param name="notRepeatColumns"></param>
        /// <returns></returns>
        public static DataTable GetWorkbook(string fileFullName, int importMaxRowCount, string[] importColumnNames, out string errorMsg, params string[] notRepeatColumns)
        {
            try
            {
                errorMsg = string.Empty;
                if (!System.IO.File.Exists(fileFullName)) { errorMsg = "未找到可导入的文件"; return null; }
                HSSFWorkbook workbook;
                using (FileStream file = new FileStream(fileFullName, FileMode.Open, FileAccess.Read))
                {
                    workbook = new HSSFWorkbook(file);
                }
                //删除文件
                try { System.IO.File.Delete(fileFullName); }
                catch { }
                #region 获取DataTable
                ISheet sheet = workbook.GetSheetAt(0);
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                DataTable dt = new DataTable();
                if (rows.MoveNext())
                {
                    HSSFRow row = (HSSFRow)rows.Current;
                    for (int i = 0; i < row.LastCellNum; i++)
                    {
                        dt.Columns.Add(row.GetCell(i).ToString());
                    }
                }
                while (rows.MoveNext())
                {
                    HSSFRow row = (HSSFRow)rows.Current;
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < row.LastCellNum; i++)
                    {
                        ICell cell = row.GetCell(i);
                        if (cell == null) { dr[i] = null; }
                        else { dr[i] = cell.ToString(); }
                    }
                    dt.Rows.Add(dr);
                }
                #endregion

                if (dt == null)
                {
                    errorMsg = "系统检查发现您更改过excel模板文件或选择了不符合规则的文件\r\n" +
                            "故您此次上传的文件无效，请还原后重新添加上传。";
                    return null;
                }
                else if (dt.Rows.Count <= 0)
                {
                    errorMsg = "您选择的Excel文件没有任何数据可供导入!";
                    return null;
                }
                else if (importColumnNames != null && !CheckColumnIsAccess(dt, importColumnNames))
                {
                    errorMsg = "系统检查发现您更改过excel模板文件或选择了不符合规则的文件\r\n" +
                            "故您此次上传的文件无效，请还原后重新添加上传。";
                }
                else if (importMaxRowCount != 0 && dt.Rows.Count > importMaxRowCount)
                {
                    errorMsg = string.Format("每次操作的数据不可超过{0}行！请自行拆分！", importMaxRowCount);
                }
                else if (notRepeatColumns != null)
                {
                    int rowCount = dt.Rows.Count;
                    List<string> listColumn = new List<string>();
                    //检测上传的excel中是否有重复的数据
                    foreach (string column in notRepeatColumns)
                    {
                        listColumn.Clear();
                        int rowIndex = 2;
                        string errorIndex = string.Empty;
                        foreach (DataRow item in dt.Rows)
                        {
                            string v = item[column].ToString();
                            if (listColumn.Contains(v))
                            {
                                errorIndex += errorIndex != string.Empty ? "," + rowIndex.ToString() : rowIndex.ToString();
                            }
                            listColumn.Add(v);
                            rowIndex++;
                        }
                        if (errorIndex != string.Empty)
                        {
                            errorMsg += "列\"" + column + "\"，第\"" + errorIndex + "\"行存在重复数据；\r\n";
                        }
                    }
                }
                return dt;
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
                return null;
            }
        }
        #endregion
    }
    //排序实现接口 不进行排序 根据添加顺序导出
    public class NoSort : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            return -1;
        }

    }
}
