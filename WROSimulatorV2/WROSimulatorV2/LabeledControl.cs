using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public partial class LabeledControl : UserControl
    {
        public bool OpenToggle { get { return checkBox == null || checkBox.Checked; } }
        CheckBox checkBox = null;
        public Label Label { get; set; }
        public Control Control { get; set; }
        public ControlNode ControlNode { get; set; }
        public Point indentAmount;
        int spaceAmount;
        public IGetSetFunc GetSetFunc { get; set; }
        public int Index { get; set; }
        public event EventHandler<VisulizedItemEventArgs> ValueChanged;
        public ControlNode ParentNode { get; set; }
        public static readonly int CheckBoxSpace = 0;
        public RadioButtonGroup RadioButtonGroup = null;
        public Form1 Form;
        public VariableGetSet? PossibleVariable = null;
        RadioButton radioButton = null;
        List<Control> precedingControls;
        public Label VariableLabel = null;
        public bool HasToogle;
        public LabeledControl(string labelText, Control control, Point indentAmount, IGetSetFunc getSetFunc, int index, ControlNode parentNode, bool toggleable, int spaceAmount, Form1 form)
        {
            InitializeComponent();
            Form = form;
            precedingControls = new List<Control>();
            Index = index;
            ParentNode = parentNode;
            GetSetFunc = getSetFunc;
            this.spaceAmount = spaceAmount;
            this.indentAmount = indentAmount;
            Label = new Label();
            Label.AutoSize = true;
            Label.Location = new Point(0, 0);
            Label.Text = labelText;
            Control = control;
            Controls.Clear();

            checkBox = new CheckBox();
            checkBox.AutoSize = true;
            checkBox.Location = new Point(0, 0);
            checkBox.Text = "";
            checkBox.Checked = true;
            checkBox.Visible = false;
            checkBox.CheckedChanged += CheckBox_CheckedChanged;
            Controls.Add(checkBox);
            precedingControls.Add(checkBox);
            HasToogle = toggleable;
            if (toggleable)
            {
                checkBox.Visible = true;
            }
            precedingControls.Add(Label);
            //precedingControls.Add(VariableLabel);
            Controls.Add(Label);
            //Controls.Add(VariableLabel);
            Controls.Add(Control);
            foreach (var c in getSetFunc.Controls)
            {
                precedingControls.Add(c);
                Controls.Add(c);
            }
            SetSize();
        }

        

        public void SetEnable(bool enabled, bool labelEnable, object specificObject = null, bool objectEnable = false)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                Control c = Controls[i];
                if(c == specificObject)
                {
                    c.Enabled = objectEnable;
                }
                else if (c == Label)
                {
                    c.Enabled = labelEnable;
                }
                else if (c == Control)
                {
                    if (c.GetType() == typeof(Panel))
                    {
                        for (int j = 0; j < c.Controls.Count; j++)
                        {
                            if (c.Controls[j].GetType() != typeof(LabeledControl))
                            {
                                c.Controls[j].Enabled = enabled;
                            }
                        }
                    }
                    else
                    {
                        c.Enabled = enabled;
                    }
                }
                else
                {
                    c.Enabled = enabled;
                }
            }

        }

        public void MakeItemInCollection()
        {
            LabeledControl parentLabeledControl = (LabeledControl)ParentNode.Control;
            radioButton = new RadioButton();
            radioButton.AutoSize = true;
            radioButton.Text = "";
            radioButton.Location = new Point(0, 0);
            parentLabeledControl.RadioButtonGroup.Buttons.Add(radioButton);
            radioButton.Checked = Index == parentLabeledControl.RadioButtonGroup.SelectedIndex;
            radioButton.CheckedChanged += RadioButton_CheckedChanged;
            precedingControls.Insert(0, radioButton);
            Controls.Add(radioButton);
            SetSize();
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton.Checked)
            {
                LabeledControl parentLabeledControl = (LabeledControl)ParentNode.Control;
                parentLabeledControl.RadioButtonGroup.ChangeIndex(Index);
            }
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Control.Visible = checkBox.Checked;
            SetSize();
            if (ParentNode != null)
            {
                ParentNode.ReLocateChildren(spaceAmount);
            }
        }

        public void Control_ValueChanged(object sender, EventArgs e)
        {
            OnValueChanged(new VisulizedItemEventArgs(GetSetFunc, Index));
        }

        private void LabeledControl_Load(object sender, EventArgs e)
        {
        }
        public void SetSize()
        {
            if (GetSetFunc.ItemInfo.Type.IsSubclassOf(typeof(VisulizableItem)))
            {
                VisulizableItem visItem = (VisulizableItem)GetSetFunc.ObjGet(Index);
                visItem.Refresh();
            }
            if (GetSetFunc.IsVariable)
            {
                Control.Visible = false;
                if (!precedingControls.Contains(VariableLabel))
                {
                    VariableLabel = new Label();
                    VariableLabel.AutoSize = true;
                    VariableLabel.ForeColor = Form1.VariableColor;
                    precedingControls.Add(VariableLabel);
                    Controls.Add(VariableLabel);
                }
                VariableLabel.Text = GetSetFunc.Variable.Value.ToString();
            }
            else
            {
                if (Controls.Contains(VariableLabel))
                {
                    Controls.Remove(VariableLabel);
                    precedingControls.Remove(VariableLabel);
                }
                VariableLabel = null;
                Control.Visible = OpenToggle;
            }
            HashSet<Control> variableControls = new HashSet<Control>() { radioButton, Label, VariableLabel };
            HashSet<Control> alwaysShownControls = new HashSet<Control>() { radioButton, checkBox, Label, VariableLabel };
            Point last = new Point(0, 0);
            int maxHeight = 0;
            Control lastPrecedingControls = null;
            checkBox.Visible = HasToogle;
            foreach (var c in precedingControls)
            {
                if (c != null && (c != checkBox || checkBox.Visible))
                {
                    if ((GetSetFunc.IsVariable && !variableControls.Contains(c)) || (!Control.Visible && !alwaysShownControls.Contains(c)))
                    {
                        c.Visible = false;
                    }
                    else
                    {
                        c.Visible = true;
                    }
                }
                //if(!alwaysShownControls.Contains(c))
                //{
                //    c.Visible = Control.Visible;
                //}
                if (c != null && c.Visible)
                {
                    c.Location = last;
                    last = new Point(last.X + c.Width + spaceAmount, last.Y);
                    maxHeight = Math.Max(maxHeight, c.Size.Height);
                    lastPrecedingControls = c;
                }
            }
            last = new Point(last.X - spaceAmount, last.Y);
            if (indentAmount.Y > 0)
            {
                Control.Location = new Point(indentAmount.X, indentAmount.Y + maxHeight);
            }
            else
            {
                Control.Location = lastPrecedingControls.Location.Add(indentAmount);
                Control.Location = new Point(Control.Location.X + lastPrecedingControls.Size.Width, Control.Location.Y);
            }
            if (Control.Visible)
            {
                Size = Control.Location.Add(Control.Size.ToPoint()).ToSize();
            }
            else
            {
                Size = lastPrecedingControls.Location.Add(lastPrecedingControls.Size.ToPoint()).ToSize();
            }
            Size = new Size(Math.Max(Size.Width, last.X), Math.Max(Size.Height, maxHeight));
        }

        protected virtual void OnValueChanged(VisulizedItemEventArgs args)
        {
            EventHandler<VisulizedItemEventArgs> handler = ValueChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
    public class VisulizedItemEventArgs : EventArgs
    {
        public IGetSetFunc GetSetFunc { get; set; }
        public int Index { get; set; }
        public VisulizedItemEventArgs(IGetSetFunc getSetFunc, int index)
        {
            Index = index;
            GetSetFunc = getSetFunc;
        }
    }
    public class RadioButtonGroup
    {
        public List<RadioButton> Buttons { get; set; }
        public int SelectedIndex { get; private set; }
        public void ChangeIndex(int newIndex)
        {
            if (newIndex != SelectedIndex)
            {
                if (SelectedIndex < Buttons.Count)
                {
                    Buttons[SelectedIndex].Checked = false;
                }
                SelectedIndex = newIndex;
                Buttons[SelectedIndex].Checked = true;
            }
        }
        public RadioButtonGroup(List<RadioButton> buttons, int selectedIndex)
        {
            Buttons = buttons;
            SelectedIndex = selectedIndex;
        }
    }
}
