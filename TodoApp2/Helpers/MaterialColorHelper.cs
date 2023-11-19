using System.Windows.Media;

namespace TodoApp2
{
    public static class MaterialColorHelper
    {
        public static string DecimalToHex(uint argb) => $"#{argb:X8}";

        public static uint HexToDecimal(string hexString)
        {
            if (hexString.StartsWith("#"))
            {
                hexString = hexString.Substring(1);
            }

            uint result = uint.Parse(hexString, System.Globalization.NumberStyles.HexNumber);

            return result;
        }

        public static Color UIntToColor(uint value)
        {
            byte a = (byte)((value & 0xFF000000) >> 24);
            byte r = (byte)((value & 0x00FF0000) >> 16);
            byte g = (byte)((value & 0x0000FF00) >> 8);
            byte b = (byte)(value & 0x000000FF);

            return Color.FromArgb(a, r, g, b);
        }

        public static Color HexToColor(string value) => 
            (Color)ColorConverter.ConvertFromString(value);
    }
}
