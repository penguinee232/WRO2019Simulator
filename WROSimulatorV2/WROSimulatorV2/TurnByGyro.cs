using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public class TurnByGyro : Command, IActionCommand
    {
        public int LeftPower { get; set; }
        public int RightPower { get; set; }
        public float Degrees { get; set; }
        public TurnByGyro()
        {
            LeftPower = 75;
            RightPower = 75;
            Degrees = 0;
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<int>((i)=>LeftPower, (v,i)=>LeftPower = v, "LeftPower",null,Extensions.PowerIsValid),
                new GetSetFunc<int>((i)=>RightPower, (v,i)=>RightPower = v, "RightPower",null, Extensions.PowerIsValid),
                new GetSetFunc<float>((i)=>Degrees, (v,i)=>Degrees = v, "Degrees"),
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
                new TurnByGyroRequest(Motors.LeftDrive,LeftPower, Degrees),
                new TurnByGyroRequest(Motors.RightDrive, RightPower, Degrees)
            };
        }

        public override Command CompleteCopy()
        {
            TurnByGyro driveByMillis = new TurnByGyro();
            CopyTo(driveByMillis);
            return driveByMillis;
        }
    }
    public class TurnByGyroRequest : Request
    {
        public static readonly float RotationDeadzone = 1;
        float endDegreePos;
        float degrees;
        public TurnByGyroRequest(Motors motor, int power, float degrees)
        {
            this.degrees = degrees;
            Motor = motor;
            Power = power;
        }
        public override void InitRequest(Robot robot)
        {
            endDegreePos = robot.Rotation + degrees;
        }
        public override bool UpdateRequest(Robot robot, long elapsedMillis)
        {
            float currentDegrees = robot.Rotation;
        }
    }
}
