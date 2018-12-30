using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public class BoolPhrase:VisulizableItem
    {
        public VariableVisulizeItem Variable { get; set; }
        public Operatiors Operatior { get; set; }
        public object Other { get; set; }
        static readonly int otherIndex = 2;
        public BoolPhrase()
        {
            Variable = new VariableVisulizeItem();
            Variable.VariableChanged = VariableChanged;
            Operatior = Operatiors.Equals;
            SetOtherToDefault();
            SetVisulizeItems();
        }
        private BoolPhrase(BoolPhrase other)
        {
            Operatior = other.Operatior;
            Variable = other.Variable.CompleteCopy();
            SetOtherToDefault();
            Other = other.Other;
            SetVisulizeItems();
            SetOtherVisItem();
            Init(true);
            Variable.VariableChanged = VariableChanged;
        }
        void SetVisulizeItems()
        {
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<VariableVisulizeItem>((i)=>Variable, (v,i)=>Variable.SetVariable(v.Variable), "Variable"),
                new GetSetFunc<Operatiors>((i)=>Operatior, (v,i)=>Operatior = v, "Operatior"),
                new GetSetFunc<object>((i)=>Other, (v,i)=>Other = v, "Operatior")
            };
            SetOtherVisItem();
            Init(false);
        }
        void VariableChanged(VariableVisulizeItem item, LabeledControl labeledControl)
        {
            if (Other.GetType() != item.Variable.Type)
            {
                SetOtherToDefault();
                SetOtherVisItem();

                IndexInit(otherIndex, true);

                var node = labeledControl.ParentNode;//.Children[otherIndex];
                Form1.UpdateItem(ref node, node.Control.GetSetFunc, node.Control.Index, node.Control.Form);
                labeledControl.ParentNode = node;
                //Extensions.UpdateVisual(parent, parent.Form);
            }
        }
        void SetOtherVisItem()
        {
            VisulizeItems[otherIndex].ItemInfo = new ItemInfo(Variable.Variable.Type, VisulizeItems[2].ItemInfo.Name);
        }

        public void SetOtherToDefault()
        {
            if (Variable.Variable.Type.IsClass)
            {
                Other = Extensions.GetDefaultFromConstructor(Variable.Variable.Type);
            }
            else
            {
                Other = Extensions.GetDefault(Variable.Variable.Type);
            }
        }

        public override VisulizableItem Copy()
        {
            return new BoolPhrase(this);
        }
        public bool IsTrue()
        {
            return Variable.IsTrue(Operatior, Other);
        }
        protected override void Deserialize(Span<char> span)
        {
            var list = DeserializeItems(span);
            for (int i = 0; i < VisulizeItems.Count; i++)
            {
                //if (list[i].Variable == null)
                //{
                VisulizeItems[i].ObjSet(list[i].Value, i);
                //}
                VisulizeItems[i].Variable = list[i].Variable;
            }
            SetOtherVisItem();
            IndexInit(otherIndex, true);
        }
        public static string GetBoolPhrase(CommandsNode node)
        {
            string code = "";
            code += node.Paramaters[0].Paramaters[0].Variable.Value.Name + " ";
            Operatiors operatior = (Operatiors)node.Paramaters[1].Value;
            code += operatior.GetActualOperator() + " ";
            code += ToRobotLanguageConverter.GetCPlusPlusCommandNodeCode(node.Paramaters[2]);
            return code;
        }
    }
}
