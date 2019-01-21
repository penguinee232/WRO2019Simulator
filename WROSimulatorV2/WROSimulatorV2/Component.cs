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
        public float Speed { get; private set; }//degrees per millisec
        public Component(Motors motor, MotorInfo motorInfo)
        {
            MotorInfo = motorInfo;
            Motor = motor;
            request = null;
            Power = 0;
            Speed = 0;
        }
        public virtual bool Update(Robot robot, long elapsedMillis)//returns true for working returns false for done
        {
            if (request == null)
            {
                return false;
            }
            bool requestWorking = request.UpdateRequest(robot,elapsedMillis);
            Power = request.Power;
            return requestWorking;
        }
        public virtual void StartRequest(Request request, Robot robot)
        {
            this.request = request;
            request.InitRequest(robot);
        }
        public float GetUpdateDistance(long elapsedMillis)
        {
            CurrentUpdateDistance = MotorInfo.GetDistance(Power, elapsedMillis);
            Speed = CurrentUpdateDistance / elapsedMillis;
            return CurrentUpdateDistance;
        }
    }
    public class MotorInfo
    {
        public float DegreesPerSecondAt100Power { get; set; }
        public float AccelTime { get; set; }//milliseconds
        float maxVelocityDegreesPerMilli;
        float accel;
        public float Velocity;
        public MotorInfo(float degreesPerSecondAt100Power, float accelTime)
        {
            DegreesPerSecondAt100Power = degreesPerSecondAt100Power;
            AccelTime = accelTime;
            maxVelocityDegreesPerMilli = (DegreesPerSecondAt100Power / 1000);
            accel = maxVelocityDegreesPerMilli / AccelTime;
            Velocity = 0;
        }

        public float GetMaxTravelVelocity(int power)
        {
            float percentPower = ((float)power) / 100f;
            return maxVelocityDegreesPerMilli * percentPower;
        }
        long time;
        public float GetDistance(int power, long elapsedMillis)
        {
            time += elapsedMillis;
            float percentPower = ((float)power) / 100f;
            float maxSpeed = GetMaxTravelVelocity(power);

            float acceleration = 0;
            float position = 0;

            if (percentPower != 0)
            {
                acceleration = accel * percentPower;
            }
            else
            {
                float absVelChange = accel * ((float)elapsedMillis);
                if (Math.Abs(Velocity) >= absVelChange)
                {
                    if (Velocity > 0)
                    {
                        acceleration = -accel;
                    }
                    else
                    {
                        acceleration = accel;
                    }
                }
                else
                {
                    Velocity = 0;
                    acceleration = 0;
                }
            }
            Velocity += acceleration * ((float)elapsedMillis);

            if (maxSpeed != 0)
            {
                if(Velocity >= maxSpeed)
                {

                }
                Velocity = Clamp(Velocity, -Math.Abs(maxSpeed), Math.Abs(maxSpeed));
            }

            //Temp
            Velocity = maxSpeed;


            position = Velocity * elapsedMillis;
            return position;
        }
        float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
    }

    public class NotApplicableComponent : Component
    {
        List<Request> requests;
        List<bool> workingRequests;
        public NotApplicableComponent()
            : base(Motors.NotApplicable, new MotorInfo(0, 0))
        {
            requests = new List<Request>();
            workingRequests = new List<bool>();
        }
        public override bool Update(Robot robot, long elapsedMillis)
        {
            bool working = false;
            for (int i = 0; i < requests.Count; i++)
            {
                if (workingRequests[i])
                {
                    request = requests[i];
                    workingRequests[i] = base.Update(robot,elapsedMillis);
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
