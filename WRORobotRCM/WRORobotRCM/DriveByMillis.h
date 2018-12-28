#pragma once
#include"Command.h"
#include"MyVector2.h"
#include"Enums.h"
#include"DriveByMillisRequest.h"
class DriveByMillis : public Command
{
public:
	DriveByMillis(int leftPower, int rightPower, float distance, MoveByMillisMode moveByMillisMode, vector<MyVector2> test);
	int LeftPower;
	int RightPower;
	float Distance;
	MoveByMillisMode MovemenMode;
	vector<MyVector2> Test;
	queue<Action*> GetActions(Robot& robot) override;
	vector<shared_ptr<Request>> GetActionRequests(Robot& robot);
private:

};