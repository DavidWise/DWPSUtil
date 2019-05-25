namespace DWPowerShell.Utility
{
    public static class DWPSUtilExtensions
    {
        public static string[] TrimAll(this string[] values)
        {
            if (values == null || values.Length<1) return values;

            var result = new string[values.Length];
            for (int pos = 0; pos < values.Length; pos++)
                result[pos] = values[pos]?.Trim();
            return result;
        }

        public static string[] TrimAll(this string[] values, char[] trimChars)
        {
            if (values == null || values.Length < 1 || 
                trimChars == null || trimChars.Length <1) return values;

            var result = new string[values.Length];
            for (int pos = 0; pos < values.Length; pos++)
                result[pos] = values[pos]?.Trim(trimChars);
            return result;
        }
    }
}
