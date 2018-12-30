using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WROSimulatorV2.Properties;

namespace WROSimulatorV2
{
    public partial class Form1 : Form
    {
        public static Dictionary<Type, List<Variable>> VariablesByType;
        public static Dictionary<Variable, object> VariableValues;
        static Point labelSpaceAmount = new Point(5, 0);
        static Point labelIndextAmount = new Point(25, 5);
        public static int spaceAmount = 10;
        public Panel Panel;
        List<Control> miscControls;
        List<Control> needOneSelectedRunCommandControls;
        List<Control> needAnySelectedRunCommandControls;
        public static List<Type> Commands;
        public ControlNode root;
        public static Color VariableColor = Color.Blue;
        public TreeNode CurrentTreeNode { get { return runCommandsTreeView.SelectedNode; } }
        HashSet<Command> selectedCommands;
        HashSet<TreeNode> selectedCommandsTreeNodes;
        HashSet<TreeNode> selectedTreeNodes;
        HashSet<TreeNode> lastCurrentTreeNodes;
        TreeNode lastSelectedNode;
        public Dictionary<TreeNode, Command> CommandsFromTreeNodes;
        public HashSet<TreeNode> DontLookAtOtherChildrenTreeNodes;
        PictureBox robotCanvas;
        Bitmap robotDrawArea;
        Graphics robotGfx;
        public Bitmap robotImage;
        public Robot robot;
        RCM rcm;
        TreeNode programNode;
        TreeNode startProgramNode;
        TreeNode lastStartProgramNode;
        Color standardBackColor;
        HashSet<TreeNode> lastCurrentlyRunningCommands;
        public PictureBox possibleCanvas;
        public Graphics possibleCanvasGfx;
        Bitmap possibleCanvasDrawArea;
        public Bitmap trasparentRobotImage;
        public int possibleAlphaValue = 150;
        public PictureBox FieldPictureBox;
        static Form1 debugForm;
        string currentFile = null;
        HashSet<TreeNode> breakPointedNodes;
        public Form1()
        {
            InitializeComponent();
            debugForm = this;
            FieldPictureBox = fieldPictureBox;
            standardBackColor = runCommandsTreeView.BackColor;
            Panel = panel1;

            #region Variables
            VariablesByType = new Dictionary<Type, List<Variable>>();
            Variable floatVariable1 = new Variable(typeof(float), "floatVariable1");
            Variable MyVector2Variable1 = new Variable(typeof(MyVector2), "MyVector2Variable1");
            VariablesByType.Add(typeof(float), new List<Variable>() { floatVariable1, new Variable(typeof(float), "floatVariable2") });
            VariablesByType.Add(typeof(int), new List<Variable>() { new Variable(typeof(int), "intVariable1"), new Variable(typeof(int), "TestVariable") });
            VariablesByType.Add(typeof(MyVector2), new List<Variable>() { MyVector2Variable1 });
            VariablesByType.Add(typeof(VisulizeableList<string>), new List<Variable>() { new Variable(typeof(VisulizeableList<string>), "idk") });
            VariablesByType.Add(typeof(bool), new List<Variable>() { new Variable(typeof(bool), "BoolVar") });
            VariableValues = new Dictionary<Variable, object>();
            foreach (var vt in VariablesByType)
            {
                foreach (var v in vt.Value)
                {
                    if (!vt.Key.IsClass)
                    {
                        VariableValues.Add(v, Extensions.GetDefault(vt.Key));
                    }
                    else
                    {
                        VariableValues.Add(v, Extensions.GetDefaultFromConstructor(vt.Key));
                    }
                }
                //if (vt.Key.IsClass)
                //{
                //    Variable v = new Variable(vt.Key, "null");
                //    vt.Value.Add(v);
                //    VariableValues.Add(v, null);
                //}
            }
            VariableValues[floatVariable1] = 3.1415f;
            VariableValues[MyVector2Variable1] = new MyVector2(4, 2);
            #endregion

            miscControls = new List<Control>() { setVariableButton, unSetVariableButton, commandListBox, addCommandButton, removeCommandButton, runCommandsTreeView, runCommandsButton, setAsStartButton, resetRobotButton, autoResetRobotCheckBox, saveButton, saveAsButton, openFileButton, breakpointButton };
            needAnySelectedRunCommandControls = new List<Control>() { removeCommandButton, breakpointButton };
            needOneSelectedRunCommandControls = new List<Control>() { setVariableButton, unSetVariableButton };
            //needSelectedRunCommandControls = new List<Control>() { setVariableButton, unSetVariableButton, removeCommandButton };

            Commands = new List<Type>() { typeof(DriveByMillis), typeof(MoveMotorByMillis), typeof(IfStatement), typeof(MultiAction), typeof(WhileCommand) };
            foreach (var t in Commands)
            {
                commandListBox.Items.Add(t.GetTypeName());
            }
            commandListBox.SelectedIndex = 0;



            programNode = new TreeNode("Program");
            runCommandsTreeView.Nodes.Add(programNode);
            InitCommands();

            #region CanvasStuff
            robotCanvas = new PictureBox();
            fieldPictureBox.Controls.Add(robotCanvas);
            robotCanvas.Size = fieldPictureBox.Size;
            robotCanvas.Location = new Point(0, 0);
            robotCanvas.BackColor = Color.Transparent;
            robotDrawArea = new Bitmap(robotCanvas.Size.Width, robotCanvas.Size.Height);
            robotCanvas.Image = robotDrawArea;
            robotGfx = Graphics.FromImage(robotDrawArea);

            possibleCanvas = new PictureBox();
            robotCanvas.Controls.Add(possibleCanvas);
            possibleCanvas.Size = fieldPictureBox.Size;
            possibleCanvas.Location = new Point(0, 0);
            possibleCanvas.BackColor = Color.Transparent;
            possibleCanvasDrawArea = new Bitmap(possibleCanvas.Size.Width, possibleCanvas.Size.Height);
            possibleCanvas.Image = possibleCanvasDrawArea;
            possibleCanvasGfx = Graphics.FromImage(possibleCanvasDrawArea);
            #endregion

            DontLookAtOtherChildrenTreeNodes = new HashSet<TreeNode>();
            FieldAndRobotInfo.Init(fieldPictureBox.Size.ToPoint());



            robotImage = Resources.abstractRobot;
            trasparentRobotImage = new Bitmap(robotImage);
            for (int x = 0; x < trasparentRobotImage.Size.Width; x++)
            {
                for (int y = 0; y < trasparentRobotImage.Size.Height; y++)
                {
                    Color c = trasparentRobotImage.GetPixel(x, y);
                    trasparentRobotImage.SetPixel(x, y, Color.FromArgb(possibleAlphaValue, c.R, c.G, c.B));
                }
            }

            robot = new Robot(robotImage);
            rcm = new RCM(robot);
            breakPointedNodes = new HashSet<TreeNode>();
            lastCurrentlyRunningCommands = new HashSet<TreeNode>();
            //robot.Update();
            // robot.Draw(robotGfx);
        }

