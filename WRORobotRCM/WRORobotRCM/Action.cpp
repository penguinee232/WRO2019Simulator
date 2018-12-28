#include "Action.h"
#include "Robot.h"
Action::Action(vector<shared_ptr<Request>> requests)
{
	this->requests = requests;
	requestsWorking = vector<bool>();
	for (int i = 0; i < requests.size(); i++)
	{
		requestsWorking.push_back(true);
	}
	haveInit = false;
}
bool Action::Update(Robot& robot)
{
	bool working = false;
	for (int i = 0; i < requests.size(); i++)
	{
		shared_ptr<Request> request = requests[i];
		if (requestsWorking[i])
		{
			if (!haveInit)
			{
				robot.Components[request->Motor].StartRequest(request, robot);
			}
			requestsWorking[i] = robot.Components[request->Motor].Update(robot);
			if (requestsWorking[i])
			{
				working = true;
			}
		}
	}
	haveInit = true;
	return working;
}