using IronRust.Asts;
using ParserCombinator;

namespace IronRust
{
    using static StringParsers;

    public class ParserCombinated
    {
        //private static readonly Parser<ExpressionAst> VarDeclareBind =
        //    Literal( "let" ).Right( Identifier() ).Left( Character( '=' ) )
        //        .Sequence( Expressions, ( s, ast ) => ast );

        //private static readonly Parser<ExpressionAst> Expressions
        //    = VarDeclareBind;

        //public static Parser<FunctionAst> FunctionParser =
        //    Literal("fn").Right(Identifier())
        //        .Left(Character('('))
        //        .Left(Character(')'))
        //        .Left(Character('{')).Map(result => );
    }
}