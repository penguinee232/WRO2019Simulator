using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{

    public class Test : Command
    {
        public int Int { get; set; }
        public float Float { get; set; }
        public TestEnum TestEnum { get; set; }
        public MyVector2 MyVector2 { get; set; }
        public VisulizeableList<string> List { get; set; }
        public VisulizeableList<VisulizeableList<double>> List2 { get; set; }
        public Test()
        {
            Int = 0;
            Float = 0;
            TestEnum = TestEnum.One;
            MyVector2 = new MyVector2(0, 0);
            List = new List<string>() { "Hello", "World", "!!!" };
            List2 = new List<VisulizeableList<double>>() { new List<double>() { 1, 2, 3 }, new List<double>() { -1, -2, -3 }, new List<double>() { 10, 20, 30 } };
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<VisulizeableList<VisulizeableList<double>>>((i)=>List2, (v,i)=>List2 = v, "List2"),
                new GetSetFunc<int>((i)=>Int, (v,i)=>Int = v, "Int"),
                new GetSetFunc<VisulizeableList<string>>((i)=>List, (v,i)=>List = v, "List"),
                new GetSetFunc<MyVector2>((i)=>MyVector2, (v,i)=>MyVector2 = v, "MyVector2"),
                new GetSetFunc<float>((i)=>Float, (v,i)=>Float = v, "Float"),
                new GetSetFunc<TestEnum>((i)=>TestEnum, (v,i)=>TestEnum = v, "TestEnum")
            };
            Init();
        }

        public override Queue<Action> GetActions(Robot robot)
        {
            throw new NotImplementedException();
        }

        public override VisulizableItem Copy()
        {
            return CopyItems(this);
        }

        public override Command CompleteCopy()
        {
            throw new NotImplementedException();
        }
    }


    public class Test2 : Command
    {
        public float Distance { get; set; }
        public Motors Motor { get; set; }
        public MyVector2 Position { get; set; }
        public VariableVisulizeItem Variable { get; set; }
        public VisulizeableList<bool> BoolTest { get; set; }
        public Test2()
        {
            Distance = 0;
            Motor = Motors.LeftDrive;
            Position = new MyVector2(0, 0);
            Variable = new VariableVisulizeItem();
            BoolTest = new VisulizeableList<bool>();
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<float>((i)=>Distance, (v,i)=>Distance = v, "Distance"),
                new GetSetFunc<Motors>((i)=>Motor, (v,i)=>Motor = v, "Motor"),
                new GetSetFunc<MyVector2>((i)=>Position, (v,i)=>Position = v, "Position"),
                new GetSetFunc<VariableVisulizeItem>((i)=>Variable, (v,i)=>Variable = v, "Variable"),
                new GetSetFunc<VisulizeableList<bool>>((i)=>BoolTest, (v,i)=>BoolTest = v, "BoolTest")
            };
            Init();
        }

        public override Queue<Action> GetActions(Robot robot)
        {
            throw new NotImplementedException();
        }
        public override VisulizableItem Copy()
        {
            return CopyItems(this);
        }

        public override Command CompleteCopy()
        {
            throw new NotImplementedException();
        }
    }
}
