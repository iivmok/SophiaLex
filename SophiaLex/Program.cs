using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace SophiaLex
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var tokens = Lexer.Tokenize("(3 * (4 + ( (-1/-3) * 3)) * 1 * 1 * 1 / 2 + 15^2)-153-3+(-10)");
            //var eval = new Evaluator();
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //for (int i = 0; i < 1000 * 100; i++)
            //{
            //    eval.Evaluate("(3 * (4 + ( (-1/-3) * 3)) * 1 * 1 * 1 / 2 + 15^2)-153-3+(-10)");
            //}
            //sw.Stop();
            //double perLoop = sw.ElapsedMilliseconds / (1000d * 100);
            //var str = Lexer.StringifyTokens(tokens);
            //var exp = Parser.Parse(tokens);
            //double val = exp.Evaluate(new Evaluator());
            //double val2 = Parser.Parse(Lexer.Tokenize("2^2^2^2")).Evaluate();
            //Console.WriteLine(str);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
