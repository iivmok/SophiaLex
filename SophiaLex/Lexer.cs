using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SophiaLex
{
    public class Lexer
    {
        public static char[] MathSymbols = "+-*/()^".ToCharArray();
        public static string StringifyTokens(List<Token> tokens)
        {
            StringBuilder sb = new StringBuilder();
            int level = 0;
            bool indented = true;
            bool lastWasParen = false;

            foreach (var token in tokens)
            {
                if(token is LeftParenthesisToken)
                {
                    if (!lastWasParen)
                        sb.AppendLine();
                    sb.Append(string.Empty.PadRight(level * 4, ' '));
                    sb.AppendFormat("{0}" + Environment.NewLine, TokenToString(token));
                    level++;
                    indented = false;
                    lastWasParen = true;
                }
                else if(token is RightParenthesisToken)
                {
                    level--;
                    if(!lastWasParen)
                        sb.AppendLine();
                    sb.Append(string.Empty.PadRight(level * 4, ' '));
                    sb.AppendFormat("{0}" + Environment.NewLine, TokenToString(token));
                    indented = false;
                    lastWasParen = true;
                }
                else
                {
                    if (!indented)
                    {
                        sb.Append(string.Empty.PadRight(level * 4, ' '));
                        indented = true;
                    }
                    sb.AppendFormat("<{0}>", TokenToString(token));
                    lastWasParen = false;

                }
            }
            return sb.ToString().Trim('\n', ' ', '\r');
        }
        private static string TokenToString(Token token)
        {
            return token.ToString().Replace("Token", "");
        }
        public static List<Token> Tokenize(string exp)
        {
            var tokens = new List<Token>();
            int idx = 0;
            int maxidx = exp.Length;

            int level = 0;

            StringBuilder acc = new StringBuilder();

            while (idx < maxidx)
            {
                char current = exp[idx];

                // ==== Number
                if(char.IsDigit(current))
                {
                    tokens.Add(IntegerToken.ReadToken(exp, ref idx));
                    continue;
                }

                // ==== Whitespace
                if(char.IsWhiteSpace(current))
                {
                    idx++;
                    continue;
                }

                // ==== Symbols
                if(MathSymbols.Contains(current))
                {
                    
                    switch(current)
                    {
                        case '+':
                            tokens.Add(new AdditionToken());
                            break;
                        case '-':
                            tokens.Add(new SubstractionToken());
                            break;
                        case '*':
                            tokens.Add(new MultiplicationToken());
                            break;
                        case '/':
                            tokens.Add(new DivisionToken());
                            break;
                        case '^':
                            tokens.Add(new ExponentiationToken());
                            break;
                        case '(':
                            tokens.Add(new LeftParenthesisToken(level));
                            level++;
                            break;
                        case ')':
                            level--;
                            tokens.Add(new RightParenthesisToken(level));
                            break;
                    }
                    idx++;
                    continue;
                }
                throw new Exception(string.Format("Invalid character '{0}' at {1}", current, idx));

            }
            while(level > 0)
            {
                level--;
                tokens.Add(new RightParenthesisToken(level));
            }
            if(level < 0)
            {
                throw new Exception("Too many right parentheses!");
            }
            return tokens;
        }
    }
    public class Token : TokenOrExpression
    {
        public static bool UseValueStrings = false;
        public string Value = "";
        public override string ToString()
        {
            if (!UseValueStrings)
                return this.GetType().Name;
            else
                return this.Value;
        }
    }
    public class OperatorToken : Token { }
    public class BinaryOperatorToken : OperatorToken { }

    public class AdditionToken : BinaryOperatorToken { public AdditionToken() { Value = "+"; } }
    public class SubstractionToken : BinaryOperatorToken { public SubstractionToken() { Value = "-"; } }
    public class DivisionToken : BinaryOperatorToken { public DivisionToken() { Value = "/"; } }
    public class MultiplicationToken : BinaryOperatorToken { public MultiplicationToken() { Value = "*"; } }
    public class ExponentiationToken : BinaryOperatorToken { public ExponentiationToken() { Value = "^"; } }

    public class ParenthesisToken : Token
    {
        public int Level;
    }
    public class LeftParenthesisToken : ParenthesisToken 
    {
        public LeftParenthesisToken(int level) { Value = "("; Level = level; }
        public override string ToString()
        {
            return "(";
        }
    }
    public class RightParenthesisToken : ParenthesisToken 
    {
        public RightParenthesisToken(int level) { Value = ")"; Level = level; }
        public override string ToString()
        {
            return ")";
        }
    }


    public class ValueToken : Token { }

    public abstract class NumberToken : ValueToken 
    {
        public abstract double GetNumber();
    }
    public class VariableToken : ValueToken
    {

    }
    public class IntegerToken : NumberToken
    {

        private int _integer;

        public int Integer
        {
            get { return _integer; }
            set 
            { 
                _integer = value; 
                Value = value.ToString(); 
            }
        }
        public static IntegerToken ReadToken(string exp, ref int idx)
        {
            var token = new IntegerToken();
            var acc = new StringBuilder();
            char c;
            while(idx < exp.Length && char.IsDigit(c = exp[idx]))
            {
                acc.Append(c);
                idx++;
            }
            token.Integer = int.Parse(acc.ToString());
            return token;
        }
        public override string ToString()
        {
            if (!UseValueStrings)
                return string.Format(" {0} ", Integer);
            else
                return Value;
        }
        public override double GetNumber()
        {
            return Integer;
        }
    }
}
