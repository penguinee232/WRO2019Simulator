#pragma once
#include<vector>
#include"Request.h"
#include <memory>
using std::vector;
using std::shared_ptr;
class Action
{
public:
	Action(vector<shared_ptr<Request>> requests);
	bool Update(Robot& robot);//returns true if working false if done
private:
	vector<shared_ptr<Request>> requests;
	vector<bool> requestsWorking;
	bool haveInit;
};