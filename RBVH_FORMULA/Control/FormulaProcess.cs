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
        public List<char> IFELSELOOP = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' }; // just support for 10 if-else loops currently
        public int g_Level = 1;
        public int g_Branch = 0;
        public List<string> IFELSESPECIAL = new List<string>() { "if", "{", "else if", "}", "else" };
        RBVH_FORMULA.Model.GlobalVariableHandling.OutComeValue[] g_OutComeValue;
        RBVH_FORMULA.Model.GlobalVariableHandling.IfElseDetected[] g_IfElseDetectedForIfAndOpenBracket;
        RBVH_FORMULA.Model.GlobalVariableHandling.IfElseDetected[] g_IfElseDetectedForClosedBracket;
        RBVH_FORMULA.Model.GlobalVariableHandling.IfElseDetected[] g_IfElseDetectedForElseIf;
        RBVH_FORMULA.Model.GlobalVariableHandling.IfElseDetected[] g_IfElseDetectedForElse;


        RBVH_FORMULA.View.DisplayLog g_View = new View.DisplayLog();                     
        ///
        ///
        ///
        public bool StatementHandling(string cSourceCode, ref string log)
        {
            int l_iCount = 0;
            int l_iStatement = 0;
            int l_iIfAndOpenBracket = 0;
            int l_iClosedBracket = 0;
            int l_iElse = 0;
            int l_iElseIf = 0;
            string l_saveCodeAsString = null;
                                                                     
            // Remove all the comment firstly but still keep the format, no removing the line              
            l_saveCodeAsString = FindCharacter(cSourceCode);
            log += l_saveCodeAsString;
            g_View.DEBUG(log);

            // COunt how many of statement in the c code
            // Then this is also the lenght of struct array   
            // Detect the position of statement
            cSourceCode = strSavetFileNew; // Update the source code file
            //var l_lines = File.ReadAllLines(cSourceCode); 
            string[] l_lines = File.ReadAllLines(cSourceCode);
            
            
            g_OutComeValue = new Model.GlobalVariableHandling.OutComeValue[l_lines.Length];
            g_IfElseDetectedForIfAndOpenBracket = new Model.GlobalVariableHandling.IfElseDetected[l_lines.Length];
            g_IfElseDetectedForClosedBracket = new Model.GlobalVariableHandling.IfElseDetected[l_lines.Length];
            g_IfElseDetectedForElse = new Model.GlobalVariableHandling.IfElseDetected[l_lines.Length];
            g_IfElseDetectedForElseIf = new Model.GlobalVariableHandling.IfElseDetected[l_lines.Length];
            //string[] l_strStatement = new string[l_lines.Length];

            //foreach(var line in l_lines)
            for (l_iCount = 0; l_iCount < l_lines.Length; l_iCount++)            
            {
                //l_iCount++;
                // Detect the statement by checking below conditions
               l_lines[l_iCount] = l_lines[l_iCount].Trim();
                //
                if (l_lines[l_iCount].Contains(";"))
                {
                    log += "\n[StatementHandling][info]This is statement: " + l_lines[l_iCount] + " -->at: " + l_iCount + 1;                   
                    //l_strStatement[l_iStatement] = line; 
                    //                    
                     // remove all the space
                    string[] split = l_lines[l_iCount].Split('=', ';');
                    try
                    {
                        g_OutComeValue[l_iStatement].outComePosition = l_iCount + 1;
                        g_OutComeValue[l_iStatement].outComeName = split[0];
                        g_OutComeValue[l_iStatement].outComeFormula = split[1];                                                
                    }
                    catch
                    {
                        log += "\n[StatementHandling][Error] Error when trying to divide statement in: " + l_lines[l_iCount] + " -->at: " + l_iCount + Environment.NewLine + "Please do a check";
                        return false;
                    }                    

                    l_iStatement++;
                }
                ///
                /// Check IF ELSE condition
                ///
                //else if (l_lines[l_iCount].Contains(IFELSEBRANCH[0]) || l_lines[l_iCount].Contains(IFELSEBRANCH[1]) || l_lines[l_iCount].Contains(IFELSEBRANCH[2])
                //    || l_lines[l_iCount].Contains(IFELSEBRANCH[3]) || l_lines[l_iCount].Contains(IFELSEBRANCH[4]))
                else
                {
                    // This is to detect the if else condition and other
                    log += "\n[StatementHandling][Info] This is if else condtion: " + l_lines[l_iCount] + " -->at: " + l_iCount; 

                    // Check whether corrected code or not
                    if(     l_lines[l_iCount].Contains("if") && !l_lines[l_iCount + 1].Contains("{")
                        || (l_lines[l_iCount].Contains("else") && !l_lines[l_iCount + 1].Contains("{"))
                        || (l_lines[l_iCount].Contains("else if") && !l_lines[l_iCount + 1].Contains("{")))
                    {
                        log += "\n[StatementHandling][Error] The line" + (l_iCount+1).ToString() +" after IF/Else/ElseIf condition at: " + l_iCount.ToString() + " does not have the '{'" +
                                    Environment.NewLine + "Please correct the orginal file by REFERENCING (not updatd on this) at: " + strSavetFileNew;
                        return false;
                    }
                    else if (l_lines[l_iCount].Contains("if") && l_lines[l_iCount + 1].Contains("{") && !l_lines[l_iCount].Contains("else if")) // Check IF condition
                    {
                        try
                        {
                            g_IfElseDetectedForIfAndOpenBracket[l_iIfAndOpenBracket].IfElseLineNo = l_iCount + 1;
                            g_IfElseDetectedForIfAndOpenBracket[l_iIfAndOpenBracket].IfElseContent = l_lines[l_iCount].ToString();
                            g_IfElseDetectedForIfAndOpenBracket[l_iIfAndOpenBracket].IfElseLevelDetection = g_Level.ToString() + IFELSELOOP[g_Branch];
                            l_iIfAndOpenBracket++;
                            //g_Level++;
                            g_Branch++;
                        }
                        catch (Exception exp)
                        {
                            log += "\n[StatementHandling][Error] Error when Check the IF condition at line: " + (l_iCount + 1).ToString() + " in file: " + strSavetFileNew.ToString()
                                    + Environment.NewLine + exp.Message;
                            return false;
                        }
                    }
                    else if (l_lines[l_iCount].Trim().Contains("}")) // in case of closed braceket
                    {
                        try
                        {
                            g_Branch--; // if detected this one, that means one block is to be closed
                            g_IfElseDetectedForClosedBracket[l_iClosedBracket].IfElseLineNo = l_iCount + 1;
                            g_IfElseDetectedForClosedBracket[l_iClosedBracket].IfElseContent = l_lines[l_iCount].ToString();
                            g_IfElseDetectedForClosedBracket[l_iClosedBracket].IfElseLevelDetection = g_Level.ToString() + IFELSELOOP[g_Branch];
                            if(g_Branch == 0)
                            {
                                g_Level++;
                            }
                            //if (g_IfElseDetectedForClosedBracket[l_iClosedBracket].IfElseLevelDetection == g_Level.ToString() + IFELSELOOP[0]) // if this equal to 1a, 1b,1c, ...branch shall be broken
                            //{
                            //    g_Level++; // Icrease the branch for the new one
                            //    g_Branch = 0; // Reset the level for the new branch
                            //}
                            //else if (l_iClosedBracket >= 1) // check if current level == previous level? increase the level
                            //{
                            //    if (g_IfElseDetectedForClosedBracket[l_iClosedBracket].IfElseLevelDetection == g_IfElseDetectedForClosedBracket[l_iClosedBracket - 1].IfElseLevelDetection)
                            //    {
                            //        g_IfElseDetectedForClosedBracket[l_iClosedBracket].IfElseLevelDetection = g_Level.ToString() + IFELSELOOP[0];
                            //        g_Branch =  0;
                            //        g_Level++; // increase the level for the new branch
                            //    }
                            //}
                            //
                            l_iClosedBracket++;
                        }
                        catch (Exception exp)
                        {
                            log += "\n[StatementHandling][Error] Error when Check the '}' at line: " + (l_iCount + 1).ToString() + " in file: " + strSavetFileNew.ToString()
                                    + Environment.NewLine + exp.Message;
                            return false;
                        }
                    }
                    else if (l_lines[l_iCount].Contains("else") && l_lines[l_iCount + 1].Contains("{") && !l_lines[l_iCount].Contains("else if"))
                    {
                        try
                        {                            
                            g_IfElseDetectedForElse[l_iElse].IfElseLineNo = l_iCount + 1;
                            g_IfElseDetectedForElse[l_iElse].IfElseContent = l_lines[l_iCount].ToString();
                            g_IfElseDetectedForElse[l_iElse].IfElseLevelDetection = g_Level.ToString() + IFELSELOOP[g_Branch];
                            g_Branch++;
                            l_iElse++;
                        }
                        catch (Exception exp)
                        {
                            log += "\n[StatementHandling][Error] Error when Check the 'else' condition at line: " + (l_iCount + 1).ToString() + Environment.NewLine +"-->in file: " + strSavetFileNew.ToString()
                                    + Environment.NewLine + exp.Message;
                            return false;
                        }
                    }
                    else if (l_lines[l_iCount].Contains("else if") && l_lines[l_iCount + 1].Contains("{") && l_lines[l_iCount - 1].Contains("}"))
                    {
                        try
                        {
                            g_IfElseDetectedForElseIf[l_iElseIf].IfElseLineNo = l_iCount + 1;
                            g_IfElseDetectedForElseIf[l_iElseIf].IfElseContent = l_lines[l_iCount].ToString();
                            g_IfElseDetectedForElseIf[l_iElseIf].IfElseLevelDetection = g_Level.ToString() + IFELSELOOP[g_Branch];
                            // Check
                            //if (g_IfElseDetectedForClosedBracket[l_iClosedBracket - 1].IfElseLevelDetection == g_Level.ToString() + IFELSELOOP[g_Branch - 1])
                            //{
                            //    g_Branch = 0; // reset the level and branch is taken care in the above code    
                            //    g_IfElseDetectedForElseIf[l_iElseIf].IfElseLevelDetection = (g_Level+1).ToString() + IFELSELOOP[g_Branch];
                            //}
                            //
                            g_Branch++;
                            l_iElseIf++;
                        }
                        catch (Exception exp)
                        {
                            log += "\n[StatementHandling][Error] Error when Check the 'ELSE - IF' condition at line: " + (l_iCount + 1).ToString() + " in file: " + strSavetFileNew.ToString()
                                   + Environment.NewLine + exp.Message;
                            return false;
                        }

                    }
                }
                //else if (l_lines[l_iCount].Contains(@"^\s+$[\r\n]*")) // blank line
                //{
                //    log += "\nThis line is blank";
                //}
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

            string resultString = Regex.Replace(noComments, blankLine, string.Empty, RegexOptions.Multiline);
           // string resultString = Regex.Replace(noComments, string.Empty, string.Empty, RegexOptions.Multiline);
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
