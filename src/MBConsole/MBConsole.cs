//
// MBConsole.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MB2D.IO;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace MB2D
{
	/// <summary>
	/// MB2D Engine debug console class. Executes attached methods and changes attached variables.
	/// </summary>
	public class MBConsole
	{
		/// <summary>
		/// The caret to display next to the text.
		/// </summary>
		private const string _CARET = "> ";
		/// <summary>
		/// The animation speed for display/hide.
		/// </summary>
		private const int _ANIM_SPEED = 55;
		/// <summary>
		/// The text offset from the left of the screen.
		/// </summary>
		private Vector2 _textOffset;

		/// <summary>
		/// Processes all text input from the keyboard.
		/// </summary>
		private TextInputHandler _textHandler;

		/// <summary>
		/// Parses and executes all command input
		/// </summary>
		private MBConsoleParser _parser;

		/// <summary>
		/// Points to the currently highlighted command.
		/// </summary>
		private int _cmdPtr;
		/// <summary>
		/// Maximum width of the text to display in the console
		/// </summary>
		private int _maxTextWidth;
		/// <summary>
		/// The current line being inputted by the user
		/// </summary>
		private string _currentLine;
		/// <summary>
		/// Wether the console is in an animation state or not (when showing/hiding)
		/// </summary>
		private bool _animating;

		/// <summary>
		/// The color of the background.
		/// </summary>
		private Color _bgColor;
		/// <summary>
		/// The color of the text.
		/// </summary>
		private Color _txtColor;
		/// <summary>
		/// The surface to render text to and show background on.
		/// </summary>
		private Rectangle _surface;

		/// <summary>
		/// The font to render the consoles text with
		/// </summary>
		private SpriteFont _font;

		/// <summary>
		/// The history of all inputted text.
		/// </summary>
		private List<string> _cmdHistory;
		/// <summary>
		/// The history of all displayed text.
		/// </summary>
		private List<string> _ioHistory;

		/// <summary>
		/// Functions attached to the console to be executed in game via run
		/// </summary>
		private Dictionary<string, Action<string[]>> _funcs;
		/// <summary>
		/// Variables attached to the console that can be altered in game
		/// </summary>
		private Dictionary<string, object> _gameVars;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MidnightBlue.MBConsole"/> class.
		/// </summary>
		/// <param name="bgColor">Background color for rendering the console</param>
		/// <param name="txtColor">Text color</param>
		public MBConsole(Color bgColor, Color txtColor, SpriteFont font)
		{
			_bgColor = bgColor;
			_txtColor = txtColor;
			_font = font;
			_textHandler = new TextInputHandler();
			_parser = new MBConsoleParser();

			_animating = false;

			_maxTextWidth = (int)_surface.Width / 2;

			_currentLine = string.Empty;
			_cmdHistory = new List<string>();
			_ioHistory = new List<string>();
			_cmdPtr = 0;
			_funcs = new Dictionary<string, Action<string[]>>();
			_gameVars = new Dictionary<string, object>();
		}

		/// <summary>
		/// Initializes a graphics target to render the console to
		/// </summary>
		/// <param name="graphics">GraphicsDevice to use for rendering</param>
		public void InitWindow(GraphicsDevice graphics)
		{
			_surface = MBGame.Graphics.Viewport.Bounds;
			_surface.Height = 0;
		}

		/// <summary>
		/// Adds a new function to the console for executing in game
		/// </summary>
		/// <param name="name">Name to use when calling the function in game</param>
		/// <param name="func">Function to attach</param>
		public void AddFunc(string name, Action<string[]> func)
		{
			_funcs.Add(name, func);
		}

		/// <summary>
		/// Adds a new variable to the console for altering in game
		/// </summary>
		/// <param name="name">Name to use when altering the variable in game</param>
		/// <param name="variable">Variable to attach</param>
		public void AddVar(string name, object variable)
		{
			_gameVars.Add(name, variable);
		}

		/// <summary>
		/// Updates any animation until no longer in an animation state. Otherwise calls ProcessInput()
		/// </summary>
		public void Update()
		{
			// Animate down. Not displayed so we need to display it
			if ( _animating && !Display )
			{
				_surface.Height += _ANIM_SPEED;
				CheckAnimationState();
				return;
			}

			if ( _animating && Display )
			{
				// Animate up
				_surface.Height -= _ANIM_SPEED;
				CheckAnimationState();
				return;
			}

			// No animation state, so process input (won't do anything if console is hidden)
			if ( Display )
			{
				ProcessInput();
			}
		}

		/// <summary>
		/// Draws the console and associated text to the attached window
		/// </summary>
		public void Draw(SpriteBatch spriteBatch)
		{
			// Don't draw if hidden
			if ( _surface.Height <= 0 )
			{
				return;
			}

			_textOffset = new Vector2(_surface.Left + 25, _surface.Bottom - 25);

			spriteBatch.FillRectangle(_surface, _bgColor);
			spriteBatch.DrawString(_font, _CARET + _currentLine, _textOffset, _txtColor);

			int i = _ioHistory.Count - 1;
			float y = _textOffset.Y;

			// print io history until top of screen or no more history
			while ( y > _surface.Top && i >= 0 )
			{
				y -= _font.MeasureString(_ioHistory[i]).Y + (_font.LineSpacing / 2);
				spriteBatch.DrawString(_font, _ioHistory[i], new Vector2(_textOffset.X, y), _txtColor);
				i--;
			}

		}

		/// <summary>
		/// Writes a line to the console to display
		/// </summary>
		/// <param name="line">Line to write</param>
		public void Write(string line)
		{
			var width = 0.0f;
			if ( _font != null )
			{
				width = _font.MeasureString(line).X;
			}
			// Split input when wider than the max width
			if ( width > _maxTextWidth )
			{
				var splitPos = (int)(_maxTextWidth / (width / line.Length));
				_ioHistory.Add(line.Substring(0, splitPos));
				_ioHistory.Add(line.Substring(splitPos));
			}
			else
			{
				// Add to history and print
				_ioHistory.Add(line);
			}
		}

		/// <summary>
		/// Writes a line to the console to display with specified string format information.
		/// </summary>
		/// <param name="line">Line to write</param>
		/// <param name="args">Arguments to add to string format</param>
		public void Write(string line, params string[] args)
		{
			var lineToPrint = line;

			if ( args.Length > 0 )
			{
				lineToPrint = string.Format(line, args);
			}

			Write(lineToPrint);
		}

		/// <summary>
		/// Writes a line to the console to display with specified string format information.
		/// </summary>
		/// <param name="line">Line to write</param>
		/// <param name="args">Arguments to format into a string</param>
		public void Write(string line, params object[] args)
		{
			Write(line, Array.ConvertAll(args, (input) => input.ToString()));
		}

		/// <summary>
		/// Writes a debug line to the console with the specified string format information
		/// </summary>
		/// <param name="line">Line to write</param>
		/// <param name="args">Arguments to format into string</param>
		[Conditional("DEBUG")]
		public void Debug(string line, params object[] args)
		{
			Write(line, args);
		}

		/// <summary>
		/// Writes a debug line to the console with the specified string format information
		/// </summary>
		/// <param name="line">Line to write</param>
		/// <param name="args">Arguments to format into string</param>
		[Conditional("DEBUG")]
		public void Debug(int line, params object[] args)
		{
			Write(line.ToString(), args);
		}

		/// <summary>
		/// Writes a debug line to the console with the specified string format information
		/// </summary>
		/// <param name="line">Line to write</param>
		/// <param name="args">Arguments to format into string</param>
		[Conditional("DEBUG")]
		public void Debug(uint line, params object[] args)
		{
			Write(line.ToString(), args);
		}

		/// <summary>
		/// Writes a debug line to the console with the specified string format information
		/// </summary>
		/// <param name="line">Line to write</param>
		/// <param name="args">Arguments to format into string</param>
		[Conditional("DEBUG")]
		public void Debug(float line, params object[] args)
		{
			Write(line.ToString(), args);
		}

		/// <summary>
		/// Checks the state of the animation to see if the
		/// console should be displayed or hidden
		/// </summary>
		private void CheckAnimationState()
		{
			if ( _surface.Height >= MBGame.Graphics.Viewport.Height / 3 )
			{
				_animating = false;
				Display = true; // show
			}
			if ( _surface.Height <= 0 )
			{
				_animating = false;
				Display = false; // hide
			}
		}

		/// <summary>
		/// Handles all input in the console and displays key inputted characters
		/// </summary>
		private void ProcessInput()
		{
			var lastChar = _textHandler.LastChar;

			// Handle completions/hints
			if ( lastChar == '\t' )
			{
				HandleCompletions();
			}
			else if ( lastChar == '\r' )
			{
				PushCommand(); // try to execute command
			}
			else if ( IOUtil.KeyTyped(Keys.Up) || IOUtil.KeyTyped(Keys.Down) )
			{
				NavigateHistory(); // navigate iohistory
			}
			else if ( IOUtil.KeyboardState.Length > 0 && lastChar != '\0' && !(char.IsControl(lastChar) && lastChar != 8) )
			{
				UpdateCurrentLine(lastChar); // display text
			}
		}

		/// <summary>
		/// Handles printing any tab completions to the console.
		/// </summary>
		private void HandleCompletions()
		{
			// Show console functions
			if ( _currentLine == "run" || _currentLine == "run " )
			{
				var funcStr = "<run> ";
				foreach ( var key in _funcs.Keys )
				{
					funcStr += "[" + key + "] ";
				}
				Write(funcStr);
			}

			// Show game variables
			if ( _currentLine == "set" || _currentLine == "set " )
			{
				var varStr = "<set> ";
				foreach ( var key in _gameVars.Keys )
				{
					varStr += "[" + key + "] ";
				}
				Write(varStr);
			}
		}

		/// <summary>
		/// Attempts to parse and execute the current line, adds to io history regardless
		/// </summary>
		private void PushCommand()
		{
			_cmdHistory.Add(_currentLine); // add to history
			Write(_CARET + _currentLine);

			// Parse the command
			var root = _parser.Parse(_currentLine);

			if ( root != null )
			{
				// Traverse the AST and execute
				root.Handle(this);
			}
			else
			{
				Write("Parse error: Unkown command");
			}

			_currentLine = string.Empty;
			_cmdPtr = _cmdHistory.Count; // reset pointer
		}

		/// <summary>
		/// Navigates the command history.
		/// </summary>
		private void NavigateHistory()
		{
			if ( IOUtil.KeyTyped(Keys.Up) )
				_cmdPtr--;
			if ( IOUtil.KeyTyped(Keys.Down) )
				_cmdPtr++;

			if ( _cmdPtr >= _cmdHistory.Count )
			{
				_cmdPtr = _cmdHistory.Count;

				// Show empty string for last entered command + 1
				if ( _cmdHistory.Count > 0 )
					_currentLine = string.Empty;

				return;
			}

			// Navigate to earliest command. Stop there in current line
			if ( _cmdPtr < 0 )
				_cmdPtr = 0;
			if ( _cmdHistory.Count > 0 )
				_currentLine = _cmdHistory[_cmdPtr];
		}

		/// <summary>
		/// Updates the text input in the current line and displays it
		/// </summary>
		private void UpdateCurrentLine(char lastChar)
		{
			if ( IOUtil.LastKeyTyped == Keys.Left || IOUtil.LastKeyTyped == Keys.Right )
				return;

			// Get ctrl+c to erase line
			if ( IOUtil.KeyDown(Keys.LeftControl) || IOUtil.KeyDown(Keys.RightControl) )
			{
				if ( lastChar != 'c' )
				{
					_currentLine = string.Empty;
				}
			}
			else if ( lastChar == '\b' && _currentLine != string.Empty )
			{
				// Backspace
				_currentLine = _currentLine.Remove(_currentLine.Length - 1);
			}
			else if ( lastChar != '\b' )
			{
				_currentLine += lastChar.ToString();
			}
		}

		/// <summary>
		/// Toggles the display/hide state of the console
		/// </summary>
		public void Toggle()
		{
			_animating = true;
		}

		/// <summary>
		/// Determines if the console is currently shown or hidden
		/// </summary>
		/// <value><c>true</c> if shown; otherwise, <c>false</c>.</value>
		public bool Display { get; set; }

		/// <summary>
		/// Gets the background color of the console
		/// </summary>
		/// <value>The background color.</value>
		public Color BGColor
		{
			get { return _bgColor; }
		}

		/// <summary>
		/// Gets the color of the console text.
		/// </summary>
		/// <value>The color of the text.</value>
		public Color TextColor
		{
			get { return _txtColor; }
		}

		/// <summary>
		/// Gets the consoles game variables.
		/// </summary>
		/// <value>The variables.</value>
		public Dictionary<string, object> Vars
		{
			get { return _gameVars; }
		}

		/// <summary>
		/// Gets the consoles game functions.
		/// </summary>
		/// <value>The functions.</value>
		public Dictionary<string, Action<string[]>> Funcs
		{
			get { return _funcs; }
		}

		public string LastOutput
		{
			get { return _ioHistory[_ioHistory.Count - 1]; }
		}
	}
}

