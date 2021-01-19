using System;
using System.Collections.Generic;


namespace Terminal
{
    class Report
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        void Create(IList<object> processes)
        {
            dynamic excelApp = Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application"));
            excelApp.Workbooks.Add();
            dynamic workSheet = excelApp.ActiveSheet;

            workSheet.Cells[1, "A"] = "Отчёт";

            string[] header = new string[] { "ИД процесса", "Имя", "CPU", "RAM" };

            for (int i = 0; i < header.Length; i++)
            {
                workSheet.Cells[4, alphabet[i].ToString()] = header[i];
            }

            int row = 5;
            foreach (dynamic proc in processes)
            {
                object[] obj = new object[] { proc.GetPID(), proc.Name, $"{ proc.CpuUsage:P1}", $"{proc.RamUsage} МB" };
                int count = obj.Length;
                for (int i = 0; i < count; i++)
                {
                    workSheet.Cells[row, alphabet[i].ToString()] = obj[i].ToString();
                }
                row++;
            }

            for (int i = 1; i <= 5; i++)
            {
                workSheet.Columns[i].AutoFit();
                workSheet.Columns[i].ColumnWidth = workSheet.Columns[i].ColumnWidth + 2;
            }

            workSheet.Range(workSheet.Cells(1, 1), workSheet.Cells(row, 6)).Font.Name = "Arial";
            workSheet.Cells(1, 1).Font.Size = 20;
            workSheet.Cells(1, 1).Font.Bold = true;
            workSheet.Rows(4).Font.Bold = true;
            workSheet.Application.ActiveWindow.SplitRow = 4;
            workSheet.Application.ActiveWindow.FreezePanes = true;
            workSheet.Cells(1, 1).Select();

            excelApp.Visible = true;
        }

        public void DoReport(IList<object> processes)
        {
            Create(processes);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
