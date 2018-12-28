﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public abstract class Command : VisulizableItem
    { 
        public Command Parent { get; set; }
        public Form1 Form { get; set; }
        public TreeNode CommandTreeNode { get; private set; }
        public string Name { get; set; }
        public Command()
        {
            CommandTreeNode = null;
            Name = GetType().GetTypeName();
        }
        public override string ToString()
        {
            return Name;
        }
        public virtual void SetCommandTreeNode(TreeNode treeNode)
        {
            CommandTreeNode = treeNode;
        }
        public abstract Queue<Action> GetActions(Robot robot);
        public virtual void StoreContainedCommands(Form1 form)
        {

        }
        public virtual Queue<Command> GetContainedCommands(Robot robot)
        {
            return null;
        }
        public virtual bool CanAdd(Command command)
        {
            return Parent == null || Parent.CanAdd(command);
        }
        public virtual VisulizableItem Copy(Command newItem)
        {
            newItem.Parent = Parent;
            newItem.Name = Name;
            newItem.Form = Form;
            newItem.CommandTreeNode = CommandTreeNode;
            return CopyItems(newItem, this);
        }
        public override VisulizableItem Copy()
        {
            Command command = (Command)Extensions.GetDefaultFromConstructor(GetType());
            return Copy(command);
        }
        public abstract Command CompleteCopy();
    }
    public interface IActionCommand
    {
        List<Request> GetActionRequests(Robot robot);
    }
}