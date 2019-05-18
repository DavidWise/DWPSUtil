using StaticAbstraction;
using System;

namespace DWPowerShell.Utility.ConsoleIO
{
    public class ConsoleState
    {
        protected IConsole _console;

        public int CursorLeft { get; protected set; }
        public int CursorTop { get; protected set; }
        public int WindowTop { get; protected set; }
        public ConsoleColor ForeColor { get; protected set; }
        public ConsoleColor BackColor { get; protected set; }


        public ConsoleState(IConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console), "A console argument is required");

            this.CursorLeft = _console.CursorLeft;
            this.CursorTop = _console.CursorTop;
            this.BackColor = _console.BackgroundColor;
            this.ForeColor = _console.ForegroundColor;
            this.WindowTop = _console.WindowTop;
        }

        public void ResetState()
        {
            ResetCursor();
            ResetColors();
        }

        public void ResetCursor()
        {
            _console.CursorTop = this.CursorTop;
            _console.CursorLeft = this.CursorLeft;
        }

        public void ResetColors()
        {
            _console.BackgroundColor = this.BackColor;
            _console.ForegroundColor = this.ForeColor;
        }
    }
}