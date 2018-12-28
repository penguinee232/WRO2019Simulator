#include"OtherComponent.h"
#include "Robot.h"
OtherComponent::OtherComponent()
	:Component(Motors::Other)
{
	requests = vector<shared_ptr<Request>>();
	workingRequests = vector<bool>();
}
bool OtherComponent::Update(Robot& robot)
{
	bool working = false;
	for (int i = 0; i < requests.size(); i++)
	{
		if (workingRequests[i])
		{
			request = requests[i];
			workingRequests[i] = Component::Update(robot);
			if (workingRequests[i])
			{
				working = true;
			}
		}
	}
	return working;
}
void OtherComponent::StartRequest(shared_ptr<Request> request, Robot& robot)
{
	Component::StartRequest(request, robot);
	requests.push_back(request);
	workingRequests.push_back(true);
}