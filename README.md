TinyScanner Compiler

A simple compiler front-end built in C# with Windows Forms, developed as a university project.
Features

Phase 1 -
Scanner (Lexical Analyzer): Tokenizes source code and detects lexical errors

Phase 2 -
Parser (Syntax Analyzer): Builds a Parse Tree from the token stream

Supported Tokens


Keywords: int, float, string, read, write, repeat, until, if, elseif, else, then, return, endl, main
Operators: :=, +, -, *, /, =, <, >, <>, &&, ||
Identifiers, Constants, String Literals


How to Use



  - Open the project in Visual Studio

  - Build and Run (F5)

  - Type or paste source code in the Source Code box

  - Click Scan to see the Token Table

  - Click Parse to see the Parse Tree



Project Structure

 TinyScanner/
 
 ├── Scanner.cs         # Lexical Analyzer
 
 ├── Parser.cs         # Syntax Analyzer
 
 ├── ScannerForm.cs    # GUI
 
 └── Program.cs        # Entry Point
 

Sample Code


    int main()

    {

    int x;
  
    read x;
  
    if x > 0 then
  
    write x; 
    
    end
    
    return 0;
    
    }


Technologies


C# .NET

Windows Forms



Developer


shahdaii1
