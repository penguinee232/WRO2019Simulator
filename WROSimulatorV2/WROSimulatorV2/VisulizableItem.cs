using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    //public interface IVisulizableItem
    //{
    //    List<IGetSetFunc> VisulizeItems { get; }
    //}
    public abstract class VisulizableItem
    {
        public virtual List<Control> GetManatoryControls(IGetSetFunc getSetFunc, int index)
        {
            return null;
        }
        //public static List<ItemInfo> VisulizeItemTypes { get; protected set; }
        public List<IGetSetFunc> VisulizeItems { get; protected set; }
        ControlNode controlNode;
        public ControlNode ControlNode
        {
            get { return controlNode; }
            set { controlNode = value; ControlNodeChanged(); }
        }
        protected virtual void ControlNodeChanged()
        {

        }
        public void Init(bool wipeControls)
        {
            for (int i = 0; i < VisulizeItems.Count; i++)
            {
                IndexInit(i, wipeControls);
            }

        }
        public void IndexInit(int i, bool wipeControls)
        {
            IGetSetFunc getSetFunc = VisulizeItems[i];
            if (getSetFunc.ItemInfo.Type.IsSubclassOf(typeof(VisulizableItem)))
            {
                var item = (VisulizableItem)getSetFunc.ObjGet(i);
                var controls = item.GetManatoryControls(getSetFunc, i);
                if (controls != null)
                {
                    if (wipeControls)
                    {
                        getSetFunc.Controls.Clear();
                    }
                    getSetFunc.Controls.AddRange(controls);
                }
            }
        }
        public VisulizableItem GetItemWithVariablesSet()
        {
            VisulizableItem item = (VisulizableItem)Extensions.GetDefaultFromConstructor(GetType());
            this.CopyTo(item);

            SetCommandVariablessR(item);

            return item;
        }
        void SetCommandVariablessR(VisulizableItem item)
        {
            for (int i = 0; i < item.VisulizeItems.Count; i++)
            {
                IGetSetFunc getSet = item.VisulizeItems[i];
                if (getSet.IsVariable)
                {
                    getSet.ObjSet(VariablesInfo.GetVariable(getSet.Variable.Value.Get()), i);
                }
                else if (getSet.ItemInfo.Type.IsSubclassOf(typeof(VisulizableItem)))
                {
                    VisulizableItem child = (VisulizableItem)getSet.ObjGet(i);
                    SetCommandVariablessR(child);
                }
                //else
                //{
                //    newGetSet.ObjSet(currentGetSet.ObjGet(i), i);
                //}
            }
        }

        public abstract void CopyTo(VisulizableItem newItem);
        public static void CopyItems(VisulizableItem current)
        {
            VisulizableItem visulizableItem = (VisulizableItem)Extensions.GetDefaultFromConstructor(current.GetType());
            CopyItems(visulizableItem, current);
        }
        public static void CopyItems(VisulizableItem newItem, VisulizableItem current)
        {
            CopyItemsR(newItem, current);
        }

        static void CopyItemsR(VisulizableItem newItem, VisulizableItem current)
        {
            for (int i = 0; i < current.VisulizeItems.Count; i++)
            {
                IGetSetFunc currentGetSet = current.VisulizeItems[i];
                if (i >= newItem.VisulizeItems.Count && newItem.GetType().GetInterfaces().Contains(typeof(IVisulizeableList)))
                {
                    IVisulizeableList list = (IVisulizeableList)newItem;
                    object value;
                    if (list.ObjectType.IsClass)
                    {
                        value = Extensions.GetDefaultFromConstructor(list.ObjectType);
                    }
                    else
                    {
                        value = Extensions.GetDefault(list.ObjectType);
                    }
                    list.Add(value);
                }
                IGetSetFunc newGetSet = newItem.VisulizeItems[i];
                currentGetSet.CopyBasicInfo(newGetSet);

                if (currentGetSet.ItemInfo.Type.IsSubclassOf(typeof(VisulizableItem)))
                {
                    VisulizableItem currentChild = (VisulizableItem)currentGetSet.ObjGet(i);
                    VisulizableItem newChild = (VisulizableItem)newGetSet.ObjGet(i);
                    //CopyItemsR(newChild, currentChild);
                    currentChild.CopyTo(newChild);
                }
                else
                {
                    newGetSet.ObjSet(currentGetSet.ObjGet(i), i);
                }
            }
        }

        public static VisulizableItem Deserialize(string serializedStuff, out Variable? variable)
        {
            return (VisulizableItem)Deserialize(new Span<char>(serializedStuff.ToArray()), out variable);
        }

        protected virtual void Deserialize(Span<char> span)
        {
            var list = DeserializeItems(span);
            for (int i = 0; i < list.Count; i++)
            {
                //if (list[i].Variable == null)
                //{
                VisulizeItems[i].ObjSet(list[i].Value, i);
                //}
                VisulizeItems[i].Variable = VariablesInfo.GetVariableGetSet(list[i].Variable);
            }
        }
        protected class PreGetSetFuncInfo
        {
            public object Value { get; set; }
            public Variable? Variable { get; set; }
            public PreGetSetFuncInfo(object value, Variable? variable)
            {
                Value = value;
                Variable = variable;
            }
        }
        protected List<PreGetSetFuncInfo> DeserializeItems(Span<char> span)
        {
            List<PreGetSetFuncInfo> list = new List<PreGetSetFuncInfo>();
            span = span.Slice(1);//slice: {
            int inBraceAmount = 0;
            int inQuoteAmount = 0;
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] == '"')
                {
                    if (inQuoteAmount > 0)
                    {
                        inQuoteAmount--;
                    }
                    else
                    {
                        inQuoteAmount++;
                    }
                }
                else if (inQuoteAmount == 0)
                {

                    if (span[i] == '{')
                    {
                        inBraceAmount++;
                    }
                    else if (span[i] == '}')
                    {
                        inBraceAmount--;
                        if (inBraceAmount < 0)
                        {
                            Variable? variable;
                            var newSpan = span.Slice(0, i);
                            if (newSpan.Length > 0)
                            {
                                object val = Deserialize(newSpan, out variable);
                                list.Add(new PreGetSetFuncInfo(val, variable));
                            }
                            break;
                        }
                    }
                    else if (inBraceAmount == 0 && span[i] == ',')
                    {
                        Variable? variable;
                        var newSpan = span.Slice(0, i);
                        if (newSpan.Length > 0)
                        {
                            object val = Deserialize(newSpan, out variable);
                            list.Add(new PreGetSetFuncInfo(val, variable));
                        }
                        span = span.Slice(i + 1);
                        i = 0;
                    }
                }
            }
            return list;
        }

        protected static object Deserialize(Span<char> span, out Variable? variable)
        {
            bool isVariable = false;
            if (span[0] == '$')
            {
                isVariable = true;
                span = span.Slice(1);
            }
            span = span.Slice(1);
            int secondParen = 0;
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] == ')')
                {
                    secondParen = i;
                    break;
                }
            }
            string typeArea = Substing(span.Slice(0, secondParen));
            string[] types = typeArea.Split('|');
            string type = types[0];
            string assemblyType = types[1];
            assemblyType = assemblyType.Substring(1, assemblyType.Length - 2);
            span = span.Slice(secondParen + 1);
            Type currentType = Type.GetType(assemblyType);
            object value;
            if (currentType.IsClass)
            {
                value = Extensions.GetDefaultFromConstructor(currentType);
            }
            else
            {
                value = Extensions.GetDefault(currentType);
            }
            if (isVariable)
            {
                variable = new Variable(currentType, Substing(span));
                return value;
            }
            else
            {
                variable = null;
                if (span.Length > 0 && span[0] == '{' && currentType.IsSubclassOf(typeof(VisulizableItem)))
                {
                    VisulizableItem visulizableItem = (VisulizableItem)value;
                    visulizableItem.Deserialize(span);
                    return visulizableItem;
                }
                else
                {
                    //if (currentType == typeof(Variable))
                    //{
                    //    return Variable.Deserialize(Substing(span));
                    //}
                    //else
                    //{
                    var converter = TypeDescriptor.GetConverter(currentType);
                    return converter.ConvertFrom(Substing(span));
                    //}
                }
            }
        }
        static string Substing(Span<char> span)
        {
            string s = "";
            for (int i = 0; i < span.Length; i++)
            {
                s += span[i];
            }
            return s;
        }

        public virtual string Serialize()
        {
            List<object> items = new List<object>();
            List<Variable?> variables = new List<Variable?>();
            for (int i = 0; i < VisulizeItems.Count; i++)
            {
                items.Add(VisulizeItems[i].ObjGet(i));
                variables.Add(VariableGetSet.GetNullableVariable(VisulizeItems[i].Variable));
            }
            return Serialize(this, items, variables);
        }

        protected static string Serialize(VisulizableItem thisItem, List<object> items, List<Variable?> variables)
        {
            string serialize = "";
            serialize += SerializeType(thisItem.GetType());//"(" + thisItem.GetType().GetTypeName() + ")";
            serialize += "{";

            for (int i = 0; i < items.Count; i++)
            {
                if (variables[i] != null)
                {
                    serialize += variables[i].Value.Serialize();
                }
                else
                {
                    Type t = items[i].GetType();
                    if (t.IsSubclassOf(typeof(VisulizableItem)))
                    {
                        serialize += ((VisulizableItem)items[i]).Serialize();
                    }
                    else
                    {
                        serialize += SerializeType(t);
                        if (t.IsEnum)
                        {
                            serialize += ((int)items[i]).ToString();
                        }
                        else
                        {
                            serialize += items[i].ToString();
                        }
                    }
                }
                if (i + 1 < items.Count)
                {
                    serialize += ",";
                }
            }

            serialize += "}";
            return serialize;
        }

        public static string SerializeType(Type type)
        {
            return "(" + type.GetTypeName() + "|" + '"' + type.AssemblyQualifiedName + '"' + ")";
        }
    }
    public struct Variable
    {
        public Type Type { get; }
        public string Name { get; }
        public Variable(Type type, string name)
        {
            Type = type;
            Name = name;
        }
        public override string ToString()
        {
            return Name + " (" + Type.GetTypeName() + ")";
        }
        public static Variable Default()
        {
            return new Variable(typeof(int), "intVariable1");
        }
        public string Serialize()
        {
            return "$" + VisulizableItem.SerializeType(Type) + Name;
        }
    }
    public interface IGetSetFunc
    {
        object ObjGet(int index);
        void ObjSet(object value, int index);
        ItemInfo ItemInfo { get; set; }
        List<Control> Controls { get; set; }
        VariableGetSet? Variable { get; set; }
        bool IsVariable { get; }
        void CopyBasicInfo(IGetSetFunc newFunc);
        bool IsValidItem(object item);
    }
    public class GetSetFunc<T> : IGetSetFunc
    {
        public ItemInfo ItemInfo { get; set; }
        public Func<int, T> Get;
        public Action<T, int> Set;
        public List<Control> Controls { get; set; }
        public VariableGetSet? Variable { get; set; }
        public bool IsVariable { get { return Variable != null; } }
        public Func<T, bool> IsValidItemFunc { get; set; }
        public GetSetFunc(Func<int, T> get, Action<T, int> set, string name, List<Control> controls = null, Func<T, bool> isValidItemFunc = null)
        {
            IsValidItemFunc = isValidItemFunc;
            if (controls == null)
            {
                controls = new List<Control>();
            }
            Controls = controls;
            ItemInfo = new ItemInfo(typeof(T), name);
            Get = get;
            Set = set;
            Variable = null;
        }
        public T GetVal(int index)
        {
            return Get.Invoke(index);
        }
        public void SetVal(T val, int index)
        {
            Set.Invoke(val, index);
        }
        public object ObjGet(int index)
        {
            return GetVal(index);
        }
        public void ObjSet(object value, int index)
        {
            T val;
            if (typeof(T) == typeof(object))
            {
                val = (T)value;
            }
            else if (typeof(T) == value.GetType())
            {
                val = (T)value;
            }
            else
            {
                val = (T)Convert.ChangeType(value, typeof(T));
            }
            SetVal(val, index);
        }

        public void CopyBasicInfo(IGetSetFunc newFunc)
        {
            if (IsVariable)
            {
                newFunc.Variable = Variable.Value;
            }
            else
            {
                newFunc.Variable = null;
            }
            newFunc.ItemInfo = ItemInfo;
            //newFunc.Controls = Controls;
        }

        public bool IsValidItem(object item)
        {
            if (IsValidItemFunc != null)
            {
                T val;
                if (typeof(T) == typeof(object))
                {
                    val = (T)item;
                }
                else
                {
                    val = (T)Convert.ChangeType(item, typeof(T));
                }
                return IsValidItemFunc.Invoke(val);
            }
            return true;
        }
    }

    public struct ItemInfo
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public ItemInfo(Type type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
