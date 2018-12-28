using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public partial class ChooseVariableForm : Form
    {
        List<Type> variableTypes;
        Form1 previousForm;
        Action<Variable> doneAction;
        public ChooseVariableForm(Form1 form, Action<Variable> doneAction)
        {
            InitializeComponent();
            this.doneAction = doneAction;
            previousForm = form;
            variableTypes = new List<Type>(); 
            foreach (var t in Form1.VariablesByType)
            {
                variableTypesListBox.Items.Add(t.Key.GetTypeName());
                variableTypes.Add(t.Key);
            }
            variableTypesListBox.SelectedIndex = 0;
        }

        private void variableTypesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            variableNamesListBox.Items.Clear();
            Type current = variableTypes[variableTypesListBox.SelectedIndex];
            foreach(var v in Form1.VariablesByType[current])
            {
                variableNamesListBox.Items.Add(v.Name);
            }
            variableNamesListBox.SelectedIndex = 0;
        }

        private void variableNamesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chooseVariableButton_Click(object sender, EventArgs e)
        {
            Type current = variableTypes[variableTypesListBox.SelectedIndex];
            previousForm.Show();
            doneAction?.Invoke(Form1.VariablesByType[current][variableNamesListBox.SelectedIndex]);
            //previousForm.SetVariable(Form1.VariablesByType[current][variableNamesListBox.SelectedIndex]);
            Close();
        }

        private void ChooseVariableForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!previousForm.Visible)
            {
                previousForm.Show();
            }
        }

        private void ChooseVariableForm_Load(object sender, EventArgs e)
        {

        }
    }
}
