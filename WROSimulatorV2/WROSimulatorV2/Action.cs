using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public class Action
    {
        List<Request> requests;
        List<bool> requestsWorking;
        bool haveInit;
        public Action(List<Request> requests)
        {
            requestsWorking = new List<bool>();
            this.requests = requests;
            HashSet<Motors> motors = new HashSet<Motors>();
            foreach(var r in requests)
            {
                requestsWorking.Add(true);
                if(r.Motor != Motors.NotApplicable)
                {
                    if(motors.Contains(r.Motor))
                    {
                        throw new IndexOutOfRangeException();
                    }
                    motors.Add(r.Motor);
                }
            }
            haveInit = false;
        }
        public bool Update(Robot robot, long elapsedMillis)//returns true if working false if done
        {
            bool working = false;
            for (int i = 0; i < requests.Count; i++)
            {
                Request request = requests[i];
                if (requestsWorking[i])
                {
                    if (!haveInit)
                    {
                        robot.Components[request.Motor].StartRequest(request, robot);
                    }
                    requestsWorking[i] = robot.Components[request.Motor].Update(robot,elapsedMillis);
                    if (requestsWorking[i])
                    {
                        working = true;
                    }
                }
            }
            haveInit = true;
            return working;
        }
    }

    
}