        void InitCommands()
        {
            programNode.Nodes.Clear();
            runCommandsTreeView.SelectedNode = programNode;
            selectedTreeNodes = new HashSet<TreeNode>() { programNode };
            lastCurrentTreeNodes = new HashSet<TreeNode>() { programNode };
            lastSelectedNode = programNode;
            runCommandsTreeView.SelectedNode = programNode;
            startProgramNode = programNode;
            lastStartProgramNode = programNode;
            CommandsFromTreeNodes = new Dictionary<TreeNode, Command>();

            if (robot != null)
            {
                robot.ResetRobot();
            }
            if (rcm != null)
            {
                StopRunningCommands(false);
            }
        }

        TreeNode currentlyRunningNode;
        bool breakPointMode = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //string serialize = CommandsFromTreeNodes[programNode.FirstNode].Serialize();
            if (runningCommandsMode && !pauseMode)
            {
                bool working = rcm.Update(out currentlyRunningNode);
                if (!working)
                {
                    StopRunningCommands(false);
                }
                else
                {
                    HashSet<TreeNode> lastCommands = new HashSet<TreeNode>(lastCurrentlyRunningCommands);
                    RunningTreeColoring();
                    breakPointMode = false;
                    foreach (var n in currentlyRunnningNodes)
                    {
                        if (!lastCommands.Contains(n) && breakPointedNodes.Contains(n))
                        {
                            breakPointMode = true;
                            pauseMode = true;
                            break;
                        }
                    }
                    if (pauseMode)
                    {
                        StopRunningCommands(pauseMode);
                    }
                }
            }
            robot.Update();
            robotGfx.Clear(Color.Transparent);
            robot.Draw(robotGfx);
            robotCanvas.Image = robotDrawArea;
            possibleCanvas.Image = possibleCanvasDrawArea;
        }
        HashSet<TreeNode> currentlyRunnningNodes;
        TreeNode lastRunningTreeNode = null;
        void RunningTreeColoring()
        {
            currentlyRunnningNodes = new HashSet<TreeNode>();
            if (currentlyRunningNode != null && runningCommandsMode)
            {
                lastRunningTreeNode = currentlyRunningNode;
                GetAllChildren(currentlyRunningNode, currentlyRunnningNodes);
            }

            if (lastCurrentlyRunningCommands != null)
            {
                foreach (var n in lastCurrentlyRunningCommands)
                {
                    SetTreeNodeColor(n);
                }
            }

            foreach (var n in currentlyRunnningNodes)
            {
                SetTreeNodeColor(n);
                //n.ForeColor = Color.Black;
                //n.BackColor = Color.Yellow;
            }

            lastCurrentlyRunningCommands = currentlyRunnningNodes;
        }

