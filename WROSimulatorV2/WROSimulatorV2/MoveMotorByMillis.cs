using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public class MoveMotorByMillis:Command,IActionCommand
    {
        public int Power { get; set; }
        public float Distance { get; set; }
        public Motors Motor { get; set; }
        public MoveMotorByMillis()
        {
            Power = 75;
            Distance = 0;
            Motor = Motors.LeftDrive;
            ItemsNotValid<Motors> itemsNotValid = new ItemsNotValid<Motors>(new HashSet<Motors>() { Motors.Other });
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<int>((i)=>Power, (v,i)=>Power = v, "Power", null, Extensions.PowerIsValid),
                new GetSetFunc<float>((i)=>Distance, (v,i)=>Distance = v, "Distance"),
                new GetSetFunc<Motors>((i)=>Motor, (v,i)=>Motor = v, "Motor", null, itemsNotValid.IsValid),
            };
            Init();
        }
        public override Queue<Action> GetActions(Robot robot)
        {
            Queue<Action> actions = new Queue<Action>();
            actions.Enqueue(new Action(GetActionRequests(robot)));
            return actions;
        }

        public List<Request> GetActionRequests(Robot robot)
        {
            return new List<Request>() { new MoveMotorByMillisRequest(Motor, Power, Distance) };
        }

        public override Command CompleteCopy()
        {
            return (Command)Copy();
        }

        //public override VisulizableItem Copy()
        //{
        //    return CopyItems(this);
        //}
    }
    public class MoveMotorByMillisRequest : Request
    {
        float startEncoder = 0;
        float distance = 0;
        public MoveMotorByMillisRequest(Motors motor, int power, float distance)
        {
            Motor = motor;
            Power = power;
            this.distance = distance;
        }
        public override void InitRequest(Robot robot)
        {
            startEncoder = robot.MotorEncoders[Motor];
        }
        public override bool UpdateRequest(Robot robot)
        {
            float currentEncoder = robot.MotorEncoders[Motor];
            float currentDistance = Math.Abs(currentEncoder - startEncoder);
            if (currentDistance >= distance)
            {
                Power = 0;
                return false;
            }
            return true;
        }
    }
}
