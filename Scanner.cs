using System;
using System.Collections.Generic;

namespace TinyScanner
{
    public enum Token_Class
    {
        // Reserved words
        Int, Float, String, Read, Write, Repeat, Until,
        If, ElseIf, Else, Then, Return, Endl, Main,
        ReservedWord,

        // Operators & symbols
        AssignOp, Semicolon, Comma,
        LParenthesis, RParenthesis, LBrace, RBrace,
        EqualOp, LessThanOp, GreaterThanOp, NotEqualOp,
        PlusOp, MinusOp, MultiplyOp, DivideOp,
        AndOp, OrOp,

        // Value tokens
        Identifier, Constant, StringConstant, End
    }

    public class Token
    {
        public string lex = null!;
        public Token_Class token_type;
        public int line;
    }

    public class ScanError
    {
        public int line;
        public string lex = null!;
        public string message = null!;
    }

    public class Scanner
    {
        public List<Token>     Tokens = new List<Token>();
        public List<ScanError> Errors = new List<ScanError>();

        private static readonly Dictionary<string, Token_Class> ReservedWords =
            new Dictionary<string, Token_Class>(StringComparer.Ordinal)
        {
            { "int",    Token_Class.Int    },
            { "float",  Token_Class.Float  },
            { "string", Token_Class.String },
            { "read",   Token_Class.Read   },
            { "write",  Token_Class.Write  },
            { "repeat", Token_Class.Repeat },
            { "until",  Token_Class.Until  },
            { "if",     Token_Class.If     },
            { "elseif", Token_Class.ElseIf },
            { "else",   Token_Class.Else   },
            { "then",   Token_Class.Then   },
            { "return", Token_Class.Return },
            { "endl",   Token_Class.Endl   },
            { "main",   Token_Class.Main   }
        };

        private static readonly Dictionary<string, Token_Class> Operators =
            new Dictionary<string, Token_Class>
        {
            { ":=", Token_Class.AssignOp      },
            { "<>", Token_Class.NotEqualOp    },
            { "&&", Token_Class.AndOp         },
            { "||", Token_Class.OrOp          },
            { ";",  Token_Class.Semicolon     },
            { ",",  Token_Class.Comma         },
            { "(",  Token_Class.LParenthesis  },
            { ")",  Token_Class.RParenthesis  },
            { "{",  Token_Class.LBrace        },
            { "}",  Token_Class.RBrace        },
            { "=",  Token_Class.EqualOp       },
            { "<",  Token_Class.LessThanOp    },
            { ">",  Token_Class.GreaterThanOp },
            { "+",  Token_Class.PlusOp        },
            { "-",  Token_Class.MinusOp       },
            { "*",  Token_Class.MultiplyOp    },
            { "/",  Token_Class.DivideOp      }
        };

        public void StartScanning(string source)
        {
            Tokens.Clear();
            Errors.Clear();

            int i = 0;
            int line = 1;

            while (i < source.Length)
            {
                char ch = source[i];

                // Newline
                if (ch == '\n') { line++; i++; continue; }

                // Whitespace
                if (char.IsWhiteSpace(ch)) { i++; continue; }

                // Comment  /* ... */
                if (ch == '/' && Peek(source, i + 1) == '*')
                {
                    int startLine = line;
                    i += 2;
                    while (i + 1 < source.Length && !(source[i] == '*' && source[i + 1] == '/'))
                    {
                        if (source[i] == '\n') line++;
                        i++;
                    }
                    if (i + 1 >= source.Length)
                        AddError(startLine, "/*...", "Unclosed comment");
                    else
                        i += 2;
                    continue;
                }

                // String literal  "..."
                if (ch == '"')
                {
                    int startLine = line;
                    string lex = "\"";
                    i++;
                    while (i < source.Length && source[i] != '"' && source[i] != '\n')
                    { lex += source[i]; i++; }

                    if (i >= source.Length || source[i] == '\n')
                    {
                        AddError(startLine, lex, "Unclosed string literal");
                    }
                    else
                    {
                        lex += "\""; i++;
                        AddToken(lex, Token_Class.StringConstant, startLine);
                    }
                    continue;
                }

                // Identifier or reserved word
                if (char.IsLetter(ch))
                {
                    string lex = "";
                    while (i < source.Length && char.IsLetterOrDigit(source[i]))
                    { lex += source[i]; i++; }

                    if (ReservedWords.TryGetValue(lex, out Token_Class tc))
                        AddToken(lex, tc, line);
                    else
                        AddToken(lex, Token_Class.Identifier, line);
                    continue;
                }

                // Number
                if (char.IsDigit(ch))
                {
                    string lex = "";
                    int dots = 0;
                    while (i < source.Length && (char.IsDigit(source[i]) || source[i] == '.'))
                    {
                        if (source[i] == '.') dots++;
                        lex += source[i]; i++;
                    }
                    if (dots > 1)
                        AddError(line, lex, "Invalid number — multiple decimal points");
                    else
                        AddToken(lex, Token_Class.Constant, line);
                    continue;
                }

                // 2-char operators
                string two = "" + ch + Peek(source, i + 1);
                if (Operators.TryGetValue(two, out Token_Class tc2))
                { AddToken(two, tc2, line); i += 2; continue; }

                // 1-char operators
                string one = ch.ToString();
                if (Operators.TryGetValue(one, out Token_Class tc1))
                { AddToken(one, tc1, line); i++; continue; }

                // Unrecognized
                AddError(line, ch.ToString(), "Unrecognized character");
                i++;
            }
        }

        private void AddToken(string lex, Token_Class tc, int line)
        {
            Tokens.Add(new Token { lex = lex, token_type = tc, line = line });
        }

        private void AddError(int line, string lex, string msg)
        {
            Errors.Add(new ScanError { line = line, lex = lex, message = msg });
        }

        private char Peek(string s, int idx) =>
            idx < s.Length ? s[idx] : '\0';
    }
}
//////////
