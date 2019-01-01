using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public class MyRectangle : VisulizableItem, IEquatable<MyRectangle>
    {
        public MyVector2 Position { get; set; }
        public MyVector2 Size { get; set; }
        public MyRectangle(float x, float y, float width, float height)
        {
            Position = new MyVector2(x, y);
            Size = new MyVector2(width, height);
            VisulizeItems = new List<IGetSetFunc>();

            VisulizeItems.Add(new GetSetFunc<MyVector2>((i) => Position, (v, i) => Position = v, "Position"));
            VisulizeItems.Add(new GetSetFunc<MyVector2>((i) => Size, (v, i) => Size = v, "Size"));
            Init(false);
        }
        public static bool operator ==(MyRectangle left, MyRectangle right)
        {
            return left.Position == right.Position && left.Size == right.Size;
        }
        public static bool operator !=(MyRectangle left, MyRectangle right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(MyRectangle) || obj.GetType().IsSubclassOf(typeof(MyRectangle)))
            {
                return ((MyRectangle)obj) == this;
            }
            return false;
        }

        public bool Equals(MyRectangle other)
        {
            return this == other;
        }

        public override void CopyTo(VisulizableItem newItem)
        {
            CopyItems(newItem, this);
        }
    }
    public class MyVector2 : VisulizableItem, IEquatable<MyVector2>
    {
        public float X { get; set; }
        public float Y { get; set; }


        public MyVector2(float x, float y)
        {
            X = x;
            Y = y;
            VisulizeItems = new List<IGetSetFunc>();

            VisulizeItems.Add(new GetSetFunc<float>((i) => X, (v, i) => X = v, "X"));
            VisulizeItems.Add(new GetSetFunc<float>((i) => Y, (v, i) => Y = v, "Y"));
            Init(false);
        }
        public static bool operator ==(MyVector2 left, MyVector2 right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        public static bool operator !=(MyVector2 left, MyVector2 right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() == typeof(MyVector2) || obj.GetType().IsSubclassOf(typeof(MyVector2)))
            {
                return ((MyVector2)obj) == this;
            }
            return false;
        }
        public bool Equals(MyVector2 other)
        {
            return this == other;
        }
        public MyVector2 Copy()
        {
            return new MyVector2(X, Y);
        }
        public override void CopyTo(VisulizableItem newItem)
        {
            CopyItems(newItem, this);
        }
    }
}
