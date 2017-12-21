using System;
using System.Collections.Generic;
using System.Text;

namespace IronRust.Asts
{
    public class VariableAst : ICodeAcceptable
    {
        public string Name { get; }
        public string Type { get; }
        public bool IsMutable { get; }

        public VariableAst(string name, string type, bool isMutable)
        {
            Name = name;
            Type = type;
            IsMutable = isMutable;
        }

        public void Accept(ICodeGenerator codeGenerator) { }
    }

    public class BindingAst : ExpressionAst
    {
        public string Name { get; }
        public ExpressionAst Expression { get; }
        public override string Value { get; } = null;
        public override string Type => Expression.Type;

        public BindingAst(string name, ExpressionAst expression)
        {
            Name = name;
            Expression = expression;
        }


        public override void Accept(ICodeGenerator codeGenerator)
        {
            codeGenerator.GenerateBind(this);
        }
    }

    public abstract class ExpressionAst : ICodeAcceptable
    {
        public abstract string Value { get; }
        public abstract string Type { get; }

        public abstract void Accept( ICodeGenerator codeGenerator );
    }

    public class IntegerAst : ExpressionAst
    {
        public override string Value { get; }
        public override string Type { get; } = "int32";

        public IntegerAst(string value)
        {
            if(!int.TryParse( value, out _ )) throw new ArgumentException(nameof(value));
            Value = value;
        }

        public override void Accept(ICodeGenerator codeGenerator)
        {
            codeGenerator.GenerateLoadInt(this);
        }
    }
}
