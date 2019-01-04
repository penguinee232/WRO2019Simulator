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
        Action<IVariableGetSet> doneAction;
        Dictionary<Type, List<IVariableGetSet>> variables;
        int spaceAmount;
        Dictionary<ListBox, (int, IVariableGetSet)> listBoxIndexesAndParents;
        List<ListBox> variableListBoxes;
        IVariableGetSet currentVariable = null;
        public ChooseVariableForm(Form1 form, Action<IVariableGetSet> doneAction, TreeNode currentTreeNode)
        {
            InitializeComponent();
            currentVariable = null;
            variableListBoxes = new List<ListBox>() { variableNamesListBox };
            listBoxIndexesAndParents = new Dictionary<ListBox, (int, IVariableGetSet)>();
            listBoxIndexesAndParents.Add(variableNamesListBox, (0,null));
            this.doneAction = doneAction;
            previousForm = form;
            variableTypes = new List<Type>();
            variables = VariablesInfo.GetVariables(currentTreeNode);
            foreach (var t in variables)
            {
                variableTypesListBox.Items.Add(t.Key.GetTypeName());
                variableTypes.Add(t.Key);
            }
            variableTypesListBox.SelectedIndex = 0;
            spaceAmount = variableNamesListBox.Left - variableTypesListBox.Right;
        }

        private void variableTypesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            variableNamesListBox.Items.Clear();
            Type current = variableTypes[variableTypesListBox.SelectedIndex];
            foreach(var v in variables[current])
            {
                variableNamesListBox.Items.Add(v.ToString());
            }
            variableNamesListBox.SelectedIndex = 0;
        }

        private void variableListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox currentListBox = (ListBox)sender;
            var listBoxInfo = listBoxIndexesAndParents[currentListBox];
            Type current = variableTypes[variableTypesListBox.SelectedIndex];
            if (listBoxInfo.Item2 == null)
            {
                currentVariable = variables[current][currentListBox.SelectedIndex];
            }
            else
            {
                currentVariable = VariablesInfo.VariableGetSet[listBoxInfo.Item2.Children[currentListBox.SelectedIndex]];
            }
            for(int i = variableListBoxes.Count - 1; i > listBoxInfo.Item1; i--)
            {
                panel1.Controls.Remove(variableListBoxes[i]);
                listBoxIndexesAndParents.Remove(variableListBoxes[i]);
                variableListBoxes.RemoveAt(i);
            }
            if(currentVariable.Children != null && currentVariable.Children.Count > 0)
            {
                ListBox newListBox = new ListBox();
                newListBox.Size = variableNamesListBox.Size;
                ListBox lastListBox = variableListBoxes[variableListBoxes.Count - 1];
                newListBox.Location = new Point(lastListBox.Right + spaceAmount, lastListBox.Location.Y);
                for(int i = 0; i < currentVariable.Children.Count; i++)
                {
                    newListBox.Items.Add(currentVariable.Children[i].Name);
                }
                panel1.Controls.Add(newListBox);
                listBoxIndexesAndParents.Add(newListBox, (variableListBoxes.Count, currentVariable));
                variableListBoxes.Add(newListBox);
                newListBox.SelectedIndexChanged += variableListBox_SelectedIndexChanged;
            }
        }

        private void chooseVariableButton_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            doneAction?.Invoke(currentVariable);
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
