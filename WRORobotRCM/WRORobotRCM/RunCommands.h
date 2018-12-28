#pragma once
#include"RCM.h"
#include<iostream>
#include<string>
class RunCommands
{
public:
	RunCommands(Robot& robot, RCM& rcm);
	void RunAllTheCommands();
	void UpdateAndAddComponent(Command* command);
private:
	Robot & robot;
	RCM& rcm;
};
