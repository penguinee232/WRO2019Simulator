using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public struct VariableGetSet
    {
        public Type Type { get; private set; }
        Func<int, Variable> get;
        Action<Variable, int> set;
        public int Index { get; private set; }
        public VariableGetSet(Func<int, Variable> get, Action<Variable, int> set, Type type, int index)
        {
            this.Index = index;
            this.get = get;
            this.set = set;
            Type = type;
        }
        public Variable Get()
        {
            return get.Invoke(Index);
        }
        public void Set(Variable variable)
        {
            set.Invoke(variable, Index);
        }
        public static Variable? GetNullableVariable(VariableGetSet? variableGetSet)
        {
            if (variableGetSet == null)
            {
                return null;
            }
            return variableGetSet.Value.Get();
        }
        public static VariableGetSet Default()
        {
            return VariablesInfo.VariableGetSet[Variable.Default()];
        }
        public override string ToString()
        {
            return Get().ToString();
        }
    }
    public static class VariablesInfo
    {
        static Dictionary<Variable, TreeNode> VariableInfos;
        public static Dictionary<Type, List<Variable>> VariablesByType;
        static Dictionary<Variable, IGetSetFunc> VariableValueGetSet;
        static Dictionary<Variable, object> VariableValues;
        static Dictionary<Variable, object> VariableInitalValues;
        public static Dictionary<Variable, VariableGetSet> VariableGetSet;
        public static void InitializeVariables()
        {
            VariablesByType = new Dictionary<Type, List<Variable>>();

            Variable floatVariable1 = new Variable(typeof(float), "floatVariable1");
            Variable MyVector2Variable1 = new Variable(typeof(MyVector2), "MyVector2Variable1");
            Variable intVariable = new Variable(typeof(int), "intVariable1");

            VariablesByType.Add(typeof(float), new List<Variable>() { floatVariable1 });
            VariablesByType.Add(typeof(int), new List<Variable>() { intVariable });
            VariablesByType.Add(typeof(MyVector2), new List<Variable>() { MyVector2Variable1 });

            VariableValueGetSet = new Dictionary<Variable, IGetSetFunc>();
            VariableValues = new Dictionary<Variable, object>();
            VariableInitalValues = new Dictionary<Variable, object>();
            VariableInfos = new Dictionary<Variable, TreeNode>();
            VariableGetSet = new Dictionary<Variable, VariableGetSet>();
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
                    VariableValues.Add(v, value);
                    VariableValueGetSet.Add(v, new GetSetFunc<object>((j) => VariableValues[v], (val, j) => VariableValues[v] = val, v.Name));
                    VariableInitalValues.Add(v, value);
                    VariableGetSet.Add(v, new VariableGetSet((j) => vt.Value[j], (val, j) => vt.Value[j] = val, v.Type, i));
                }
            }
        }

        public static VariableGetSet? GetVariableGetSet(Variable? variable)
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
                VariableValueGetSet[v.Key].ObjSet(v.Value, 0);
            }
        }
        public static VariableGetSet AddVariable(Variable variable, object value, TreeNode currentTreeNode, int variableByTypeIndex = -1)
        {
            if (variableByTypeIndex < 0)
            {
                if (!VariablesByType.ContainsKey(variable.Type))
                {
                    VariablesByType.Add(variable.Type, new List<Variable>());
                }
                VariablesByType[variable.Type].Add(variable);
                variableByTypeIndex = VariablesByType[variable.Type].Count - 1;
            }
            var variableGetSet = new VariableGetSet((i) => VariablesByType[variable.Type][i], (val, i) => VariablesByType[variable.Type][i] = val, variable.Type, variableByTypeIndex);
            VariableGetSet.Add(variable, variableGetSet);
            VariableValues.Add(variable, value);
            VariableValueGetSet.Add(variable, new GetSetFunc<object>((i) => VariableValues[variable], (val, i) => VariableValues[variable] = val, variable.Name));

            if (currentTreeNode != null)
            {
                VariableInfos.Add(variable, currentTreeNode);
            }
            return variableGetSet;
        }
        public static void RemoveVariable(Variable variable, bool ignoreVariablesByType)
        {
            if (!ignoreVariablesByType)
            {
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
            return VariableValueGetSet[variable].ObjGet(0);
        }
        public static void SetVariable(Variable variable, object value)
        {
            VariableValueGetSet[variable].ObjSet(value, 0);
        }

        public static Dictionary<Type, List<Variable>> GetVariables(TreeNode currentTreeNode)
        {
            Dictionary<Type, List<Variable>> variables = new Dictionary<Type, List<Variable>>();
            foreach (var vt in VariablesByType)
            {
                variables.Add(vt.Key, new List<Variable>());
                foreach (var v in vt.Value)
                {
                    bool addVariable = true;
                    if (VariableInfos.ContainsKey(v))
                    {
                        addVariable = IsNodeAfter(VariableInfos[v], currentTreeNode);
                    }
                    if (addVariable)
                    {
                        variables[vt.Key].Add(v);
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
