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
        public object CurrentPossiblility;
        HashSet<object> possibilities;
        Action<object, LabeledControl> currentPossiblilityChanged;
        public PossibleListItem()
            : this(null, null)
        {

        }
        public PossibleListItem(HashSet<object> possibilities, Action<object, LabeledControl> currentPossiblilityChanged)
        {
            this.currentPossiblilityChanged = currentPossiblilityChanged;
            this.possibilities = possibilities;
            SetDefaultCurrentPossiblility();
        }
        void SetDefaultCurrentPossiblility()
        {
            if (possibilities == null || possibilities.Count == 0)
            {
                CurrentPossiblility = null;
            }
            else
            {
                CurrentPossiblility = possibilities.First();
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
                if(v==CurrentPossiblility)
                {
                    selectedIndex = index;
                }
                comboBox.Items.Add(v);
                index++;
            }
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
            CurrentPossiblility = comboBox.Items[comboBox.SelectedIndex];
            currentPossiblilityChanged?.Invoke(CurrentPossiblility, labeledControl);
        }
    }
}
