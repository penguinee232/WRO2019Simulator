#pragma once
#include"Command.h"
class RCM
{
public:
	RCM(Robot& robot);
	~RCM();
	void SetCommands(queue<Command*> commands);
	void AddCommand(Command* command);
	bool Update();//returns true if working false if done
private:
	Robot& robot;
	queue<Command*> commands;
	queue<Action*> actions;
	Action* currentAction;
	Command* currentCommand;
	RCM* childRCM;
	bool hasActions;
	bool hasCommands;
};
