using System;
using System.Collections.Generic;
using System.Linq;

namespace DWPowerShell.Utility.ConsoleIO
{
    public class ConsoleColors
    {
        private static Dictionary<string, ConsoleColor> _consoleColors;

        static ConsoleColors()
        {
            _consoleColors = new Dictionary<string, ConsoleColor>(StringComparer.InvariantCultureIgnoreCase);
            BuildConsoleColors();
        }

        public ConsoleColor? this[string key]
        {
            get
            {
                var inKey = key == null ? string.Empty : key.Trim();
                if (inKey != string.Empty && _consoleColors.ContainsKey(inKey)) return _consoleColors[inKey];
                return null; 
            }
        }

        public string[] Names => _consoleColors.Keys.ToArray();

        private static void BuildConsoleColors()
        {
            var ctype = typeof(ConsoleColor);

            var colors = Enum.GetNames(ctype);
            foreach (var color in colors)
            {
                _consoleColors.Add(color, (ConsoleColor)Enum.Parse(ctype, color));
            }
        }
    }
}
