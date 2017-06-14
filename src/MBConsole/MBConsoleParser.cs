//
// 	MBConsoleParser.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by jacobmilligan on 30/10/2016.
// 	Copyright  All rights reserved
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace MB2D
{
  /// <summary>
  /// Parses command string input and executes it using
  /// a debug console.
  /// </summary>
  public class MBConsoleParser
  {
    /// <summary>
    /// Scanner for tokenizing the input string
    /// </summary>
    /// <remarks>
    /// Internally the parser uses an ad hoc lexer for scanning
    /// and builds an abstract syntax tree using a basic LL(1) recursive
    /// descent approach. The AST is then traversed with each node receiving
    /// a debug console object in its 'handle' method which executes some
    /// logic on it.
    /// </remarks>
    private MBConsoleLexer _lexer;

    /// <summary>
    /// The current token being analyzed
    /// </summary>
    private Token _currTok;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.MBConsoleParser"/> class.
    /// </summary>
    public MBConsoleParser()
    {
      _lexer = new MBConsoleLexer();
    }

    /// <summary>
    /// Processes the command string and executes to the given
    /// console.
    /// </summary>
    /// <param name="console">Console to execute on.</param>
    /// <param name="command">Command to parse.</param>
    /// <returns>The entry point for the AST</returns>
    public RootASTNode Parse(string command)
    {
      // Scan and tokenize
      _lexer.Lex(command);

      // Build the AST
      return ParseRoot();
    }

    /// <summary>
    /// Parses the entry point for a command building
    /// the root node of the AST
    /// </summary>
    /// <returns>The root node.</returns>
    private RootASTNode ParseRoot()
    {
      _currTok = _lexer.NextToken();

      // Handles all top level tokens
      switch ( _currTok ) {
        case Token.Unknown:

          return null;
        case Token.Set:

          // Get the identifier and eat that token
          var ident = ParseIdent();
          _currTok = _lexer.NextToken();

          var setNode = new SetASTNode(ident, ParseVariable());
          return new RootASTNode(setNode);
        case Token.Run:

          var runNode = new RunASTNode(ParseIdent(), ParseFuncArgs());
          return new RootASTNode(runNode);
        case Token.Print:

          // Get the string representation and the tokenized representation
          // of the variable to print
          var print = new PrintASTNode(
            _lexer.RawToken(_lexer.CurrentPos), _lexer.NextToken()
          );
          return new RootASTNode(print);
        case Token.Quit:
          return new RootASTNode(new QuitASTNode());
      }

      return null; // invalid token
    }

    /// <summary>
    /// Gets an identifiers string representation
    /// </summary>
    /// <returns>The identifier.</returns>
    private string ParseIdent()
    {
      return _lexer.RawToken(_lexer.CurrentPos);
    }

    /// <summary>
    /// Gets a list of all the arguments to use
    /// for a console function
    /// </summary>
    /// <returns>The functions arguments.</returns>
    private string[] ParseFuncArgs()
    {
      var results = new List<string>();

      // Gather all the remaining tokens to use as an
      // argument list
      var next = _lexer.NextPos;
      var arg = _lexer.RawToken(next);
      while ( arg != string.Empty && next < _lexer.NumTokens ) {
        results.Add(arg);

        next++;
        arg = _lexer.RawToken(next);
      }

      return results.ToArray();
    }

    /// <summary>
    /// Gets a variable AST node representing the type and
    /// value of the current token.
    /// </summary>
    /// <returns>The variable node.</returns>
    private VariableASTNode ParseVariable()
    {
      // Get a typed representation of the string
      // value
      var val = GetObjectFromString(
        _lexer.RawToken(_lexer.CurrentPos)
      );

      // Check if the token is outside the bounds of the
      // max number of tokens
      if ( _lexer.CurrentPos < _lexer.NumTokens - 1 )
        val = new object();

      return new VariableASTNode(val.GetType(), val);
    }

    /// <summary>
    /// Turns a string value into a typed object. For instance if
    /// the string is "9.5" a floating point object is returned
    /// with the value 9.5f.
    /// </summary>
    /// <returns>The object from string.</returns>
    /// <param name="val">Value to convert.</param>
    private object GetObjectFromString(string val)
    {
      // Check for boolean
      if ( val.ToLower() == "true" || val.ToLower() == "false" ) {
        return bool.Parse(val);
      }

      // Check for floating point value. Return string if the
      // entire value isn't purely numeric
      if ( val.Contains(".") ) {
        double doubleVal = 0.0f;

        if ( double.TryParse(val, out doubleVal) )
          return doubleVal;

        return val;
      }

      // Check for integer value
      int intVal = 0;
      if ( int.TryParse(val, out intVal) )
        return intVal;

      // Check for string token and value. Return a pure object
      // as an error state if not true
      _currTok = _lexer.NextToken();
      if ( _currTok == Token.String ) {
        return val;
      } else {
        return new object();
      }
    }

  }
}
