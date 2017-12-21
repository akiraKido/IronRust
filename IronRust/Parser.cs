using System;
using System.Collections.Generic;
using System.Linq;
using IronRust.Asts;

namespace IronRust
{
    internal sealed class Parser
    {
        private readonly Lexer _lexer;

        internal Parser(Lexer lexer) => _lexer = lexer;

        internal ICodeAcceptable Parse()
        {
            ICodeAcceptable ast = null;
            while (_lexer.MoveNext())
            {
                CreateFunction(out ast);
            }
            return ast;
        }

        private bool CreateFunction(out ICodeAcceptable functionAst)
        {
            functionAst = null;
            if (_lexer.Current.token != Token.Reserved_fn) return false;
            _lexer.MoveNext(); // eat fn

            if (_lexer.Current.token != Token.Identifier) throw new Exception("expected identifier");
            var name = _lexer.Current.value;
            _lexer.MoveNext(); // eat name

            var accessibility = Accecibility.Public; // TODO
            var isStatic = true; // TODO
            var returnType = "void"; // TODO
            var isEntryPoint = name == "main";

            // TODO 引数
            if (_lexer.Current.token != Token.L_Paren) throw new Exception("expected (");
            _lexer.MoveNext(); // eat (
            if (_lexer.Current.token != Token.R_Paren) throw new Exception("expected )");
            _lexer.MoveNext(); // eat )

            CreateBlock(out var block);

            functionAst = new FunctionAst(name, accessibility, isStatic, returnType, isEntryPoint, block);
            return true;
        }

        private bool CreateBlock(out IEnumerable<ICodeAcceptable> block)
        {
            block = null;
            if (_lexer.Current.token != Token.L_Bracket) return false;
            _lexer.MoveNext(); // eat {

            var expressions = new List<ICodeAcceptable>();
            while (_lexer.Current.token != Token.R_Bracket)
            {
                CreateExpression(ref expressions, out var expression);
                if(expression != null) expressions.Add(expression);
                if (_lexer.Current.token != Token.Semicolon) throw new Exception("expected semicolon");
                _lexer.MoveNext();
            }
            _lexer.MoveNext(); // eat }
            block = expressions;
            return true;
        }

        private bool CreateExpression(ref List<ICodeAcceptable> block, out ExpressionAst expression)
            => CreateVarDeclareBind(ref block, out expression)
            || CreateNumber(out expression)
            || CreateSimpleBind( ref block, out expression );

        private bool CreateVarDeclareBind(ref List<ICodeAcceptable> block, out ExpressionAst expression)
        {
            expression = null;
            if (_lexer.Current.token != Token.Reserved_let) return false;
            _lexer.MoveNext(); // eat let

            bool isMutable = false;
            if ( _lexer.Current.token == Token.Reserved_mut )
            {
                isMutable = true;
                _lexer.MoveNext();
            }

            if (_lexer.Current.token != Token.Identifier) throw new Exception("expected identifier");
            var name = _lexer.Current.value;
            // TODO shadow
            _lexer.MoveNext(); // eat name

            if (_lexer.Current.token != Token.Equal) throw new Exception("expected =");
            _lexer.MoveNext(); // eat =

            if (!CreateExpression(ref block, out var value)) throw new Exception("expected expression");

            var declareExpression = new VariableAst( name, value.Type, isMutable );
            var bindExpression = new BindingAst(name, value);
            block.Add( declareExpression );
            block.Add( bindExpression );
            return true;
        }

        private bool CreateSimpleBind(ref List<ICodeAcceptable> block, out ExpressionAst expression)
        {
            expression = null;
            if ( _lexer.Current.token != Token.Identifier ) return false;
            var name = _lexer.Current.value;
            _lexer.MoveNext();

            if(_lexer.Current.token != Token.Equal) throw new Exception("expected =");
            _lexer.MoveNext();

            if ( !CreateExpression( ref block, out var _expression ) ) throw new Exception("expected expression");
            var declaration = block.OfType<VariableAst>().FirstOrDefault( variable => variable.Name == name );
            if ( declaration != null && !declaration.IsMutable )
            {
                throw new Exception($"re-assignment of immutable variable `{name}`");
            }

            expression = new BindingAst( name, _expression );
            return true;
        }

        private bool CreateNumber(out ExpressionAst expression)
        {
            // todo float

            expression = null;
            if (_lexer.Current.token != Token.Integer) return false;
            expression = new IntegerAst(_lexer.Current.value);

            _lexer.MoveNext(); // eat integer
            return true;
        }
    }
}