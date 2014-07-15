using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SophiaLex
{
    public class Parser
    {
        public static Expression Parse(List<Token> tokens)
        {
            var pExp = new ParenthesesExpression();
            pExp.Parse(tokens);
            return pExp;
        }
    }
    public class Evaluator
    {
        public Dictionary<string, double> Hashtable = new Dictionary<string, double>();
        public double GetVariable(string var)
        {
            return Hashtable[var];
        }
        public double Evaluate(string exp)
        {
            return Parser.Parse(Lexer.Tokenize(exp)).Evaluate(this);
        }
    }
    public static class ExpressionHelpers
    {
        public static Expression AsExpression(this TokenOrExpression toe)
        {
            if (toe is Expression)
                return toe as Expression;
            else
                return new LiteralExpression(toe as Token);
        }
        public static Expression Negate(this Expression e)
        {
            e.Negate = !e.Negate;
            return e;
        }
    }
    public abstract class Expression : TokenOrExpression
    {
        public abstract double Evaluate(Evaluator z);
        public double Evaluate()
        {
            return Evaluate(new Evaluator());
        }
        public bool Negate = false;
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
    public class LiteralExpression : Expression 
    {
        public Token Literal;
        public override double Evaluate(Evaluator z)
        {
            if (Literal is NumberToken)
            {
                double val = (Literal as NumberToken).GetNumber();
                if (Negate)
                    return -val;
                else
                    return val;
            }
            else if (Literal is VariableToken)
            {
                double val = z.GetVariable((Literal as VariableToken).Value);
                if (Negate)
                    return -val;
                else
                    return val;
            }
            else
                throw new Exception("Unknown literal type!");
        }
        public LiteralExpression(Token token)
        {
            Literal = token;
        }
        public override string ToString()
        {
            if (Negate)
                return "-" + Literal.ToString();
            else
                return Literal.ToString();
        }
    }
    public class BinaryExpression : Expression
    {
        public Expression Left;
        public BinaryOperatorToken Operator;
        public Expression Right;

        public override double Evaluate(Evaluator z)
        {
            double left = Left.Evaluate(z), right = Right.Evaluate(z);

            if (Operator is MultiplicationToken)
                return left * right;
            else if (Operator is DivisionToken)
                return left / right;
            else if (Operator is AdditionToken)
                return left + right;
            else if (Operator is SubstractionToken)
                return left - right;
            else if (Operator is ExponentiationToken)
                return Math.Pow(left, right);
            else
                throw new Exception("Unknown BinaryOperatorToken");
        }
        public override string ToString()
        {
            return string.Format("{1} {0}", base.ToString(), Operator.Value);
        }
    }
    public class ParenthesesExpression : Expression
    {
        public List<TokenOrExpression> Items = new List<TokenOrExpression>();

        public void Parse(List<Token> tokens)
        {
            int idx = 0;
            for(;idx < tokens.Count; idx++)
            {
                var token = tokens[idx];
                if (token is LeftParenthesisToken)
                {
                    var innerExp = tokens.Skip(idx + 1).ToList();
                    int lastIdx = 0;
                    int level = 1;
                    for (int t = 0; t < innerExp.Count; t++)
                    {
                        if (innerExp[t] is LeftParenthesisToken)
                            level++;
                        if (innerExp[t] is RightParenthesisToken)
                        {
                            level--;
                            if (level == 0)
                            {
                                lastIdx = t;
                                break;
                            }
                        }
                    }
                    var tlist = innerExp.Take(lastIdx).ToList();
                    var e = new ParenthesesExpression();
                    e.Parse(tlist);
                    Items.Add(e);
                    idx += tlist.Count + 1;

                }
                else
                    Items.Add(token);
                
            }

            int opIndex = 0;

            int minSubIndex = 0;
            
            while ((opIndex = Items.FindIndex(minSubIndex, (Predicate<TokenOrExpression>)(x => x is SubstractionToken))) >= 0)
            {
                int start = opIndex - 1;
                TokenOrExpression left = null;
                if (start >= 0)
                {
                    left = Items[start];
                }
                var op = Items[start + 1];
                var right = Items[start + 2];

                if(left == null)
                {
                    Items.RemoveRange(start + 1, 2);
                    Items.Insert(start + 1, right.AsExpression().Negate());
                }
                else if(!(left is ValueToken || left is Expression))
                {
                    Items.RemoveRange(start + 1, 2);
                    Items.Insert(start + 1, right.AsExpression().Negate());
                }

                minSubIndex = opIndex + 1;


                //Items.RemoveRange(start, 3);
                //Items.Insert(start, new BinaryExpression()
                //{
                //    Left = left is Token ? new LiteralExpression((Token)left) : (Expression)left,
                //    Operator = (BinaryOperatorToken)op,
                //    Right = right is Token ? new LiteralExpression((Token)right) : (Expression)right,
                //});
            }
            while ((opIndex = Items.FindLastIndex((Predicate<TokenOrExpression>)(x => x is ExponentiationToken))) > 0)
            {
                int start = opIndex - 1;
                var left = Items[start];
                var op = Items[start + 1];
                var right = Items[start + 2];


                Items.RemoveRange(start, 3);
                Items.Insert(start, new BinaryExpression()
                {
                    Left = left.AsExpression(),
                    Operator = (BinaryOperatorToken)op,
                    Right = right.AsExpression(),
                });
            }
            while ((opIndex = Items.FindIndex((Predicate<TokenOrExpression>)(x => x is MultiplicationToken || x is DivisionToken))) > 0)
            {
                int start = opIndex - 1;
                var left = Items[start];
                var op = Items[start + 1];
                var right = Items[start + 2];


                Items.RemoveRange(start, 3);
                Items.Insert(start, new BinaryExpression()
                {
                    Left = left.AsExpression(),
                    Operator = (BinaryOperatorToken)op,
                    Right = right.AsExpression(),
                });
            }
            while ((opIndex = Items.FindIndex((Predicate<TokenOrExpression>)(x => x is AdditionToken || x is SubstractionToken))) > 0)
            {
                int start = opIndex - 1;
                var left = Items[start];
                var op = Items[start + 1];
                var right = Items[start + 2];


                Items.RemoveRange(start, 3);
                Items.Insert(start, new BinaryExpression()
                {
                    Left = left.AsExpression(),
                    Operator = (BinaryOperatorToken)op,
                    Right = right.AsExpression(),
                });
            }
            while ((opIndex = Items.FindIndex((Predicate<TokenOrExpression>)(x => x is Token))) >= 0)
            {
                var token = Items[opIndex];


                Items.RemoveAt(opIndex);
                Items.Insert(opIndex, token.AsExpression());
            }
        }
        public override double Evaluate(Evaluator z)
        {
            if(Items.Count == 1)
            {
                var item = Items[0];
                if (item is Expression)
                {
                    double val = (item as Expression).Evaluate(z);
                    if (Negate)
                        return -val;
                    else
                        return val;
                }
                else
                    throw new Exception("Can not evaluate ParenthesesExpression - uknown item type");
            }
            else
                throw new Exception("Can not evaluate ParenthesesExpression - more than one item");
        }

        //public bool ContainsExpressions
        //{
        //    get
        //    {
        //        return Expressions.Any(t => t is ParenthesesExpression);
        //    }
        //}
        //public override double Evaluate(Evaluator z)
        //{
        //    return Expression.Evaluate(z);
        //}
    }

}
