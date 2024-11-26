using System;
using System.Collections.Concurrent;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;



namespace SaiSignatures
{
    internal static class SaiSignatures
    {
        public class SignatureInfo(string signature, int offset, int index)
        {
            public string Signature { get; set; } = signature;
            public int Offset { get; set; } = offset;
            public int Index { get; set; } = index;
        }

        private static byte[] sai;

        public class PatternData
        {
            public Dictionary<string, SignatureInfo> founds { get; set; }
            public Dictionary<string, SignatureInfo> notFound { get; set; }
        }

        static byte[] TransformAobToPattern(string hexString)
        {

            string[] hexValues = hexString.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            var byteArray = new List<byte>();

            for (int i = 0; i < hexValues.Length; i++)
            {
                
                if (hexValues[i] == "?" || hexValues[i] == "??")
                {
                    byteArray.Add( 0x3F ); 
                }
                else
                {
                    byteArray.Add(Convert.ToByte(hexValues[i], 16));
                }
            }

            return [.. byteArray];
        }


        static int ScanBinary(byte[] binary, byte?[] pattern)
        {
            int matches = 0;
            int binaryLength = binary.Length;
            int patternLength = pattern.Length;
            int offset = 0;
            while (offset <= binaryLength - patternLength)
            {
                int j = patternLength - 1;

                while (j >= 0 && (pattern[j] == null || pattern[j] == '?' || binary[offset + j] == pattern[j]))
                {
                    j--;
                }

                if (j < 0)
                {
                    matches++;
                    offset += (offset + patternLength < binaryLength) ? patternLength : 1;
                }
                else
                {
                    offset += Math.Max(1, j);
                }
            }

            return matches;
        }

        static readonly int[] Empty = [];
        internal static readonly char[] separator = new[] { ' ' };

        static bool IsMatch(byte[] array, int position, byte[] candidate)
        {
            if (candidate.Length > (array.Length - position))
                return false;

            for (int i = 0; i < candidate.Length; i++)

            {
                if (candidate[i] == 0x3F || array[position + i] == 0x3F) continue;

                  if (array[position + i] != candidate[i])
                    return false;
            }

      

            return true;
        }

        public static int[] Locate(this byte[] self, byte[] candidate)
        {
            if (IsEmptyLocate(self, candidate))
                return Empty;

            var list = new List<int>();

            for (int i = 0; i < self.Length; i++)
            {

                if (!IsMatch(self, i, candidate))
                    continue;

                list.Add(i);
            }

            return list.Count == 0 ? Empty : [.. list];
        }

        static bool IsEmptyLocate(byte[] array, byte[] candidate)
        {
            return array == null
                || candidate == null
                || array.Length == 0
                || candidate.Length == 0
                || candidate.Length > array.Length;
        }




        private static List<string> ScanPatterns(this PatternData patterns,
            byte[] raw)
        {

            List<string>  addrs = new();
            int losses = 0;

            foreach (var pattern in patterns.founds)
            {
                string aob = pattern.Value.Signature;
                var p = TransformAobToPattern(aob);

                var founds = raw.Locate(p);


                if (founds.Length >= 5)
                {
                    Console.WriteLine($"{pattern.Key} aob {aob}");
                    losses++;
                    continue;
                }
                //foreach (var found in founds)
                //{
                //    Console.WriteLine(found.ToString("X"));
                //}
            }

            Console.WriteLine($"Losses: {losses}");
            return addrs;
           
        }



        static void Main(string[] args)
        {
            // sai file
            // pattern.json

#if DEBUG
            sai = File.ReadAllBytes("PaintOriginal.exe");
            var jsonContent = File.ReadAllText("pattern.json");
#else
            sai = File.ReadAllBytes(args[0]);
            var jsonContent = File.ReadAllText(args[1]);
#endif


            var JsonPatterns = JsonSerializer.Deserialize<PatternData>(jsonContent);
            ScanPatterns(JsonPatterns, sai);



        }
    }
}
