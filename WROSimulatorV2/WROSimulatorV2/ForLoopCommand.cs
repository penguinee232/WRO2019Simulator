using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class ForLoopCommand:Command
    {
        public BoolPhrase BoolPhrase { get; set; }
        public TreeNode Loop { get; private set; }
        Queue<Command> loopCommands;
        public ForLoopCommand()
        {
            BoolPhrase = new BoolPhrase();
            SetVisulizeItems();
        }
        private ForLoopCommand(ForLoopCommand original)
        {
            Form = original.Form;
            if (original.Loop != null)
            {
                original.StoreContainedCommands(Form);
            }
            LoopFunctions.CopyLoopCommands(original.loopCommands, ref loopCommands);

            BoolPhrase = new BoolPhrase();
            original.BoolPhrase.CopyTo(BoolPhrase);
            SetVisulizeItems();
        }
        void SetVisulizeItems()
        {
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<BoolPhrase>((i)=>BoolPhrase, (v,i)=>BoolPhrase=v, "BoolPhrase")
            };
            Init(false);
        }
        public override void SetCommandTreeNode(TreeNode treeNode)
        {
            base.SetCommandTreeNode(treeNode);
            Loop = treeNode.Nodes.Add("Loop");
            LoopFunctions.AddCurrentCommands(loopCommands, Loop, Form);
            Form.DontLookAtOtherChildrenTreeNodes.Add(treeNode);
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
            if (BoolPhrase.IsTrue())
            {
                return loopCommands;
            }
            else
            {
                return new Queue<Command>();
            }
        }

        public override void Copy(Command command)
        {
            ForLoopCommand item = (ForLoopCommand)command;
            item.Loop = Loop;
            item.loopCommands = new Queue<Command>(loopCommands);
            base.Copy(command);
        }

        public override Command CompleteCopy()
        {
            return new ForLoopCommand(this);
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
            LoopFunctions.Serialize(items, variables, loopCommands);
            return Serialize(this, items, variables);
        }
        protected override void Deserialize(Span<char> span)
        {
            var list = DeserializeItems(span);
            for (int i = 0; i < VisulizeItems.Count; i++)
            {
                VisulizeItems[i].ObjSet(list[i].Value, i);
                VisulizeItems[i].Variable = VariablesInfo.GetVariableGetSet(list[i].Variable);
            }
            LoopFunctions.Deserialize((VisulizeableList<Command>)list[VisulizeItems.Count].Value, ref loopCommands);

        }
        public override bool RepeatCommand(Robot robot)
        {
            return BoolPhrase.IsTrue();
        }
    }
}