        void GetAllChildren(TreeNode currentNode, HashSet<TreeNode> treeNodes)
        {
            treeNodes.Add(currentNode);
            foreach (TreeNode n in currentNode.Nodes)
            {
                GetAllChildren(n, treeNodes);
            }
        }


        public void EnableMiscControls(bool enable, Control ignoreControl)
        {
            foreach (var c in miscControls)
            {
                if (c != ignoreControl)
                {
                    c.Enabled = enable;
                }
                if (c == saveButton && enable)
                {
                    saveButton.Enabled = currentFile != null;
                }
            }
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateCommandPanel();
        }


        public static ControlNode LoadItem(IGetSetFunc item, Point position, ControlNode parent, int index, bool partOfRadioButtonGroup, Form1 form)
        {
            string name = item.ItemInfo.Name;
            if (item.ItemInfo.Type.IsSubclassOf(typeof(VisulizableItem)))
            {
                Panel panel = new Panel();
                List<ControlNode> controls = new List<ControlNode>();
                VisulizableItem visulizableItem = (VisulizableItem)item.ObjGet(index);
                var labeledControl = GetLabeledControl(name, panel, item, index, parent, visulizableItem, spaceAmount, partOfRadioButtonGroup, form);
                
                labeledControl.Location = position;
                var node = new ControlNode(labeledControl, parent);
                visulizableItem.ControlNode = node;
                if (item.ItemInfo.Type.GetInterfaces().Contains(typeof(IVisulizeableList)))
                {
                    labeledControl.RadioButtonGroup = new RadioButtonGroup(new List<RadioButton>(), 0);
                }
                for (int i = 0; i < visulizableItem.VisulizeItems.Count; i++)
                {
                    IGetSetFunc getSetFunc = visulizableItem.VisulizeItems[i];

                    var control = LoadItem(getSetFunc, new Point(0, 0), node, i, labeledControl.RadioButtonGroup != null, form);

                    panel.Controls.Add(control.Control);
                    controls.Add(control);
                }

                node.Children = controls;
                node.ReLocateChildren(spaceAmount);
                return node;
            }
            else
            {
                if (item.ItemInfo.Type.IsEnum)
                {
                    ComboBox comboBox = new ComboBox();

                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    var values = Enum.GetValues(item.ItemInfo.Type);
                    foreach (var v in values)
                    {
                        if (item.IsValidItem(v))
                        {
                            comboBox.Items.Add(v);
                        }
                    }
                    comboBox.SelectedIndex = comboBox.Items.IndexOf(item.ObjGet(index));

                    var control = GetLabeledControl(name, comboBox, item, index, parent, null, spaceAmount, partOfRadioButtonGroup, form);
                    comboBox.SelectedIndexChanged += control.Control_ValueChanged;
                    control.ValueChanged += Control_TextValueChanged;

                    control.Location = position;
                    return new ControlNode(control, parent);
                }
                else if (item.ItemInfo.Type == typeof(bool))
                {
                    CheckBox cBox = new CheckBox();
                    cBox.AutoSize = true;
                    cBox.Text = "";
                    cBox.Checked = (bool)item.ObjGet(index);
                    var control = GetLabeledControl(name, cBox, item, index, parent, null, spaceAmount, partOfRadioButtonGroup, form);
                    cBox.CheckedChanged += control.Control_ValueChanged;
                    control.ValueChanged += Bool_ValueChanged;
                    control.Location = position;

                    return new ControlNode(control, parent);
                }
                else
                {
                    var textBox = new TextBox();
                    textBox.Text = item.ObjGet(index).ToString();
                    var control = GetLabeledControl(name, textBox, item, index, parent, null, spaceAmount, partOfRadioButtonGroup, form);
                    textBox.TextChanged += control.Control_ValueChanged;
                    control.ValueChanged += Control_TextValueChanged;
                    control.Location = position;

                    return new ControlNode(control, parent);
                }
            }
        }

