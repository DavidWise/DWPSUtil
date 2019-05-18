using StaticAbstraction;
using System;

namespace DWPowerShell.Utility.ConsoleIO
{
    public class ColorPair
    {
        public ConsoleColor? Background { get; set; }
        public ConsoleColor? Foreground { get; set; }

        public ConsoleColor OriginalBackground { get; set; }
        public ConsoleColor OriginalForeground { get; set; }

        private readonly IConsole _console = null;

        public ColorPair(): this(null, null, null) { }

        public ColorPair(ConsoleColor? foreColor, ConsoleColor? backColor) : this(null, foreColor, backColor) { }

        public ColorPair(IConsole console, ConsoleColor? foreColor, ConsoleColor? backColor)
        {
            _console = console?? new StAbConsole();
            Init(foreColor, backColor);
        }

        protected void Init(ConsoleColor? foreColor, ConsoleColor? backColor)
        {
            this.OriginalBackground = _console.BackgroundColor;
            this.OriginalForeground = _console.ForegroundColor;

            if (foreColor.HasValue) this.Foreground = foreColor.Value;
            if (backColor.HasValue) this.Background = backColor.Value;
        }

        public void ApplyColors()
        {
            if (Foreground.HasValue) _console.ForegroundColor = this.Foreground.Value;
            if (Background.HasValue) _console.BackgroundColor = this.Background.Value;
        }

        public void ResetColors()
        {
            _console.ForegroundColor = OriginalForeground;
            _console.BackgroundColor = OriginalBackground;
        }
    }
}
