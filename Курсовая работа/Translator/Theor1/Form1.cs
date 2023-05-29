using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Theor1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _form1 = this;
        }
        public static Form1 _form1;
        public void Conclusion1()
        {
            
        }
        public void Conclusion(string EXPR)
        {
            textBox1.Text += EXPR + Environment.NewLine;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Analys s = new Analys(richTextBox1.Text);
            try
            {
                s.Parse();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                MessageBox.Show($"Error! {ex.Message}");
            }
            foreach (Lex a in s.lexes)
            {
                listBox1.Items.Add($"{a.Lexema}    :    {a.Type}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Analys s = new Analys(richTextBox1.Text);
            s.Parse();
            List<Token> tokens = new List<Token>();
            foreach (Lex a in s.lexes)
            {
                string currentLexem = a.Lexema;
                if (a.Type == "разделитель")
                {
                    if (Token.IsSpecialSymbol(Convert.ToChar(a.Lexema)))
                    {
                        Token token = new Token(Token.SpecialSymbols[Convert.ToChar(a.Lexema)]);
                        token.DCR = currentLexem;
                        tokens.Add(token);
                    }
                }
                else if(a.Type =="идентификатор")
                {
                    if (Token.IsSpecialWord(a.Lexema))
                    {
                        Token token = new Token(Token.SpecialWords[a.Lexema]);
                        token.DCR = currentLexem;
                        tokens.Add(token);
                    }
                    else
                    {
                        Token token = new Token(TokenType.IDENTIFIER);
                        token.Value = currentLexem;
                        token.DCR = currentLexem;
                        tokens.Add(token);
                    }
                }
                else
                {
                    Token token = new Token(TokenType.LITERAL);
                    token.Value = currentLexem;
                    token.DCR = currentLexem;
                    tokens.Add(token);
                }
            }
            foreach (Token a in tokens)
            {
                listBox1.Items.Add($"{a.Type}    :    {a.Value}");
            }
            }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Файлы txt (*.txt)|*.txt";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader rdr = new StreamReader(fileDialog.FileName);
                string line = rdr.ReadToEnd();
                rdr.Close();
                richTextBox1.Text = line;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Analys s = new Analys(richTextBox1.Text);
            s.Parse();
            List<Token> tokens = new List<Token>();
            foreach (Lex a in s.lexes)
            {
                string currentLexem = a.Lexema;
                if (a.Type == "разделитель")
                {
                    if (Token.IsSpecialSymbol(Convert.ToChar(a.Lexema)))
                    {
                        Token token = new Token(Token.SpecialSymbols[Convert.ToChar(a.Lexema)]);
                        token.DCR = currentLexem;
                        tokens.Add(token);
                    }
                }
                else if (a.Type == "идентификатор")
                {
                    if (Token.IsSpecialWord(a.Lexema))
                    {
                        Token token = new Token(Token.SpecialWords[a.Lexema]);
                        token.DCR = currentLexem;
                        tokens.Add(token);
                    }
                    else
                    {
                        Token token = new Token(TokenType.IDENTIFIER);
                        token.Value = currentLexem;
                        token.DCR = currentLexem;
                        tokens.Add(token);
                    }
                }
                else
                {
                    Token token = new Token(TokenType.LITERAL);
                    token.Value = currentLexem;
                    token.DCR = currentLexem;
                    tokens.Add(token);
                }
            }
            LR rule = new LR(tokens);
            try
            {
                rule.Programm();
                MessageBox.Show("Разбор завершён");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                MessageBox.Show($"Error! {ex.Message}");
            }           //rule.Programm();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}