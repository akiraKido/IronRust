using System.Collections;
using System.Collections.Generic;

namespace IronRust.Asts {
    public interface ICodeAcceptable {
        void Accept(ICodeGenerator codeGenerator);
    }

    public class FunctionAst : ICodeAcceptable {
        public Accecibility Accessibility { get; }
        public string Name { get; }
        public bool IsStatic { get; }
        public string ReturnType { get; }
        public bool IsEntryPoint { get; }
        public IEnumerable<ICodeAcceptable> Block { get; }

        internal FunctionAst(
            string name
            , Accecibility accessibility
            , bool isStatic
            , string returnType
            , bool isEntryPoint
            , IEnumerable<ICodeAcceptable> block) {
            Name = name;
            Accessibility = accessibility;
            IsStatic = isStatic;
            ReturnType = returnType;
            IsEntryPoint = isEntryPoint;
            Block = block;
        }

        public void Accept(ICodeGenerator codeGenerator) {
            codeGenerator.GenerateFunction(this);
        }
    }

    public class Accecibility {
        public static Accecibility Public = new Accecibility("public");
        public static Accecibility Private = new Accecibility("private");

        public string StringValue { get; }

        public Accecibility(string stringValue) {
            StringValue = stringValue;
        }

        public override string ToString() {
            return StringValue;
        }
    }


}