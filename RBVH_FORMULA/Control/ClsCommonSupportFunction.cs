using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;

namespace RBVH_FORMULA.Control
{
    class ClsCommonSupportFunction
    {
        public const int MAX_ROW = 10000;
        public const int MAX_COL = 2000;

        // FindRowAll: Find the key where the row located in the specific column
        public static int FindRowAll(Excel.Workbook xlWB, string SheetName, int col, string key, int startRow, ref string log)
        {
            // Check if key is null
            if(key == null)
            {
                log += "\n[Error] The " + key + " have different from null string"; 
                return -1;
            }
            if(startRow > MAX_ROW)
            {
                log += "\n[Error] The start row [" + startRow + "] have to less than " + MAX_ROW; 
                return -2;
            }
            Excel.Worksheet xlWS;
            xlWS = (Excel.Worksheet)xlWB.Worksheets.get_Item(SheetName);
            xlWS.Activate();
            Excel.Range currentFind = null, selectRange = null;
            Excel.Range startRng, endRgn;
            try
            {
                startRng = xlWS.Cells[startRow, col];
                endRgn = xlWS.Cells[MAX_ROW, col];
            }
            catch (Exception exp)
            {
                log += "[Error] " + exp.Message + Environment.NewLine + "Hint: Excel should be .xlsx. The .xls extension musts not allow";
                return -1;
            }

            selectRange = (Excel.Range)xlWS.get_Range(startRng, endRgn);

            //currentFind = selectRange.Find(key, Missing.Value, Excel.XlFindLookIn.xlFormulas, Excel.XlLookAt.xlWhole, Excel.XlSearchOrder.xlByColumns,
            //                                Excel.XlSearchDirection.xlNext, true, false, Missing.Value);
            currentFind = selectRange.Find(key, Missing.Value, Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlWhole, Excel.XlSearchOrder.xlByRows,
                                            Excel.XlSearchDirection.xlNext, true, false, Missing.Value);
            
            
            if (currentFind == null)
            {
                log += "\n[Error] Not found the " + key + Environment.NewLine + "Please check manually";
                return -1;
            }
            //if (currentFind.Row == startRow)
            //{
            //    return startRow;
            //}

            return currentFind.Row;

        }
        // FindColAll: Find the key where the Col located in the specific Row
        public static int FindColAll(Excel.Workbook xlWB, string SheetName, int row, string key, int startCol, ref string log)
        {
            // Check if key is null
            if (key == null)
            {
                log += "\n[Error] The " + key + " have different from null string";
                return -1;
            }
            if (startCol >= MAX_COL)
            {
                log += "\n[Error] The start row [" + startCol + "] have to less than " + MAX_COL;
                return -1;
            }
            Excel.Worksheet xlWS;
            xlWS = (Excel.Worksheet)xlWB.Worksheets.get_Item(SheetName);
            xlWS.Activate();
            Excel.Range currentFind = null, selectRange = null;
            Excel.Range startRng, endRgn;
            try
            {
                startRng = (Excel.Range)xlWS.Cells[row, startCol];
                endRgn = (Excel.Range)xlWS.Cells[row, MAX_COL];
            }
            catch (Exception exp)
            {
                log += "[Error] " + exp.Message + Environment.NewLine + "Hint: Excel should be .xlsx. The .xls extension musts not allow";
                return -1;
            }

            selectRange = (Excel.Range)xlWS.get_Range(startRng, endRgn);
            currentFind = selectRange.Find(key, Missing.Value, Excel.XlFindLookIn.xlFormulas, Excel.XlLookAt.xlWhole, Excel.XlSearchOrder.xlByColumns,
                                            Excel.XlSearchDirection.xlNext, true, false, Missing.Value);
            if (currentFind == null)
            {
                log += "\n[Error]Not found the " + key + Environment.NewLine + "Please check manually";
                return -1;
            }

            return currentFind.Column;

        }

    }
}
