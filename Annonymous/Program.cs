using System;
// ReSharper disable MemberCanBePrivate.Global

namespace Annonymous
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var lexer = new Lexer( "1200 + 200 - 400 + 20" );
            var parser = new Parser( lexer );
            var tree = parser.GenerateTree();
        }
    }

    internal class Parser
    {
        private readonly ILexer _lexer;

        internal Parser(ILexer lexer) => _lexer = lexer;

        internal AbstractSyntaxTree GenerateTree()
        {
            AbstractSyntaxTree tree = Sum();
            //while (_lexer.Match(TokenType.EndOfFile) == false)
            //{
            //}
            return tree;
        }

        /// <summary>
        /// sum = number [(+|-) number]*
        /// </summary>
        /// <returns></returns>
        private AbstractSyntaxTree Sum()
        {
            AbstractSyntaxTree lhs = _lexer.Check( TokenType.AddOrSubtract ) ? new NumberAst( "0" ) : Number();
            while ( _lexer.Check( TokenType.AddOrSubtract ) )
            {
                var op = _lexer.Match( TokenType.AddOrSubtract );
                var rhs = Number();
                lhs = new BinopAst( lhs, op.Value, rhs );
            }

            return lhs;
        }

        /// <summary>
        /// number = [0-9]+(\.[0-9]+)?
        /// </summary>
        /// <returns></returns>
        private AbstractSyntaxTree Number()
        {
            var number = _lexer.Match( TokenType.Number );
            return new NumberAst(number.Value);
        }
    }
    
    internal abstract class AbstractSyntaxTree { }

    internal class BinopAst : AbstractSyntaxTree
    {
        internal AbstractSyntaxTree LeftHandSide { get; }
        internal string Operator { get; }
        internal AbstractSyntaxTree RightHandSide { get; }

        internal BinopAst( AbstractSyntaxTree lhs, string op, AbstractSyntaxTree rhs )
        {
            LeftHandSide = lhs;
            Operator = op;
            RightHandSide = rhs;
        }
    }

    internal class NumberAst : AbstractSyntaxTree
    {
        internal string Value { get; }
        internal NumberAst( string value ) => Value = value;
    }
}
