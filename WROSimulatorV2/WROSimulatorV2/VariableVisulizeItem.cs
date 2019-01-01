using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class VariableVisulizeItem : VisulizableItem
    {
        public Variable Variable;
        Label label;
        public Action<VariableVisulizeItem, LabeledControl> VariableChanged { get; set; }
        LabeledControl parent;
        public VariableVisulizeItem()
        {
            VariableChanged = null;
            Variable = Variable.Default();
            VisulizeItems = new List<IGetSetFunc>();
            Init(false);
        }
        public override List<Control> GetManatoryControls(IGetSetFunc getSetFunc, int index)
        {
            label = new Label();
            label.ForeColor = Form1.VariableColor;
            label.AutoSize = true;
            label.Text = Variable.ToString();
            Button button = new Button();
            button.Text = "Get Variable";
            button.AutoSize = true;
            button.Click += Button_Click;
            return new List<Control>() { label, button };
        }
        private void Button_Click(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            parent = (LabeledControl)control.Parent;
            parent.Form.ShowChooseVariableForm(NewVariable);
        }
        public void SetVariable(Variable v)
        {
            Variable = v;
            label.Text = Variable.ToString();
        }
        void NewVariable(Variable v)
        {
            Variable = v;
            label.Text = Variable.ToString();
            VariableChanged?.Invoke(this, parent);
            parent.ControlNode.ReLocateChildren(Form1.spaceAmount);
        }

        public override void CopyTo(VisulizableItem newItem)
        {
            VariableVisulizeItem item = (VariableVisulizeItem)newItem;
            item.label = label;
            item.Variable = Variable;
            item.VariableChanged = VariableChanged;
            item.parent = parent;
            CopyItems(item, this);
        }

        public VariableVisulizeItem CompleteCopy()
        {
            VariableVisulizeItem item = new VariableVisulizeItem();
            item.Variable = Variable;
            //item.NewVariable(Variable);
            item.VariableChanged = VariableChanged;
            CopyItems(item, this);
            return item;
        }
        public override string Serialize()
        {
            return VisulizableItem.SerializeType(GetType()) + "{" + Variable.Serialize() + "}";
        }
        protected override void Deserialize(Span<char> span)
        {
            var list = DeserializeItems(span);
            Variable = list[0].Variable.Value;
        }

        public bool IsTrue(CompareOperatiors operatior, object other)
        {
            object variableValue = Form1.GetVariable(Variable);
            switch (operatior)
            {
                case (CompareOperatiors.Equals):
                    if (variableValue == null && other != null)
                    {
                        return false;
                    }
                    return variableValue.Equals(other);
                case (CompareOperatiors.NotEqual):
                    if (variableValue == null && other != null)
                    {
                        return true;
                    }
                    return !variableValue.Equals(other);
                default:
                    if (variableValue.GetType().GetInterface("IComparable") != null)
                    {
                        return Extensions.CompareObjects(variableValue, operatior, other);
                        //IComparable variableCompare = (IComparable)variableValue;
                        //IComparable otherCompare = (IComparable)other;
                        //switch (operatior)
                        //{
                        //    case (CompareOperatiors.LessThan):
                        //        return variableCompare.CompareTo(other) < 0;
                        //    case (CompareOperatiors.GreaterThan):
                        //        return variableCompare.CompareTo(other) > 0;
                        //    case (CompareOperatiors.LessThanEqual):
                        //        return variableCompare.CompareTo(other) <= 0;
                        //    case (CompareOperatiors.GreaterThanEqual):
                        //        return variableCompare.CompareTo(other) >= 0;
                        //    default:
                        //        return false;
                        //}
                    }
                    else
                    {
                        return false;
                    }
            }
        }
    }
}
