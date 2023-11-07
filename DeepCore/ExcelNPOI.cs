using System;
using System.Collections.Generic;
using System.Linq;
using System.IO; 
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel; 
using System.Data;
using Newtonsoft.Json.Linq;
using System.Windows.Documents;

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
                xlsRow.CreateCell(colIndex++).SetCellValue(item.Team.Name);
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

        public static string LoadJson(string file)
        { 
            var list = new JArray() as dynamic;

            using (FileStream fstream = new FileStream(file, FileMode.Open))
            {
                IWorkbook wbook = new XSSFWorkbook(fstream);

                //only 1 sheet
                ISheet sheet = wbook.GetSheetAt(0);

                var header = sheet.GetRow(0).Cells;

                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);

                    if (row == null) continue;

                    dynamic obj = new JObject();

                    for (int j = 0; j < header.Count; j++)
                    {
                        ICell cell = row.GetCell(j);

                        if (cell != null)
                        {
                            obj.Add(header[j].ToString(), cell.ToString());
                        }
                    } 

                    list.Add(obj);
                }
            }

            return list.ToString();
        }
    }
}
