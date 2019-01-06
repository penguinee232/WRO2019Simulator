using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public interface IVariableGetSet
    {
        int Index { get; }
        IVariableGetSet Parent { get; }
        List<Variable> Children { get; set; }
        Variable Get();
        void Set(Variable variable);
        bool VariableExists();
        Type Type { get; }
        bool IsChild { get; }
    }

    public struct ChildVariableGetSet:IVariableGetSet
    {
        public IVariableGetSet Parent { get; private set; }
        public List<Variable> Children { get; set; }
        public Type Type { get; private set; }
        public int Index { get; private set; }
        public bool IsChild => true;

        public ChildVariableGetSet(Type type, int index, IVariableGetSet parent)
        {
            Parent = parent;
            Type = type;
            Index = index;
            Children = new List<Variable>();
        }
        public Variable Get()
        {
            return Parent.Children[Index];
        }
        public void Set(Variable variable)
        {
            Parent.Children[Index] = variable;
        }
        public override string ToString()
        {
            return Get().ToString();
        }
        public bool VariableExists()
        {
            return Parent.Children.Count > Index && Parent.VariableExists();
        }
    }

    public struct VariableGetSet : IVariableGetSet
    {
        public List<Variable> Children { get; set; }
        public Type Type { get; private set; }
        public int Index { get; private set; }
        public bool IsChild => false;

        public IVariableGetSet Parent { get; }

        public VariableGetSet(Type type, int index)
        {
            Parent = null;
            Index = index;
            Type = type;
            Children = new List<Variable>();
        }
        public Variable Get()
        {
            return VariablesInfo.VariablesByType[Type][Index];
        }
        public void Set(Variable variable)
        {
            VariablesInfo.VariablesByType[Type][Index] = variable;
        }
        public static Variable? GetNullableVariable(IVariableGetSet variableGetSet)
        {
            if (variableGetSet == null)
            {
                return null;
            }
            return variableGetSet.Get();
        }
        public static IVariableGetSet Default()
        {
            return VariablesInfo.VariableGetSet[Variable.Default()];
        }
        public override string ToString()
        {
            return Get().ToString();
        }
        public bool VariableExists()
        {
            return VariablesInfo.VariablesByType.ContainsKey(Type) && VariablesInfo.VariablesByType[Type].Count > Index;
        }
    }
}
