using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public struct VariableValueGetSet
    {
        public ItemInfo ItemInfo { get; set; }
        Func<object, object> Get;
        Action<object, object> Set;
        public object Info { get; set; }
        public VariableValueGetSet(Func<object, object> get, Action<object, object> set, string name, Type type, object info)
        {
            ItemInfo = new ItemInfo(type, name);
            Get = get;
            Set = set;
            Info = info;
        }
        public object GetVal()
        {
            return Get.Invoke(Info);
        }
        public void SetVal(object val)
        {
            Set.Invoke(val, Info);
        }
    }
    public static class VariablesInfo
    {
        static Dictionary<Variable, TreeNode> VariableInfos;
        public static Dictionary<Type, List<Variable>> VariablesByType;
        static Dictionary<Variable, VariableValueGetSet> VariableValueGetSet;
        static Dictionary<Variable, object> VariableValues;
        static Dictionary<Variable, object> VariableInitalValues;
        public static Dictionary<Variable, IVariableGetSet> VariableGetSet;
        public static void InitializeVariables()
        {
            VariablesByType = new Dictionary<Type, List<Variable>>();

            Variable floatVariable1 = new Variable(typeof(float), "floatVariable1");
            Variable MyVector2Variable1 = new Variable(typeof(MyVector2), "MyVector2Variable1");
            Variable intVariable = new Variable(typeof(int), "intVariable1");
            Variable rectVariable = new Variable(typeof(MyRectangle), "rectVariable");

            VariablesByType.Add(typeof(float), new List<Variable>() { floatVariable1 });
            VariablesByType.Add(typeof(int), new List<Variable>() { intVariable });
            VariablesByType.Add(typeof(MyVector2), new List<Variable>() { MyVector2Variable1 });
            VariablesByType.Add(typeof(MyRectangle), new List<Variable>() { rectVariable });

            VariableValueGetSet = new Dictionary<Variable, VariableValueGetSet>();
            VariableValues = new Dictionary<Variable, object>();
            VariableInitalValues = new Dictionary<Variable, object>();
            VariableInfos = new Dictionary<Variable, TreeNode>();
            VariableGetSet = new Dictionary<Variable, IVariableGetSet>();
            foreach (var vt in VariablesByType)
            {
                for (int i = 0; i < vt.Value.Count; i++)
                {
                    Variable v = vt.Value[i];
                    object value;
                    if (vt.Key.IsClass)
                    {
                        value = Extensions.GetDefaultFromConstructor(vt.Key);
                    }
                    else
                    {
                        value = Extensions.GetDefault(vt.Key);
                    }
                    //VariableValues.Add(v, value);
                    //VariableValueGetSet.Add(v, new VariableValueGetSet((j) => VariableValues[v], (val, j) => VariableValues[v] = val, v.Name, v.Type, null));
                    AddVariable(v, value, null, i, null);
                    VariableInitalValues.Add(v, value);
                    //VariableGetSet.Add(v, new VariableGetSet(v.Type, i));
                }
            }
        }

        public static IVariableGetSet GetVariableGetSet(Variable? variable)
        {
            if (variable == null)
            {
                return null;
            }
            return VariableGetSet[variable.Value];
        }

        public static void ResetVariables()
        {
            foreach (var v in VariableInitalValues)
            {
                object value = v.Value;
                if (v.Key.Type.IsSubclassOf(typeof(VisulizableItem)))
                {
                    VisulizableItem visulizableItem = (VisulizableItem)value;
                    VisulizableItem newVisItem = (VisulizableItem)Extensions.GetDefaultFromConstructor(v.Key.Type);
                    visulizableItem.CopyTo(newVisItem);
                    value = newVisItem;
                }
                VariableValueGetSet[v.Key].SetVal(value);
            }
        }
        public static IVariableGetSet AddVariable(Variable variable, object value, TreeNode currentTreeNode, int variableByTypeIndex = -1, IVariableGetSet parent = null, IVariableGetSet previousGetSet = null)//, bool addVarGetSetWithoutAddingTypeIndex = false)
        {
            IVariableGetSet variableGetSet;
            bool addToVariablesByType = variableByTypeIndex < 0;
            if (parent == null)
            {
                if (addToVariablesByType)
                {
                    if (!VariablesByType.ContainsKey(variable.Type))
                    {
                        VariablesByType.Add(variable.Type, new List<Variable>());
                    }
                    VariablesByType[variable.Type].Add(variable);
                    variableByTypeIndex = VariablesByType[variable.Type].Count - 1;
                }

                variableGetSet = new VariableGetSet(variable.Type, variableByTypeIndex);
                VariableValueGetSet.Add(variable, new VariableValueGetSet((i) => VariableValues[variable], (val, i) => VariableValues[variable] = val, variable.Name, variable.Type, null));

                VariableValues.Add(variable, value);
            }
            else
            {
                variableGetSet = new ChildVariableGetSet(variable.Type, variableByTypeIndex, parent);
                VariableValueGetSet.Add(variable, GetChildValueGetSet(parent, variableByTypeIndex, variable));
            }

            if (currentTreeNode != null)
            {
                VariableInfos.Add(variable, currentTreeNode);
            }
            //if (addToVariablesByType || addVarGetSetWithoutAddingTypeIndex)
            //{
            if (previousGetSet == null)
            {
                if (variable.Type.IsSubclassOf(typeof(VisulizableItem)))
                {
                    VisulizableItem item = (VisulizableItem)value;
                    for (int i = 0; i < item.VisulizeItems.Count; i++)
                    {
                        Variable child = new Variable(item.VisulizeItems[i].ItemInfo.Type, variable.Name + "." + item.VisulizeItems[i].ItemInfo.Name);
                        variableGetSet.Children.Add(child);
                        AddVariable(child, item.VisulizeItems[i].ObjGet(i), currentTreeNode, i, variableGetSet);
                    }
                }
            }
            else
            {
                variableGetSet = previousGetSet;
            }

            VariableGetSet.Add(variable, variableGetSet);
            //}
            //else
            //{
            //    variableGetSet = VariableGetSet[variable];
            //}

            return variableGetSet;
        }

        static VariableValueGetSet GetChildValueGetSet(IVariableGetSet parent, int index, Variable variable)
        {
            (int Index, IVariableGetSet Parent) childInfo = (index, parent);
            return new VariableValueGetSet(
                (i) =>
                {
                    var info = ((int Index, IVariableGetSet Parent))i;
                    var parentVariable = info.Parent.Get();
                    VisulizableItem parentVal;
                    if (VariableValues.ContainsKey(parentVariable))
                    {
                        parentVal = (VisulizableItem)VariableValues[parentVariable];
                    }
                    else
                    {
                        VariableValueGetSet parentGetSet = GetChildValueGetSet(info.Parent.Parent, info.Parent.Index, parentVariable);
                        parentVal = (VisulizableItem)parentGetSet.GetVal();
                    }

                    return parentVal.VisulizeItems[info.Index].ObjGet(info.Index);
                },
                (val, i) =>
                {
                    var info = ((int Index, IVariableGetSet Parent))i;
                    var parentVariable = info.Parent.Get();
                    VisulizableItem parentVal;
                    if (VariableValues.ContainsKey(parentVariable))
                    {
                        parentVal = (VisulizableItem)VariableValues[parentVariable];
                    }
                    else
                    {
                        VariableValueGetSet parentGetSet = GetChildValueGetSet(info.Parent.Parent, info.Parent.Index, parentVariable);
                        parentVal = (VisulizableItem)parentGetSet.GetVal();
                    }
                    parentVal.VisulizeItems[info.Index].ObjSet(val, info.Index);
                }, variable.Name, variable.Type, childInfo);
        }
        public static IVariableGetSet UpdateVariableName(Variable currentVariable, Variable newVariable, TreeNode treeNode, int index)
        {
            object value = GetVariable(currentVariable);
            var variableGetSet = VariableGetSet[currentVariable];
            RemoveVariable(currentVariable, true);
            return VariablesInfo.AddVariable(newVariable, value, treeNode, index, null, variableGetSet);
        }
        public static void RemoveVariable(Variable variable, bool ignoreVariablesByType)
        {
            if (!ignoreVariablesByType)
            {
                var currentGetSet = VariableGetSet[variable];
                if (currentGetSet.Children != null)
                {
                    foreach (var child in currentGetSet.Children)
                    {
                        RemoveVariable(child, ignoreVariablesByType);
                    }
                }
                VariablesByType[variable.Type].Remove(variable);
            }
            VariableValues.Remove(variable);
            VariableValueGetSet.Remove(variable);
            VariableGetSet.Remove(variable);
            if (VariableInitalValues.ContainsKey(variable))
            {
                VariableInitalValues.Remove(variable);
            }
            if (VariableInfos.ContainsKey(variable))
            {
                VariableInfos.Remove(variable);
            }
        }
        public static bool VariableExists(Variable variable)
        {
            return VariableValueGetSet.ContainsKey(variable);
        }
        public static object GetVariable(Variable variable)
        {
            return VariableValueGetSet[variable].GetVal();
        }
        public static void SetVariable(Variable variable, object value)
        {
            VariableValueGetSet[variable].SetVal(value);
        }

        public static Dictionary<Type, List<IVariableGetSet>> GetVariables(TreeNode currentTreeNode)
        {
            Dictionary<Type, List<IVariableGetSet>> variables = new Dictionary<Type, List<IVariableGetSet>>();
            foreach (var vt in VariablesByType)
            {
                variables.Add(vt.Key, new List<IVariableGetSet>());
                foreach (var v in vt.Value)
                {
                    bool addVariable = true;
                    if (VariableInfos.ContainsKey(v))
                    {
                        addVariable = IsNodeAfter(VariableInfos[v], currentTreeNode);
                    }
                    if (addVariable)
                    {
                        variables[vt.Key].Add(VariableGetSet[v]);
                    }
                }
            }
            return variables;
        }
        static bool IsNodeAfter(TreeNode init, TreeNode current)
        {
            if (init.Level > current.Level)
            {
                return false;
            }
            else if (init.Level == current.Level)
            {
                var currentNode = init;
                while (currentNode.NextNode != null)
                {
                    if (currentNode.NextNode == current)
                    {
                        return true;
                    }
                    currentNode = currentNode.NextNode;
                }
                return false;
            }
            else
            {
                if (init == current.Parent)
                {
                    return true;
                }
                return IsNodeAfter(init, current.Parent);
            }
        }

    }

}
