using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH_FORMULA.Model
{
    class GlobalVariableHandling
    {
        public struct OutComeValue
        {
            public int outComePosition;
            public string outComeName;
            public string outComeFormula;
        };
        public struct IfElseDetected
        {
            public int IfElseLineNo;
            public string IfElseContent;
            public string IfElseLevelDetection;
        //    public int intLevel;
        };
        public struct ConditionLevelDetected
        {
            public int position;
            public int level;
        };

        public struct GetStringBlock
        {
            public string content;
            public int level;
        }
        
    }
}
