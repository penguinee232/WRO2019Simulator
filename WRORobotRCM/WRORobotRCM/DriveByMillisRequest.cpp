#include"DriveByMillisRequest.h"
#include "Robot.h"
DriveByMillisRequest::DriveByMillisRequest(Motors motor, int power, float distance, MoveByMillisMode moveByMillisMode)
	:Request()
{
	this->distance = distance;
	this->moveByMillisMode = moveByMillisMode;
	Motor = motor;
	Power = power;
}
void DriveByMillisRequest::InitRequest(Robot& robot) {
	startEncoder = GetEncoder(moveByMillisMode, robot);
}
bool DriveByMillisRequest::UpdateRequest(Robot& robot)
{
	float currentEncoder = GetEncoder(moveByMillisMode, robot);
	float currentDistance =std::abs(currentEncoder - startEncoder);
	if (currentDistance >= distance)
	{
		Power = 0;
		return false;
	}
	return true;
}
static float GetEncoder(MoveByMillisMode moveByMillisMode, Robot& robot)
{
	switch (moveByMillisMode)
	{
	case (MoveByMillisMode::LeftDriveMode):
		return robot.tempMotorEncoders[Motors::LeftDrive];
	case (MoveByMillisMode::RightDriveMode):
		return robot.tempMotorEncoders[Motors::RightDrive];
	default:
		return (robot.tempMotorEncoders[Motors::LeftDrive] + robot.tempMotorEncoders[Motors::RightDrive]) / 2;
	}
}