using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class DriveByMillis : Command, IActionCommand
    {
        public int LeftPower { get; set; }
        public int RightPower { get; set; }
        public float Distance { get; set; }
        public MoveByMillisMode MoveByMillisMode { get; set; }
        public VisulizeableList<MyVector2> Test { get; set; }
        public DriveByMillis()
        {
            LeftPower = 75;
            RightPower = 75;
            Distance = 0;
            MoveByMillisMode = MoveByMillisMode.AverageMode;
            Test = new VisulizeableList<MyVector2>();
            MiscItemControls distanceControl = new MiscItemControls(true, MiscItemControls.DistanceStoreVal, MiscItemControls.DistanceCancel, MiscItemControls.DistanceUpdate);
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<int>((i)=>LeftPower, (v,i)=>LeftPower = v, "LeftPower",null,Extensions.PowerIsValid),
                new GetSetFunc<int>((i)=>RightPower, (v,i)=>RightPower = v, "RightPower",null, Extensions.PowerIsValid),
                new GetSetFunc<float>((i)=>Distance, (v,i)=>Distance = v, "Distance", new List<Control>(){ distanceControl.GetInfoFromFieldButton("Get Distance") }),

                new GetSetFunc<VisulizeableList<MyVector2>>((i)=>Test, (v,i)=>Test = v, "Test"),
                new GetSetFunc<MoveByMillisMode>((i)=>MoveByMillisMode, (v,i)=>MoveByMillisMode = v, "MoveByMillisMode"),
            };
            Init(false);
        }

        public override Queue<Action> GetActions(Robot robot)
        {
            Queue<Action> actions = new Queue<Action>();
            actions.Enqueue(new Action(GetActionRequests(robot)));
            return actions;
        }
        public List<Request> GetActionRequests(Robot robot)
        {
            return new List<Request>()
            {
                new DriveByMillisRequest(Motors.LeftDrive,LeftPower, Distance, MoveByMillisMode),
                new DriveByMillisRequest(Motors.RightDrive, RightPower, Distance, MoveByMillisMode)
            };
        }

        public override Command CompleteCopy()
        {
            DriveByMillis driveByMillis = new DriveByMillis();
            CopyTo(driveByMillis);
            return driveByMillis;
        }

        //public override VisulizableItem Copy()
        //{
        //    return CopyItems(this);
        //}
    }
    public class DriveByMillisRequest : Request
    {
        float startEncoder = 0;
        MoveByMillisMode moveByMillisMode;
        float distance = 0;
        float accel;
        float accelTime;
        float timeToStartDeceleration;
        float totalTime;
        long time;
        float travelVel;
        float expectedPos;
        public DriveByMillisRequest(Motors motor, int power, float distance, MoveByMillisMode moveByMillisMode)//motor must be drive motor
        {
            this.distance = FieldAndRobotInfo.MillisToDegrees(distance);
            this.moveByMillisMode = moveByMillisMode;
            Motor = motor;
            Power = power;
        }
        public override void InitRequest(Robot robot)
        {
            startEncoder = GetEncoder(moveByMillisMode, robot);
            GetTrapInfo(robot);
        }
        static float GetEncoder(MoveByMillisMode moveByMillisMode, Robot robot)
        {
            switch (moveByMillisMode)
            {
                case (MoveByMillisMode.LeftDriveMode):
                    return robot.MotorEncoders[Motors.LeftDrive];
                case (MoveByMillisMode.RightDiveMode):
                    return robot.MotorEncoders[Motors.RightDrive];
                default:
                    return (robot.MotorEncoders[Motors.LeftDrive] + robot.MotorEncoders[Motors.RightDrive]) / 2;
            }
        }
        void GetTrapInfo(Robot robot)
        {
            expectedPos = 0;
            time = 0;
            travelVel = robot.Components[Motor].MotorInfo.GetMaxTravelVelocity(Power);
            accelTime = robot.Components[Motor].MotorInfo.AccelTime;
            float decelTime = accelTime * ((float)Power / 100);
            float decel = travelVel / decelTime;
            accel = travelVel / accelTime;
            float decelDistance = ((float)Math.Pow(accelTime, 2) * accel)/2;
            float accelDistance = ((float)Math.Pow(decelTime, 2) * decel)/2;
            float changePosAtMaxVel = distance - decelDistance- accelDistance;
            float timeAtMaxVel = 0;
            if (travelVel != 0)
            {
                timeAtMaxVel = changePosAtMaxVel / travelVel;
            }
            timeToStartDeceleration = accelTime + timeAtMaxVel;
            totalTime = timeToStartDeceleration + accelTime;
        }

        public override bool UpdateRequest(Robot robot, long elapsedMillis)
        {
            float currentEncoder = GetEncoder(moveByMillisMode, robot);
            float currentDistance = Math.Abs(currentEncoder - startEncoder);
            //if (time > timeToStartDeceleration)//time < totalTime
            //{
            //    Power = 0;
            //}
            time += elapsedMillis;
            if (currentDistance >= distance)
            {
                Power = 0;
                return false;
            }
            return true;
        }
    }
}