        public static bool UpdateItem(ref ControlNode node, IGetSetFunc item, int index, Form1 form)
        {
            string name = item.ItemInfo.Name;
            node.Control.Label.Text = name + " (" + item.ItemInfo.Type.GetTypeName() + ")";
            //if (item.ItemInfo.Type == node.Control.GetSetFunc.ItemInfo.Type)
            //{
            node.Control.Index = index;
            node.Control.Form = form;
            if (item.ItemInfo.Type.IsSubclassOf(typeof(VisulizableItem)))
            {
                VisulizableItem visulizableItem = (VisulizableItem)item.ObjGet(index);
                visulizableItem.ControlNode = node;
                if (node.Control.Control.GetType() != typeof(Panel))
                {
                    node = LoadItem(item, new Point(0, 0), node.Parent, index, node.Parent == null ? false : node.Parent.Control.RadioButtonGroup != null, form);
                    return true;
                }
                else
                {
                    for (int i = visulizableItem.VisulizeItems.Count; i < node.Children.Count; i++)
                    {
                        node.Control.Control.Controls.RemoveAt(i);
                        node.Children.RemoveAt(i);
                    }
                    for (int i = 0; i < visulizableItem.VisulizeItems.Count; i++)
                    {
                        IGetSetFunc getSetFunc = visulizableItem.VisulizeItems[i];
                        if (i >= node.Children.Count)
                        {
                            var control = LoadItem(getSetFunc, new Point(0, 0), node, i, node.Control.RadioButtonGroup != null, form);
                            node.Control.Control.Controls.Add(control.Control);
                            node.Children.Add(control);
                        }
                        else
                        {
                            //bool differentType = getSetFunc.ItemInfo.Type != node.Children[i].Control.GetSetFunc.ItemInfo.Type;
                            ControlNode child = node.Children[i];
                            bool differentType = UpdateItem(ref child, getSetFunc, i, form);
                            node.Children[i] = child;

                            node.Children[i].Parent = node;
                            node.Children[i].Control.ParentNode = node;
                            node.Children[i].Control.ControlNode = node.Children[i];
                            if (differentType)
                            {
                                var controls = node.Control.Control.Controls;
                                //node.Control.Controls.Clear();
                                controls.RemoveAt(i);
                                controls.Add(node.Children[i].Control);
                                controls.SetChildIndex(node.Children[i].Control, i);
                                
                                // node.Control.Controls. = node.Children[i].Control
                            }
                        }
                    }
                    node.Control.HasToogle = visulizableItem.VisulizeItems.Count > 0;
                }
                node.ReLocateChildren(spaceAmount);
            }
            else
            {
                if (node.Control.Control.GetType() == typeof(Panel))
                {
                    node = LoadItem(item, new Point(0, 0), node.Parent, index, node.Parent == null ? false : node.Parent.Control.RadioButtonGroup != null, form);
                    node.ReLocateChildren(spaceAmount);
                    return true;
                }
                else
                {
                    if (item.ItemInfo.Type == typeof(bool))
                    {
                        CheckBox cBox = (CheckBox)node.Control.Control;
                        cBox.Checked = (bool)item.ObjGet(index);
                    }
                    else
                    {
                        node.Control.Control.Text = item.ObjGet(index).ToString();
                    }
                }
            }
            return false;
            //}
            //else
            //{
            //    node = LoadItem(item, new Point(0, 0), node.Parent, index, node.Parent == null ? false : node.Parent.Control.RadioButtonGroup != null, form);
            //}
        }

        private static void Bool_ValueChanged(object sender, VisulizedItemEventArgs e)
        {
            LabeledControl control = (LabeledControl)sender;
            CheckBox cBox = (CheckBox)control.Control;
            if (e.GetSetFunc.IsValidItem(cBox.Checked))
            {
                e.GetSetFunc.ObjSet(cBox.Checked, e.Index);
            }
            else
            {
                cBox.Checked = (bool)e.GetSetFunc.ObjGet(e.Index);
            }
        }

