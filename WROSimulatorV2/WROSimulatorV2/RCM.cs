using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class RCM
    {
        Robot robot;
        Queue<Command> commands;
        Queue<Action> actions;
        Action currentAction = null;
        Command currentCommand;
        Command varialbeCurrentCommand;
        RCM childRCM = null;
        public RCM(Robot robot)
        {
            this.robot = robot;
        }
        public static void GetCommands(TreeNode currentNode, int index, Form1 form, Queue<Command> commands, bool handleParents, bool isParent = false, bool getCommands = true)
        {
            if (handleParents && isParent && form.DontLookAtOtherChildrenTreeNodes.Contains(currentNode))
            {
                getCommands = false;
            }
            if (getCommands)
            {
                if (form.CommandsFromTreeNodes.ContainsKey(currentNode))
                {
                    form.CommandsFromTreeNodes[currentNode].StoreContainedCommands(form);
                    commands.Enqueue(form.CommandsFromTreeNodes[currentNode]);
                }
                else
                {
                    for (int i = index; i < currentNode.Nodes.Count; i++)
                    {
                        GetCommands(currentNode.Nodes[i], 0, form, commands, false);
                    }
                }
            }
            if(handleParents && currentNode.Parent != null)
            {
                GetCommands(currentNode.Parent, currentNode.Index + 1, form, commands, true, true);
            }
        }
        public void SetCommands(Queue<Command> commands, bool wipeOtherInfo)
        {
            this.commands = commands;
            if(wipeOtherInfo)
            {
                currentCommand = null;
                currentAction = null;
                actions = null;
                childRCM = null;
            }
        }

        public bool Update(out TreeNode currentlyRunCommand)//returns true if working false if done
        {
            if (childRCM != null)
            {
                if (!childRCM.Update(out currentlyRunCommand))
                {
                    childRCM = null;
                    currentCommand = (Command)varialbeCurrentCommand.GetItemWithVariablesSet();
                    if (currentCommand.RepeatCommand(robot))
                    {
                        actions = currentCommand.GetActions(robot);
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (commands != null)
                {
                    if(actions == null)
                    {
                        if (commands.Count == 0)
                        {
                            commands = null;
                        }
                        else
                        {
                            varialbeCurrentCommand = commands.Dequeue();
                            currentCommand = (Command)varialbeCurrentCommand.GetItemWithVariablesSet();
                            actions = currentCommand.GetActions(robot);
                        }
                    }
                    else
                    {
                        if(currentAction == null)
                        {
                            if (actions.Count > 0)
                            {
                                currentAction = actions.Dequeue();
                            }
                            else
                            {
                                actions = null;
                                Queue<Command> containedCommands = currentCommand.GetContainedCommands(robot);
                                if (containedCommands != null && containedCommands.Count > 0)
                                {
                                    childRCM = new RCM(robot);
                                    childRCM.SetCommands(containedCommands, true);
                                }
                                else
                                {
                                    currentCommand = (Command)varialbeCurrentCommand.GetItemWithVariablesSet();
                                    if (currentCommand.RepeatCommand(robot))
                                    {
                                        actions = currentCommand.GetActions(robot);
                                    }
                                }
                            }
                        }
                        if(currentAction != null)
                        {
                            if(!currentAction.Update(robot))
                            {
                                currentAction = null;
                            }
                        }
                    }
                }
                if(currentCommand != null)
                {
                    currentlyRunCommand = currentCommand.CommandTreeNode;
                }
                else
                {
                    currentlyRunCommand = null;
                }
            }
            if (commands == null)
            {
                return false;
            }
            return true;
        }
        
    }
}
