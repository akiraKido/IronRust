namespace IronRust.Asts
{
    public interface ICodeGenerator {
        void GenerateFunction(FunctionAst functionAst);
        void GenerateBind( BindingAst bindingAst );
        void GenerateLoadInt( IntegerAst integerAst );
    }
}