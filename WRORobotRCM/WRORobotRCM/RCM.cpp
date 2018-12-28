#include"RCM.h"
RCM::RCM(Robot& robot) : robot(robot)
{
	actions = queue<Action*>();
	commands = queue<Command*>();
	currentCommand = nullptr;
	currentAction = nullptr;
	childRCM = nullptr;
	hasActions = false;
	hasCommands = false;
}
void RCM::SetCommands(queue<Command*> commands)
{
	this->commands = commands;
	hasCommands = true;
	actions = queue<Action*>();
	currentCommand = nullptr;
	currentAction = nullptr;
	childRCM = nullptr;
	hasActions = false;
}
void::RCM::AddCommand(Command* command)
{
	commands.push(command);
	hasCommands = true;
}

bool RCM::Update()
{
	if (childRCM != nullptr)
	{
		if (!childRCM->Update())
		{
			childRCM = nullptr;
		}
		else
		{
			return true;
		}
	}
	else
	{
		if (hasCommands)
		{
			if (!hasActions)
			{
				if (commands.size() == 0)
				{
					hasCommands = false;
				}
				else
				{
					currentCommand = commands.front();
					commands.pop();
					actions = currentCommand->GetActions(robot);
					hasActions = true;
				}
			}
			if (hasActions)
			{
				if (currentAction == nullptr)
				{
					if (actions.size() > 0)
					{
						currentAction = actions.front();
						actions.pop();
					}
					else
					{
						actions = queue<Action*>();
						hasActions = false;
						queue<Command*> containedCommands = currentCommand->GetContainedCommands(robot);
						if (containedCommands.size() > 0)
						{
							childRCM = new RCM(robot);
							childRCM->SetCommands(containedCommands);
						}
					}
				}
				if (currentAction != nullptr)
				{
					if (!currentAction->Update(robot))
					{
						currentAction = nullptr;
					}
				}
			}
		}
	}
	return hasCommands;
}


RCM::~RCM() {
	delete currentCommand;
	delete currentAction;
	delete childRCM;
}