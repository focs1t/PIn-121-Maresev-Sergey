using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theor1
{
    public class LL
    {
        List<Token> tokens = new List<Token>();
        Token currentLex;
        Token nextLex;
        int i = 0;
        public LL(List<Token> vvodtoken)
        {
            tokens = vvodtoken;
            currentLex = tokens[i];
        }
        private void Next()
        {
            if (i != tokens.Count - 1)
            {
                i++;
                currentLex = tokens[i];
            }
        }
        public void Programm()
        {
            OpisList();
            OperList();
        }
        private void OpisList()
        {
            Opis();
            if (currentLex.Type != TokenType.LINEBREAK)
                Error(TokenType.LINEBREAK, currentLex.Type);
            Next();
            ListOfInitilization();
        }
        private void ListOfInitilization()
        {
            if (currentLex.Type == TokenType.DIM)
                OpisList();
            else if (currentLex.Type == TokenType.FOR || currentLex.Type == TokenType.IDENTIFIER)
                return;
            else
                throw new Exception($"Ожидалось Dim или for, но было получено {currentLex.Type}");
        }
        private void Opis()
        {
            if (currentLex.Type != TokenType.DIM)
                Error(TokenType.DIM, currentLex.Type);
            Next();
            PeremList();
            if (currentLex.Type != TokenType.AS)
                Error(TokenType.AS, currentLex.Type);
            Next();
            Type();
        }
        private void PeremList()
        {
            if (currentLex.Type != TokenType.IDENTIFIER)
                Error(TokenType.IDENTIFIER, currentLex.Type);
            Next();
            AdditionalPerem();
        }
        private void AdditionalPerem()
        {
            if (currentLex.Type == TokenType.AS)
                return;
            else if (currentLex.Type == TokenType.COMMA)
                Dop1();
            else
                throw new Exception($"Ожидалось AS или COMMA, но было получено {currentLex.Type}");
        }
        private void Dop1()
        {
            if (currentLex.Type != TokenType.COMMA)
                Error(TokenType.COMMA, currentLex.Type);
            Next();
            if (currentLex.Type != TokenType.IDENTIFIER)
                Error(TokenType.IDENTIFIER, currentLex.Type);
            Next();
            AdditionalPerem();
        }
        private void Type()
        {
            if (currentLex.Type == TokenType.INTEGER)
                Next();
            else if (currentLex.Type == TokenType.BOOL)
                Next();
            else if (currentLex.Type == TokenType.STRING)
                Next();
            else
                throw new Exception($"Ожидалось INTEGER, BOOL или STRING,но было получено {currentLex.Type}");
        }
        private void OperList()
        {
            Oper();
            AdditionalOper();
        }
        private void AdditionalOper()
        {
            if (i == tokens.Count - 1)
                return;
            else if (currentLex.Type == TokenType.LINEBREAK)
            {
                nextLex = tokens[i + 1];
                if (nextLex.Type == TokenType.NEXT)
                    return;
                else if (nextLex.Type == TokenType.FOR)
                    Dop2();
                else if (nextLex.Type == TokenType.IDENTIFIER)
                    Dop2();
                else
                    throw new Exception($"Ожидалось next, for или INDENTIFIER, но было получено {nextLex.Type}");
            }
            else
                //throw new Exception($"Ожидалось LINEBREAK или $, но было получено {currentLex.Type}");
                return;
        }
        private void Dop2()
        {
            //if (currentLex.Type != TokenType.LINEBREAK)
            //    Error(TokenType.LINEBREAK, currentLex.Type);
            Next();
            Oper();
            AdditionalOper();
        }
        private void Oper()
        {
            if (currentLex.Type == TokenType.FOR)
                Uslov();
            else if (currentLex.Type == TokenType.IDENTIFIER)
                Prisv();
            else
                throw new Exception($"Ожидалось for или IDENTIFIER, но было получено {currentLex.Type}");
        }
        private void Uslov()
        {
            if (currentLex.Type != TokenType.FOR)
                Error(TokenType.FOR, currentLex.Type);
            Next();
            Expression();
            if (currentLex.Type != TokenType.TO)
                Error(TokenType.TO, currentLex.Type);
            Next();
            Operand();
            //if (currentLex.Type != TokenType.LINEBREAK)
            //    Error(TokenType.LINEBREAK, currentLex.Type);
            Next();
            OperList();
            //if (currentLex.Type != TokenType.LINEBREAK)
            //    Error(TokenType.LINEBREAK, currentLex.Type);
            Next();
            Pref4();
        }
        private void Expression()
        {
            Next();
            Next();
            Next();
        }
        private void Pref4()
        {
            if (currentLex.Type == TokenType.NEXT)
            {
                Next();
                if (currentLex.Type != TokenType.FOR)
                    Error(TokenType.FOR, currentLex.Type);
            }
            else
                //throw new Exception($"Ожидалось next, но было получено {currentLex.Type}");
            return;
        }

        private void Prisv()
        {
            if (currentLex.Type != TokenType.IDENTIFIER)
                Error(TokenType.IDENTIFIER, currentLex.Type);
            Next();
            if (currentLex.Type != TokenType.EQUAL)
                Error(TokenType.EQUAL, currentLex.Type);
            Next();
            Expr();
        }
        private void Expr()
        {
            Next();
            Next();
            Next();
            Next();
            Next();
        }
        private void Operand()
        {
            if (currentLex.Type == TokenType.IDENTIFIER)
                Next();
            else if (currentLex.Type == TokenType.LITERAL)
                Next();
            else
                throw new Exception($"Ожидалось IDENTIFIER или LITERAL, но было получено {currentLex.Type}");
        }
        public string Error(TokenType type1, TokenType type2)
        {
            throw new Exception($"Ожидалось {type1}, но было получено {type2}");
        }
    }
}