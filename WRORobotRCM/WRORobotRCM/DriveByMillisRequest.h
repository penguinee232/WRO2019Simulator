#pragma once
#include "Request.h"
#include"Enums.h"
#include <cmath>  
class DriveByMillisRequest: public Request
{
public:
	DriveByMillisRequest(Motors motor, int power, float distance, MoveByMillisMode moveByMillisMode);
	virtual void InitRequest(Robot& robot)  override;
	virtual bool UpdateRequest(Robot& robot) override;

private:
	float startEncoder = 0;
	MoveByMillisMode moveByMillisMode;
	float distance = 0;
};
static float GetEncoder(MoveByMillisMode moveByMillisMode, Robot& robot);
