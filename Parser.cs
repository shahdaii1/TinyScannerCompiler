using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TinyScanner
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }

    public class Parser
    {
        public List<string> ParseErrors = new List<string>();
       
        int InputPointer = 0;
        List<Token> TokenStream = null!;
        public Node root = null!;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
   
        // Program → Function_List Main_Functionnnnn
       
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Function_List());
            program.Children.Add(Main_Function());
            //MessageBox.Show("Success");
            return program;
        }     
        // Function_List → Function Function_List |E
       
        Node Function_List()
        {
            Node functionList = new Node("Function_List");
            while (IsDatatype(Current()) && !IsMainAhead())
            {
                functionList.Children.Add(Function());
            }
            return functionList;
        }     
        // Function → Function_Declaration Function_Body
        
        Node Function()
        {
            Node function = new Node("Function");
            function.Children.Add(Function_Declaration());
            function.Children.Add(Function_Body());
            return function;
        }
    
        // Function_Declaration → Datatype Identifier ( Parameters )
        
        Node Function_Declaration()
        {
            Node funcDecl = new Node("Function_Declaration");
            funcDecl.Children.Add(Datatype());
            funcDecl.Children.Add(match(Token_Class.Identifier));
            funcDecl.Children.Add(match(Token_Class.LParenthesis));
            funcDecl.Children.Add(Parameters());
            funcDecl.Children.Add(match(Token_Class.RParenthesis));
            return funcDecl;
        }    
        // Parameters → Parameter_List | E
       
        Node Parameters()
        {
            Node parameters = new Node("Parameters");
            if (IsDatatype(Current()))
            {
                parameters.Children.Add(Parameter());
                parameters.Children.Add(More_Params());
            }
            return parameters;
        }

        Node More_Params()
        {
            Node moreParams = new Node("More_Params");
            if (Current() == Token_Class.Comma)
            {
                moreParams.Children.Add(match(Token_Class.Comma));
                moreParams.Children.Add(Parameter());
                moreParams.Children.Add(More_Params());
            }
            return moreParams;
        }     
        // Parameter → Datatype Identifier
        
        Node Parameter()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Token_Class.Identifier));
            return parameter;
        }
     
        // Main_Function → Datatype main ( ) Function_Body
       
        Node Main_Function()
        {
            Node mainFunc = new Node("Main_Function");
            mainFunc.Children.Add(Datatype());
            mainFunc.Children.Add(match(Token_Class.Main));
            mainFunc.Children.Add(match(Token_Class.LParenthesis));
            mainFunc.Children.Add(match(Token_Class.RParenthesis));
            mainFunc.Children.Add(Function_Body());
            return mainFunc;
        }
    
        // Function_Body →  Statements Return_Statement 
       
        Node Function_Body()
        {
            Node funcBody = new Node("Function_Body");
            funcBody.Children.Add(match(Token_Class.LBrace));
            funcBody.Children.Add(Statements());
            funcBody.Children.Add(Return_Statement());
            funcBody.Children.Add(match(Token_Class.RBrace));
            return funcBody;
        }
     
        // Statements → Statement Statements | E 
       
        Node Statements()
        {
            Node statements = new Node("Statements");
            while (IsStatementFirst(Current()))
            {
                statements.Children.Add(Statement());
            }
            return statements;
        }
     
        // Statement → Declaration_Statement,,,
        
        Node Statement()
        {
            Node statement = new Node("Statement");
            Token_Class cur = Current();

            if (IsDatatype(cur))
            {
                statement.Children.Add(Declaration_Statement());
            }
            else if (cur == Token_Class.Identifier)
            {
                statement.Children.Add(Assignment_Statement());
                statement.Children.Add(match(Token_Class.Semicolon));
            }
            else if (cur == Token_Class.Write)
            {
                statement.Children.Add(Write_Statement());
            }
            else if (cur == Token_Class.Read)
            {
                statement.Children.Add(Read_Statement());
            }
            else if (cur == Token_Class.If)
            {
                statement.Children.Add(If_Statement());
            }
            else if (cur == Token_Class.Repeat)
            {
                statement.Children.Add(Repeat_Statement());
            }
            else
            {
                ParseErrors.Add("Parsing Error: Unexpected token '"
                    + TokenStream[InputPointer].lex + "' in Statement\r\n");
                InputPointer++;
            }
            return statement;
        }
    
        // Declaration_Statement → Datatype Decl_List ;
        // Decl_List → Decl More_Decl
   
        Node Declaration_Statement()
        {
            Node declStmt = new Node("Declaration_Statement");
            declStmt.Children.Add(Datatype());
            declStmt.Children.Add(Decl_List());
            declStmt.Children.Add(match(Token_Class.Semicolon));
            return declStmt;
        }

        Node Decl_List()
        {
            Node declList = new Node("Decl_List");
            declList.Children.Add(Decl());
            declList.Children.Add(More_Decl());
            return declList;
        }

        Node More_Decl()
        {
            Node moreDecl = new Node("More_Decl");
            if (Current() == Token_Class.Comma)
            {
                moreDecl.Children.Add(match(Token_Class.Comma));
                moreDecl.Children.Add(Decl());
                moreDecl.Children.Add(More_Decl());
            }
            return moreDecl;
        }

        Node Decl()
        {
            Node decl = new Node("Decl");
            decl.Children.Add(match(Token_Class.Identifier));
            decl.Children.Add(Assign_Opt());
            return decl;
        }

        Node Assign_Opt()
        {
            Node assignOpt = new Node("Assign_Opt");
            if (Current() == Token_Class.AssignOp)
            {
                assignOpt.Children.Add(match(Token_Class.AssignOp));
                assignOpt.Children.Add(Expression());
            }
            return assignOpt;
        }
      
        // Assignment_Statement → Identifier := Expression
      
        Node Assignment_Statement()
        {
            Node assignStmt = new Node("Assignment_Statement");
            assignStmt.Children.Add(match(Token_Class.Identifier));
            assignStmt.Children.Add(match(Token_Class.AssignOp));
            assignStmt.Children.Add(Expression());
            return assignStmt;
        }

        // Write_Statement → write Write_Value ;
        // Write_Value → Expression | endl
      
        Node Write_Statement()
        {
            Node writeStmt = new Node("Write_Statement");
            writeStmt.Children.Add(match(Token_Class.Write));
            writeStmt.Children.Add(Write_Value());
            writeStmt.Children.Add(match(Token_Class.Semicolon));
            return writeStmt;
        }

        Node Write_Value()
        {
            Node writeVal = new Node("Write_Value");
            if (Current() == Token_Class.Endl)
                writeVal.Children.Add(match(Token_Class.Endl));
            else
                writeVal.Children.Add(Expression());
            return writeVal;
        }
        // Read_Statement → read Identifier ;
        
        Node Read_Statement()
        {
            Node readStmt = new Node("Read_Statement");
            readStmt.Children.Add(match(Token_Class.Read));
            readStmt.Children.Add(match(Token_Class.Identifier));
            readStmt.Children.Add(match(Token_Class.Semicolon));
            return readStmt;
        }

        // Return_Statement → return Expression ;
       
        Node Return_Statement()
        {
            Node returnStmt = new Node("Return_Statement");
            returnStmt.Children.Add(match(Token_Class.Return));
            returnStmt.Children.Add(Expression());
            returnStmt.Children.Add(match(Token_Class.Semicolon));
            return returnStmt;
        }

        // If_Statement → if Condition_Statement then Statements Else_Part
  
        Node If_Statement()
        {
            Node ifStmt = new Node("If_Statement");
            ifStmt.Children.Add(match(Token_Class.If));
            ifStmt.Children.Add(Condition_Statement());
            ifStmt.Children.Add(match(Token_Class.Then));
            ifStmt.Children.Add(Statements());
            ifStmt.Children.Add(Else_Part());
            return ifStmt;
        }

        Node Else_Part()
        {
            Node elsePart = new Node("Else_Part");
            if (Current() == Token_Class.ElseIf)
            {
                elsePart.Children.Add(match(Token_Class.ElseIf));
                elsePart.Children.Add(Condition_Statement());
                elsePart.Children.Add(match(Token_Class.Then));
                elsePart.Children.Add(Statements());
                elsePart.Children.Add(Else_Part());
            }
            else if (Current() == Token_Class.Else)
            {
                elsePart.Children.Add(match(Token_Class.Else));
                elsePart.Children.Add(Statements());
                elsePart.Children.Add(match(Token_Class.End));
            }
            else
            {
                elsePart.Children.Add(match(Token_Class.End));
            }
            return elsePart;
        }

        // Repeat_Statement → repeat Statements until Condition_Statement
        
        Node Repeat_Statement()
        {
            Node repeatStmt = new Node("Repeat_Statement");
            repeatStmt.Children.Add(match(Token_Class.Repeat));
            repeatStmt.Children.Add(Statements());
            repeatStmt.Children.Add(match(Token_Class.Until));
            repeatStmt.Children.Add(Condition_Statement());
            return repeatStmt;
        }

        // Condition_Statement → Condition Condition_Tail
        // Condition_Tail → Boolean_Operator Condition Condition_Tail |E 
       
        Node Condition_Statement()
        {
            Node condStmt = new Node("Condition_Statement");
            condStmt.Children.Add(Condition());
            condStmt.Children.Add(Condition_Tail());
            return condStmt;
        }

        Node Condition_Tail()
        {
            Node condTail = new Node("Condition_Tail");
            if (Current() == Token_Class.AndOp || Current() == Token_Class.OrOp)
            {
                condTail.Children.Add(Boolean_Operator());
                condTail.Children.Add(Condition());
                condTail.Children.Add(Condition_Tail());
            }
            return condTail;
        }

        // Condition → Identifier Condition_Operator Term
       
        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Identifier));
            condition.Children.Add(Condition_Operator());
            condition.Children.Add(Term());
            return condition;
        }

        Node Condition_Operator()
        {
            Node condOp = new Node("Condition_Operator");
            Token_Class cur = Current();
            if (cur == Token_Class.LessThanOp)
                condOp.Children.Add(match(Token_Class.LessThanOp));
            else if (cur == Token_Class.GreaterThanOp)
                condOp.Children.Add(match(Token_Class.GreaterThanOp));
            else if (cur == Token_Class.EqualOp)
                condOp.Children.Add(match(Token_Class.EqualOp));
            else if (cur == Token_Class.NotEqualOp)
                condOp.Children.Add(match(Token_Class.NotEqualOp));
            else
            {
                ParseErrors.Add("Parsing Error: Expected condition operator but found '"
                    + (InputPointer < TokenStream.Count ? TokenStream[InputPointer].lex : "EOF") + "'\r\n");
            }
            return condOp;
        }

        Node Boolean_Operator()
        {
            Node boolOp = new Node("Boolean_Operator");
            if (Current() == Token_Class.AndOp)
                boolOp.Children.Add(match(Token_Class.AndOp));
            else
                boolOp.Children.Add(match(Token_Class.OrOp));
            return boolOp;
        }
        // Expression → StringConstant | Equation
        // Equation → Term Equation_Tail
    
        Node Expression()
        {
            Node expr = new Node("Expression");
            if (Current() == Token_Class.StringConstant)
                expr.Children.Add(match(Token_Class.StringConstant));
            else
                expr.Children.Add(Equation());
            return expr;
        }

        Node Equation()
        {
            Node equation = new Node("Equation");
            equation.Children.Add(Term());
            equation.Children.Add(Equation_Tail());
            return equation;
        }

        Node Equation_Tail()
        {
            Node eqTail = new Node("Equation_Tail");
            if (IsArithmeticOp(Current()))
            {
                eqTail.Children.Add(Arithmetic_Operator());
                eqTail.Children.Add(Term());
                eqTail.Children.Add(Equation_Tail());
            }
            return eqTail;
        }

        // Term → Constant | Identifier | Function_Call | ( Equation )
       
        Node Term()
        {
            Node term = new Node("Term");
            Token_Class cur = Current();

            if (cur == Token_Class.Constant)
            {
                term.Children.Add(match(Token_Class.Constant));
            }
            else if (cur == Token_Class.LParenthesis)
            {
                term.Children.Add(match(Token_Class.LParenthesis));
                term.Children.Add(Equation());
                term.Children.Add(match(Token_Class.RParenthesis));
            }
            else if (cur == Token_Class.Identifier)
            {
                if (Peek() == Token_Class.LParenthesis)
                    term.Children.Add(Function_Call());
                else
                    term.Children.Add(match(Token_Class.Identifier));
            }
            else
            {
                ParseErrors.Add("Parsing Error: Expected Term but found '"
                    + (InputPointer < TokenStream.Count ? TokenStream[InputPointer].lex : "EOF") + "'\r\n");
                InputPointer++;
            }
            return term;
        }

        // Function_Call → Identifier ( Arguments )
        Node Function_Call()
        {
            Node funcCall = new Node("Function_Call");
            funcCall.Children.Add(match(Token_Class.Identifier));
            funcCall.Children.Add(match(Token_Class.LParenthesis));
            funcCall.Children.Add(Arguments());
            funcCall.Children.Add(match(Token_Class.RParenthesis));
            return funcCall;
        }

        Node Arguments()
        {
            Node args = new Node("Arguments");
            if (IsExpressionFirst(Current()))
            {
                args.Children.Add(Expression());
                args.Children.Add(More_Args());
            }
            return args;
        }

        Node More_Args()
        {
            Node moreArgs = new Node("More_Args");
            if (Current() == Token_Class.Comma)
            {
                moreArgs.Children.Add(match(Token_Class.Comma));
                moreArgs.Children.Add(Expression());
                moreArgs.Children.Add(More_Args());
            }
            return moreArgs;
        }

        // Arithmetic_Operator → + | - | * | /
       
        Node Arithmetic_Operator()
        {
            Node arithOp = new Node("Arithmetic_Operator");
            Token_Class cur = Current();
            if (cur == Token_Class.PlusOp)
                arithOp.Children.Add(match(Token_Class.PlusOp));
            else if (cur == Token_Class.MinusOp)
                arithOp.Children.Add(match(Token_Class.MinusOp));
            else if (cur == Token_Class.MultiplyOp)
                arithOp.Children.Add(match(Token_Class.MultiplyOp));
            else
                arithOp.Children.Add(match(Token_Class.DivideOp));
            return arithOp;
        }

        // Datatype → int | float | string
        
        Node Datatype()
        {
            Node datatype = new Node("Datatype");
            Token_Class cur = Current();
            if (cur == Token_Class.Int)
                datatype.Children.Add(match(Token_Class.Int));
            else if (cur == Token_Class.Float)
                datatype.Children.Add(match(Token_Class.Float));
            else if (cur == Token_Class.String)
                datatype.Children.Add(match(Token_Class.String));
            else
            {
                ParseErrors.Add("Parsing Error: Expected Datatype but found '"
                    + (InputPointer < TokenStream.Count ? TokenStream[InputPointer].lex : "EOF") + "'\r\n");
                InputPointer++;
            }
            return datatype;
        }

        // Helper Methods
       
        Token_Class Current()
        {
            if (InputPointer < TokenStream.Count)
                return TokenStream[InputPointer].token_type;
            return Token_Class.Identifier;
        }

        Token_Class Peek()
        {
            if (InputPointer + 1 < TokenStream.Count)
                return TokenStream[InputPointer + 1].token_type;
            return Token_Class.Identifier;
        }

        bool IsDatatype(Token_Class tc) =>
            tc == Token_Class.Int || tc == Token_Class.Float || tc == Token_Class.String;

        bool IsArithmeticOp(Token_Class tc) =>
            tc == Token_Class.PlusOp || tc == Token_Class.MinusOp ||
            tc == Token_Class.MultiplyOp || tc == Token_Class.DivideOp;

        bool IsStatementFirst(Token_Class tc) =>
            IsDatatype(tc) ||
            tc == Token_Class.Identifier ||
            tc == Token_Class.Write ||
            tc == Token_Class.Read ||
            tc == Token_Class.If ||
            tc == Token_Class.Repeat;

        bool IsExpressionFirst(Token_Class tc) =>
            tc == Token_Class.StringConstant ||
            tc == Token_Class.Constant ||
            tc == Token_Class.Identifier ||
            tc == Token_Class.LParenthesis;

        bool IsMainAhead()
        {
            if (InputPointer + 1 < TokenStream.Count)
                return TokenStream[InputPointer + 1].token_type == Token_Class.Main;
            return false;
        }
 
        public Node match(Token_Class ExpectedToken)
        {
            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());
                    return newNode;
                }
                else
                {
                    ParseErrors.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                ParseErrors.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }
        public static TreeNode? PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
       
        static TreeNode? PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