        private static void Control_TextValueChanged(object sender, VisulizedItemEventArgs e)
        {
            LabeledControl control = (LabeledControl)sender;
            var converter = TypeDescriptor.GetConverter(e.GetSetFunc.ItemInfo.Type);
            bool valid = false;
            if (converter.IsValid(control.Control.Text))
            {
                var result = converter.ConvertFrom(control.Control.Text);
                if (e.GetSetFunc.IsValidItem(result))
                {
                    e.GetSetFunc.ObjSet(result, e.Index);
                    valid = true;
                }
            }
            if (valid)
            {
                control.Control.ForeColor = Color.Black;
            }
            else
            {
                control.Control.ForeColor = Color.Red;
            }

        }

        static LabeledControl GetLabeledControl(string name, Control control, IGetSetFunc getSetFunc, int index, ControlNode parentNode, VisulizableItem visulizableItem, int spaceAmount, bool partOfRadioButtonGroup, Form1 form)
        {
            bool isVisulizableItem = visulizableItem != null && visulizableItem.VisulizeItems.Count > 0;
            LabeledControl labeledControl = new LabeledControl(name + " (" + getSetFunc.ItemInfo.Type.GetTypeName() + ")" + ":", control, GetIndentAmount(visulizableItem), getSetFunc, index, parentNode, isVisulizableItem, spaceAmount, form);
            if (partOfRadioButtonGroup)
            {
                labeledControl.MakeItemInCollection();
            }
            return labeledControl;
        }
        static Point GetIndentAmount(VisulizableItem item)
        {
            bool isVisulizableItem = item != null;
            return isVisulizableItem ? labelIndextAmount : labelSpaceAmount;
        }

        public static void ApplyToControlNodes(ControlNode node, Action<ControlNode, object> action, object info)
        {
            action.Invoke(node, info);
            foreach (var c in node.Children)
            {
                ApplyToControlNodes(c, action, info);
            }
        }

        private void setVariableButton_Click(object sender, EventArgs e)
        {
            if (OnSetVariableMode)
            {
                StopSetVariableMode();
            }
            else
            {
                ShowChooseVariableForm(SetVariable);
            }
        }

        public void ShowChooseVariableForm(Action<Variable> doneAction)
        {
            ChooseVariableForm chooseVariableForm = new ChooseVariableForm(this, doneAction);
            chooseVariableForm.Show();
            this.Hide();
        }
        public bool OnSetVariableMode = false;
        bool unSet = false;
        public void StopSetVariableMode()
        {
            OnSetVariableMode = false;
            ApplyToControlNodes(root, (c, o) => ControlSetVariableStuff(c, false, (Type)o, new Variable()), null);
            if (unSet)
            {
                unSetVariableButton.Text = "Un Set Variable";
            }
            else
            {
                setVariableButton.Text = "Set Variable";
            }
            EnableMiscControls(true, null);
        }
        public void SetVariable(Variable variable)
        {
            setVariableButton.Text = "Cancel Variable";
            OnSetVariableMode = true;
            unSet = false;

            EnableMiscControls(false, setVariableButton);
            ApplyToControlNodes(root, (c, o) => ControlSetVariableStuff(c, true, (Type)o, variable), variable.Type);
        }
        public void UnSetVariable()
        {
            unSetVariableButton.Text = "Cancel Un Set Variable";
            OnSetVariableMode = true;
            unSet = true;

            EnableMiscControls(false, unSetVariableButton);
            ApplyToControlNodes(root, (c, o) => ControlSetVariableStuff(c, true, (Type)o, null), null);
        }
        void ControlSetVariableStuff(ControlNode c, bool setVariable, Type t, Variable? v)
        {
            if (setVariable)
            {
                c.Control.SetEnable(false, v != null ? t.CanImplicitCast(c.Control.GetSetFunc.ItemInfo.Type) : c.Control.VariableLabel != null);
                c.Control.PossibleVariable = v;
                c.Control.Label.Click += Label_Click;
            }
            else
            {
                c.Control.PossibleVariable = null;
                c.Control.SetEnable(true, true);
                c.Control.Label.Click -= Label_Click;
            }
        }


        private void Label_Click(object sender, EventArgs e)
        {
            if (OnSetVariableMode)
            {
                Control control = (Control)sender;
                LabeledControl parent = (LabeledControl)control.Parent;
                parent.GetSetFunc.Variable = parent.PossibleVariable;

                parent.ControlNode.ReLocateChildren(spaceAmount);
                parent.Form.StopSetVariableMode();
            }
        }

        private void unSetVariableButton_Click(object sender, EventArgs e)
        {
            if (OnSetVariableMode)
            {
                StopSetVariableMode();
            }
            else
            {
                UnSetVariable();
            }
        }

