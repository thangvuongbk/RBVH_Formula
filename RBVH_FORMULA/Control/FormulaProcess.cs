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
        RBVH_FORMULA.Model.GlobalVariableHandling.OutComeValue[] g_OutComeValueFinal;
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
                    //
                    l_iStatement++;
                }
                ///
                /// Check IF ELSE condition
                ///                   
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
                            //g_IfElseDetectedForIfAndOpenBracket[l_iIfAndOpenBracket].IfElseLineNo = l_iCount + 1;
                            //g_IfElseDetectedForIfAndOpenBracket[l_iIfAndOpenBracket].IfElseContent = l_lines[l_iCount].ToString();
                            //g_IfElseDetectedForIfAndOpenBracket[l_iIfAndOpenBracket].IfElseLevelDetection = g_Level.ToString() + IFELSELOOP[g_Branch];
                            g_IfElseDetectedForIfAndOpenBracket[l_iCount].IfElseLineNo = l_iCount + 1;
                            g_IfElseDetectedForIfAndOpenBracket[l_iCount].IfElseContent = l_lines[l_iCount].ToString();
                            g_IfElseDetectedForIfAndOpenBracket[l_iCount].IfElseLevelDetection = IFELSELOOP[g_Branch].ToString();
                         //   l_iIfAndOpenBracket++;
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
                            //g_IfElseDetectedForClosedBracket[l_iClosedBracket].IfElseLineNo = l_iCount + 1;
                            //g_IfElseDetectedForClosedBracket[l_iClosedBracket].IfElseContent = l_lines[l_iCount].ToString();
                            //g_IfElseDetectedForClosedBracket[l_iClosedBracket].IfElseLevelDetection = g_Level.ToString() + IFELSELOOP[g_Branch];
                            g_IfElseDetectedForClosedBracket[l_iCount].IfElseLineNo = l_iCount + 1;
                            g_IfElseDetectedForClosedBracket[l_iCount].IfElseContent = l_lines[l_iCount].ToString();
                            g_IfElseDetectedForClosedBracket[l_iCount].IfElseLevelDetection = IFELSELOOP[g_Branch].ToString();
                            if(g_Branch == 0)
                            {
                                g_Level++;
                            }                            
                            //l_iClosedBracket++;
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
                            //g_IfElseDetectedForElse[l_iElse].IfElseLineNo = l_iCount + 1;
                            //g_IfElseDetectedForElse[l_iElse].IfElseContent = l_lines[l_iCount].ToString();
                            //g_IfElseDetectedForElse[l_iElse].IfElseLevelDetection = g_Level.ToString() + IFELSELOOP[g_Branch];
                            g_IfElseDetectedForElse[l_iCount].IfElseLineNo = l_iCount + 1;
                            g_IfElseDetectedForElse[l_iCount].IfElseContent = l_lines[l_iCount].ToString();
                            g_IfElseDetectedForElse[l_iCount].IfElseLevelDetection = IFELSELOOP[g_Branch].ToString();
                            g_Branch++;
                          //  l_iElse++;
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
                        //    g_IfElseDetectedForElseIf[l_iElseIf].IfElseLineNo = l_iCount + 1;
                        //    g_IfElseDetectedForElseIf[l_iElseIf].IfElseContent = l_lines[l_iCount].ToString();
                            //g_IfElseDetectedForElseIf[l_iElseIf].IfElseLevelDetection = g_Level.ToString() + IFELSELOOP[g_Branch];                            
                            g_IfElseDetectedForElseIf[l_iCount].IfElseLineNo = l_iCount + 1;
                            g_IfElseDetectedForElseIf[l_iCount].IfElseContent = l_lines[l_iCount].ToString();
                            g_IfElseDetectedForElseIf[l_iCount].IfElseLevelDetection = IFELSELOOP[g_Branch].ToString();    
                            g_Branch++;
                            //l_iElseIf++;
                        }
                        catch (Exception exp)
                        {
                            log += "\n[StatementHandling][Error] Error when Check the 'ELSE - IF' condition at line: " + (l_iCount + 1).ToString() + " in file: " + strSavetFileNew.ToString()
                                   + Environment.NewLine + exp.Message;
                            return false;
                        }

                    }
                }               
            }
            ///
            /// Handle the formula
            ///
            int iOutComeValueFinal = 0;
            bool bPreElseDetected = false;
            string currentLevel = "A";
            string previousLevel = "K";
            string strPreviousOutCome = " ";
            g_OutComeValueFinal = new Model.GlobalVariableHandling.OutComeValue[g_OutComeValue.Length];
            for (int iLocal = 0; iLocal < g_OutComeValue.Length; iLocal++) // calculate the formula for individual outcome
            {
                if (g_OutComeValue[iLocal].outComeName == " " || g_OutComeValue[iLocal].outComeName == null || g_OutComeValue[iLocal].outComeName == string.Empty)
                {
                    log += "\n[StatementHandling][info] Done at line: " + g_OutComeValue[iLocal - 1].outComePosition.ToString();
                    return true; // end of the outcome value
                    //break;
                }
                
                g_OutComeValueFinal[iOutComeValueFinal].outComeName = g_OutComeValue[iLocal].outComeName;
                g_OutComeValueFinal[iOutComeValueFinal].outComePosition = g_OutComeValue[iLocal].outComePosition;
                // Get the previous value of current outcome in the first met
                if (iOutComeValueFinal > 0)
                {
                    for (int i = (iOutComeValueFinal - 1); i >= 0; i--)
                    {
                        if (g_OutComeValueFinal[i].outComeName == g_OutComeValueFinal[iOutComeValueFinal].outComeName)
                        {
                            //strPreviousOutCome = g_OutComeValueFinal[i].outComeFormula;
                            strPreviousOutCome = g_OutComeValueFinal[i].outComePosition.ToString(); // get the position first
                            //i = -1; // out of the loop
                            log += "\n[StatementHandling][info] Get the last value of outcome variable: " + g_OutComeValueFinal[iOutComeValueFinal].outComeName.ToString() +
                                " at: " + strPreviousOutCome;
                            break;
                        }
                    }
                }
                /// A < B < C < D < E < F< G < H < I < J
                ///
                if (strPreviousOutCome == " ")
                {
                    g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = g_OutComeValue[iLocal].outComeFormula + ", " + "IntialValue";
                }
                else
                {
                    g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = g_OutComeValue[iLocal].outComeFormula + ", " + "Line_" + strPreviousOutCome; 
                }
                //
                for (int iLocal1 = g_OutComeValue[iLocal].outComePosition - 1; iLocal1 >=  0; iLocal1--)
                {   
                    ///
                    /// Case 1: Detected the "}" and level contain "A"
                    /// 
                    if ((previousLevel == "A"))
                    {
                        g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "=" + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula;
                        break;
                    }
                    else if ((l_lines[iLocal1].Contains("}") && g_IfElseDetectedForClosedBracket[iLocal1].IfElseLevelDetection == "A")) // for handling the else in level A
                    {
                        g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = g_OutComeValue[iLocal].outComeFormula;
                        break;
                    }
                    else if (l_lines[iLocal1].Trim().Contains("}"))
                    {
                        currentLevel = g_IfElseDetectedForClosedBracket[iLocal1].IfElseLevelDetection;
                        if (string.Compare(currentLevel, previousLevel) < 0)
                        {
                            previousLevel = currentLevel; // update the level e.x C -> B                           
                        }
                        
                    }
                    ///
                    /// Case 2: Detected the one of the conditions
                    /// 
                   else if(l_lines[iLocal1].Contains("else") && l_lines[iLocal1 + 1].Contains("{") && !l_lines[iLocal1].Contains("else if"))
                    {
                        bPreElseDetected = true;
                        currentLevel = g_IfElseDetectedForElse[iLocal1].IfElseLevelDetection;
                        //
                        if (currentLevel == "A")
                       {
                           for (int i = iLocal1; i >= 0; i--)
                           {
                               if(g_IfElseDetectedForIfAndOpenBracket[i].IfElseLevelDetection == "A")
                               {
                                   g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF(" + g_IfElseDetectedForIfAndOpenBracket[i].IfElseContent + " = FALSE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                                   break;
                               }
                               else if(g_IfElseDetectedForElseIf[i].IfElseLevelDetection == "A")
                               {
                                   g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF(" + g_IfElseDetectedForElseIf[i].IfElseContent + " = FALSE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                                   break;
                               }
                               
                           }
                       }
                       //
                        if (string.Compare(currentLevel, previousLevel) >= 0)
                        {
                            // do not update the formula
                            bPreElseDetected = false;
                        }
                        else
                        {
                            previousLevel = currentLevel; // update the level e.x C -> B                           
                        }

                    }
                    else if (l_lines[iLocal1].Contains("else if") && l_lines[iLocal1 + 1].Contains("{") && l_lines[iLocal1 - 1].Contains("}"))
                    {
                        currentLevel = g_IfElseDetectedForElseIf[iLocal1].IfElseLevelDetection;

                        if (currentLevel == previousLevel && bPreElseDetected == true) // else if and else the same level
                        {
                            g_IfElseDetectedForElseIf[iLocal1].IfElseContent = conditionStatementProcess(g_IfElseDetectedForElseIf[iLocal1].IfElseContent);
                            g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF(" + g_IfElseDetectedForElseIf[iLocal1].IfElseContent + " = FALSE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                            bPreElseDetected = false;
                        }
                        else if (string.Compare(currentLevel, previousLevel) < 0) // if this line has the higher level e.x C-> B
                        {
                            g_IfElseDetectedForElseIf[iLocal1].IfElseContent = conditionStatementProcess(g_IfElseDetectedForElseIf[iLocal1].IfElseContent);
                            g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF(" + g_IfElseDetectedForElseIf[iLocal1].IfElseContent + " = TRUE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                            previousLevel = currentLevel;
                        }

                    }
                    else if (l_lines[iLocal1].Contains("if") && l_lines[iLocal1 + 1].Contains("{") && !l_lines[iLocal1].Contains("else if"))
                    {

                        currentLevel = g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseLevelDetection;

                        if (currentLevel == previousLevel && bPreElseDetected == true) // else if and else the same level
                        {
                            g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent = conditionStatementProcess(g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent);
                            g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF(" + g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent + " = FALSE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                            bPreElseDetected = false;
                        }
                        else if (string.Compare(currentLevel, previousLevel) < 0) // if this line has the higher level e.x C-> B
                        {
                            g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent = conditionStatementProcess(g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent);
                            g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF(" + g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent + " = TRUE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                            previousLevel = currentLevel;
                        }
                       
                    }

                    // Case 3: else that belonged to the first of line
                    else
                    {
                        // do nothing
                     //   g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = g_OutComeValue[iLocal].outComeFormula;
                    }                    
                }
                //
                currentLevel = "A";
                previousLevel = "K";
                strPreviousOutCome = " ";
                iOutComeValueFinal++;                
            }


            return true;
        }

        /**************************************************************************
        *  Decsription: Handle the string from the condition
        *     Input: string statement
        *     Output: - if condition needs to be handle
        *             - else if condition needs to be handle
        *             - else condition to be handled
        ****************************************************************************/
        static string conditionStatementProcess(string strCondition)
        {
            StringBuilder l_strCondition = new StringBuilder(strCondition);
            List<string> conditionSpecialList = new List<string>()           {"else if","else","if","true" ,"false","=="}; // to be updated follow the next version
            List<string> conditionSpecialListReplaced = new List<string>()   {   ""    , ""   , "" , "TRUE","FALSE","="}; // to be updated follow the next version

            for (int i = 0; i < conditionSpecialList.Count; i++)
            {
                if(strCondition.Contains(conditionSpecialList[i]))
                {
                    l_strCondition.Replace(conditionSpecialList[i], conditionSpecialListReplaced[i]);
                }
            }


            return l_strCondition.ToString();
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
