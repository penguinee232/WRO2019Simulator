using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class MultiAction : Command
    {
        public TreeNode CommandsNode { get; private set; }
        

        //List<IActionCommand> actions;
        List<Command> commands;
        public MultiAction()
        {
            VisulizeItems = new List<IGetSetFunc>();
            Init();
        }
        private MultiAction(MultiAction original)
        {
            VisulizeItems = new List<IGetSetFunc>();
            original.StoreContainedCommands(original.Form);
            if(original.commands != null)
            {
                commands = new List<Command>();
                foreach(var c in original.commands)
                {
                    commands.Add(c.CompleteCopy());
                }
            }
            Init();
        }
        public override bool CanAdd(Command command)
        {
            return command.GetType().GetInterface("IActionCommand") != null && base.CanAdd(command);
        }

        public override void SetCommandTreeNode(TreeNode treeNode)
        {
            base.SetCommandTreeNode(treeNode);
            CommandsNode = treeNode.Nodes.Add("Commands");
            if (commands != null)
            {
                var current = CommandsNode;
                foreach (var c in commands)
                {
                    current = Form1.AddCommand(c, current, Form);
                }
            }
            Form.DontLookAtOtherChildrenTreeNodes.Add(treeNode);
        }

        public override void StoreContainedCommands(Form1 form)
        {
            commands = new List<Command>();
            foreach (TreeNode n in CommandsNode.Nodes)
            {
                Command c = form.CommandsFromTreeNodes[n];
                commands.Add(c);
            }
        }
        public override Queue<Command> GetContainedCommands(Robot robot)
        {
            return base.GetContainedCommands(robot);
        }
        public override Queue<Action> GetActions(Robot robot)
        {
            List<Request> requests = new List<Request>();
            foreach(var c in commands)
            {
                IActionCommand actionCommand = (IActionCommand)c;
                requests.AddRange(actionCommand.GetActionRequests(robot));
            }
            Queue<Action> q = new Queue<Action>();
            q.Enqueue(new Action(requests));
            return q;
        }
        public override VisulizableItem Copy(Command command)
        {
            MultiAction multiAction = (MultiAction)command;
            multiAction.CommandsNode = CommandsNode;
            multiAction.commands = commands;
            return base.Copy(command);
        }

        public Action GetAction(Robot robot)
        {
            throw new NotImplementedException();
        }

        public override Command CompleteCopy()
        {
            return new MultiAction(this);
        }
        public override string Serialize()
        {
            StoreContainedCommands(Form);
            VisulizeableList<Command> list = new VisulizeableList<Command>(commands);
            return Serialize(this, new List<object>() { list }, new List<Variable?>() { null });
        }
        protected override void Deserialize(Span<char> span)
        {
            var list = DeserializeItems(span);
            commands = new List<Command>();
            VisulizeableList<Command> visulizeableList = (VisulizeableList<Command>)list[0].Value;
            //VisulizeableList<Command> visulizeableList = (VisulizeableList<Command>)list[0].Value;
            foreach (var c in visulizeableList.List)
            {
                commands.Add((Command)c);
            }
        }
    }
}
