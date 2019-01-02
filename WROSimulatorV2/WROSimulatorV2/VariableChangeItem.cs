using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{

    public class VariableChangeItem : VisulizableItem
    {
        public VariableVisulizeItem Variable { get; set; }
        public List<IGetSetFunc> MiddleItems { get; set; }
        public object Other { get; set; }
        int otherIndex = 1;
        Func<VariableChangeItem, List<IGetSetFunc>> getMiddleItems;
        bool haveVariableButton = false;
        public VariableChangeItem()
        {
            Variable = new VariableVisulizeItem();
            Variable.VariableChanged = VariableChanged;
            SetVisulizeItems();
            SetOtherToDefault();
        }
        public void SetGetMiddleItems(Func<VariableChangeItem, List<IGetSetFunc>> getMiddleItems, bool haveVariableButton)
        {
            this.haveVariableButton = haveVariableButton;
            this.getMiddleItems = getMiddleItems;
            MiddleItems = getMiddleItems.Invoke(this);
            SetVisulizeItems();
        }

        //private VariableChangeItem(VariableChangeItem other)
        //{
        //    getMiddleItems = other.getMiddleItems;
        //    MiddleItems = getMiddleItems.Invoke();
        //    for(int i =0; i < other.MiddleItems.Count; i++)
        //    {
        //        MiddleItems[i].ObjSet(other.MiddleItems[i].ObjGet(i + 1), i + 1);
        //    }
        //    Variable = other.Variable.CompleteCopy();
        //    Other = other.Other;
        //    SetVisulizeItems();
        //    SetOtherToDefault();
        //    SetOtherVisItem();
        //    Init(true);
        //    Variable.VariableChanged = VariableChanged;
        //}
        void SetVisulizeItems()
        {
            VisulizeItems = new List<IGetSetFunc>();

            if (haveVariableButton)
            {
                VisulizeItems.Add(new GetSetFunc<VariableVisulizeItem>((i) => Variable, (v, i) => Variable.SetVariable(v.Variable), "Variable"));
            }
            if (MiddleItems != null)
            {
                VisulizeItems.AddRange(MiddleItems);
            }
            otherIndex = VisulizeItems.Count;

            VisulizeItems.Add(new GetSetFunc<object>((i) => Other, (v, i) => Other = v, "Operatior"));

            SetOtherVisItem();
            Init(false);
        }
        public ControlNode VariableChanged(VariableGetSet variable, ControlNode node)
        {
            if (Other.GetType() != variable.Get().Type)
            {
                SetOtherToDefault();
                SetOtherVisItem();

                IndexInit(otherIndex, true);
                Form1.UpdateItem(ref node, node.Control.GetSetFunc, node.Control.Index, node.Control.Form);
            }
            return node;
        }
        void VariableChanged(VariableVisulizeItem item, LabeledControl labeledControl)
        {
            labeledControl.ParentNode = VariableChanged(item.Variable, labeledControl.ParentNode);
        }


        void SetOtherVisItem()
        {
            VisulizeItems[otherIndex].ItemInfo = new ItemInfo(Variable.Variable.Type, VisulizeItems[otherIndex].ItemInfo.Name);
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

        public override void CopyTo(VisulizableItem newItem)
        {
            VariableChangeItem variableChangeItem = (VariableChangeItem)newItem;
            variableChangeItem.getMiddleItems = getMiddleItems;
            variableChangeItem.MiddleItems = getMiddleItems.Invoke(variableChangeItem);
            for (int i = 0; i < MiddleItems.Count; i++)
            {
                variableChangeItem.MiddleItems[i].ObjSet(MiddleItems[i].ObjGet(i + 1), i + 1);
            }
            variableChangeItem.Variable = Variable.CompleteCopy();
            variableChangeItem.SetVisulizeItems();
            variableChangeItem.SetOtherVisItem();
            variableChangeItem.Init(true);
            variableChangeItem.Other = Other;
            variableChangeItem.Variable.VariableChanged = variableChangeItem.VariableChanged;
            //return new VariableChangeItem(this);
        }
        protected override void Deserialize(Span<char> span)
        {
            var list = DeserializeItems(span);
            for (int i = 0; i < VisulizeItems.Count; i++)
            {
                VisulizeItems[i].ObjSet(list[i].Value, i);
                VisulizeItems[i].Variable = VariablesInfo.GetVariableGetSet(list[i].Variable);
            }
            SetOtherVisItem();
            IndexInit(otherIndex, true);
        }
    }
}
