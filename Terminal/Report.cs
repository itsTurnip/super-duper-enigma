using System;


namespace Terminal
{   
    class Report
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        void Create(object[][] array)
        {
            dynamic excelApp = Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application"));
            excelApp.Workbooks.Add();
            dynamic workSheet = excelApp.ActiveSheet;

            workSheet.Cells[1, "A"] = "Отчёт";

            string[] header = new string[] { "ИД процесса", "Имя", "RAM", "CPU" };

            for (int i = 0; i < header.Length; i++)
            {
                workSheet.Cells[4, alphabet[i].ToString()] = header[i];
            }

            int row = 5;
            foreach (object[] proc in array)
            {
                for (int i = 0; i < proc.Length; i++)
                {
                    for (int j = 0; j < proc.Length; j++)
                    {
                        workSheet.Cells[row, alphabet[i].ToString()] = proc[j].ToString();
                    }
                }
            }

            for (int i = 1; i <= 5; i++)
            {
                workSheet.Columns[i].AutoFit();
                workSheet.Columns[i].ColumnWidth = workSheet.Columns[i].ColumnWidth + 2;
            }

            workSheet.Range(workSheet.Cells(1, 1), workSheet.Cells(row, 6)).Font.Name = "Arial";
            workSheet.Rows(4).Font.Bold = true;
            workSheet.Application.ActiveWindow.SplitRow = 4;
            workSheet.Application.ActiveWindow.FreezePanes = true;
            workSheet.Cells(1, 1).Select();

            excelApp.Visible = true;
        }

        void DoReport(object[][] array)
        {
            Create(array);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect(); 
            GC.WaitForPendingFinalizers();
        }


    }
}
