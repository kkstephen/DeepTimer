using System;
using System.Collections.Generic;
using System.Linq;
using System.IO; 
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using Excel;
using System.Data;

namespace DeepCore
{
    public class ExcelNPOI
    {
        private IList<DeepLap> data;

        public ExcelNPOI(IList<DeepLap> cars)
        {
            this.data = cars;
        }

        public void Save(string file)
        {
            string[] cols = new string[]
            {
                 "Id",
                 "Racer",                 
                 "Time",
                 "Ticks",
                 "Lap",
                 "Log On",
                 "Status"
            };

            // excel 2013 XLSX
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("AWS");

            ushort rowIndex = 0;
            ushort colIndex = 0;

            IRow headerRow = sheet.CreateRow(rowIndex);

            //set cloumn header
            foreach (string col_name in cols)
            {
                headerRow.CreateCell(colIndex++).SetCellValue(col_name);
            }

            foreach (var item in this.data)
            {
                rowIndex++;
                colIndex = 0; 

                IRow xlsRow = sheet.CreateRow(rowIndex);

                xlsRow.CreateCell(colIndex++).SetCellValue(rowIndex);
                xlsRow.CreateCell(colIndex++).SetCellValue(item.Team);
                xlsRow.CreateCell(colIndex++).SetCellValue(item.Record.ToTimespan());
                xlsRow.CreateCell(colIndex++).SetCellValue(item.Record);
                xlsRow.CreateCell(colIndex++).SetCellValue(item.Lap);
                xlsRow.CreateCell(colIndex++).SetCellValue(item.Date.ToString());
                xlsRow.CreateCell(colIndex++).SetCellValue(item.Status);

            }

            using (FileStream stream = new FileStream(file, FileMode.CreateNew))
            {
                workbook.Write(stream);
            }
        }

        public static DataTable ImportXls(string filepath)
        {
            if (filepath.Length == 0)
                return null;

            DataSet result = null;

            using (FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelReader;

                if (filepath.IndexOf(".xlsx") != -1)
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                else
                    excelReader = ExcelReaderFactory.CreateBinaryReader(fs);

                excelReader.IsFirstRowAsColumnNames = true;

                result = excelReader.AsDataSet();

                excelReader.Close();
            }

            return result.Tables[0];
        }
    }
}
