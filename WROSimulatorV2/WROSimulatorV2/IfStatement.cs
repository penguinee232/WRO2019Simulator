using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class IfStatement : Command
    {
        public BoolPhrase BoolPhrase { get; set; }
        //public VariableVisulizeItem Variable { get; set; }
        //public Operatiors Operatior { get; set; }
        //public object Other { get; set; }
        //static readonly int otherIndex = 2;
        public TreeNode Then { get; private set; }
        public TreeNode Else { get; private set; }
        Queue<Command> thenCommands;
        Queue<Command> elseCommands;
        public IfStatement()
        {
            BoolPhrase = new BoolPhrase();
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<BoolPhrase>((i)=>BoolPhrase, (v,i)=>BoolPhrase = v, "BoolPhrase")
            };
            //Variable = new VariableVisulizeItem();
            //Variable.VariableChanged = VariableChanged;
            //Operatior = Operatiors.Equals;
            //SetOtherToDefault();
            //SetVisulizeItems();
        }
        //void SetVisulizeItems()
        //{
        //    VisulizeItems = new List<IGetSetFunc>()
        //    {
        //        new GetSetFunc<VariableVisulizeItem>((i)=>Variable, (v,i)=>Variable.SetVariable(v.Variable), "Variable"),
        //        new GetSetFunc<Operatiors>((i)=>Operatior, (v,i)=>Operatior = v, "Operatior"),
        //        new GetSetFunc<object>((i)=>Other, (v,i)=>Other = v, "Operatior")
        //    };
        //    SetOtherVisItem();
        //    Init();
        //}
        private IfStatement(IfStatement original)
        {
            Form = original.Form;
            if (original.Then != null && original.Else != null)
            {
                original.StoreContainedCommands(Form);
            }
            if (original.thenCommands != null)
            {
                thenCommands = new Queue<Command>();
                foreach (var c in original.thenCommands)
                {
                    thenCommands.Enqueue(c.CompleteCopy());
                }
            }
            if (original.elseCommands != null)
            {
                elseCommands = new Queue<Command>();
                foreach (var c in original.elseCommands)
                {
                    elseCommands.Enqueue(c.CompleteCopy());
                }
            }
            original.BoolPhrase.CopyTo(BoolPhrase);
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<BoolPhrase>((i)=>BoolPhrase, (v,i)=>BoolPhrase = v, "BoolPhrase")
            };
            Init(true);
            //Variable = original.Variable.CompleteCopy();
            //Variable.VariableChanged = VariableChanged;
            //Operatior = original.Operatior;
            //if (original.Other.GetType().IsSubclassOf(typeof(VisulizableItem)))
            //{
            //    Other = ((VisulizableItem)original.Other).Copy();
            //}
            //else
            //{
            //    Other = original.Other;
            //}
            //SetVisulizeItems();
        }

        //void VariableChanged(VariableVisulizeItem item, LabeledControl labeledControl)
        //{
        //    if (Other.GetType() != item.Variable.Type)
        //    {
        //        SetOtherToDefault();
        //        SetOtherVisItem();

        //        IndexInit(otherIndex, true);

        //        var node = labeledControl.ParentNode;//.Children[otherIndex];
        //        Form1.UpdateItem(ref node, node.Control.GetSetFunc, node.Control.Index, node.Control.Form);
        //        labeledControl.ParentNode = node;
        //        //Extensions.UpdateVisual(parent, parent.Form);
        //    }
        //}

        //void SetOtherVisItem()
        //{
        //    VisulizeItems[otherIndex].ItemInfo = new ItemInfo(Variable.Variable.Type, VisulizeItems[2].ItemInfo.Name);
        //}

        //public void SetOtherToDefault()
        //{
        //    if (Variable.Variable.Type.IsClass)
        //    {
        //        Other = Extensions.GetDefaultFromConstructor(Variable.Variable.Type);
        //    }
        //    else
        //    {
        //        Other = Extensions.GetDefault(Variable.Variable.Type);
        //    }
        //}
        public override void SetCommandTreeNode(TreeNode treeNode)
        {
            base.SetCommandTreeNode(treeNode);
            Then = treeNode.Nodes.Add("Then");
            if (thenCommands != null)
            {
                var current = Then;
                foreach (var c in thenCommands)
                {
                    current = Form1.AddCommand(c, current, Form);
                }
            }
            Else = treeNode.Nodes.Add("Else");
            if (elseCommands != null)
            {
                var current = Else;
                foreach (var c in elseCommands)
                {
                    current = Form1.AddCommand(c, current, Form);
                }
            }
            Form.DontLookAtOtherChildrenTreeNodes.Add(treeNode);
        }

        

        public override Queue<Action> GetActions(Robot robot)
        {
            return new Queue<Action>();
        }
        public override void StoreContainedCommands(Form1 form)
        {
            thenCommands = new Queue<Command>();
            RCM.GetCommands(Then, 0, form, thenCommands, false);

            elseCommands = new Queue<Command>();
            RCM.GetCommands(Else, 0, form, elseCommands, false);
        }
        public override Queue<Command> GetContainedCommands(Robot robot)
        {
            if (BoolPhrase.IsTrue())
            {
                return thenCommands;
            }
            else
            {
                return elseCommands;
            }
        }

        public override void Copy(Command command)
        {
            IfStatement item = (IfStatement)command;
            item.Then = Then;
            item.Else = Else;
            item.thenCommands = new Queue<Command>(thenCommands);
            item.elseCommands = new Queue<Command>(elseCommands);
            //item.Variable = Variable;
            //item.SetOtherToDefault();
            //item.SetOtherVisItem();
            base.Copy(command);
        }

        public override Command CompleteCopy()
        {
            return new IfStatement(this);
        }
        public override string Serialize()
        {
            List<object> items = new List<object>();
            List<Variable?> variables = new List<Variable?>();
            for (int i = 0; i < VisulizeItems.Count; i++)
            {
                items.Add(VisulizeItems[i].ObjGet(i));
                variables.Add(VariableGetSet.GetNullableVariable(VisulizeItems[i].Variable));
            }

            StoreContainedCommands(Form);
            VisulizeableList<Command> thenList = new VisulizeableList<Command>(thenCommands);
            VisulizeableList<Command> elseList = new VisulizeableList<Command>(elseCommands);
            items.Add(thenList);
            items.Add(elseList);
            variables.Add(null);
            variables.Add(null);
            return Serialize(GetType(), items, variables);
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
                VisulizeItems[i].Variable = VariablesInfo.GetVariableGetSet(list[i].Variable);
            }
            //SetOtherVisItem();
            //IndexInit(otherIndex, true);
            VisulizeableList<Command> thenList = (VisulizeableList<Command>)list[VisulizeItems.Count].Value;
            thenCommands = new Queue<Command>();
            foreach(var c in thenList.List)
            {
                thenCommands.Enqueue(c);
            }
            VisulizeableList<Command> elseList = (VisulizeableList<Command>)list[VisulizeItems.Count + 1].Value;
            elseCommands = new Queue<Command>();
            foreach (var c in elseList.List)
            {
                elseCommands.Enqueue(c);
            }
        }
    }
}
