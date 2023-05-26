using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Theor1;

namespace Theor1
{
    public class Expression
    {
        List<Token> ExpressionStack = new List<Token>();
        Stack<string> Operations = new Stack<string>();
        Stack<int> Prioritis = new Stack<int>();
        int index = 0;
        string output = null;
        Dictionary<string, int> priority = new Dictionary<string, int>() //Список приоритетов для операций
        {
            {"+", 0},
            {"-", 0},
            {"*", 1},
            {"/", 1}
        };
        public void TakeToken(Token token)//Получение из анализатора логического выражения
        {

            ExpressionStack.Add(token);
        }
        public void StartOPN()//Запуск программы
        {
            Decstra();
            PolishNotation();
        }
        private void HighPriority(string operation)//Вспомогательная функция для метода Дейкстры
        {
            int count = Operations.Count();
            Stack<string> temp = new Stack<string>();//Вспомогательный стек для выполнения выталкивания операций с большим приоритетом
            Stack<int> priorityTemp = new Stack<int>();//Вспомогательный стек приоритетов для выполнения выталкивания операций с большим приоритетом
            for (int i = 0; i < count; i++)//Цикл выталкивания
            {
                if (Prioritis.Peek() >= priority[operation])
                {
                    output += Operations.Pop();
                    Prioritis.Pop();
                }
                else
                {
                    temp.Push(Operations.Pop());
                    priorityTemp.Push(Prioritis.Pop());
                }
            }
            temp.Reverse();
            priorityTemp.Reverse();
            int countTemp = temp.Count();//Цикл возврата операций, которые не были вытолкнуты
            for (int i = 0; i < countTemp; i++)
            {
                Operations.Push(temp.Pop());
                Prioritis.Push(priorityTemp.Pop());
            }
            Operations.Push(ExpressionStack[index].DCR);
            Prioritis.Push(priority[operation]);
        }

        private void Decstra()//Метод Дейкстры
        {
            if (ExpressionStack[index].Type == TokenType.IDENTIFIER || ExpressionStack[index].Type == TokenType.LITERAL)
            {
                Prioritis.Push(0);

                while (index != ExpressionStack.Count())
                {
                    if (ExpressionStack[index].Type == TokenType.LITERAL || ExpressionStack[index].Type == TokenType.IDENTIFIER)
                    {
                        output += ExpressionStack[index].DCR + " ";
                        index++;
                    }
                    else if (ExpressionStack[index].Type == TokenType.PLUS)
                    {
                        string operation = "+";

                        if ((priority[operation] > Prioritis.Peek()) || Operations.Count() ==
                        0)
                        {
                            Operations.Push(ExpressionStack[index].DCR);
                            Prioritis.Push(priority[operation]);
                        }
                        else
                        {
                            HighPriority(operation);
                        }
                        index++;
                    }
                    else if (ExpressionStack[index].Type == TokenType.MINUS)
                    {
                        string operation = "-";
                        if ((priority[operation] > Prioritis.Peek()) || Operations.Count() ==
                        0)
                        {
                            Operations.Push(ExpressionStack[index].DCR);
                            Prioritis.Push(priority[operation]);
                        }
                        else
                        {
                            HighPriority(operation);
                        }
                        index++;
                    }
                    else if (ExpressionStack[index].Type == TokenType.MULTIPLY)
                    {
                        string operation = "*";
                        if ((priority[operation] > Prioritis.Peek()) || Operations.Count() ==
                        0)
                        {
                            Operations.Push(ExpressionStack[index].DCR);
                            Prioritis.Push(priority[operation]);
                        }
                        else
                        {
                            HighPriority(operation);
                        }
                        index++;
                    }
                    else if (ExpressionStack[index].Type == TokenType.DIVIDE)
                    {
                        string operation = "/";
                        if ((priority[operation] > Prioritis.Peek()) || Operations.Count() ==
                        0)

                        {
                            Operations.Push(ExpressionStack[index].DCR);
                            Prioritis.Push(priority[operation]);
                        }
                        else
                        {
                            HighPriority(operation);
                        }
                        index++;
                    }
                    else if (ExpressionStack[index].Type == TokenType.IDENTIFIER || ExpressionStack[index].Type == TokenType.LITERAL)
                    {
                        Operations.Pop();
                        Operations.Pop();
                        Prioritis.Pop();
                        Prioritis.Pop();
                    }
                    else
                    {
                        throw new Exception("Неверно составлено выражение.");
                    }
                }
                int countOperations = Operations.Count();
                for (int i = 0; i < countOperations; i++)//Выталкивание всех оставшихся операций в стеке
                {
                    output += Operations.Pop();
                }
            }
            else
                throw new Exception("Неверно составлено выражение.");
        }
        public void PolishNotation()//Метод выполняющий преобразование обратную польскую нотацию в матричный вид
        {
            Dictionary<int, string> M = new Dictionary<int, string>();
            Stack<string> stackOperand = new Stack<string>();
            int key = 1;
            for (int i = 0; i < output.Count(); i++)
            {
                char currentChar = output[i];
                switch (currentChar)
                {

                    case ('+'):
                        {
                            M.Add(key, stackOperand.Pop() + " " + stackOperand.Pop() + " " + "+");
                            stackOperand.Push("M" + key.ToString());
                            key++;
                            break;
                        }
                    case ('-'):
                        {
                            M.Add(key, stackOperand.Pop() + " " + stackOperand.Pop() + " " + "-");
                            stackOperand.Push("M" + key.ToString());
                            key++;
                            break;
                        }
                    case ('*'):
                        {
                            M.Add(key, stackOperand.Pop() + " " + stackOperand.Pop() + " " + "*");
                            stackOperand.Push("M" + key.ToString());
                            key++;
                            break;
                        }
                    case ('/'):
                        {
                            M.Add(key, stackOperand.Pop() + " " + stackOperand.Pop() + " " + "/");
                            stackOperand.Push("M" + key.ToString());
                            key++;
                            break;
                        }

                    default:
                        {
                            if (Regex.IsMatch(currentChar.ToString(), "^[a-zA-Z]+$") || Regex.IsMatch(currentChar.ToString(), "^[0-9]+$"))
                            {
                                string temp = null;
                                while (output[i] != ' ')
                                {
                                    temp += output[i].ToString();
                                    i++;
                                }
                                stackOperand.Push(temp);
                            }
                            else if (currentChar == ' ')
                            {

                            }
                            else
                            {
                                throw new System.Exception("Ошибка перевода в матричный вид");
                            }
                            break;
                        }
                }
            }
            Form1._form1.Conclusion1();
            Form1._form1.Conclusion("Обратная польская нотация:");
            Form1._form1.Conclusion(output);
            Form1._form1.Conclusion("Матричный вид:");
            int countOutput = stackOperand.Count;
            for (int i = 0; i < countOutput; i++)
            {
                Form1._form1.Conclusion(stackOperand.Pop());
            }
            int countM = M.Count;
            for (int i = 1; i < countM + 1; i++)
            {
                Form1._form1.Conclusion("M" + i + ":" + M[i]);
            }
        }
    }
}
