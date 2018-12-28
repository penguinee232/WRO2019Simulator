#pragma once
#include<map>
#include"Enums.h"
#include "Component.h"
using std::map;

class Robot {
public:
	Robot();
	void Update();
	map<Motors, Component> Components;
	map<Motors, float> tempMotorEncoders;
private:
};