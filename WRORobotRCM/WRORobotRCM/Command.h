#pragma once
#include<queue>
#include"Action.h"
using std::queue;
class Command
{
public:
	Command();
	virtual queue<Action*> GetActions(Robot& robot);
	virtual queue<Command*> GetContainedCommands(Robot& robot);
private:

};
