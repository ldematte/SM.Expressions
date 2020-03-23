using System;
using Pidgin;
using Pidgin.Expression;
using static Pidgin.Parser;

namespace SM.Expressions
{
    public static class SMExpressionParser
    {
        private class IdentifierAndApplication
        {
            private readonly string m_id;
            private readonly string m_appName;
            private readonly bool m_isRaw;

            public IdentifierAndApplication(string id, string appName, bool isRaw)
            {
                m_id = id;
                m_appName = appName;
                m_isRaw = isRaw;
            }

            internal string Id => m_id;
            internal string AppName => m_appName;
            internal bool IsRaw => m_isRaw;
        }

        private static Parser<char, T> Tok<T>(Parser<char, T> token) => Try(token).Before(SkipWhitespaces);
        private static Parser<char, string> Tok(string token) => Tok(String(token));

        private static Parser<char, T> Parenthesized<T>(Parser<char, T> parser) => parser.Between(Tok("("), Tok(")"));

        private static Parser<char, Func<IExpression, IExpression, IExpression>> Binary(Parser<char, BinaryOperatorType> op)
            => op.Select<Func<IExpression, IExpression, IExpression>>(type => (l, r) => new BinaryOp(type, l, r));
        private static Parser<char, Func<IExpression, IExpression>> Unary(Parser<char, UnaryOperatorType> op)
            => op.Select<Func<IExpression, IExpression>>(type => o => new UnaryOp(type, o));

        private static readonly Parser<char, Func<IExpression, IExpression, IExpression>> Add = Binary(Tok("+").ThenReturn(BinaryOperatorType.Add));
        private static readonly Parser<char, Func<IExpression, IExpression, IExpression>> Sub = Binary(Tok("-").ThenReturn(BinaryOperatorType.Sub));
        private static readonly Parser<char, Func<IExpression, IExpression, IExpression>> Mul = Binary(Tok("*").ThenReturn(BinaryOperatorType.Mul));
        private static readonly Parser<char, Func<IExpression, IExpression, IExpression>> Div = Binary(Tok("/").ThenReturn(BinaryOperatorType.Div));
        private static readonly Parser<char, Func<IExpression, IExpression, IExpression>> Mod = Binary(Tok("%").ThenReturn(BinaryOperatorType.Mod));
        private static readonly Parser<char, Func<IExpression, IExpression, IExpression>> Pow = Binary(Tok("^").ThenReturn(BinaryOperatorType.Pow));
        private static readonly Parser<char, Func<IExpression, IExpression, IExpression>> And = Binary(Tok("&").ThenReturn(BinaryOperatorType.And));
        private static readonly Parser<char, Func<IExpression, IExpression, IExpression>> Or = Binary(Tok("|").ThenReturn(BinaryOperatorType.Or));
        private static readonly Parser<char, Func<IExpression, IExpression, IExpression>> Lsh = Binary(Tok("<<").ThenReturn(BinaryOperatorType.Lsh));
        private static readonly Parser<char, Func<IExpression, IExpression, IExpression>> Rsh = Binary(Tok(">>").ThenReturn(BinaryOperatorType.Rsh));

        private static readonly Parser<char, Func<IExpression, IExpression>> Plus = Tok("+").Select<Func<IExpression, IExpression>>(_ => o => o);
        private static readonly Parser<char, Func<IExpression, IExpression>> Neg = Unary(Tok("-").ThenReturn(UnaryOperatorType.Neg));
        private static readonly Parser<char, Func<IExpression, IExpression>> Complement = Unary(Tok("~").ThenReturn(UnaryOperatorType.Complement));

        private static readonly Parser<char, Maybe<string>> Application = Tok(Char('_').Then(LetterOrDigit.ManyString())).Optional();

        private static readonly Parser<char, IdentifierAndApplication> PlainIdentifier
            = Tok(Char('$')
                    .Then(Letter
                        .Then(LetterOrDigit.ManyString(), (h, t) => h + t))
                    .Then(Application, (id, app) => new IdentifierAndApplication(id, app.GetValueOrDefault(string.Empty), false)))
                .Labelled("identifier");

        private static readonly Parser<char, IdentifierAndApplication> RawIdentifier
            = Tok(String("$$")
                    .Then(Letter
                        .Then(LetterOrDigit.ManyString(), (h, t) => h + t))
                    .Then(Application, (id, app) => new IdentifierAndApplication(id, app.GetValueOrDefault(string.Empty), true)))
                .Labelled("raw identifier");

        private static readonly Parser<char, IExpression> NoLogIdentifier
            = Tok(CIString("nolog"))
                    .Then(Parenthesized(Try(PlainIdentifier.Or(RawIdentifier))))
                .Select<IExpression>(x => new Identifier(x.Id, x.AppName, x.IsRaw, true))
                .Labelled("nolog identifier");

        private static readonly Parser<char, IExpression> Identifier
            = Try(PlainIdentifier.Or(RawIdentifier))
                .Select<IExpression>(x => new Identifier(x.Id, x.AppName, x.IsRaw, false))
                .Labelled("identifier");

        private static readonly Parser<char, IExpression> Literal
            = Tok(Real)
                .Select<IExpression>(value => new Literal(value))
                .Labelled("constant");

        private static readonly Parser<char, IExpression> Pi
            = Tok(CIString("PI"))
                .Select<IExpression>(value => new Literal(Math.PI))
                .Labelled("pi");


        private static readonly Parser<char, string> FunctionName
            = Tok(Letter.Then(LetterOrDigit.ManyString(), (h, t) => h + t))
                .Labelled("function name");

        //private static Parser<char, IEnumerable<IExpression>> FunctionArguments(Parser<char, IExpression> subExpr) =>
        //    Parenthesized(subExpr.Separated(Tok(",")));

        private static Parser<char, IExpression> FunctionArgument(Parser<char, IExpression> subExpr) => Parenthesized(subExpr);

        private static Parser<char, IExpression> Call(Parser<char, IExpression> subExpr)
            => Map<char, string, IExpression, IExpression>((name, arg) => new Call(name, arg), FunctionName, FunctionArgument(subExpr))
                .Labelled("function call");

        private static readonly Parser<char, IExpression> Expr = ExpressionParser.Build<char, IExpression>(
            expr => (
                OneOf(
                    Identifier,
                    Try(Pi),
                    Try(NoLogIdentifier),
                    Literal,
                    Call(expr),
                    Parenthesized(expr).Labelled("parenthesized expression")
                ),
                new[]
                {
                    Operator.Prefix(Plus).And(Operator.Prefix(Neg).And(Operator.Prefix(Complement))),
                    Operator.InfixL(Pow),
                    Operator.InfixL(Mul).And(Operator.InfixL(Div).And(Operator.InfixL(Mod))),
                    Operator.InfixL(Add).And(Operator.InfixL(Sub)),
                    Operator.InfixL(Lsh).And(Operator.InfixL(Rsh)),
                    Operator.InfixL(And),
                    Operator.InfixL(Or)
                }
            )
        ).Labelled("expression");

        public static IExpression ParseOrThrow(string input)
            => Expr.ParseOrThrow(input);
    }
}