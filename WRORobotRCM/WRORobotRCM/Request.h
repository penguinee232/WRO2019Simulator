#pragma once
#include"Enums.h"

class Robot;

class Request
{
public:
	Request();
	Motors Motor;
	int Power;
	virtual void InitRequest(Robot& robot);
	virtual bool UpdateRequest(Robot& robot);//returns true to contrinue, false to stop
private:

};
