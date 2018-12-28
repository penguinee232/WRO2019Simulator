#pragma once
#include "Request.h"
#include<memory>
using std::shared_ptr;
class Component
{
public:
	Component(Motors motor);
	Component();
	Motors Motor;
	int Power;
	virtual bool Update(Robot& robot);
	virtual void StartRequest(shared_ptr<Request> request, Robot& robot);
protected:
	shared_ptr<Request> request;
	bool hasRequest;
};
