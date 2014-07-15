using System;
using System.Collections.Generic;
using SophiaLex;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SophiaLexTests
{
    [TestClass]
    public class UnitTests
    {
        Evaluator eval;
        [TestInitialize]
        public void Init()
        {
            eval = new Evaluator();
        }
        [TestMethod]
        public void SimpleValues()
        {
            Dictionary<string, double> tests = new Dictionary<string, double>()
            {
               {"3", 3},
               {"-3", -3},
               {"(3)", 3},
               {"(-3)", -3},
               {"-(3)", -3},
            };
            foreach (var expr in tests.Keys)
            {
                Assert.AreEqual(tests[expr], eval.Evaluate(expr));
            }
        }
        [TestMethod]
        public void BinaryExpressions()
        {
            Dictionary<string, double> tests = new Dictionary<string, double>()
            {
               {"3*4", 3*4},
               {"(3)*4", 3*4},
               {"3*(4)", 3*4},
               {"(3*4)", 3*4},
               {"((3)*(4))", 3*4d},
               {"3/4", 3/4d},
               {"3/(4)", 3/4d},
               {"3+4", 3+4},
               {"(3)+4", 3+4},
               {"3-(4)", 3-4},
               {"(3)-4", 3-4},
               {"(3)-(4)", 3-4},
               {"(3)+(-4)", 3-4},
            };
            foreach (var expr in tests.Keys)
            {
                Assert.AreEqual(tests[expr], eval.Evaluate(expr));
            }
        }
        [TestMethod]
        public void ExponentExpressions()
        {
            Dictionary<string, double> tests = new Dictionary<string, double>()
            {
               {"3^3", 27},
               {"(3)^(3)", 27},
               {"(3)^3", 27},
               {"3^(3)", 27},
            };
            foreach (var expr in tests.Keys)
            {
                Assert.AreEqual(tests[expr], eval.Evaluate(expr));
            }
        }
        [TestMethod]
        public void ExponentRightToLeft()
        {
            Dictionary<string, double> tests = new Dictionary<string, double>()
            {
               {"3^3^2", 19683},
            };
            foreach (var expr in tests.Keys)
            {
                Assert.AreEqual(tests[expr], eval.Evaluate(expr));
            }
        }
    }
}
