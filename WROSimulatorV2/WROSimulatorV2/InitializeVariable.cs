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
        IVariableGetSet variable;
        bool variableIsNull = false;
        TreeNode treeNode;
        public InitializeVariablePhrase()
        {
            VariableName = "a";
            variableTypes = new HashSet<TypeNameInfo>();
            TypeNameInfo defaultType = new TypeNameInfo();
            if (PossibleListItem.StaticPossiblilities != null)
            {
                foreach (var tn in PossibleListItem.StaticPossiblilities[PossibleListEnums.VariableTypes])
                {
                    TypeNameInfo typeNameInfo = (TypeNameInfo)tn;
                    if (typeNameInfo.Type == Variable.Variable.Type)
                    {
                        defaultType = typeNameInfo;
                    }
                    variableTypes.Add(typeNameInfo);
                }
            }
            VariableTypes = new PossibleListItem(defaultType, PossibleListEnums.VariableTypes, PossibleVariableTypeChanged);
            SetGetMiddleItems(GetMiddleItems, false);

        }
        protected override void ControlNodeChanged()
        {
            if (variableIsNull && VariableTypes.CurrentPossiblility != null)
            {
                PossibleVariableChanged(VariableTypes.CurrentPossiblility, null, null);
            }
            base.ControlNodeChanged();
        }
        public void SetCommandTreeNode(TreeNode treeNode)
        {
            this.treeNode = treeNode;
            variableIsNull = true;
            //if (VariableTypes.CurrentPossiblility != null)
            //{
            //    PossibleVariableChanged(VariableTypes.CurrentPossiblility, null);
            //}
        }

        public static List<IGetSetFunc> GetMiddleItems(VariableChangeItem item)
        {
            InitializeVariablePhrase initializeVariable = (InitializeVariablePhrase)item;
            return new List<IGetSetFunc>()
            {
                new GetSetFunc<PossibleListItem>((i)=>initializeVariable.VariableTypes, (v,i) => initializeVariable.VariableTypes = v, "VariableTypes"),
                new GetSetFunc<string>((i)=>initializeVariable.VariableName, (v,i) =>
                {
                    initializeVariable.PossibleVariableChanged(initializeVariable.VariableTypes.CurrentPossiblility, null, v);
                    initializeVariable.VariableName = v;
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
        public void PossibleVariableTypeChanged(object type, LabeledControl labeledControl)
        {
           PossibleVariableChanged(type, labeledControl, null);
        }
        public void PossibleVariableChanged(object type, LabeledControl labeledControl, string newName)
        {
            if (ControlNode != null)
            {
                var typeInfo = (TypeNameInfo)type;
                if (variableIsNull || (variable != null && variable.Type != typeInfo.Type))
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
                    Variable newVariable = new Variable(variable.Get().Type, newName);
                    variable = VariablesInfo.UpdateVariableName(variable.Get(), newVariable, treeNode, ((VariableGetSet)variable).Index);
                    variable.Set(new Variable(variable.Type, newName));
                    Variable.Variable = variable;
                }
            }
        }
        public void Remove()
        {
            if (!variableIsNull)
            {
                VariablesInfo.RemoveVariable(variable.Get(), false);
            }
        }
        public static string GetInitPhrase(CommandsNode node)
        {
            string code = "";
            code += ((PossibleListItem)node.Paramaters[0].Value).CurrentPossiblility.ToString();
            code += " "+ node.Paramaters[1].Value + " = ";
            code += ToRobotLanguageConverter.GetCPlusPlusCommandNodeCode(node.Paramaters[2]);
            return code;
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

        public override void Remove()
        {
            InitializeVariablePhrase.Remove();

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
        public override bool UpdateRequest(Robot robot, long elapsedMillis)
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
        public static bool operator ==(TypeNameInfo left, TypeNameInfo right)
        {
            return left.Type == right.Type;
        }
        public static bool operator !=(TypeNameInfo left, TypeNameInfo right)
        {
            return !(left.Type == right.Type);
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() == GetType())
            {
                return this == (TypeNameInfo)obj;
            }
            return false;
        }
    }
}
