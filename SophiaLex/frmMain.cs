using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SophiaLex
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        Evaluator evaluator = new Evaluator();

        private void btnEval_Click(object sender, EventArgs e)
        {
            var tokens = Lexer.Tokenize(txtMath.Text);
            txtTokens.Text = Lexer.StringifyTokens(tokens);
            var expression = Parser.Parse(tokens);
            var tn = new TreeNode();
            tn.Text = expression.ToString();
            tn.Tag = expression;
            tn.Nodes.Add("");

            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(tn);
            txtAnswer.Text = expression.Evaluate(evaluator).ToString();
            //Lexer.
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var tn = e.Node;

            tn.Nodes.Clear();

            var exp = e.Node.Tag as Expression;
            if (exp is ParenthesesExpression)
            {
                foreach (var item in (exp as ParenthesesExpression).Items)
                {
                    var n = new TreeNode();
                    n.Text = item.ToString();
                    n.Tag = item;
                    n.Nodes.Add("");

                    tn.Nodes.Add(n);
                }
            }
            else if (exp is BinaryExpression)
            {
                var bexp = exp as BinaryExpression;
                {
                    var n = new TreeNode();
                    n.Text = "Left: " + bexp.Left.ToString();
                    n.Tag = bexp.Left;
                    if (!(bexp.Left is LiteralExpression))
                        n.Nodes.Add("");

                    tn.Nodes.Add(n);
                }
                {
                    var n = new TreeNode();
                    n.Text = "Op: " + bexp.Operator.Value;
                    n.Tag = bexp.Operator;

                    tn.Nodes.Add(n);
                }
                {
                    var n = new TreeNode();
                    n.Text = "Right: " + bexp.Right.ToString();
                    n.Tag = bexp.Right;
                    if(!(bexp.Right is LiteralExpression))
                        n.Nodes.Add("");

                    tn.Nodes.Add(n);
                }
            }
        }
    }
}
