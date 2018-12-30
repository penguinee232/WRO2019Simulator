using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public static class ToRobotLanguageConverter
    {
        static Dictionary<RobotLanguages, Func<List<CommandsNode>, string>> languageConverters;
        static Dictionary<RobotLanguages, List<ExceptionInfo>> converterExeptions;
        static HashSet<Type> CPlusPlusNoCommandFuncTypes;
        static void Init()
        {
            if (languageConverters == null)
            {
                languageConverters = new Dictionary<RobotLanguages, Func<List<CommandsNode>, string>>();
                languageConverters.Add(RobotLanguages.CPlusPlus, CPLusPlusConverter);
            }
            if (converterExeptions == null)
            {
                converterExeptions = new Dictionary<RobotLanguages, List<ExceptionInfo>>();
                var cPlusPlusExceptions = new List<ExceptionInfo>();
                cPlusPlusExceptions.Add(new ExceptionInfo((t) => t.IsEnum, (n) => n.Type.GetTypeName() + "::" + n.Value.ToString()));
                cPlusPlusExceptions.Add(new ExceptionInfo((t) => t == typeof(bool), (n) => ((bool)n.Value) ? "true" : "false"));
                cPlusPlusExceptions.Add(new ExceptionInfo((t) => t.IsTheSameGenericType(typeof(VisulizeableList<>)), ListConverter));
                cPlusPlusExceptions.Add(new ExceptionInfo((t) => t == typeof(IfStatement), IfConverter));
                cPlusPlusExceptions.Add(new ExceptionInfo((t) => t == typeof(VariableVisulizeItem),
                    (n) => n.Paramaters[0].Variable.Value.Name));
                converterExeptions.Add(RobotLanguages.CPlusPlus, cPlusPlusExceptions);
                CPlusPlusNoCommandFuncTypes = new HashSet<Type>() { typeof(IfStatement) };
            }
        }
        public static string GetRobotCode(string serializedCommands, RobotLanguages robotLanguage)
        {
            Init();
            List<CommandsNode> commandTrees = GetCommandTrees(new Span<char>(serializedCommands.ToArray()));
            return languageConverters[robotLanguage].Invoke(commandTrees);
        }
        static List<CommandsNode> GetCommandTrees(Span<char> span)
        {
            var allCommands = GetCommandTree(span);
            return allCommands.Paramaters;
        }
        static CommandsNode GetCommandTree(Span<char> span)
        {
            Variable? variable;
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
                return new CommandsNode(currentType, value, variable, null);
            }
            else
            {
                variable = null;
                if (span.Length > 0 && span[0] == '{' && currentType.IsSubclassOf(typeof(VisulizableItem)))
                {
                    //VisulizableItem visulizableItem = (VisulizableItem)value;
                    var paramaters = GetItemsParamaters(span);
                    return new CommandsNode(currentType, null, variable, paramaters);
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
                    value = converter.ConvertFrom(Substing(span));
                    return new CommandsNode(currentType, value, variable, null);
                    //}
                }
            }
        }

        static List<CommandsNode> GetItemsParamaters(Span<char> span)
        {
            List<CommandsNode> paramaters = new List<CommandsNode>();
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
                            var newSpan = span.Slice(0, i);
                            if (newSpan.Length > 0)
                            {
                                var commandTree = GetCommandTree(newSpan);
                                paramaters.Add(commandTree);
                            }
                            break;
                        }
                    }
                    else if (inBraceAmount == 0 && span[i] == ',')
                    {
                        var newSpan = span.Slice(0, i);
                        if (newSpan.Length > 0)
                        {
                            var commandTree = GetCommandTree(newSpan);
                            paramaters.Add(commandTree);
                        }
                        span = span.Slice(i + 1);
                        i = 0;
                    }
                }
            }
            return paramaters;
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
        static string tab = "   ";
        static string CPLusPlusConverter(List<CommandsNode> commandTrees)
        {
            return CPLusPlusConverter(commandTrees, 0);
        }
        static string CPLusPlusConverter(List<CommandsNode> commandTrees, int tabAmount)
        {
            string funcName = "UpdateAndAddComponent";
            string code = "";
            string tabStrAmount = RepeatString(tab, tabAmount);
            for (int i = 0; i < commandTrees.Count; i++)
            {
                if (i != 0)
                {
                    code += "\n";
                }
                code += tabStrAmount;
                bool useFunc = !CPlusPlusNoCommandFuncTypes.Contains(commandTrees[i].Type);
                if (useFunc)
                {
                    code += funcName + "(new ";
                }
                string commandCode = GetCPlusPlusCommandNodeCode(commandTrees[i]);
                for(int j = 0;j < commandCode.Length; j++)
                {
                    code += commandCode[j];
                    if(commandCode[j] == '\n')
                    {
                        code += tabStrAmount;
                    }
                }
                if (useFunc)
                {
                    code += ");";
                }
            }
            return code;
        }

        public static string GetCPlusPlusCommandNodeCode(CommandsNode node)
        {
            string code;
            if (node.Variable != null)
            {
                return node.Variable.Value.Name;
            }
            else if (HasException(node, RobotLanguages.CPlusPlus, out code))
            {
                return code;
            }
            else if (node.Paramaters == null && node.Value != null)
            {
                return node.Value.ToString();
            }
            else
            {
                code = GetCPlusPlusTypeName(node.Type) + "(";
                for (int i = 0; i < node.Paramaters.Count; i++)
                {
                    if (i != 0)
                    {
                        code += ", ";
                    }
                    code += GetCPlusPlusCommandNodeCode(node.Paramaters[i]);
                }
                code += ")";
                return code;
            }
        }

        static string ListConverter(CommandsNode node)
        {
            string code = GetCPlusPlusTypeName(node.Type) + "{";
            for (int i = 0; i < node.Paramaters.Count; i++)
            {
                if (i != 0)
                {
                    code += ", ";
                }
                code += GetCPlusPlusCommandNodeCode(node.Paramaters[i]);
            }
            code += "}";
            return code;
        }
        static string IfConverter(CommandsNode node)
        {
            string code = "if(";
            //BoolPhrase boolPhrase = (BoolPhrase)node.Paramaters[0].Value;
            code += BoolPhrase.GetBoolPhrase(node.Paramaters[0]);
            code += ")" + "\n" + "{" + "\n";
            List<CommandsNode> thenCommands = node.Paramaters[1].Paramaters;
            code += CPLusPlusConverter(thenCommands, 1);
            code += "\n" + "}";
            List<CommandsNode> elseCommands = node.Paramaters[2].Paramaters;
            if (elseCommands != null && elseCommands.Count > 0)
            {
                code += "\n" + "else" + "\n" + "{" + "\n";
                code += CPLusPlusConverter(elseCommands, 1);
                code += "\n" + "}";
            }
            return code;
        }

        static string GetCPlusPlusTypeName(Type type)
        {
            if (Extensions.typeNames == null) { Extensions.InitTypeNames(); }
            if (Extensions.typeNames.ContainsKey(type)) { return Extensions.typeNames[type]; }
            string name = type.Name;

            if (!type.IsConstructedGenericType) { return name; }
            var genericTypes = type.GetGenericArguments();
            name = name.Substring(0, name.Length - (1 + genericTypes.Length.ToString().Length));
            if (type.IsTheSameGenericType(typeof(VisulizeableList<>)))
            {
                name = "vector";
            }
            name += "<";
            for (int i = 0; i < genericTypes.Length; i++)
            {
                if (i != 0)
                {
                    name += ", ";
                }
                name += GetCPlusPlusTypeName(genericTypes[i]);
            }
            name += ">";
            return name;
        }


        static bool HasException(CommandsNode node, RobotLanguages robotLanguage, out string code)
        {
            var exceptions = converterExeptions[robotLanguage];
            foreach (var exception in exceptions)
            {
                if (exception.HasException(node.Type))
                {
                    code = exception.Converter(node);
                    return true;
                }
            }
            code = "";
            return false;
        }

        static string RepeatString(string str, int amount)
        {
            string s = "";
            for (int i = 0; i < amount; i++)
            {
                s += str;
            }
            return s;
        }
    }

    struct ExceptionInfo
    {
        public Func<Type, bool> HasException { get; set; }
        public Func<CommandsNode, string> Converter { get; set; }
        public ExceptionInfo(Func<Type, bool> hasException, Func<CommandsNode, string> converter)
        {
            HasException = hasException;
            Converter = converter;
        }
    }

    public class CommandsNode
    {
        public Type Type { get; set; }
        public object Value { get; set; }
        public Variable? Variable { get; set; }
        public List<CommandsNode> Paramaters { get; set; }
        public CommandsNode(Type type, object value, Variable? variable, List<CommandsNode> paramaters)
        {
            Type = type;
            Value = value;
            Variable = variable;
            Paramaters = paramaters;
        }
    }
    public enum RobotLanguages
    {
        CPlusPlus
    }
}
