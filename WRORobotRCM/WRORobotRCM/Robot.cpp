#include"Robot.h"
#include"OtherComponent.h"

Robot::Robot() {
	Components = map<Motors, Component>();
	Components.emplace(Motors::LeftDrive, Component(Motors::LeftDrive));
	Components.emplace(Motors::RightDrive, Component(Motors::RightDrive));
	Components.emplace(Motors::Attachment1, Component(Motors::Attachment1));
	Components.emplace(Motors::Attachment2, Component(Motors::Attachment2));
	Components.emplace(Motors::Other, OtherComponent());
	tempMotorEncoders = map<Motors, float>();
	for (auto c : Components)
	{
		tempMotorEncoders.emplace(c.first, 0);
	}
}
void Robot::Update() {
	for (auto c : Components)
	{
		tempMotorEncoders[c.first] += Components[c.first].Power * 0.05f;
	}
}