        private void addCommandButton_Click(object sender, EventArgs e)
        {
            //Type commandType = Commands[commandListBox.SelectedIndex];
            //Command command = (Command)Extensions.GetDefaultFromConstructor(commandType);
            //command.Form = this;
            Command command = GetSelectedCommand();
            AddCommand(command, CurrentTreeNode, this);

        }
        public static TreeNode AddCommand(Command command, TreeNode treeNode, Form1 form)
        {
            TreeNode newTreeNode = null;
            command.Form = form;
            if (form.CommandsFromTreeNodes.ContainsKey(treeNode))
            {
                if (treeNode.Parent != null && treeNode.Parent.Parent != null && form.CommandsFromTreeNodes.ContainsKey(treeNode.Parent.Parent))
                {
                    command.Parent = form.CommandsFromTreeNodes[treeNode.Parent.Parent];
                }
                if (command.Parent == null || command.Parent.CanAdd(command))
                {
                    newTreeNode = treeNode.Parent.Nodes.Insert(treeNode.Index + 1, command.Name);
                    treeNode.Parent.Expand();
                }
            }
            else
            {
                if (treeNode.Parent != null && form.CommandsFromTreeNodes.ContainsKey(treeNode.Parent))
                {
                    command.Parent = form.CommandsFromTreeNodes[treeNode.Parent];
                }
                if (command.Parent == null || command.Parent.CanAdd(command))
                {
                    newTreeNode = treeNode.Nodes.Insert(0, command.Name);
                    treeNode.Expand();
                }
            }
            if (newTreeNode != null)
            {
                command.SetCommandTreeNode(newTreeNode);


                form.CommandsFromTreeNodes.Add(newTreeNode, command);
            }
            return newTreeNode;
        }

        public Command GetSelectedCommand()
        {
            Type commandType = Commands[commandListBox.SelectedIndex];
            Command command = (Command)Extensions.GetDefaultFromConstructor(commandType);
            command.Form = this;
            return command;
        }

        private void removeCommandButton_Click(object sender, EventArgs e)
        {
            RemoveTreeNode(selectedCommandsTreeNodes);
        }

        void RemoveTreeNode(HashSet<TreeNode> nodes)
        {
            foreach (var n in nodes)
            {
                if (CommandsFromTreeNodes.ContainsKey(n))
                {
                    CommandsFromTreeNodes.Remove(n);
                    n.Remove();
                    UpdateCommandPanel();
                }
            }
        }

        void UpdateCommandPanel()
        {
            panel1.Controls.Clear();
            if (CommandsFromTreeNodes.ContainsKey(CurrentTreeNode))
            {
                foreach (var c in needAnySelectedRunCommandControls)
                {
                    c.Enabled = true;
                }
                foreach (var c in needOneSelectedRunCommandControls)
                {
                    c.Enabled = selectedTreeNodes.Count <= 1;
                }
                if (selectedTreeNodes.Count <= 1)
                {
                    Command command = CommandsFromTreeNodes[CurrentTreeNode];
                    root = LoadItem(new GetSetFunc<Command>((i) => command, (v, i) => command = v, command.Name), new Point(0, 0), null, 0, false, this);

                    panel1.Controls.Add(root.Control);
                }
            }
            else
            {
                foreach (var c in needAnySelectedRunCommandControls)
                {
                    c.Enabled = false;
                }
                foreach (var c in needOneSelectedRunCommandControls)
                {
                    c.Enabled = false;
                }
            }
        }


        private void runCommandsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedTreeNodes = new HashSet<TreeNode>();
            selectedCommandsTreeNodes = new HashSet<TreeNode>();
            selectedCommands = new HashSet<Command>();
            foreach (var tn in lastCurrentTreeNodes)
            {
                SetTreeNodeColor(tn);
            }

            //lastCurrentTreeNode.BackColor = runCommandsTreeView.BackColor;
            //lastCurrentTreeNode.ForeColor = Color.Black;

            if (Control.ModifierKeys == Keys.Shift)
            {
                GetNodesInBetween(lastSelectedNode, runCommandsTreeView.SelectedNode, selectedTreeNodes, selectedCommandsTreeNodes);
                selectedCommandsTreeNodes.OrderBy((tn) => tn.Level);
                foreach (var tn in selectedCommandsTreeNodes)
                {
                    selectedCommands.Add(CommandsFromTreeNodes[tn]);
                }
            }
            else
            {
                selectedTreeNodes.Add(runCommandsTreeView.SelectedNode);
                if (CommandsFromTreeNodes.ContainsKey(runCommandsTreeView.SelectedNode))
                {
                    selectedCommandsTreeNodes.Add(runCommandsTreeView.SelectedNode);
                    selectedCommands.Add(CommandsFromTreeNodes[runCommandsTreeView.SelectedNode]);
                }
            }
            foreach (var tn in selectedTreeNodes)
            {
                SetTreeNodeColor(tn);
            }

