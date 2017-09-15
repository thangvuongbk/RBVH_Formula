using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace RBVH_FORMULA.Control
{
    class FormulaProcess
    {
        const string LOCAL_VARIABLE = "LOCAL VARIABLES";
        const string TCNo_NAME = "TC No.";
        const string OUTPUT_NAME = "OUTPUTS";
        const int TCNAME_COLUMN = 1;

        static string CurrentPath = Directory.GetCurrentDirectory();
        static string strSavetFileNew = CurrentPath + @"\CodeAdjusted.c";
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
        public bool StatementHandling(Excel.Workbook xlWB, string sheetName, string cSourceCode, ref string log)
        {
            int l_iCount = 0;
            int l_iStatement = 0;
            //int l_iIfAndOpenBracket = 0;
            //int l_iClosedBracket = 0;
            //int l_iElse = 0;
            //int l_iElseIf = 0;
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
            // Handle the line
            // DEBUG
           
            // l_lines = conditionInALine(l_lines);
           // strSavetFileNew = l_lines.ToString();
                 
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
//                if (l_lines[l_iCount] == null || l_lines[l_iCount] == string.Empty || l_lines[l_iCount] == " ") break;

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
                        log += "\n[StatementHandling][Error] The LINE " + (l_iCount+1).ToString() +" NEXT TO if/else/else-if condition at line: " + l_iCount.ToString() + " DOES NOT have the '{'" +
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
                    //return true; // end of the outcome value
                    iLocal = g_OutComeValue.Length; // to terminate
                    break;
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
                ///
                /// A < B < C < D < E < F< G < H < I < J
                ///
                g_OutComeValue[iLocal].outComeFormula = StatementProcess(g_OutComeValue[iLocal].outComeFormula,ref log);
                g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = strPreviousOutCome == " " ? g_OutComeValue[iLocal].outComeFormula + ", " + "IntialValue" : g_OutComeValue[iLocal].outComeFormula + ", " + "Line_" + strPreviousOutCome;
                
                ///
                for (int iLocal1 = g_OutComeValue[iLocal].outComePosition - 1; iLocal1 >=  0; iLocal1--)
                {   
                    ///
                    /// Case 1: Detected the "}" and level contain "A"
                    ///                     
                    if ((l_lines[iLocal1].Contains("}") && g_IfElseDetectedForClosedBracket[iLocal1].IfElseLevelDetection == "A")) // for handling the else in level A
                    {
                        previousLevel = g_IfElseDetectedForClosedBracket[iLocal1].IfElseLevelDetection; // it have to level "A"
                        g_OutComeValue[iLocal].outComeFormula = StatementProcess(g_OutComeValue[iLocal].outComeFormula, ref log);
                        g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "'=" + g_OutComeValue[iLocal].outComeFormula;
                        break;
                    }
                        ///Handle for this case
                        ///if ()
                        ///{
                        ///.....
                        ///}
                        ///else if(...)
                        ///{
                        ///} ----> This point
                        ///else
                        ///{
                        /// if()
                        ///     {}
                        ///}

                    else if (l_lines[iLocal1].Trim().Contains("}")) // handle for "}" inside the loop
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

                        if (currentLevel == "A")
                        {
                            for (int i = iLocal1; i >= 0; i--)
                            {
                                if (g_IfElseDetectedForIfAndOpenBracket[i].IfElseLevelDetection == "A")
                                {
                                    g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF((" + g_IfElseDetectedForIfAndOpenBracket[i].IfElseContent + ") = FALSE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                                    break;
                                }
                                else if (g_IfElseDetectedForElseIf[i].IfElseLevelDetection == "A")
                                {
                                    g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF((" + g_IfElseDetectedForElseIf[i].IfElseContent + ") = FALSE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                                    break;
                                }

                            }
                        }                       

                        //
                        if (string.Compare(currentLevel, previousLevel) >= 0)
                        {
                            // do not update the formula
                           // bPreElseDetected = false;
                        }
                        else
                        {
                            previousLevel = currentLevel; // update the level e.x C -> B                           
                        }

                    }
                        // Handle the elseif condition
                    else if (l_lines[iLocal1].Contains("else if") && l_lines[iLocal1 + 1].Contains("{") && l_lines[iLocal1 - 1].Contains("}"))
                    {
                        currentLevel = g_IfElseDetectedForElseIf[iLocal1].IfElseLevelDetection;

                        if (currentLevel == previousLevel && bPreElseDetected == true) // else if and else the same level
                        {
                            g_IfElseDetectedForElseIf[iLocal1].IfElseContent = ConditionProcess(g_IfElseDetectedForElseIf[iLocal1].IfElseContent, ref log);
                            g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF((" + g_IfElseDetectedForElseIf[iLocal1].IfElseContent + ") = FALSE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                            bPreElseDetected = false;
                        }
                        else if (string.Compare(currentLevel, previousLevel) < 0) // if this line has the higher level e.x C-> B
                        {
                            g_IfElseDetectedForElseIf[iLocal1].IfElseContent = ConditionProcess(g_IfElseDetectedForElseIf[iLocal1].IfElseContent, ref log);
                            g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF((" + g_IfElseDetectedForElseIf[iLocal1].IfElseContent + ") = TRUE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                            previousLevel = currentLevel;
                        }                                               
                        
                    }
                        // Handle the if condition
                    else if (l_lines[iLocal1].Contains("if") && l_lines[iLocal1 + 1].Contains("{") && !l_lines[iLocal1].Contains("else if"))
                    {
                        currentLevel = g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseLevelDetection;

                        if (currentLevel == previousLevel && bPreElseDetected == true) // if and else the same level
                        {
                            g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent = ConditionProcess(g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent, ref log);
                            g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF((" + g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent + ") = FALSE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                            bPreElseDetected = false;
                        }
                        else if (string.Compare(currentLevel, previousLevel) < 0) // if this line has the higher level e.x C-> B
                        {
                            g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent = ConditionProcess(g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent, ref log);
                            g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "IF((" + g_IfElseDetectedForIfAndOpenBracket[iLocal1].IfElseContent + ") = TRUE," + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula + ")";
                            previousLevel = currentLevel;
                        }                        
                       
                    }

                    // Case 3: else that belonged to the first of line
                    else
                    {
                        // do nothing
                     //   g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = g_OutComeValue[iLocal].outComeFormula;
                    }
                    // breake the loop if the levelA reached
                    if ((previousLevel == "A"))
                    {
                        g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = "'=" + g_OutComeValueFinal[iOutComeValueFinal].outComeFormula;
                        break;
                    }
                }
                //
                g_OutComeValueFinal[iOutComeValueFinal].outComeFormula = previousLevel == "A" ? g_OutComeValueFinal[iOutComeValueFinal].outComeFormula : "'=" + g_OutComeValue[iLocal].outComeFormula;
                //
                currentLevel = "A";
                previousLevel = "K";
                strPreviousOutCome = " ";
                iOutComeValueFinal++;                
            }
            ///
            /// Write the result to excel file
            /// 
            bool bCheckExcelReturn = WriteResult2Excel(xlWB, sheetName, g_OutComeValueFinal, ref log);
            if (bCheckExcelReturn == false) return false;


            return true;
        }

        /**************************************************************************
        *  Decsription: Handle the string from the condition
        *     Input: string statement
        *     Output: - if condition needs to be handle
        *             - else if condition needs to be handle
        *             - else condition to be handled
        ****************************************************************************/
        public string ConditionProcess(string strCondition, ref string log)
        {
            strCondition = strCondition.Trim();
            StringBuilder l_strCondition = new StringBuilder(strCondition);
            List<string> conditionSpecialList = new List<string>()           {"else if","else","if","true" ,"false","==" , "!=" , "&&", "||", " ", "\t"}; // to be updated follow the next version
            List<string> conditionSpecialListReplaced = new List<string>()   {   ""    , ""   , "" , "TRUE","FALSE","=" , "<>" , "&" , "|",  "" , "" }; // to be updated follow the next version
                       
            for (int i = 0; i < conditionSpecialList.Count; i++)
            {
                if(strCondition.Contains(conditionSpecialList[i]))
                {
                  
                   l_strCondition.Replace(conditionSpecialList[i], conditionSpecialListReplaced[i]);
                    
                }
            }
            //Check whether this include the if-else condition or not
            // If not, return here
            strCondition = l_strCondition.ToString();
            if (!strCondition.Contains("&") && !strCondition.Contains("|"))
            {
                if (strCondition[0] == '(' && strCondition[strCondition.Length - 1] == ')')
                {
                    //removeSpecialKey.Replace('(', ' ', 1, 1);
                    strCondition = strCondition.Substring(1, strCondition.Length - 2);
                }
                return strCondition;
            }          
            
            ///
            /// Process the LOGICAL OPERATOR, Especially && and ||
            ///
            //var posOpen = strCondition.IndexOf("(");
            //var posClose = strCondition.IndexOf(")");
            //var subtring = strCondition.Substring(posOpen, posClose + 1);
            //string[] getProcessed = strCondition.Split('&');
            int iOpen = 0, iClose = 0, iLevel = 0, iGetString = 0;           
            RBVH_FORMULA.Model.GlobalVariableHandling.ConditionLevelDetected[] posOpen = new Model.GlobalVariableHandling.ConditionLevelDetected[strCondition.Length];
            RBVH_FORMULA.Model.GlobalVariableHandling.ConditionLevelDetected[] posClose = new Model.GlobalVariableHandling.ConditionLevelDetected[strCondition.Length];
           
            
            /// Get all the position includes the '(' and ')' seperatly
            for (int i = 0; i < strCondition.Length; i++)                                     
            {
                try
                {
                    if (strCondition[i] == '(')
                    {
                        posOpen[iOpen].position = i;
                        posOpen[iOpen].level = iLevel;
                        iOpen++;
                        iLevel++;
                    }
                    else if (strCondition[i] == ')')
                    {
                        iLevel--;
                        posClose[iClose].position = i;
                        posClose[iClose].level = iLevel;
                        iClose++;
                        if (iLevel == 0) break;
                    }
                    //getStringBlock = strCondition.Substring(posOpen, posClose - posOpen + 1);
                }
                catch(Exception exp)
                {
                    log += "[Error][ConditionProcess] Error when find the syschonise '(' and ')' at line includes the content: " + strCondition.ToString() 
                        + Environment.NewLine + exp.Message;
                    return null;
                }
            }
            RBVH_FORMULA.Model.GlobalVariableHandling.GetStringBlock[] getStringBlock = new Model.GlobalVariableHandling.GetStringBlock[iOpen];
            // get the desired string and level
            for (int i = 0; i < iOpen; i++)
            {
                for (int j = 0; j < iOpen; j++)
                {
                    if (posClose[j].position == 0)
                    {
                        i = posOpen.Length + 1;
                        j = posClose.Length + 1;
                        //break;
                    }
                    else if (posOpen[i].position < posClose[j].position
                             && posOpen[i].level == posClose[j].level)
                    {
                      getStringBlock[iGetString].content = strCondition.Substring(posOpen[i].position, posClose[j].position - posOpen[i].position + 1);
                      getStringBlock[iGetString].level = posOpen[i].level;
                      iGetString++;
                      break;                        
                    }                    
                }                               
            }
            // Last step: Handle the formula from Maxlevel to lower level
            // get the Max level first
            int maxLevel = 0;
            string finalCondition = ""; 

            for (int i = 0; i < getStringBlock.Length; i++)
            {
                if(maxLevel < getStringBlock[i].level)
                {
                    maxLevel = getStringBlock[i].level;
                }
                StringBuilder removeSpecialKey = new StringBuilder(getStringBlock[i].content);
                if (getStringBlock[i].content[0] == '(' && getStringBlock[i].content[getStringBlock[i].content.Length - 1] == ')')
                {
                    //removeSpecialKey.Replace('(', ' ', 1, 1);
                    getStringBlock[i].content = getStringBlock[i].content.Substring(1, getStringBlock[i].content.Length - 2);
                }
            }
            //
            for (int i = maxLevel; i >= 0; i--) //Hanlding form max to min (0)
            {
                for (int j = 0; j < getStringBlock.Length; j++)
                {                                                         
                    if (getStringBlock[j].level == i && (getStringBlock[j].content.Contains("&") || getStringBlock[j].content.Contains("|")))
                    {
                        try
                        {
                            if (getStringBlock[j].content.Contains("&"))
                            {
                                string[] charDivided = getStringBlock[j].content.Split('&'); // first: handle the & first
                                if (charDivided.Length == 2)
                                {
                                    //finalCondition = "AND(" + charDivided[0] + "," + charDivided[1] + ")";
                                    finalCondition = "AND(" + charDivided[0] + "," + charDivided[1] + ")";
                                }
                                else
                                {
                                    finalCondition = "AND(" + charDivided[charDivided.Length - 1] + "," + charDivided[charDivided.Length - 2] + ")";
                                    for (int iRun = charDivided.Length - 3; iRun >= 0; iRun--)
                                    {
                                        finalCondition = "AND(" + charDivided[iRun] + "," + finalCondition + ")";
                                    }
                                }
                                // Update the content in case of the string has mixed && and ||
                               // getStringBlock[j].content = finalCondition;
                            }                            
                            if (getStringBlock[j].content.Contains("|"))
                            {
                                string[] charDivided = getStringBlock[j].content.Split('|'); // first: handle the & first
                                if (charDivided.Length == 2)
                                {
                                    finalCondition = "OR(" + charDivided[0] + "," + charDivided[1] + ")";
                                }
                                else
                                {
                                    finalCondition = "OR(" + charDivided[charDivided.Length - 1] + "," + charDivided[charDivided.Length - 2] + ")";
                                    for (int iRun = charDivided.Length - 3; iRun >= 0; iRun--)
                                    {
                                        finalCondition = "OR(" + charDivided[iRun] + "," + finalCondition + ")";
                                    }
                                }
                            }
                        }
                        catch (Exception exp)
                        {

                        }
                        // Update the change
                        for (int iUpdate = 0; iUpdate < getStringBlock.Length; iUpdate++)
                        {
                            if (getStringBlock[iUpdate].content.Contains(getStringBlock[j].content))
                            {
                                StringBuilder updated = new StringBuilder(getStringBlock[iUpdate].content);
                                //updated.Replace("(" + getStringBlock[j].content + ")", finalCondition); // if (strCondition[0] == '(' && strCondition[strCondition.Length - 1] == ')')
                                //if (getStringBlock[iUpdate].content[0] == '(' && getStringBlock[iUpdate].content[getStringBlock[iUpdate].content.Length - 1] == ')')
                                //{
                                //    updated.Replace("(" + getStringBlock[j].content + ")", finalCondition);
                                //}
                                //else updated.Replace(getStringBlock[j].content, finalCondition);
                               updated.Replace(getStringBlock[j].content, finalCondition);
                                getStringBlock[iUpdate].content = updated.ToString();
                            }
                        }
                    }   
                    // 0 is root 
                    finalCondition = getStringBlock[0].content;
                }
            }

            return finalCondition;
        }


        /**************************************************************************
        *  Decsription: Handle the string from the statement
        *     Input: string statement: 
         *      C/ESDL Code    ---->     Excel code
         *      A.max(B)                MAX(A,B)
         *      A.min(B)                MIN(A,B)
         *      A.getAt(B)              Not Possible
         * 
        *     Output: - if condition needs to be handle
        *             - else if condition needs to be handle
        *             - else condition to be handled
        ****************************************************************************/
        public string StatementProcess(string strStatement, ref string log)
        {
            strStatement = strStatement.Trim();
            StringBuilder l_strStatement = new StringBuilder(strStatement);
            List<string> conditionSpecialList = new List<string>()          { "true", "false", "==" }; // to be updated follow the next version
            List<string> conditionSpecialListReplaced = new List<string>()  { "TRUE", "FALSE", "=" }; // to be updated follow the next version

            for (int i = 0; i < conditionSpecialList.Count; i++)
            {
                if (strStatement.Contains(conditionSpecialList[i]))
                {
                    l_strStatement.Replace(conditionSpecialList[i], conditionSpecialListReplaced[i]);
                }
            }
            // in case of
            // A.min(B) -> MIN(A,B_)
            // A.max(B) -> MAX(A,B)
            if (strStatement.Contains(".min"))
            {
                
            }

            return l_strStatement.ToString();
        }

        /**************************************************************************
      *  Decsription: Write the result to excel file
      *     Input: Final Outcome formula
      *     Output: - this shall be written into LOCAL VARIABLES afterwards
      *****************************************************************************/

        static Boolean WriteResult2Excel(Excel.Workbook xlWB, string sheetName, RBVH_FORMULA.Model.GlobalVariableHandling.OutComeValue[] g_OutComeValueFinal, ref string log)
        {
            int iTCRow = 0;
            int iLocalVariableColumn = 0;
            int iOutput = 0;

            /// Get the sheet name
            Excel.Worksheet xlWS;
            try
            {
                xlWS = (Excel.Worksheet)xlWB.Worksheets.get_Item(sheetName);
            }
            catch (Exception exp)
            {
                log += "\n[Error][SourceFile] The sheet name is not found -> shoud have " + sheetName + Environment.NewLine + exp.Message;
                return false;
            }
            xlWS.Activate();
            /// Find the INPUT row
            /// 
            // Check the INPUTS world column is exsit or not
            iTCRow = ClsCommonSupportFunction.FindRowAll(xlWB, sheetName, TCNAME_COLUMN, TCNo_NAME, 1, ref log);
            if (iTCRow < 0)
            {
                log += "\n[Error][WriteResult2Excel] Not Found the " + TCNo_NAME + " in the sheet name " + sheetName.ToString() + Environment.NewLine +
                            "Please check it manually";
                return false;
            }
            // Check the CSIS Customer column is exsit or not
            iLocalVariableColumn = ClsCommonSupportFunction.FindColAll(xlWB, sheetName, iTCRow, LOCAL_VARIABLE, 1, ref log);
            if (iLocalVariableColumn < 0)
            {
                log += "\n[Error][WriteResult2Excel] Not Found the " + LOCAL_VARIABLE + " in the sheet name " + sheetName.ToString() + Environment.NewLine +
                            "Please check it manually";
                return false;
            }

            Excel.Range rResultFill = xlWS.Cells[iTCRow, iLocalVariableColumn + 1];
            // Write the content to excel file
            for (int iResult = 0; iResult < g_OutComeValueFinal.Length - 1; iResult++)
            {
                if(g_OutComeValueFinal[iResult].outComeName == "" || g_OutComeValueFinal[iResult].outComeName == null)
                {
                    // do nothing
                }
                else
                {
                    try
                    {
                        rResultFill = xlWS.Cells[iTCRow, iLocalVariableColumn + 1 + iResult];
                        rResultFill.EntireColumn.Insert(XlInsertShiftDirection.xlShiftToRight); // insert a colmn
                        xlWS.Cells[iTCRow + 1, iLocalVariableColumn + iResult] = g_OutComeValueFinal[iResult].outComeName.ToString() + " (Line " + g_OutComeValueFinal[iResult].outComePosition.ToString() + ")"; // write the outcome name
                        xlWS.Cells[iTCRow + 2, iLocalVariableColumn + iResult] = g_OutComeValueFinal[iResult].outComeFormula.ToString(); // write the formula in the next row
                    }
                    catch(Exception msg)
                    {
                        log += "\n[Error][WriteResult2Excel] Error when writing line " + g_OutComeValueFinal[iResult].outComePosition.ToString() + " With OutComeName: " + g_OutComeValueFinal[iResult].outComeName.ToString()
                                  + Environment.NewLine + "Formula is: " + g_OutComeValueFinal[iResult].outComeFormula.ToString() + Environment.NewLine + " Please check it manually";
                        return false;
                    }
                }
                
            }            
            // release the worksheet
            object oMissing = System.Reflection.Missing.Value;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWS);
            xlWS = null;
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

            // Write the result to file
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

     /**************************************************************************
     *  Decsription:
     *     Input: array of string
     *     Output: - Handling the if-elseif condition to in a line only
     ****************************************************************************/
      private string[] conditionInALine(string[] lines)
        {
            int l_iLine = 0;
            string[] l_Line = new string[lines.Length];
          //
            for (int iLine = 0; iLine < lines.Length - 1; iLine++)
            {
                lines[iLine] = lines[iLine].Trim();

                if (lines[iLine].Contains("if") || lines[iLine].Contains("else if")) // check current line includes the if or else if and the next line not included the "{"
                {
                    if (lines[iLine + 1].Contains("{"))
                    {
                        l_Line[l_iLine] = lines[iLine];
                        l_iLine++;
                    }
                    else lines[iLine + 1] = lines[iLine] + lines[iLine + 1];                                        
                   // iLine++;
                }
                else
                {
                    l_Line[l_iLine] = lines[iLine];
                    l_iLine++;
                }
            }

            return l_Line;
        }

    }
}
