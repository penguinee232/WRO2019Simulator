using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class InitializeVariablePhrase : VariableChangeItem
    {
        public PossibleListItem VariableTypes { get; set; }
        public string VariableName { get; set; }
        HashSet<TypeNameInfo> variableTypes;
        VariableGetSet variable;
        bool variableIsNull = true;
        TreeNode treeNode;
        public InitializeVariablePhrase()
        {
            VariableName = "a";
            variableTypes = new HashSet<TypeNameInfo>();
            HashSet<object> objectSet = new HashSet<object>();
            foreach (var t in Form1.variableTypes)
            {
                var current = new TypeNameInfo(t);
                variableTypes.Add(current);
                objectSet.Add(current);
            }
            VariableTypes = new PossibleListItem(objectSet, PossibleVariableChanged);
            SetGetMiddleItems(GetMiddleItems, false);
        }
        public void SetCommandTreeNode(TreeNode treeNode)
        {
            this.treeNode = treeNode;

            if (VariableTypes.CurrentPossiblility != null)
            {
                PossibleVariableChanged(VariableTypes.CurrentPossiblility, null);
            }
        }

        public static List<IGetSetFunc> GetMiddleItems(VariableChangeItem item)
        {
            InitializeVariablePhrase initializeVariable = (InitializeVariablePhrase)item;
            return new List<IGetSetFunc>()
            {
                new GetSetFunc<PossibleListItem>((i)=>initializeVariable.VariableTypes, (v,i) => initializeVariable.VariableTypes = v, "VariableTypes"),
                new GetSetFunc<string>((i)=>initializeVariable.VariableName, (v,i) =>
                {
                    initializeVariable.VariableName = v;
                    initializeVariable.PossibleVariableChanged(initializeVariable.VariableTypes.CurrentPossiblility, null);
                }, "VariableName")
            };
        }
        void SetNewVariable(Type type, string name)
        {
            object value;
            if (type.IsClass)
            {
                value = Extensions.GetDefaultFromConstructor(type);
            }
            else
            {
                value = Extensions.GetDefault(type);
            }
            variable = VariablesInfo.AddVariable(new Variable(type, name), value, treeNode);
        }

        public void PossibleVariableChanged(object type, LabeledControl labeledControl)
        {
            var typeInfo = (TypeNameInfo)type;
            if (variable.Type != typeInfo.Type || variableIsNull)
            {
                if (!variableIsNull)
                {
                    VariablesInfo.RemoveVariable(variable.Get(), false);
                }
                else
                {
                    variableIsNull = false;
                }
                SetNewVariable(typeInfo.Type, VariableName);
                Variable.Variable = variable;
                if (labeledControl != null)
                {
                    labeledControl.ParentNode = VariableChanged(variable, labeledControl.ParentNode);
                }
            }
            else
            {
                object value = VariablesInfo.GetVariable(variable.Get());
                VariablesInfo.RemoveVariable(variable.Get(), true);
                variable.Set(new Variable(variable.Type, VariableName));
                variable = VariablesInfo.AddVariable(variable.Get(), value, treeNode, variable.Index);
                Variable.Variable = variable;
            }
        }
    }

    public class InitializeVariable : Command
    {
        InitializeVariablePhrase InitializeVariablePhrase { get; set; }
        public InitializeVariable()
        {
            InitializeVariablePhrase = new InitializeVariablePhrase();
            SetVisulizeItems();
        }
        private InitializeVariable(InitializeVariable original)
        {
            Form = original.Form;
            InitializeVariablePhrase = new InitializeVariablePhrase();
            original.InitializeVariablePhrase.CopyTo(InitializeVariablePhrase);
            SetVisulizeItems();
        }
        void SetVisulizeItems()
        {
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<InitializeVariablePhrase>((i)=>InitializeVariablePhrase, (v,i)=>InitializeVariablePhrase=v, "InitializeVariablePhrase")
            };
            Init(false);
        }

        public override Command CompleteCopy()
        {
            return new InitializeVariable(this);
        }

        public override Queue<Action> GetActions(Robot robot)
        {
            Queue<Action> queue = new Queue<Action>();
            queue.Enqueue(new Action(new List<Request>() { new InitializeVariableRequest(InitializeVariablePhrase) }));
            return queue;
        }

        public override void SetCommandTreeNode(TreeNode treeNode)
        {
            base.SetCommandTreeNode(treeNode);
            InitializeVariablePhrase.SetCommandTreeNode(treeNode);
        }
    }

    public class InitializeVariableRequest : Request
    {
        InitializeVariablePhrase initializeVariablePhrase;
        public InitializeVariableRequest(InitializeVariablePhrase initializeVariablePhrase)
        {
            this.initializeVariablePhrase = initializeVariablePhrase;
        }
        public override void InitRequest(Robot robot)
        {
        }
        public override bool UpdateRequest(Robot robot)
        {
            VariablesInfo.SetVariable(initializeVariablePhrase.Variable.Variable.Get(), initializeVariablePhrase.Other);
            return false;
        }
    }

    public struct TypeNameInfo
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public TypeNameInfo(Type type)
        {
            Type = type;
            Name = type.GetTypeName();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
