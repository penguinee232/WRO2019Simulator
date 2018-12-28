using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public class Component
    {
        public Motors Motor { get; }
        public int Power { get; set; }
        protected Request request;
        public MotorInfo MotorInfo { get; private set; }
        public float CurrentUpdateDistance { get; private set; }
        public Component(Motors motor, MotorInfo motorInfo)
        {
            MotorInfo = motorInfo;
            Motor = motor;
            request = null;
            Power = 0;
        }
        public virtual bool Update(Robot robot)//returns true for working returns false for done
        {
            if(request == null)
            {
                return false;
            }
            bool requestWorking = request.UpdateRequest(robot);
            Power = request.Power;
            return requestWorking;
        }
        public virtual void StartRequest(Request request, Robot robot)
        {
            this.request = request;
            request.InitRequest(robot);
        }
        public float GetUpdateDistance()
        {
            CurrentUpdateDistance = MotorInfo.GetDistance(Power);
            return CurrentUpdateDistance;
        }
    }
    public struct MotorInfo
    {
        public float MillisPerPowerPerTime { get; set; }
        public MotorInfo(float millisPerPowerPerTime)
        {
            MillisPerPowerPerTime = millisPerPowerPerTime;
        }
        public float GetDistance(int power)
        {
            return MillisPerPowerPerTime * power;
        }
    }

    public class OtherComponent : Component
    {
        List<Request> requests;
        List<bool> workingRequests;
        public OtherComponent()
            :base(Motors.Other, new MotorInfo(0))
        {
            requests = new List<Request>();
            workingRequests = new List<bool>();
        }
        public override bool Update(Robot robot)
        {
            bool working = false;
            for(int i = 0; i < requests.Count; i++)
            {
                if (workingRequests[i])
                {
                    request = requests[i];
                    workingRequests[i] = base.Update(robot);
                    if (workingRequests[i])
                    {
                        working = true;
                    }
                }
            }
            return working;
        }
        public override void StartRequest(Request request, Robot robot)
        {
            base.StartRequest(request, robot);
            requests.Add(request);
            workingRequests.Add(true);
        }
    }
}
