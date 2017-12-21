using System;
using System.Collections.Generic;
using System.Linq;

namespace IronRust.Asts
{
    using static System.Environment;

    public sealed class IlGenerator : ICodeGenerator
    {
        public string Result { get; private set; } = ".assembly hello{}"; // todo

        private IReadOnlyList<(int index, VariableAst variable)> _currentLocalVariables;

        public void GenerateFunction(FunctionAst functionAst)
        {
            Result += $".method " +
                      $"{functionAst.Accessibility} " +
                      $"{(functionAst.IsStatic ? "static" : string.Empty)} " +
                      $"{functionAst.ReturnType} " +
                      $"{functionAst.Name} " +
                      $"(" +
                      $"){NewLine}" +
                      $"{{{NewLine}" +
                      $"{(functionAst.IsEntryPoint ? ".entrypoint" : string.Empty)}{NewLine}";
            var localVariables = functionAst.Block.OfType<VariableAst>().ToArray();

            if (localVariables.Any())
            {
                var localVariableDescriptions = new List<(int, VariableAst)>();

                Result += $".locals init({NewLine}";
                for (int i = 0; i < localVariables.Length; i++)
                {
                    var variable = localVariables[i];
                    Result += $"[{i}] {variable.Type} {variable.Name}{NewLine}";
                    localVariableDescriptions.Add((i, variable));
                }
                Result += $"){NewLine}";

                _currentLocalVariables = localVariableDescriptions;
            }

            foreach (var line in functionAst.Block)
            {
                line.Accept(this);
            }
            Result += $"ret{NewLine}}}{NewLine}";
        }

        public void GenerateBind(BindingAst bindingAst)
        {
            if (_currentLocalVariables == null
                || !_currentLocalVariables.Select(item => item.variable.Name).Contains(bindingAst.Name))
            {
                throw new Exception($"variable {bindingAst.Name} is not declared");
            }
            var targetVariable = _currentLocalVariables.First(item => item.variable.Name == bindingAst.Name);

            bindingAst.Expression.Accept(this);
            Result += $"stloc.{targetVariable.index} // {targetVariable.variable.Name}{NewLine}";
        }

        public void GenerateLoadInt(IntegerAst integerAst)
        {
            var code = string.Empty;
            switch (int.Parse(integerAst.Value))
            {
                case int x when x == -1:
                code = $"ldc.i4.m1";
                break;
                case int x when 0 <= x && x <= 8:
                code = $"ldc.i4.{x}";
                break;
                case int x when (8 < x && x <= 0x7F) || (-0x80 <= x && x < -1):
                code = $"ldc.i4.s {x}";
                break;
                case int x:
                code = $"ldc.i4 {x}";
                break;
            }
            Result += $"{code}{NewLine}";
        }
    }
}