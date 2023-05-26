using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Theor1
{
    public class LR
    {
        List<Token> tokens = new List<Token>();
        Stack<Token> lexemStack = new Stack<Token>();
        Stack<int> stateStack = new Stack<int>();
        private static List<KeyValuePair<string, int>> inputOPN = new List<KeyValuePair<string, int>>();
        int nextLex = 0;
        int state = 0;
        bool isEnd = false;
        public LR(List<Token> vvodtoken)
        {
            tokens = vvodtoken;
        }
        private Token GetLexeme(int nextLex)
        {
            return tokens[nextLex];
        }

        private void Shift()
        {
            lexemStack.Push(GetLexeme(nextLex));
            nextLex++;
        }
        private void GoToState(int state)
        {
            stateStack.Push(state);
            this.state = state;
        }
        private void Reduce(int num, string neterm)
        {
            for (int i = 0; i < num; i++)
            {
                lexemStack.Pop();
                stateStack.Pop();
            }
            state = stateStack.Peek();
            Token k = new Token(TokenType.NETERM);
            k.Value = neterm;
            lexemStack.Push(k);
        }
        public void Programm()
        {
            Start();
        }
        private void Expr()
        {
            Expression expr = new Expression();
            while (GetLexeme(nextLex).Type != TokenType.LINEBREAK)
            {
                expr.TakeToken(GetLexeme(nextLex));
                Shift();
            }
            Token k = new Token(TokenType.EXPR);
            lexemStack.Push(k);
            expr.StartOPN();
        }

        void State0()
        {
            if (lexemStack.Count == 0)
                Shift();
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<программа>":
                            if (nextLex == tokens.Count)
                                isEnd = true;
                            break;
                        case "<спис.объяв>":
                            GoToState(1);
                            break;
                    }
                    break;
                case TokenType.DIM:
                    GoToState(2);
                    break;
                default:
                    throw new Exception($"Ожидалось Dim, но было получено {lexemStack.Peek().DCR}. State: 0");
            }
        }
        void State1()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<спис.опер>":
                            GoToState(3);
                            if (lexemStack.Count > 2)
                            {
                                Reduce(2, "<спис.опер>");
                                GoToState(1);
                            }
                            break;
                        case "<опер>":
                            GoToState(4);
                            break;
                        case "<иниц>":
                            GoToState(40);
                            break;
                        case "<присвоен>":
                            GoToState(17);
                            break;
                        case "<условн>":
                            GoToState(18);
                            break;
                        case "<спис.объяв>":
                            Shift();
                            break;
                    }
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(19);
                    break;
                case TokenType.FOR:
                    GoToState(20);
                    break;
                case TokenType.DIM:
                    GoToState(41);
                    break;
                default:
                    throw new Exception($"Ожидалось Dim, id или for, но было получено {lexemStack.Peek().DCR}. State: 1");
            }
        }
        void State2()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<спис.идент>":
                            GoToState(5);
                            break;
                    }
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(6);
                    break;
                case TokenType.DIM:
                    Shift();
                    break;
                default:
                    throw new Exception($"Ожидалось id, но было получено {lexemStack.Peek().DCR}. State: 2");
            }
        }
        void State3()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<спис.опер>")
                Reduce(2, "<программа>");
            else
                throw new Exception($"Ожидалось правило <спис.опер>, но было получено {lexemStack.Peek().DCR}. State: 3");
        }
        void State4()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<опер>":
                            Shift();
                            break;
                    }
                    break;
                case TokenType.LINEBREAK:
                    GoToState(15);
                    break;
                default:
                    throw new Exception($"Ожидался linebreak, но было получено {lexemStack.Peek().DCR}. State: 4");
            }
        }
        void State5()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<спис.идент>":
                            Shift();
                            break;
                    }
                    break;
                case TokenType.AS:
                    GoToState(7);
                    break;
                default:
                    throw new Exception($"Ожидалось as, но было получено {lexemStack.Peek().DCR}. State: 5");
            }
        }
        void State6()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.IDENTIFIER:
                    Shift();
                    if (lexemStack.Peek().Type == TokenType.AS)
                    {
                        lexemStack.Pop();
                        Reduce(1, "<спис.идент>");
                        lexemStack.Push(new Token(TokenType.AS));
                        GoToState(5);
                    }
                    break;
                case TokenType.COMMA:
                    GoToState(13);
                    break;
                default:
                    throw new Exception($"Ожидалась запятая, но было получено {lexemStack.Peek().DCR}. State: 6");
            }
        }
        void State7()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<тип>":
                            GoToState(8);
                            break;
                    }
                    break;
                case TokenType.AS:
                    Shift();
                    break;
                case TokenType.INTEGER:
                    GoToState(9);
                    break;
                case TokenType.STRING:
                    GoToState(11);
                    break;
                case TokenType.BOOL:
                    GoToState(10);
                    break;
                default:
                    throw new Exception($"Ожидалось integer, bool или string, но было получено {lexemStack.Peek().DCR}. State: 7");
            }
        }
        void State8()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<тип>":
                            Shift();
                            break;
                    }
                    break;
                case TokenType.LINEBREAK:
                    GoToState(12);
                    break;
                default:
                    throw new Exception($"Ожидался linebreak, но было получено {lexemStack.Peek().DCR}. State: 8");
            }
        }
        void State9()
        {
            if (lexemStack.Peek().Type == TokenType.INTEGER)
                Reduce(1, "<тип>");
            else
                throw new Exception($"Ожидалась лексема integer, но было получено {lexemStack.Peek().DCR}. State: 9");
        }
        void State10()
        {
            if (lexemStack.Peek().Type == TokenType.BOOL)
                Reduce(1, "<тип>");
            else
                throw new Exception($"Ожидалась лексема bool, но было получено {lexemStack.Peek().DCR}. State: 10");
        }
        void State11()
        {
            if (lexemStack.Peek().Type == TokenType.STRING)
                Reduce(1, "<тип>");
            else
                throw new Exception($"Ожидалась лексема string, но было получено {lexemStack.Peek().DCR}. State: 11");
        }
        void State12()
        {
            if (lexemStack.Peek().Type == TokenType.LINEBREAK && lexemStack.Count < 6)
                Reduce(5, "<спис.объяв>");
            else if (lexemStack.Peek().Type == TokenType.LINEBREAK && lexemStack.Count > 6)
                Reduce(7, "<спис.объяв>");
            else
                throw new Exception($"Ожидалась лексема linebreak, но было получено {lexemStack.Peek().DCR}. State: 12");
        }
        void State13()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.COMMA:
                    Shift();
                    break;
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<спис.идент>":
                            GoToState(14);
                            break;
                    }
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(6);
                    break;
                default:
                    throw new Exception($"Ожидалось id, но было получено {lexemStack.Peek().DCR}. State: 13");
            }
        }
        void State14()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<спис.идент>")
                Reduce(3, "<спис.объяв>");
            else
                throw new Exception($"Ожидалось правило <спис.идент>, но было получено {lexemStack.Peek().DCR}. State: 14");
        }
        void State15()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<спис.опер>":
                            GoToState(16);
                            break;
                    }
                    break;
                case TokenType.LINEBREAK:
                    if (nextLex == tokens.Count)
                    {
                        Reduce(2, "<спис.опер>");
                        break;
                    }
                    if (GetLexeme(nextLex).Type == TokenType.FOR || GetLexeme(nextLex).Type == TokenType.IDENTIFIER)
                    {
                        Reduce(2, "<спис.опер>");
                        Shift();
                    }
                    if (GetLexeme(nextLex).Type == TokenType.NEXT)
                    {
                        Reduce(2, "<спис.опер>");
                    }
                    break;
                default:
                    throw new Exception($"Ожидался linebreak, но было получено {lexemStack.Peek().DCR}. State: 15");
            }
        }
        void State16()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<спис.опер>")
                Reduce(3, "<спис.опер>");
            else
                throw new Exception($"Ожидалось правило <спис.опер>, но было получено {lexemStack.Peek().DCR}. State: 16");
        }
        void State17()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<присвоен>")
                Reduce(1, "<опер>");
            else
                throw new Exception($"Ожидалась правило <присвоен>, но было получено {lexemStack.Peek().DCR}. State: 17");
        }
        void State18()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<условн>")
                Reduce(1, "<опер>");
            else
                throw new Exception($"Ожидалась правило <условн>, но было получено {lexemStack.Peek().DCR}. State: 18");
        }
        void State40()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<иниц>")
                Reduce(1, "<опер>");
            else
                throw new Exception($"Ожидалась правило <условн>, но было получено {lexemStack.Peek().DCR}. State: 40");
        }
        void State19()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.IDENTIFIER:
                    Shift();
                    break;
                case TokenType.EQUAL:
                    GoToState(21);
                    break;
                default:
                    throw new Exception($"Ожидалось =, но было получено {lexemStack.Peek().DCR}. State: 19");
            }
        }
        void State20()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<слож.операнд>":
                            GoToState(23);
                            break;
                    }
                    break;
                case TokenType.FOR:
                    Shift();
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(25);
                    break;
                default:
                    throw new Exception($"Ожидалось id, но было получено {lexemStack.Peek().DCR}. State: 20");
            }
        }
        void State21()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.EQUAL:
                    Expr();
                    break;
                case TokenType.EXPR:
                    GoToState(22);
                    break;
                default:
                    throw new Exception($"Ожидалось выражение, но было получено {lexemStack.Peek().DCR}. State: 21");
            }
        }
        void State22()
        {
            if (lexemStack.Peek().Type == TokenType.EXPR)
                Reduce(3, "<присвоен>");
            else
                throw new Exception($"Ожидалось выражение, но было получено {lexemStack.Peek().DCR}. State: 22");
        }
        void State23()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<слож.операнд>":
                            Shift();
                            break;
                    }
                    break;
                case TokenType.TO:
                    GoToState(24);
                    break;
                default:
                    throw new Exception($"Ожидалось to, но было получено {lexemStack.Peek().DCR}. State: 23");
            }
        }
        void State24()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.TO:
                    Shift();
                    break;
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<операнд>":
                            GoToState(34);
                            break;
                    }
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(26);
                    break;
                case TokenType.LITERAL:
                    GoToState(27);
                    break;
                default:
                    throw new Exception($"Ожидалось id или lit, но было получено {lexemStack.Peek().DCR}. State: 24");
            }
        }
        void State25()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.IDENTIFIER:
                    Shift();
                    break;
                case TokenType.EQUAL:
                    GoToState(28);
                    break;
                default:
                    throw new Exception($"Ожидалось +-*/=, но было получено {lexemStack.Peek().DCR}. State: 25");
            }
        }
        void State26()
        {
            if (lexemStack.Peek().Type == TokenType.IDENTIFIER)
                Reduce(1, "<операнд>");
            else
                throw new Exception($"Ожидался терминал IDENTIFIER, но было получено {lexemStack.Peek().DCR}. State: 26");
        }
        void State27()
        {
            if (lexemStack.Peek().Type == TokenType.LITERAL)
                Reduce(1, "<операнд>");
            else
                throw new Exception($"Ожидался терминал LITERAL, но было получено {lexemStack.Peek().DCR}. State: 27");
        }
        void State28()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<операнд>":
                            GoToState(35);
                            break;
                    }
                    break;
                case TokenType.EQUAL:
                    Shift();
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(26);
                    break;
                case TokenType.LITERAL:
                    GoToState(27);
                    break;
                default:
                    throw new Exception($"Ожидалось id или lit, но было получено {lexemStack.Peek().DCR}. State: 28");
            }
        }
        void State29()
        {
            if (lexemStack.Peek().Type == TokenType.PLUS)
                Reduce(1, "<знак>");
            else
                throw new Exception($"Ожидался терминал +, но было получено {lexemStack.Peek().DCR}. State: 29");
        }
        void State30()
        {
            if (lexemStack.Peek().Type == TokenType.MINUS)
                Reduce(1, "<знак>");
            else
                throw new Exception($"Ожидался терминал -, но было получено {lexemStack.Peek().DCR}. State: 30");
        }

        void State31()
        {
            if (lexemStack.Peek().Type == TokenType.MULTIPLY)
                Reduce(1, "<знак>");
            else
                throw new Exception($"Ожидался терминал *, но было получено {lexemStack.Peek().DCR}. State: 31");
        }
        void State32()
        {
            if (lexemStack.Peek().Type == TokenType.DIVIDE)
                Reduce(1, "<знак>");
            else
                throw new Exception($"Ожидался терминал /, но было получено {lexemStack.Peek().DCR}. State: 32");
        }
        void State34()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<операнд>":
                            Shift();
                            break;
                    }
                    break;
                case TokenType.LINEBREAK:
                    GoToState(36);
                    break;
                default:
                    throw new Exception($"Ожидался linebreak, но было получено {lexemStack.Peek().DCR}. State: 34");
            }
        }
        void State35()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<операнд>")
                Reduce(3, "<слож.операнд>");
            else
                throw new Exception($"Ожидалась правило <операнд>, но было получено {lexemStack.Peek().DCR}. State: 35");
        }
        void State36()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<спис.опер>":
                            GoToState(37);
                            break;
                        case "<опер>":
                            GoToState(4);
                            break;
                        case "<иниц>":
                            GoToState(40);
                            break;
                        case "<присвоен>":
                            GoToState(17);
                            break;
                        case "<условн>":
                            GoToState(18);
                            break;
                        case "<спис.объяв>":
                            Shift();
                            break;
                    }
                    break;
                case TokenType.LINEBREAK:
                    Shift();
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(19);
                    break;
                case TokenType.FOR:
                    GoToState(20);
                    break;
                case TokenType.DIM:
                    GoToState(41);
                    break;
                default:
                    throw new Exception($"Ожидалось id или for, но было получено {lexemStack.Peek().DCR}. State: 36");
            }
        }
        void State37()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<спис.опер>":
                            Shift();
                            break;
                    }
                    break;
                case TokenType.NEXT:
                    GoToState(38);
                    break;
                default:
                    throw new Exception($"Ожидалось next, но было получено {lexemStack.Peek().DCR}. State: 37");
            }
        }
        void State38()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NEXT:
                    Shift();
                    break;
                case TokenType.FOR:
                    GoToState(39);
                    break;
                default:
                    throw new Exception($"Ожидалось for, но было получено {lexemStack.Peek().DCR}. State: 38");
            }
        }
        void State39()
        {
            if (lexemStack.Peek().Type == TokenType.FOR)
                Reduce(8, "<опер>");
            else
                throw new Exception($"Ожидался терминал IF, но было получено {lexemStack.Peek().DCR}. State: 39");
        }
        void State41()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<спис.идент>":
                            GoToState(42);
                            break;
                    }
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(46);
                    break;
                case TokenType.DIM:
                    Shift();
                    break;
                default:
                    throw new Exception($"Ожидалось правило id, но было получено {lexemStack.Peek().DCR}. State: 41");
            }
        }
        void State42()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<спис.идент>":
                            Shift();
                            break;
                    }
                    break;
                case TokenType.AS:
                    GoToState(43);
                    break;
                default:
                    throw new Exception($"Ожидалось as, но было получено {lexemStack.Peek().DCR}. State: 42");
            }
        }
        void State43()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<тип>":
                            GoToState(44);
                            break;
                    }
                    break;
                case TokenType.AS:
                    Shift();
                    break;
                case TokenType.INTEGER:
                    GoToState(49);
                    break;
                case TokenType.STRING:
                    GoToState(51);
                    break;
                case TokenType.BOOL:
                    GoToState(50);
                    break;
                default:
                    throw new Exception($"Ожидалось integer/BOOL/string, но было получено {lexemStack.Peek().DCR}. State: 43");
            }
        }
        void State44()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<тип>" && lexemStack.Count < 5)
                Reduce(4, "<иниц>");
            else if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<тип>" && lexemStack.Count > 5)
                Reduce(6, "<иниц>");
            else
                throw new Exception($"Ожидалась лексема <тип>, но было получено {lexemStack.Peek().DCR}. State: 44");
        }

        void State46()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.IDENTIFIER:
                    Shift();
                    if (lexemStack.Peek().Type == TokenType.AS)
                    {
                        lexemStack.Pop();
                        Reduce(1, "<спис.идент>");
                        lexemStack.Push(new Token(TokenType.AS));
                        GoToState(42);
                    }
                    break;
                case TokenType.COMMA:
                    GoToState(47);
                    break;
                default:
                    throw new Exception($"Ожидалась запятая, но было получено {lexemStack.Peek().DCR}. State: 46");
            }
        }
        void State47()
        {
            switch (lexemStack.Peek().Type)
            {
                case TokenType.COMMA:
                    Shift();
                    break;
                case TokenType.NETERM:
                    switch (lexemStack.Peek().Value)
                    {
                        case "<спис.идент>":
                            GoToState(48);
                            break;
                    }
                    break;
                case TokenType.IDENTIFIER:
                    GoToState(46);
                    break;
                default:
                    throw new Exception($"Ожидалось id, но было получено {lexemStack.Peek().DCR}. State: 47");
            }
        }
        void State48()
        {
            if (lexemStack.Peek().Type == TokenType.NETERM && lexemStack.Peek().Value == "<спис.идент>")
                Reduce(3, "<иниц>");
            else
                throw new Exception($"Ожидалось правило <спис.идент>, но было получено {lexemStack.Peek().DCR}. State: 48");
        }
        void State49()
        {
            if (lexemStack.Peek().Type == TokenType.INTEGER)
                Reduce(1, "<тип>");
            else
                throw new Exception($"Ожидалась лексема integer, но было получено {lexemStack.Peek().DCR}. State: 49");
        }
        void State50()
        {
            if (lexemStack.Peek().Type == TokenType.BOOL)
                Reduce(1, "<тип>");
            else
                throw new Exception($"Ожидалась лексема bool, но было получено {lexemStack.Peek().DCR}. State: 50");
        }
        void State51()
        {
            if (lexemStack.Peek().Type == TokenType.STRING)
                Reduce(1, "<тип>");
            else
                throw new Exception($"Ожидалась лексема string, но было получено {lexemStack.Peek().DCR}. State: 51");
        }
        public void Start()
        {
            stateStack.Push(0);
            while (isEnd != true)
            {
                switch (state)
                {
                    case 0:
                        State0();
                        break;
                    case 1:
                        State1();
                        break;
                    case 2:
                        State2();
                        break;
                    case 3:
                        State3();
                        break;
                    case 4:
                        State4();
                        break;
                    case 5:
                        State5();
                        break;
                    case 6:
                        State6();
                        break;
                    case 7:
                        State7();
                        break;
                    case 8:
                        State8();
                        break;
                    case 9:
                        State9();
                        break;
                    case 10:
                        State10();
                        break;
                    case 11:
                        State11();
                        break;
                    case 12:
                        State12();
                        break;
                    case 13:
                        State13();
                        break;
                    case 14:
                        State14();
                        break;
                    case 15:
                        State15();
                        break;
                    case 16:
                        State16();
                        break;
                    case 17:
                        State17();
                        break;
                    case 18:
                        State18();
                        break;
                    case 19:
                        State19();
                        break;
                    case 20:
                        State20();
                        break;
                    case 21:
                        State21();
                        break;
                    case 22:
                        State22();
                        break;
                    case 23:
                        State23();
                        break;
                    case 24:
                        State24();
                        break;
                    case 25:
                        State25();
                        break;
                    case 26:
                        State26();
                        break;
                    case 27:
                        State27();
                        break;
                    case 28:
                        State28();
                        break;
                    case 29:
                        State29();
                        break;
                    case 30:
                        State30();
                        break;
                    case 31:
                        State31();
                        break;
                    case 32:
                        State32();
                        break;
                    case 34:
                        State34();
                        break;
                    case 35:
                        State35();
                        break;
                    case 36:
                        State36();
                        break;
                    case 37:
                        State37();
                        break;
                    case 38:
                        State38();
                        break;
                    case 39:
                        State39();
                        break;
                    case 40:
                        State40();
                        break;
                    case 41:
                        State41();
                        break;
                    case 42:
                        State42();
                        break;
                    case 43:
                        State43();
                        break;
                    case 44:
                        State44();
                        break;
                    case 46:
                        State46();
                        break;
                    case 47:
                        State47();
                        break;
                    case 48:
                        State48();
                        break;
                    case 49:
                        State49();
                        break;
                    case 50:
                        State50();
                        break;
                    case 51:
                        State51();
                        break;
                }
            }
        }
    }
}
