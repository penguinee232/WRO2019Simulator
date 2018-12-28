#include"Component.h"
#include "Robot.h"

Component::Component(Motors motor)
{
	Motor = motor;
	Power = 0;
	hasRequest = false;
}
Component::Component()
{
	Motor = Motors::LeftDrive;
	Power = 0;
	hasRequest = false;
}
bool Component::Update(Robot& robot)
{
	if (!hasRequest)
	{
		return false;
	}
	bool requestWorking = request->UpdateRequest(robot);
	Power = request->Power;
	hasRequest = requestWorking;
	return requestWorking;
}

void Component::StartRequest(shared_ptr<Request> request, Robot& robot)
{
	hasRequest = true;
	this->request = request;
	request->InitRequest(robot);
}