using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public class SetVariablePhrase : VariableChangeItem
    {
        public SetOperatiors Operatior { get; set; }
        public SetVariablePhrase()
        {
            SetGetMiddleItems(GetMiddleItems);
        }
        public static List<IGetSetFunc> GetMiddleItems(VariableChangeItem item)
        {
            SetVariablePhrase setVariablePhrase = (SetVariablePhrase)item;
            return new List<IGetSetFunc>()
            {
                new GetSetFunc<SetOperatiors>((i)=>setVariablePhrase.Operatior, (v,i) => setVariablePhrase.Operatior = v, "Operatior")
            };
        }

        public static string GetBoolPhrase(CommandsNode node)
        {
            string code = "";
            code += node.Paramaters[0].Paramaters[0].Variable.Value.Name + " ";
            SetOperatiors operatior = (SetOperatiors)node.Paramaters[1].Value;
            code += operatior.GetActualOperator() + " ";
            code += ToRobotLanguageConverter.GetCPlusPlusCommandNodeCode(node.Paramaters[2]);
            return code;
        }

        public void SetVariableValue()
        {
            MathOperatiors? math = Operatior.GetMathOperator();
            object newVarialbeValue;
            if (math != null)
            {
                object variableValue = Form1.GetVariable(Variable.Variable);
                newVarialbeValue = Extensions.UseMathOnObject(variableValue, math.Value, Other);
            }
            else
            {
                newVarialbeValue = Other;
            }
            Form1.SetVariable(Variable.Variable, newVarialbeValue);
        }
    }
}
