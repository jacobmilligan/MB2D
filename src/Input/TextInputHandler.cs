//
// 	TextInputHandler.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by jacobmilligan on 30/10/2016.
// 	Copyright  All rights reserved
//
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Input;

namespace MB2D.IO
{
	public class TextInputHandler
	{

		public string EscapeString(char c)
		{
			if ( char.IsControl(c) )
				return "";

			if ( !IsSpecialChar(c) )
				return c.ToString();

			switch ( c )
			{
				case ' ': return "Space";
				case '\t': return "Tab";
				case '\r': return "Enter";
				case '\b': return "Backspace";
				default:
					return "None";
			}

		}

		public bool IsSpecialChar(char c)
		{
			return (c < 33 || c > 126);
		}

		/// <summary>
		/// Translates the last typed key using the current keyboard
		/// state into the appropriate char
		/// </summary>
		/// <param name="keys">Keys from the latest keyboard state.</param>
		/// <param name="lastKey">The last key typed.</param>
#if TESTING
    public char Translate(Keys[] keys, Keys lastKey)
#else
		private char Translate(Keys[] keys, Keys lastKey)
#endif
		{
			var numKeys = keys.Length;
			var shiftPressed = keys.Contains(Keys.LeftShift)
								   || keys.Contains(Keys.RightShift);

			var ascii = (char)lastKey;

			if ( !shiftPressed && (ascii > 64 && ascii < 91) )
			{
				if ( Keyboard.GetState().CapsLock )
				{
					ascii = GetNonLower(lastKey, true);
				}
				else
				{
					ascii = GetLower(lastKey);
				}

				return ascii;
			}

			return GetNonLower(lastKey, shiftPressed);
		}

		/// <summary>
		/// Translates any alpha, backspace or space keys without their
		/// shift-modified state
		/// </summary>
		/// <returns>The char of the last key typed.</returns>
		/// <param name="lastKey">Last key typed.</param>
		private char GetLower(Keys lastKey)
		{
			return (char)(lastKey + 32);
		}

		/// <summary>
		/// Translates any non-alpha characters into their shift-modified
		/// state
		/// </summary>
		/// <returns>The the modified character.</returns>
		/// <param name="lastKey">Last key typed.</param>
		private char GetNonLower(Keys lastKey, bool shift)
		{
			switch ( lastKey )
			{
				case Keys.D0: return (shift ? ')' : '0');
				case Keys.D1: return (shift ? '!' : '1');
				case Keys.D2: return (shift ? '@' : '2');
				case Keys.D3: return (shift ? '#' : '3');
				case Keys.D4: return (shift ? '$' : '4');
				case Keys.D5: return (shift ? '%' : '5');
				case Keys.D6: return (shift ? '^' : '6');
				case Keys.D7: return (shift ? '&' : '7');
				case Keys.D8: return (shift ? '*' : '8');
				case Keys.D9: return (shift ? '(' : '9');
				case Keys.OemMinus: return (shift ? '_' : '-');
				case Keys.OemPlus: return (shift ? '+' : '=');
				case Keys.OemOpenBrackets: return (shift ? '{' : '[');
				case Keys.OemCloseBrackets: return (shift ? '}' : ']');
				case Keys.OemPipe: return (shift ? '|' : '\\');
				case Keys.OemSemicolon: return (shift ? ':' : ';');
				case Keys.OemQuotes: return (shift ? '\"' : '\'');
				case Keys.OemComma: return (shift ? '<' : ',');
				case Keys.OemPeriod: return (shift ? '>' : '.');
				case Keys.OemQuestion: return (shift ? '?' : '/');
				case Keys.OemTilde: return (shift ? '~' : '`');
			}

			return (char)lastKey;
		}

		/// <summary>
		/// Returns the last character entered by the user. Translates the keycode
		/// to an ASCII character taking into account space, backspace, and special
		/// character modifiers.
		/// </summary>
		/// <returns>The char.</returns>
		public char LastChar
		{
			get
			{
				var lastChar = Translate(
				  IOUtil.KeyboardState,
				  IOUtil.LastKeyTyped
				);

				if ( lastChar > 127 )
				{
					lastChar = '\0';
				}

				return lastChar;
			}
		}
	}
}
