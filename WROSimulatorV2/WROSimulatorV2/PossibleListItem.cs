using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class PossibleListItem
    {
        public static Dictionary<PossibleListEnums, HashSet<object>> StaticPossiblilities;
        public int SelectedIndex { get; private set; }
        public object CurrentPossiblility;
        HashSet<object> possibilities;
        Action<object, LabeledControl> currentPossiblilityChanged;
        object defaultPossibility;
        PossibleListEnums Code = 0;
        public static void AddStaticPossiblilities(PossibleListEnums code, HashSet<object> possibilities)
        {
            if(StaticPossiblilities == null)
            {
                StaticPossiblilities = new Dictionary<PossibleListEnums, HashSet<object>>();
            }
            StaticPossiblilities.Add(code, possibilities);
        }
        public PossibleListItem()
            : this(null,PossibleListEnums.VariableTypes, null)
        {

        }
        public PossibleListItem(object defaultPossibility, PossibleListEnums code, Action<object, LabeledControl> currentPossiblilityChanged)
        {
            Code = code;
            this.defaultPossibility = defaultPossibility;
            this.currentPossiblilityChanged = currentPossiblilityChanged;
            if (StaticPossiblilities != null && StaticPossiblilities.ContainsKey(Code))
            {
                possibilities = StaticPossiblilities[Code];
            }
            SetDefaultCurrentPossiblility();
        }
        void SetDefaultCurrentPossiblility()
        {
            if (possibilities == null || possibilities.Count == 0)
            {
                CurrentPossiblility = null;
            }
            else if(defaultPossibility == null)
            {
                CurrentPossiblility = possibilities.First();
            }
            else
            {
                CurrentPossiblility = defaultPossibility;
            }
        }
        public ControlNode GetControlNode(string name, IGetSetFunc item, Point position, bool partOfRadioButtonGroup, Form1 form, ControlNode parent)
        {
            ComboBox comboBox = new ComboBox();
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            int selectedIndex = -1;
            int index = 0;
            foreach (var v in possibilities)
            {
                if(v.Equals(CurrentPossiblility))
                {
                    selectedIndex = index;
                }
                comboBox.Items.Add(v);
                index++;
            }
            SelectedIndex = selectedIndex;
            comboBox.SelectedIndex = selectedIndex;

            var control = Form1.GetLabeledControl(name, comboBox, item, index, parent, null, Form1.spaceAmount, partOfRadioButtonGroup, form);
            comboBox.SelectedIndexChanged += control.Control_ValueChanged;
            control.ValueChanged += Control_ValueChanged;

            control.Location = position;
            return new ControlNode(control, parent);

        }

        private void Control_ValueChanged(object sender, VisulizedItemEventArgs e)
        {
            LabeledControl labeledControl = (LabeledControl)sender;
            ComboBox comboBox = (ComboBox)(labeledControl).Control;
            SelectedIndex = comboBox.SelectedIndex;
            CurrentPossiblility = comboBox.Items[comboBox.SelectedIndex];
            currentPossiblilityChanged?.Invoke(CurrentPossiblility, labeledControl);
        }
        public string Serialize()
        {
            List<object> items = new List<object>() { Code, SelectedIndex };
            List<Variable?> variables = new List<Variable?>() { null, null };
            return VisulizableItem.Serialize(GetType(), items, variables);
        }
        public static PossibleListItem Deserialize(Span<char> span)
        {
            var list = VisulizableItem.DeserializeItems(span);
            var enumType = (PossibleListEnums)list[0].Value;
            var selectedIndex = (int)list[1].Value;
            var selectedObject = PossibleListItem.StaticPossiblilities[enumType].ToList()[selectedIndex];
            return new PossibleListItem(selectedObject, enumType, null);
        }
    }
}