            //runCommandsTreeView.SelectedNode.BackColor = Color.CornflowerBlue;
            //runCommandsTreeView.SelectedNode.ForeColor = Color.White;
            lastSelectedNode = runCommandsTreeView.SelectedNode;
            lastCurrentTreeNodes = selectedTreeNodes;
            UpdateCommandPanel();
        }

        void GetNodesInBetween(TreeNode node1, TreeNode node2, HashSet<TreeNode> treeNodes, HashSet<TreeNode> commandTreeNodes)
        {
            if (node1.Level == node2.Level)
            {
                TreeNode firstNode;
                TreeNode secondNode;
                if (node1.Index <= node2.Index)
                {
                    firstNode = node1;
                    secondNode = node2;
                }
                else
                {
                    firstNode = node2;
                    secondNode = node1;
                }
                TreeNode current = firstNode;
                while (current != secondNode)
                {
                    treeNodes.Add(current);
                    if (CommandsFromTreeNodes.ContainsKey(current))
                    {
                        commandTreeNodes.Add(current);
                    }
                    current = current.NextNode;
                }
                if (!treeNodes.Contains(secondNode))
                {
                    treeNodes.Add(secondNode);
                    if (CommandsFromTreeNodes.ContainsKey(secondNode))
                    {
                        commandTreeNodes.Add(secondNode);
                    }
                }
            }
            else
            {
                TreeNode toLowNode;
                TreeNode other;
                if (node1.Level > node2.Level)
                {
                    toLowNode = node1;
                    other = node2;
                }
                else
                {
                    toLowNode = node2;
                    other = node1;
                }
                bool addToCommands = toLowNode.Level + 1 == other.Level && !CommandsFromTreeNodes.ContainsKey(other);
                while (true)
                {
                    treeNodes.Add(toLowNode);
                    if (addToCommands && CommandsFromTreeNodes.ContainsKey(toLowNode))
                    {
                        commandTreeNodes.Add(toLowNode);
                    }
                    if (toLowNode.Index > 0)
                    {
                        toLowNode = toLowNode.PrevNode;
                    }
                    else
                    {
                        break;
                    }
                }
                GetNodesInBetween(other, toLowNode.Parent, treeNodes, commandTreeNodes);
            }
        }


        void SetTreeNodeColor(TreeNode node)
        {
            if (runningCommandsMode && currentlyRunnningNodes != null && currentlyRunnningNodes.Contains(node))
            {
                node.BackColor = Color.Yellow;
                node.ForeColor = Color.Black;
            }
            else if (node == runCommandsTreeView.SelectedNode || selectedTreeNodes.Contains(node))
            {
                node.BackColor = Color.CornflowerBlue;
                node.ForeColor = Color.White;
            }
            else if (breakPointedNodes.Contains(node))
            {
                node.BackColor = Color.Red;
                node.ForeColor = Color.White;
            }
            else if (node == startProgramNode)
            {
                node.BackColor = Color.Green;
                node.ForeColor = Color.White;
            }
            else
            {
                node.BackColor = standardBackColor;
                node.ForeColor = Color.Black;
            }
        }

        bool runningCommandsMode = false;
        bool pauseMode = false;
        private void runCommandsButton_Click(object sender, EventArgs e)
        {
            if (runningCommandsMode)
            {
                if (pauseMode)
                {
                    StartRunningCommands();
                }
                else
                {
                    StopRunningCommands(true);
                }
            }
            else
            {
                StartRunningCommands();
            }
        }

        Dictionary<Motors, int> storedMotorPowers;
        void StartRunningCommands()
        {
            if (!pauseMode)
            {
                if (autoResetRobotCheckBox.Checked)
                {
                    robot.ResetRobot();
                    rcm.SetCommands(null, true);
                }
                Queue<Command> commands = new Queue<Command>();
                RCM.GetCommands(startProgramNode, 0, this, commands, true);
                rcm.SetCommands(commands, true);
            }
            else
            {
                foreach (var c in robot.Components)
                {
                    c.Value.Power = storedMotorPowers[c.Key];
                }
                Queue<Command> commands = new Queue<Command>();
                RCM.GetCommands(lastRunningTreeNode, 0, this, commands, true, false, breakPointMode);
                rcm.SetCommands(commands, breakPointMode);
            }
            breakPointMode = false;
            runCommandsButton.ForeColor = Color.White;
            runCommandsButton.BackColor = Color.Red;
            runCommandsButton.Text = "Pause";
            runningCommandsMode = true;
            pauseMode = false;
            panel1.Enabled = false;
            EnableMiscControls(false, runCommandsButton);
        }
        void StopRunningCommands(bool pause)
        {
            if (pause)
            {
                runCommandsButton.BackColor = Color.Yellow;
                runCommandsButton.Text = "Continue";
                runCommandsButton.ForeColor = Color.Black;
                storedMotorPowers = new Dictionary<Motors, int>();

                runningCommandsMode = true;
                pauseMode = true;
            }
            else
            {
                breakPointMode = false;
                runningCommandsMode = false;
                RunningTreeColoring();
                runCommandsButton.ForeColor = Color.White;
                runCommandsButton.BackColor = Color.Green;
                runCommandsButton.Text = "Run Commands";

                pauseMode = false;

                rcm.SetCommands(null, true);
            }
            EnableMiscControls(true, null);
            foreach (var c in robot.Components)
            {
                if (pause)
                {
                    storedMotorPowers.Add(c.Key, c.Value.Power);
                }
                c.Value.Power = 0;
            }
            panel1.Enabled = true;
        }

        private void resetRobotButton_Click(object sender, EventArgs e)
        {
            robot.ResetRobot();
            StopRunningCommands(false);
        }

        private void setAsStartButton_Click(object sender, EventArgs e)
        {
            startProgramNode = runCommandsTreeView.SelectedNode;

            SetTreeNodeColor(startProgramNode);
            SetTreeNodeColor(lastStartProgramNode);
            lastStartProgramNode = startProgramNode;
        }

        List<Command> clipboard = null;

        private void runCommandsTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveTreeNode(selectedCommandsTreeNodes);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case (Keys.C):
                        Copy();
                        e.SuppressKeyPress = true;
                        break;
                    case (Keys.X):
                        Copy();
                        RemoveTreeNode(selectedCommandsTreeNodes);
                        e.SuppressKeyPress = true;
                        break;
                    case (Keys.V):
                        if (clipboard != null)
                        {
                            TreeNode current = CurrentTreeNode;
                            foreach (var c in clipboard)
                            {
                                current = AddCommand(c.CompleteCopy(), current, this);
                            }
                        }
                        e.SuppressKeyPress = true;
                        break;

                }
            }
        }

        void Copy()
        {
            clipboard = new List<Command>();
            foreach (var tn in selectedCommands)
            {
                Command c = tn.CompleteCopy();
                c.SetCommandTreeNode(new TreeNode());
                clipboard.Add(c);
            }
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            currentFile = saveFileDialog1.FileName;
            saveButton.Enabled = true;
            string serialized = SerializeCommands();
            VisulizeableList<MyVector2> list = new VisulizeableList<MyVector2>();
            string test = ToRobotLanguageConverter.GetRobotCode(serialized, RobotLanguages.CPlusPlus);
            File.WriteAllText(currentFile, serialized);
        }


        private void openFileButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            currentFile = openFileDialog1.FileName;
            saveButton.Enabled = true;
            InitCommands();

            string serializeCommands = File.ReadAllText(currentFile);
            Variable? variable;
            object item = VisulizableItem.Deserialize(serializeCommands, out variable);
            VisulizeableList<Command> list = (VisulizeableList<Command>)item;
            TreeNode current = CurrentTreeNode;
            foreach (var c in list.List)
            {
                current = AddCommand(c, current, this);
            }
        }

        string SerializeCommands()
        {
            Queue<Command> commands = new Queue<Command>();
            RCM.GetCommands(programNode, 0, this, commands, false);
            VisulizeableList<Command> list = new VisulizeableList<Command>(commands);
            return list.Serialize();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (currentFile == null)
            {
                saveButton.Enabled = false;
            }
            else
            {
                File.WriteAllText(currentFile, SerializeCommands());
            }
        }

        private void breakpointButton_Click(object sender, EventArgs e)
        {
            foreach (var n in selectedCommandsTreeNodes)
            {
                if (CommandsFromTreeNodes.ContainsKey(n))
                {
                    if (breakPointedNodes.Contains(n))
                    {
                        breakPointedNodes.Remove(n);
                    }
                    else
                    {
                        breakPointedNodes.Add(n);
                    }
                    SetTreeNodeColor(n);
                }
            }
        }
    }


}
