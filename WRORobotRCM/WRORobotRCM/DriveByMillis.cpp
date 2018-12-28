#include"DriveByMillis.h"
#include "Robot.h"
DriveByMillis::DriveByMillis(int leftPower, int rightPower, float distance, MoveByMillisMode moveByMillisMode, vector<MyVector2> test)
{
	LeftPower = leftPower;
	RightPower = rightPower;
	Distance = distance;
	MovemenMode = moveByMillisMode;
	Test = test;
}
queue<Action*> DriveByMillis::GetActions(Robot& robot)
{
	queue<Action*> actions = queue<Action*>();
	actions.push(new Action(GetActionRequests(robot)));
	return actions;
}
vector<shared_ptr<Request>> DriveByMillis::GetActionRequests(Robot& robot)
{
	vector<shared_ptr<Request>> requests = vector<shared_ptr<Request>>();
	requests.push_back(std::make_shared<DriveByMillisRequest>(DriveByMillisRequest(Motors::LeftDrive, LeftPower, Distance, MovemenMode)));
	requests.push_back(std::make_shared<DriveByMillisRequest>(DriveByMillisRequest(Motors::RightDrive, RightPower, Distance, MovemenMode)));
	return requests;
}