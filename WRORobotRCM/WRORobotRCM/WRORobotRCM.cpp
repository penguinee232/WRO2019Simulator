// WRORobotRCM.cpp : Defines the entry point for the console application.
//

#include "RCM.h"
#include <iostream>
#include <string>
#include<queue>
#include"RunCommands.h"
#include "Robot.h"
#include "DriveByMillisRequest.h"
int main()
{
	Robot robot = Robot();
	

	RCM rcm = RCM(robot);
	rcm.SetCommands(std::queue<Command*>());
	RunCommands runCommands = RunCommands(robot, rcm);
	runCommands.RunAllTheCommands();
}


