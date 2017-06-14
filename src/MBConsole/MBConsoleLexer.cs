//
// 	MBConsoleL.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by jacobmilligan on 30/10/2016.
// 	Copyright  All rights reserved
//

using System.Collections.Generic;

namespace MB2D
{
  /// <summary>
  /// Category of tokens created by the lexer
  /// </summary>
  public enum Token
  {
    /// <summary>
    /// A value or an identifier
    /// </summary>
    Unknown,
    /// <summary>
    /// A string sequence
    /// </summary>
    String,
    /// <summary>
    /// A set command statement
    /// </summary>
    Set,
    /// <summary>
    /// A run command statement
    /// </summary>
    Run,
    /// <summary>
    /// A print command statement
    /// </summary>
    Print,
    /// <summary>
    /// A quit command statement
    /// </summary>
    Quit
  }

  /// <summary>
  /// Breaks a string into a series of tokens to
  /// use for parsing the debug consoles command language
  /// </summary>
  public class MBConsoleLexer
  {
    /// <summary>
    /// The token representation of the string
    /// </summary>
    private List<Token> _tokens;
    /// <summary>
    /// The command broken up on whitespace and
    /// quotations represented as raw strings
    /// </summary>
    private List<string> _rawTokens;
    /// <summary>
    /// The current token being analyzed
    /// </summary>
    private int _currTok;
    /// <summary>
    /// The index of the current character being analyzed
    /// in the un-tokenized command string
    /// </summary>
    private int _currCharPos;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.MBConsoleLexer"/> class.
    /// </summary>
    public MBConsoleLexer()
    {
      _currTok = 0;
      _rawTokens = new List<string>();
      _tokens = new List<Token>();
    }

    /// <summary>
    /// Lexes the command string breaking it up into token
    /// representation and a second raw string array for retrieving
    /// values.
    /// </summary>
    /// <param name="command">Command string to scan.</param>
    public void Lex(string command)
    {
      _currTok = 0;
      _rawTokens.Clear();
      _tokens.Clear();

      _currCharPos = 0;
      var currStr = string.Empty;

      while ( _currCharPos < command.Length ) {

        var currChar = command[_currCharPos]; // get the next char

        // Check for any split character and handle tokenizing
        // otherwise just append the char to the string being scanned
        switch ( currChar ) {
          case ' ':
            _rawTokens.Add(currStr);
            _tokens.Add(GetToken(currStr));
            currStr = string.Empty;
            break;
          case '\'':
            _rawTokens.Add(GetString(command));
            _tokens.Add(Token.String);
            currStr = string.Empty;
            break;
          default:
            currStr += currChar;
            break;
        }
        _currCharPos++;
      }

      // Add the last string if it's not an empty one aka end of line
      if ( currStr != string.Empty ) {
        _rawTokens.Add(currStr);
        _tokens.Add(GetToken(currStr));
      }

      _currTok = 0;
      _currCharPos = 0;
    }

    /// <summary>
    /// Scans a quoted sequence of characters between two
    /// single quotes and transforms it into a string token.
    /// </summary>
    /// <returns>The unquoted string.</returns>
    /// <param name="command">Command string being scanned.</param>
    private string GetString(string command)
    {
      string result = string.Empty;
      var currChar = '\0';
      _currCharPos++;

      while ( _currCharPos < command.Length && command[_currCharPos] != '\'' ) {
        currChar = command[_currCharPos];
        result += currChar;
        _currCharPos++;
      }

      return result;
    }

    /// <summary>
    /// Gets the token representation of a string
    /// </summary>
    /// <returns>The token.</returns>
    /// <param name="nextToken">Next string to transform into a token.</param>
    private Token GetToken(string nextToken)
    {
      // Everything not an explicit keyword is just an unknown token
      switch ( nextToken ) {
        case "set": return Token.Set;
        case "run": return Token.Run;
        case "print": return Token.Print;
        case "quit": return Token.Quit;
        default:
          return Token.Unknown;
      }
    }

    /// <summary>
    /// Gets an untokenized representation of a string at a
    /// specific index
    /// </summary>
    /// <returns>The string representation.</returns>
    /// <param name="index">Index.</param>
    public string RawToken(int index)
    {
      // Check if string exists
      if ( index > _rawTokens.Count - 1 )
        return string.Empty;

      return _rawTokens[index];
    }

    /// <summary>
    /// Gets the next token in the tokenized representation of
    /// the command
    /// </summary>
    /// <value>The next token.</value>
    public Token NextToken()
    {
      if ( _currTok > _tokens.Count - 1 )
        return Token.Unknown;

      var tok = _tokens[_currTok];
      _currTok++;
      return tok;
    }

    /// <summary>
    /// Gets the next token index in the lexer
    /// </summary>
    /// <value>The next position.</value>
    public int NextPos
    {
      get { return _currTok + 1; }
    }

    /// <summary>
    /// Gets the current token index in the lexer
    /// </summary>
    /// <value>The current position.</value>
    public int CurrentPos
    {
      get { return _currTok; }
    }

    /// <summary>
    /// Gets the number of tokens scanned.
    /// </summary>
    /// <value>The number of tokens.</value>
    public int NumTokens
    {
      get { return _tokens.Count; }
    }
  }
}
