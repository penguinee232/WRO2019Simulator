using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class WhileCommand:Command
    {
        public VariableVisulizeItem Variable { get; set; }
        public Operatiors Operatior { get; set; }
        public object Other { get; set; }
        static readonly int otherIndex = 2;
        public TreeNode Loop { get; private set; }
        Queue<Command> loopCommands;
        public WhileCommand()
        {
            Variable = new VariableVisulizeItem();
            Variable.VariableChanged = VariableChanged;
            Operatior = Operatiors.Equals;
            SetOtherToDefault();
            SetVisulizeItems();
        }
        private WhileCommand(WhileCommand original)
        {
            Form = original.Form;
            original.StoreContainedCommands(Form);
            if (original.loopCommands != null)
            {
                loopCommands = new Queue<Command>();
                foreach (var c in original.loopCommands)
                {
                    loopCommands.Enqueue(c.CompleteCopy());
                }
            }
            Variable = original.Variable.CompleteCopy();
            Variable.VariableChanged = VariableChanged;
            Operatior = original.Operatior;
            if (original.Other.GetType().IsSubclassOf(typeof(VisulizableItem)))
            {
                Other = ((VisulizableItem)original.Other).Copy();
            }
            else
            {
                Other = original.Other;
            }
            SetVisulizeItems();
        }
        void SetVisulizeItems()
        {
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<VariableVisulizeItem>((i)=>Variable, (v,i)=>Variable.SetVariable(v.Variable), "Variable"),
                new GetSetFunc<Operatiors>((i)=>Operatior, (v,i)=>Operatior = v, "Operatior"),
                new GetSetFunc<object>((i)=>Other, (v,i)=>Other = v, "Operatior")
            };
            SetOtherVisItem();
            Init(false);
        }
        void VariableChanged(VariableVisulizeItem item, LabeledControl labeledControl)
        {
            if (Other.GetType() != item.Variable.Type)
            {
                SetOtherToDefault();
                SetOtherVisItem();
                IndexInit(otherIndex, true);

                LabeledControl parent = labeledControl.ParentNode.Children[otherIndex].Control;
                Extensions.UpdateVisual(parent, parent.Form);
            }
        }
        void SetOtherVisItem()
        {
            VisulizeItems[otherIndex].ItemInfo = new ItemInfo(Variable.Variable.Type, VisulizeItems[2].ItemInfo.Name);
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
        public override void SetCommandTreeNode(TreeNode treeNode)
        {
            base.SetCommandTreeNode(treeNode);
            Loop = treeNode.Nodes.Add("Loop");
            if (loopCommands != null)
            {
                var current = Loop;
                foreach (var c in loopCommands)
                {
                    current = Form1.AddCommand(c, current, Form);
                }
            }
        }

        public override Queue<Action> GetActions(Robot robot)
        {
            return new Queue<Action>();
        }
        public override void StoreContainedCommands(Form1 form)
        {
            loopCommands = new Queue<Command>();
            RCM.GetCommands(Loop, 0, form, loopCommands, false);
        }
        public override Queue<Command> GetContainedCommands(Robot robot)
        {
            if (Variable.IsTrue(Operatior, Other))
            {
                return loopCommands;
            }
            else
            {
                return new Queue<Command>();
            }
        }

        public override VisulizableItem Copy(Command command)
        {
            WhileCommand item = (WhileCommand)command;
            item.Loop = Loop;
            item.loopCommands = new Queue<Command>(loopCommands);
            item.Variable = Variable;
            item.SetOtherToDefault();
            item.SetOtherVisItem();
            return base.Copy(command);
        }

        public override Command CompleteCopy()
        {
            return new WhileCommand(this);
        }
        public override string Serialize()
        {
            List<object> items = new List<object>();
            List<Variable?> variables = new List<Variable?>();
            for (int i = 0; i < VisulizeItems.Count; i++)
            {
                items.Add(VisulizeItems[i].ObjGet(i));
                variables.Add(VisulizeItems[i].Variable);
            }

            StoreContainedCommands(Form);
            VisulizeableList<Command> loopList = new VisulizeableList<Command>(loopCommands);
            items.Add(loopList);
            variables.Add(null);
            return Serialize(this, items, variables);
        }
        protected override void Deserialize(Span<char> span)
        {
            var list = DeserializeItems(span);
            for (int i = 0; i < VisulizeItems.Count; i++)
            {
                //if (list[i].Variable == null)
                //{
                VisulizeItems[i].ObjSet(list[i].Value, i);
                //}
                VisulizeItems[i].Variable = list[i].Variable;
            }
            SetOtherVisItem();
            IndexInit(otherIndex, true);
            VisulizeableList<Command> loopList = (VisulizeableList<Command>)list[VisulizeItems.Count].Value;
            loopCommands = new Queue<Command>();
            foreach (var c in loopList.List)
            {
                loopCommands.Enqueue(c);
            }
        }
        public override bool RepeatCommand(Robot robot)
        {
            return Variable.IsTrue(Operatior, Other);
        }
    }
}
