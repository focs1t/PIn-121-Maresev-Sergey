using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theor1
{
    public enum TokenType
    {
        INTEGER, BOOL, STRING, DIM, AS, FOR, TO, 
        NEXT, PLUS, MINUS, MULTIPLY, DIVIDE, 
        OR, DEGREE, EQUAL, MORE, LESS, COMMA, DOT,
        COLON, SEMICOLON, LPAR, RPAR, UNDERSCORE,
        IDENTIFIER, LITERAL, LINEBREAK, NETERM, EXPR
    }
    public class Token
    {
        public TokenType Type;
        public string Value;
        public string DCR;
        public Token(TokenType type)
        {
            Type = type;
        }
        public override string ToString()
        {
            return string.Format("{0}, {1}", Type, Value);

        }
        static TokenType[] Delimiters = new TokenType[]
        {
            TokenType.PLUS, TokenType.MINUS, TokenType.MULTIPLY, 
            TokenType.DIVIDE, TokenType.OR, TokenType.DEGREE,
            TokenType.EQUAL, TokenType.MORE, TokenType.LESS,
            TokenType.COMMA, TokenType.DOT, TokenType.COLON,
            TokenType.SEMICOLON, TokenType.LPAR, TokenType.RPAR,
            TokenType.UNDERSCORE, TokenType.LINEBREAK
        };
        public static bool IsDelimiter(Token token)
        {
            return Delimiters.Contains(token.Type);
        }
        public static Dictionary<string, TokenType> SpecialWords = new Dictionary<string, TokenType>() {
            {"integer", TokenType.INTEGER},
            {"BOOL", TokenType.BOOL},
            {"string", TokenType.STRING},
            {"Dim", TokenType.DIM},
            {"as", TokenType.AS},
            {"for", TokenType.FOR},
            {"to", TokenType.TO},
            {"next", TokenType.NEXT},
            {"expr", TokenType.EXPR}
        };
        public static bool IsSpecialWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }
            return (SpecialWords.ContainsKey(word));
        }
        public static Dictionary<char, TokenType> SpecialSymbols = new Dictionary<char, TokenType>() 
        {
            {'+', TokenType.PLUS},
            {'-', TokenType.MINUS},
            {'*', TokenType.MULTIPLY},
            {'/', TokenType.DIVIDE},
            {'|', TokenType.OR},
            {'^', TokenType.DEGREE},
            {'=', TokenType.EQUAL},            
            {'>', TokenType.MORE},
            {'<', TokenType.LESS},
            {',', TokenType.COMMA},
            {'.', TokenType.DOT},
            {':', TokenType.COLON},
            {';', TokenType.SEMICOLON},
            {'(', TokenType.LPAR},
            {')', TokenType.RPAR},
            {'_', TokenType.UNDERSCORE},
            {'\n',TokenType.LINEBREAK}
        };
        public static bool IsSpecialSymbol(char ch)
        {
            return SpecialSymbols.ContainsKey(ch);
        }
    }
}