using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public static class Extensions
    {
        public static Point Add(this Point pt, Point other)
        {
            return new Point(pt.X + other.X, pt.Y + other.Y);
        }
        public static Point Subtract(this Point pt, Point other)
        {
            return new Point(pt.X - other.X, pt.Y - other.Y);
        }
        public static Point Multiply(this Point pt, int other)
        {
            return new Point(pt.X * other, pt.Y * other);
        }
        public static PointF Add(this PointF pt, PointF other)
        {
            return new PointF(pt.X + other.X, pt.Y + other.Y);
        }
        public static PointF Subtract(this PointF pt, PointF other)
        {
            return new PointF(pt.X - other.X, pt.Y - other.Y);
        }
        public static PointF Multiply(this PointF pt, PointF other)
        {
            return new PointF(pt.X * other.X, pt.Y * other.Y);
        }
        public static PointF Divide(this PointF pt, PointF other)
        {
            return new PointF(pt.X / other.X, pt.Y / other.Y);
        }

        public static PointF Multiply(this PointF pt, float other)
        {
            return new PointF(pt.X * other, pt.Y * other);
        }
        public static PointF Divide(this PointF pt, float other)
        {
            return new PointF(pt.X / other, pt.Y / other);
        }
        public static Point ToPoint(this Size size)
        {
            return new Point(size.Width, size.Height);
        }
        public static Size ToSize(this Point pt)
        {
            return new Size(pt.X, pt.Y);
        }
        public static PointF ToPoint(this SizeF size)
        {
            return new PointF(size.Width, size.Height);
        }
        public static SizeF ToSize(this PointF pt)
        {
            return new SizeF(pt.X, pt.Y);
        }
        public static object GetDefaultFromConstructor(Type t)
        {
            var cs = t.GetConstructors();
            if (cs.Length == 0)
            {
                if (t.IsValueType)
                {
                    return Activator.CreateInstance(t);
                }
                return null;
            }

            ConstructorInfo c = null;
            foreach(var con in cs)
            {
                if(con.GetParameters().Length == 0)
                {
                    c = con;
                }
            }
            if (c == null)
            {
                c = cs[0];
            }
            var paramaterInfos = c.GetParameters();
            object[] paramaters = new object[paramaterInfos.Length];
            for (int i = 0; i < paramaterInfos.Length; i++)
            {
                object val = paramaterInfos[i].DefaultValue;
                if (val.GetType() != paramaterInfos[i].ParameterType)
                {
                    val = GetDefault(paramaterInfos[i].ParameterType);
                }
                paramaters[i] = val;
            }
            return c.Invoke(paramaters);
        }
        public static T GetDefaultFromConstructor<T>()
        {
            return (T)GetDefaultFromConstructor(typeof(T));
        }
        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        public static double Distance(this PointF p, PointF point)
        {
            return Math.Sqrt(Math.Pow(point.X - p.X, 2) + Math.Pow(point.Y - p.Y, 2));
        }
        public static double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }
        public static double ToDegrees(double val)
        {
            return (180 / Math.PI) * val;
        }
        public static PointF[] GetDestinationPoints(RectangleF rectangle, PointF origin, float rotation)
        {
            rotation *= -1;
            PointF[] points = new PointF[] { new PointF(0, 0), new PointF(rectangle.Size.Width, 0), new PointF(0, rectangle.Size.Height) };

            for (int i = 0; i < points.Length; i++)
            {
                PointF p = points[i];
                PointF originDistance = new PointF(p.X - origin.X, p.Y - origin.Y);
                double angle = Math.Atan2(originDistance.Y, originDistance.X);
                angle += Extensions.ToRadians(rotation);
                double h = originDistance.Distance(new PointF(0, 0));
                double x = Math.Cos(angle) * h;
                double y = Math.Sin(angle) * h;
                points[i] = new PointF((float)x + rectangle.X + origin.X, (float)y + rectangle.Y + origin.Y);
            }

            return points;
        }
        static Dictionary<Type, HashSet<Type>> ImplicitNumericConversions;
        static void InitImplicitNumericConversions()
        {
            ImplicitNumericConversions = new Dictionary<Type, HashSet<Type>>();
            ImplicitNumericConversions.Add(typeof(sbyte), new HashSet<Type> { typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(byte), new HashSet<Type> { typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(short), new HashSet<Type> { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(ushort), new HashSet<Type> { typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(int), new HashSet<Type> { typeof(long), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(uint), new HashSet<Type> { typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(long), new HashSet<Type> { typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(char), new HashSet<Type> { typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(float), new HashSet<Type> { typeof(double) });
            ImplicitNumericConversions.Add(typeof(ulong), new HashSet<Type> { typeof(float), typeof(double), typeof(decimal) });
        }
        public static bool CanImplicitCast(this Type type, Type other)
        {

            if (type.IsEquivalentTo(other) || type.IsSubclassOf(other))
            {
                return true;
            }
            if (type.IsAssignableFrom(other))
            {
                return true;
            }
            if (ImplicitNumericConversions == null)
            {
                InitImplicitNumericConversions();
            }
            if (ImplicitNumericConversions.ContainsKey(type) && ImplicitNumericConversions[type].Contains(other))
            {
                return true;
            }
            return isImplicitFunction(type, other);
        }
        public static LabeledControl UpdateVisual(LabeledControl parent, Form1 form)
        {
            //throw new NotImplementedException();
            bool parentPartOfRadioButtonGroup = parent.ParentNode != null && parent.ParentNode.Control.GetType() == typeof(LabeledControl);
            if (parentPartOfRadioButtonGroup)
            {
                LabeledControl parentParent = (LabeledControl)parent.ParentNode.Control;
                parentPartOfRadioButtonGroup = parentPartOfRadioButtonGroup && parentParent.RadioButtonGroup != null;
            }
            var newNode = Form1.LoadItem(parent.GetSetFunc, new System.Drawing.Point(0, 0), parent.ParentNode, parent.Index, parentPartOfRadioButtonGroup, form);

            ControlNode parentParentNode = parent.ParentNode;

            //int index = parent.Parent.Controls.IndexOf(parentParentNode.Children[parent.Index].Control);
            Point initScroll = form.Panel.AutoScrollPosition;
            parentParentNode.Children[parent.Index] = newNode;
            Control parentParentControl = parent.Parent;
            Control[] controls = new Control[parentParentControl.Controls.Count];
            parentParentControl.Controls.CopyTo(controls, 0);
            parentParentControl.Controls.Clear();
            for (int i = 0; i < controls.Length; i++)
            {
                if (i != parent.Index)
                {
                    parentParentControl.Controls.Add(controls[i]);
                }
                else
                {
                    parentParentControl.Controls.Add(newNode.Control);
                }
            }
            newNode.ReLocateChildren(Form1.spaceAmount);
            form.Panel.AutoScrollPosition = new Point(-initScroll.X, -initScroll.Y);
            return (LabeledControl)newNode.Control;
            //form.Panel.VerticalScroll.Value = initScroll.Y * -1;
            //parentParentControl.Controls.RemoveAt(index);
            //parentParentControl.Controls.Add(newNode.Control);
            //parentParentControl.Controls.SetChildIndex(newNode.Control, index);

            //parent.ControlNode.Children.Clear();
            //parent.ControlNode.Children.AddRange(newNode.Children);
            //parent.ControlNode.Control.Controls.Clear();
            //foreach (var child in newNode.Children)
            //{
            //    parent.ControlNode.Control.Controls.Add(child.Control);
            //}
            //parentParentNode.ReLocateChildren(Form1.spaceAmount);
        }

        static bool isImplicitFunction(Type type, Type other)
        {
            var methodA = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            List<MethodInfo> methods = methodA.ToList();
            methods.AddRange(other.GetMethods(BindingFlags.Public | BindingFlags.Static));
            foreach (var m in methods)
            {
                if (m.Name == "op_Implicit")
                {
                    ParameterInfo pi = m.GetParameters().FirstOrDefault();
                    if (pi == null)
                    {
                        continue;
                    }
                    Type piType = pi.ParameterType;
                    if ((piType == type && m.ReturnType == other) ||
                        (piType == other && m.ReturnType == piType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static Dictionary<Type, string> typeNames;
        public static void InitTypeNames()
        {
            typeNames = new Dictionary<Type, string>();
            typeNames.Add(typeof(int), "int");
            typeNames.Add(typeof(float), "float");
            typeNames.Add(typeof(double), "double");
            typeNames.Add(typeof(long), "long");
            typeNames.Add(typeof(short), "short");
            typeNames.Add(typeof(uint), "uint");
            typeNames.Add(typeof(ulong), "ulong");
            typeNames.Add(typeof(ushort), "ushort");
            typeNames.Add(typeof(byte), "byte");
            typeNames.Add(typeof(sbyte), "sbyte");
            typeNames.Add(typeof(char), "char");
            typeNames.Add(typeof(string), "string");
        }
        public static string GetTypeName(this Type type)
        {
            if (typeNames == null) { InitTypeNames(); }
            if (typeNames.ContainsKey(type)) { return typeNames[type]; }
            string name = type.Name;
            if (!type.IsConstructedGenericType) { return name; }
            var genericTypes = type.GetGenericArguments();
            name = name.Substring(0, name.Length - (1 + genericTypes.Length.ToString().Length));
            name += "<";
            for (int i = 0; i < genericTypes.Length; i++)
            {
                if (i != 0)
                {
                    name += ", ";
                }
                name += GetTypeName(genericTypes[i]);
            }
            name += ">";
            return name;
        }
        public static bool PowerIsValid(int power)
        {
            return power >= -100 && power <= 100;
        }
        public static bool IsTheSameGenericType(this Type type1, Type type2)
        {
            if(type1 == type2)
            {
                return true;
            }
            return type1.Name == type2.Name;
        }
        public static string GetActualOperator(this Operatiors operatior)
        {
            switch(operatior)
            {
                case (Operatiors.NotEqual):
                    return "!=";
                case (Operatiors.LessThan):
                    return "<";
                case (Operatiors.GreaterThan):
                    return ">";
                case (Operatiors.LessThanEqual):
                    return "<=";
                case (Operatiors.GreaterThanEqual):
                    return ">=";
                default:
                    return "==";
            }
        }
    }
}
