using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RBVH_FORMULA.Control
{
    class FormulaProcess
    {        
        static string CurrentPath = Directory.GetCurrentDirectory();
        static string strSavetFileNew = CurrentPath + @"\ELOC.c";

        RBVH_FORMULA.Model.GlobalVariableHandling.OutComeValue[] g_GlobalVariable;
        
        RBVH_FORMULA.View.DisplayLog g_View = new View.DisplayLog();
                     
        ///
        ///
        ///
        public bool StatementHandling(string cSourceCode, ref string log)
        {
            int l_iCount = 0;
            int l_iStatement = 0;
            string l_saveCodeAsString = null;
                                                                     
            // Remove all the comment firstly but still keep the format, no removing the line              
            l_saveCodeAsString = FindCharacter(cSourceCode);
            log += l_saveCodeAsString;
            g_View.DEBUG(log);

            // COunt how many of statement in the c code
            // Then this is also the lenght of struct array   
            // Detect the position of statement
            cSourceCode = strSavetFileNew; // Update the source code file
            var l_lines = File.ReadAllLines(cSourceCode); 
            g_GlobalVariable = new Model.GlobalVariableHandling.OutComeValue[l_lines.Length];
            //string[] l_strStatement = new string[l_lines.Length];

            foreach(var line in l_lines)
            {
                l_iCount++;
                // Detect the statement by checking below conditions
                if (line.Contains(";"))
                {
                    log += "\nThis is statement: " + line + " -->at: " + l_iCount;                   
                    //l_strStatement[l_iStatement] = line; 
                    //                    
                    line.Trim(); // remove all the space
                    string[] split = line.Split('=', ';');
                    try
                    {
                        g_GlobalVariable[l_iStatement].outComePosition = l_iCount;
                        g_GlobalVariable[l_iStatement].outComeName = split[0];
                        g_GlobalVariable[l_iStatement].outComeFormula = split[1];
                    }
                    catch
                    {
                        log += "\n[StatementHandling] error when trying to divide statement in: " + line + " -->at: " + l_iCount + Environment.NewLine + "Please do a check";
                        return false;
                    }


                    l_iStatement++;
                }
                else if (line.Contains("if") || line.Contains("{") || line.Contains("else") || line.Contains("else if")
                    || line.Contains("}"))
                {
                    // This is to detect the if else condition and other
                    log += "\nThis is if else condtion or other" + line;
                }                              
                else if (line.Contains(@"^\s+$[\r\n]*")) // blank line
                {
                    log += "\nThis line is blank";
                }
            }

            return true;
        }
        /**************************************************************************
      *  Decsription:
      *     Input: .txt or .c file
      *     Output: - Number
      *             - Number of Line code
      *             - Number of Line comment
      ****************************************************************************/
        static string FindCharacter(string iFilePath)
        {
            //int iCountAll = 0;
            //int i_l_count = 0;
            string str_l_Line = null;

            var blockComments = @"/\*(.*?)\*/";
            var lineComments = @"//(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";
            var blankLine = @"^\s+$[\r\n]*";

            System.IO.StreamReader lFile = new System.IO.StreamReader(iFilePath.ToString());

            str_l_Line = lFile.ReadToEnd();
            string noComments = Regex.Replace(str_l_Line,
                         blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                         me =>
                         {
                             if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                                 return me.Value.StartsWith("//") ? Environment.NewLine : "";
                             // Keep the literal strings
                             return me.Value;
                         }, RegexOptions.Singleline);
            // DEBUG  
            // Console.WriteLine("Original String: {0}", str_l_Line);
            //Console.WriteLine("Replacement String: {0}", noComments);

            //string resultString = Regex.Replace(noComments, blankLine, string.Empty, RegexOptions.Multiline);
            string resultString = Regex.Replace(noComments, string.Empty, string.Empty, RegexOptions.Multiline);
            // DEBUG
            System.IO.File.WriteAllText(strSavetFileNew, resultString);

            // DEBUG  
            //Console.WriteLine("Number of line is: " + i_l_count);
            //Console.WriteLine("Done!!!\n");
            lFile.Dispose();
            lFile.Close();
            // Suspend the screen
            // Console.ReadLine();

            return resultString;
        }


    }
}
