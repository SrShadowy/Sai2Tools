using System.Collections.Generic;
using System.Drawing;
using System.IO;


namespace Sai2Color.src
{
    public static class ColorsExtensions
    {
        /// <summary>
        /// Extracts the file name without extension.
        /// </summary>
        public static string GetFileNameWithoutExtension(this string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            return Path.GetFileNameWithoutExtension(filePath);
        }

        /// <summary>
        /// Extracts and truncates the file name to 45 characters if necessary.
        /// </summary>
        public static string GetTruncatedFileName(this string filePath, int maxLength = 45)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            var name = filePath.GetFileNameWithoutExtension();
            return name.Length > maxLength ? $"{name[..maxLength]}..." : name;
        }

        /// <summary>
        /// Converts a hexadecimal string to its decimal equivalent.
        /// </summary>
        public static int ToDecimal(this string hexAddress)
        {
            if (string.IsNullOrWhiteSpace(hexAddress)) throw new ArgumentNullException(nameof(hexAddress));
            return int.Parse(hexAddress.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Converts a HEX color string (#AARRGGBB) to a BGRA byte array.
        /// </summary>
        public static byte[] ToByteColor(this string hexColor)
        {
            if (string.IsNullOrWhiteSpace(hexColor)) throw new ArgumentNullException(nameof(hexColor));
   
            if (!hexColor.Contains('#'))
            {
                hexColor = "#" + hexColor;
            }
            var color = ColorTranslator.FromHtml(hexColor);
            return new[] { color.B, color.G, color.R, color.A }; // BGRA format
        }

        /// <summary>
        /// Converts a byte array to a hexadecimal color string.
        /// </summary>
        public static string ToHexColor(this byte[] byteColor)
        {
            if (byteColor == null || (byteColor.Length != 3 && byteColor.Length != 4))
                throw new ArgumentException("Invalid byte array for color.");

            // Se for RGB, adiciona o valor alfa máximo (255)
            if (byteColor.Length == 3)
            {
                byteColor = new byte[] { 255, byteColor[0], byteColor[1], byteColor[2] };
            }

            return BitConverter.ToString(byteColor).Replace("-", string.Empty);
        }



        /// <summary>
        /// Removes the alpha channel from a BGRA byte array.
        /// </summary>
        public static byte[] RemoveAlpha(this byte[] byteColor)
        {
            if (byteColor == null || byteColor.Length < 3) throw new ArgumentException("Invalid byte array for color.");
            return new[] { byteColor[0], byteColor[1], byteColor[2] }; // BGR format
        }

        /// <summary>
        /// Reverses the byte order in a color array (BGR to RGB or vice versa).
        /// </summary>
        public static byte[] ReverseBytes(this byte[] byteColor)
        {
            if (byteColor == null || byteColor.Length < 3) throw new ArgumentException("Invalid byte array for color.");
            return byteColor.Reverse().ToArray();
        }

        /// <summary>
        /// Converts a dictionary of hexadecimal addresses to decimal addresses.
        /// </summary>
        public static Dictionary<string, int> ToDecimalAddressDictionary(this Dictionary<string, string> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            return dictionary.ToDictionary(pair => pair.Key, pair => pair.Value.ToDecimal());
        }

        /// <summary>
        /// Converts a dictionary of HEX color strings to BGRA byte arrays.
        /// </summary>
        public static Dictionary<string, byte[]> ToByteColorDictionary(this Dictionary<string, string> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            return dictionary.ToDictionary(pair => pair.Key, pair => $"#00{pair.Value[1..]}".ToByteColor());
        }

        /// <summary>
        /// Converts a dictionary of HEX color strings to BGR byte arrays (removes alpha channel).
        /// </summary>
        public static Dictionary<string, byte[]> ToByteColorDictionaryWithoutAlpha(this Dictionary<string, string> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            return dictionary.ToDictionary(pair => pair.Key, pair => pair.Value.ToByteColor().RemoveAlpha());
        }
    }

    public class ColorS2CE
    {
        // Ternary Artifacts Colors
        public byte[] TernaryArtifactsColor1 { get; set; } = Array.Empty<byte>();
        public byte[] TernaryArtifactsColor2 { get; set; } = Array.Empty<byte>();
        public byte[] TernaryArtifactsColor3 { get; set; } = Array.Empty<byte>();
        public byte[] TernaryArtifactsColor4 { get; set; } = Array.Empty<byte>();
        public byte[] TernaryArtifactsColor5 { get; set; } = Array.Empty<byte>();
        public byte[] TernaryArtifactsColor6 { get; set; } = Array.Empty<byte>();
        public byte[] TernaryArtifactsColor7 { get; set; } = Array.Empty<byte>();
        public byte[] TernaryArtifactsColor8 { get; set; } = Array.Empty<byte>();

        // Secondary Artifacts Colors
        public byte[] SecondaryArtifactsColor1 { get; set; } = Array.Empty<byte>();
        public byte[] SecondaryArtifactsColor2 { get; set; } = Array.Empty<byte>();
        public byte[] SecondaryArtifactsColor3 { get; set; } = Array.Empty<byte>();
        public byte[] SecondaryArtifactsColor4 { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Configures artifact colors using provided secondary and ternary colors.
        /// </summary>
        /// <param name="secondary">The secondary color in byte format.</param>
        /// <param name="ternary">The ternary color in byte format.</param>
        public void ConfigureArtifactsColors(byte[] secondary, byte[] ternary)
        {
            if (secondary == null || secondary.Length < 3)
                throw new ArgumentException("Invalid secondary color array.");
            if (ternary == null || ternary.Length < 3)
                throw new ArgumentException("Invalid ternary color array.");


            string s = secondary.ToHexColor();
            s = $"{s[0]}{s[1]}";
            string t = ternary.ToHexColor();
            t = $"{t[0]}{t[1]}";
            //var s = secondary.ToHexColor()[..2];
            //var t = ternary.ToHexColor()[..2];

            // Configure Secondary Colors
            SecondaryArtifactsColor1 = GenerateColor($"00F8F8{s}");
            SecondaryArtifactsColor2 = GenerateColor($"00{s}F8F8");
            SecondaryArtifactsColor3 = GenerateColor($"00F8{s}{s}");
            SecondaryArtifactsColor4 = GenerateColor($"00{s}{s}F8");

            // Configure Ternary Colors
            TernaryArtifactsColor1 = GenerateColor($"00FFFF{t}");
            TernaryArtifactsColor2 = GenerateColor($"00{t}FFFF");
            TernaryArtifactsColor3 = GenerateColor($"00FF{t}{t}");
            TernaryArtifactsColor4 = GenerateColor($"00{t}{t}FF");
            TernaryArtifactsColor5 = GenerateColor($"00F8F8{t}");
            TernaryArtifactsColor6 = GenerateColor($"00{t}F8F8");
            TernaryArtifactsColor7 = GenerateColor($"00F8{t}{t}");
            TernaryArtifactsColor8 = GenerateColor($"00{t}{t}F8");
        }

        /// <summary>
        /// Converts a HEX color string to a BGRA byte array and removes the alpha channel.
        /// </summary>
        /// <param name="hexColor">The HEX color string.</param>
        /// <returns>The byte array representing the color.</returns>
        private static byte[] GenerateColor(string hexColor)
        {
            return hexColor.ToByteColor().RemoveAlpha();
        }
    }


}
