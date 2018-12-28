#pragma once
#include"Request.h"
#include"Component.h"
#include<vector>
using std::vector;
class OtherComponent : public Component
{
public:
	OtherComponent();
	bool Update(Robot& robot) override;
	void StartRequest(shared_ptr<Request> request, Robot& robot) override;
private:
	vector<shared_ptr<Request>> requests;
	vector<bool> workingRequests;
};
