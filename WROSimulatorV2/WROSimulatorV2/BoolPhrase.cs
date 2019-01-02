using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public class BoolPhrase : VariableChangeItem
    {
        public CompareOperatiors Operatior { get; set; }
        public BoolPhrase()
        {
            SetGetMiddleItems(GetMiddleItems, true);
        }
        public static List<IGetSetFunc> GetMiddleItems(VariableChangeItem item)
        {
            BoolPhrase boolPhrase = (BoolPhrase)item;
            return new List<IGetSetFunc>()
            {
                new GetSetFunc<CompareOperatiors>((i)=>boolPhrase.Operatior, (v,i) => boolPhrase.Operatior = v, "Operatior")
            };
        }

        public static string GetBoolPhrase(CommandsNode node)
        {
            string code = "";
            code += node.Paramaters[0].Paramaters[0].Variable.Value.Name + " ";
            CompareOperatiors operatior = (CompareOperatiors)node.Paramaters[1].Value;
            code += operatior.GetActualOperator() + " ";
            code += ToRobotLanguageConverter.GetCPlusPlusCommandNodeCode(node.Paramaters[2]);
            return code;
        }

        public bool IsTrue()
        {
            return Variable.IsTrue(Operatior, Other);
        }
    }
}
