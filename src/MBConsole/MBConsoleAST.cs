//
// 	MBConsoleAST.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by jacobmilligan on 30/10/2016.
// 	Copyright  All rights reserved
//
using System;
using System.Text.RegularExpressions;

namespace MB2D
{
  /// <summary>
  /// Class all AST nodes inherit from
  /// </summary>
  public abstract class MBConsoleASTNode
  {
    /// <summary>
    /// Executes specific logic on the console
    /// </summary>
    /// <param name="console">Game console.</param>
    public abstract void Handle(MBConsole console);
  }

  /// <summary>
  /// The entry point for command execution with a single child.
  /// </summary>
  public class RootASTNode : MBConsoleASTNode
  {
    /// <summary>
    /// The command to execute
    /// </summary>
    private MBConsoleASTNode _child;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.RootASTNode"/> class.
    /// </summary>
    /// <param name="child">Command AST node to handle.</param>
    public RootASTNode(MBConsoleASTNode child)
    {
      _child = child;
    }

    /// <summary>
    /// Calls the child commands handle method
    /// </summary>
    /// <param name="console">Console to handle.</param>
    public override void Handle(MBConsole console)
    {
      _child.Handle(console);
    }
  }

  /// <summary>
  /// AST node representing the entry point for a 'set' command
  /// with an identifier and a value child
  /// </summary>
  public class SetASTNode : MBConsoleASTNode
  {
    /// <summary>
    /// The variables identifier
    /// </summary>
    private string _ident;
    /// <summary>
    /// The variables value
    /// </summary>
    private VariableASTNode _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.SetASTNode"/> class.
    /// </summary>
    /// <param name="ident">Identifier of the variable.</param>
    /// <param name="val">Value to assign.</param>
    public SetASTNode(string ident, VariableASTNode val)
    {
      _ident = ident;
      _value = val;
    }

    /// <summary>
    /// Handles setting the variable using the consoles Vars property.
    /// Checks if the identifier only starts with an alpha or underscore
    /// and handles any type checking or parse errors.
    /// </summary>
    /// <param name="console">Console to handle.</param>
    public override void Handle(MBConsole console)
    {
      // Get the type-specific value from object
      var typedVal = Convert.ChangeType(_value.Value, _value.Type);

      // Check if identifier exists
      if ( _ident.Length <= 0 ) {
        console.Write("Parse error: Identifier expected.");
        return;
      }

      // Check if value is valid
      if ( typedVal.GetType() == typeof(object) ) {
        console.Write("Parse error: Value expected.");
        return;
      }

      // Check if identifier is valid
      if ( !Regex.Match(_ident, "[a-zA-Z_][a-zA-Z0-9_]*").Success ) {
        console.Write(
          "Parse error: Identifier must start with an" +
          " alpha or underscore character."
        );
        return;
      }

      // Check if the console already has the variable and assign
      // the new value if so
      if ( !console.Vars.ContainsKey(_ident) ) {
        console.AddVar(_ident, typedVal);
        return;
      }

      // Otherwise create a new variable
      console.Vars[_ident] = typedVal;
    }
  }

  /// <summary>
  /// Represents a variable with a type and a value
  /// </summary>
  public class VariableASTNode
  {
    /// <summary>
    /// The variables value
    /// </summary>
    private object _value;

    /// <summary>
    /// The type of the variable
    /// </summary>
    private Type _type;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.VariableASTNode"/> class.
    /// </summary>
    /// <param name="type">Type of the variable.</param>
    /// <param name="value">Value to assign.</param>
    public VariableASTNode(Type type, object value)
    {
      _value = value;
      _type = type;
    }

    /// <summary>
    /// Gets the variables type info
    /// </summary>
    /// <value>The type.</value>
    public Type Type
    {
      get { return _type; }
    }

    /// <summary>
    /// Gets the value to assign to the variable
    /// </summary>
    /// <value>The value.</value>
    public object Value
    {
      get { return _value; }
    }
  }

  /// <summary>
  /// AST node entry point for executing a run command
  /// </summary>
  public class RunASTNode : MBConsoleASTNode
  {
    /// <summary>
    /// Identifier of the function to run
    /// </summary>
    private string _ident;

    /// <summary>
    /// The arguments to pass to the function
    /// </summary>
    private string[] _args;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.RunASTNode"/> class.
    /// </summary>
    /// <param name="ident">Identifier of the function.</param>
    /// <param name="args">Arguments to pass to the function.</param>
    public RunASTNode(string ident, params string[] args)
    {
      _ident = ident;
      _args = args;
    }

    /// <summary>
    /// Checks for valid identifier and executes the given function
    /// from the console if correctly defined.
    /// </summary>
    /// <param name="console">Console to handle.</param>
    public override void Handle(MBConsole console)
    {
      // Check for valid identifier
      if ( _ident.Length <= 0 ) {
        console.Write("Parse error: Identifier expected.");
        return;
      }

      // Check if function exists
      if ( console.Funcs.ContainsKey(_ident) ) {
        console.Funcs[_ident].Invoke(_args);
      } else {
        console.Write(
          "Parse error: Unknown function: '{0}'",
          _ident
        );
      }
    }
  }

  /// <summary>
  /// Prints a variable to the console.
  /// </summary>
  public class PrintASTNode : MBConsoleASTNode
  {
    /// <summary>
    /// Name of the variable to print
    /// </summary>
    private string _ident;

    /// <summary>
    /// The variables token type
    /// </summary>
    private Token _tokenType;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.PrintASTNode"/> class.
    /// </summary>
    /// <param name="var">
    /// The variables identifier or, if not found, 
    /// the print statements argument.
    /// </param>
    /// <param name="type">The variables token type</param>
    public PrintASTNode(string var, Token type)
    {
      _ident = var;
      _tokenType = type;
    }

    /// <summary>
    /// Prints the variable to the console. If it's not a previously
    /// assigned variable, it will print the variable name itself as
    /// if it's an immediate value.
    /// </summary>
    /// <param name="console">Console to print to.</param>
    public override void Handle(MBConsole console)
    {
      // Check for valid identifier
      if ( _ident == string.Empty ) {
        console.Write("Parse error: Identifier expected.");
        return;
      }

      // Check if the variable exists already
      if ( console.Vars.ContainsKey(_ident) ) {
        var printFmt = console.Vars[_ident].ToString();
        // Format string between quotes for printing
        if ( console.Vars[_ident] is string ) {
          printFmt = "\'" + printFmt + "\'";
        }
        console.Write(printFmt);
        return;
      }

      double testDouble;
      bool testBool;

      // Checks immediate values. If not a valid string, double, or
      // bool returns parse error, otherwise prints the variable name
      // as if it was an immediate value.
      if ( _tokenType == Token.String ) {

        console.Write("'{0}'", _ident);

      } else if ( double.TryParse(_ident, out testDouble)
            || bool.TryParse(_ident, out testBool) ) {

        console.Write(_ident);

      } else {
        console.Write(
          "Parse error: Unknown variable: '{0}'", _ident
        );
      }

    }
  }

  /// <summary>
  /// Handles quitting the game
  /// </summary>
  public class QuitASTNode : MBConsoleASTNode
  {
    /// <summary>
    /// Quits the game
    /// </summary>
    /// <param name="console">Console to handle.</param>
    public override void Handle(MBConsole console)
    {
      MBGame.ForceQuit = true;
    }
  }
}
