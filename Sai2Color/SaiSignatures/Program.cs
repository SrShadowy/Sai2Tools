using System;
using System.Collections.Concurrent;
using System.Text.Json;



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

        public class PatternData
        {
            public Dictionary<string, SignatureInfo> founds { get; set; }
            public Dictionary<string, SignatureInfo> notFound { get; set; }
        }

        private static int losses = 0;
        private const int BackSteps = 0x30;
        private const int MinSignatureLength = 8;
        private const int MaxSignatureLength = 0x190;


        public static int ToDecimal(this string hexAddress)
        {
            if (string.IsNullOrWhiteSpace(hexAddress)) throw new ArgumentNullException(nameof(hexAddress));
            return int.Parse(hexAddress.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
        }

        public static Dictionary<string, int> ReadAddress()
        {
            var fileAddr = "Address.json";
            if (!File.Exists(fileAddr)) return new Dictionary<string, int>();

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(fileAddr));
            if (data == null) return new Dictionary<string, int>();

            return data.ToDictionary(pair => pair.Key, pair => ToDecimal(pair.Value));
        }

        static byte?[] CreateSignature(byte?[] binary, long address, int length)
        {
            if (address < 0 || address >= binary.Length)
            {
                return [];
            }

            int start = (int)address;
            int end = (int)Math.Min(binary.Length, address + length);

            var signature = new byte?[end - start];

            for (int i = start; i < end; i++)
            {
                signature[i - start] = binary[i];
            }

            return signature;
        }

        static readonly int[] Empty = [];


        static bool IsMatch(byte[] array, int position, byte?[] candidate)
        {
            if (candidate.Length > (array.Length - position))
                return false;

            for (int i = 0; i < candidate.Length; i++)
            {
                if (candidate[i] == null) continue;

                if (array[position + i] != candidate[i])
                    return false;
            }



            return true;
        }
        public static int[] Locate(this byte[] self, byte?[] candidate)
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

        static bool IsEmptyLocate(byte[] array, byte?[] candidate)
        {
            return array == null
                || candidate == null
                || array.Length == 0
                || candidate.Length == 0
                || candidate.Length > array.Length;
        }



        private static byte?[] source;
        public static byte[] dest;

        private static string converteToPatternString(byte?[] pattern)
        {
            string p = "";

            for (int i = 0; i < pattern.Length; i++)
            {
                p += pattern[i] == null ? "?" : pattern[i].Value.ToString("X2");
                p += " ";
            }

            return p;
        }
        internal static readonly char[] separator = new[] { ' ' };
        static byte[] TransformAobToPattern(string hexString)
        {
            string[] hexValues = hexString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var byteArray = new List<byte>();

            for (int i = 0; i < hexValues.Length; i++)
            {
                if (hexValues[i] == "?" || hexValues[i] == "??")
                {
                    byteArray.Add(0x3F);
                }
                else
                {
                    if (byte.TryParse(hexValues[i], System.Globalization.NumberStyles.HexNumber, null, out byte newB))
                    {
                        byteArray.Add(newB);
                    }
                    else
                    {
                        Console.Write($"Valor hexadecimal inválido: {hexValues[i]}\n");
                    }
                }
            }

            return [.. byteArray];
        }

        private static PatternData BackScan( this PatternData BackThis,
          Dictionary<string, int> addresses)
        {
            Dictionary<string, SignatureInfo> NotFounds = new();
            Dictionary<string, SignatureInfo> NewFounds = new();

            Parallel.ForEach(BackThis.notFound, x => //(var x in )
            {
                int length = TransformAobToPattern(x.Value.Signature).Length > 10? TransformAobToPattern(x.Value.Signature).Length : 0x10;
                int backsetp = 4;
                bool found = false;
                int min_founds = 0;
                byte?[] last_pattern = [];
                while (length <= MaxSignatureLength + 1 && backsetp <= BackSteps)
                {

                    
                    var pattern = CreateSignature(source, (addresses[x.Key] - backsetp), length);

                    var finders = dest.Locate(pattern);

                    if (finders.Length == 0)
                    {
                        var p = converteToPatternString(pattern);
                        source[addresses[x.Key] - backsetp + 1] = null;
                        //length++;
                        continue;
                    }


                    if (finders.Length > 0)
                    {
                        if (min_founds == 0 || min_founds > finders.Length)
                        {
                            min_founds = finders.Length;
                            last_pattern = pattern;
                        }

                    }

                    if (finders.Length == 1)
                    {
                        var p = converteToPatternString(pattern);
                        NewFounds.Add(x.Key, new(p, backsetp, 0));
                        found = true;
                        break;
                    }


                    backsetp++;

                }

                if (!found)
                {
                    var p = converteToPatternString(last_pattern);
                    NotFounds.Add(x.Key, new(p, backsetp, 0));
                    Console.WriteLine($"{x.Key} : {addresses[x.Key].ToString("X2")} não foi encontrado\n{min_founds} : {p}\nNecessario pensar outro metodo!");
                }

            });

            var data = new PatternData();
            data.founds = NewFounds;
            data.notFound = NotFounds;


            return data;
        }

        private static PatternData Scan(this PatternData patterns, 
            Dictionary<string, int> addresses)
        {
            Dictionary<string, SignatureInfo> NotFounds = new();
            Dictionary<string, SignatureInfo> NewFounds = new();
            var result = new List<string>();
            Parallel.ForEach(patterns.notFound, x =>
            {

                int length = 4;
                bool found = false;
                int min_founds = 0;
                byte?[] last_pattern = [];


                while (length <= MaxSignatureLength)
                {
                    var pattern = CreateSignature(source, addresses[x.Key], length);

                    var finders = dest.Locate(pattern);

                    if (finders.Length == 0)
                    {
                        var p = converteToPatternString(pattern);
                        source[addresses[x.Key] + length - 1] = null;
                        length++;
                        continue;
                    }


                    if (finders.Length > 0)
                    {
                        if (min_founds == 0 || min_founds > finders.Length)
                        {
                            min_founds = finders.Length;
                            last_pattern = pattern;
                        }

                    }

                    if (finders.Length == 1)
                    {
                        var p = converteToPatternString(pattern);
                        NewFounds.Add(x.Key, new(p, 0, 0));
                        found = true;
                        break;
                    }


                    length++;

                }

                if (!found)
                {
                    var p = converteToPatternString(last_pattern);
                    NotFounds.Add(x.Key, new(p, 0, 0));
                    //Console.WriteLine($"{x.Key} : {addresses[x.Key].ToString("X2")} não foi encontrado\n{min_founds} : {p}\nNecessario pensar outro metodo!");
                }

            });

            foreach( var x in NewFounds)
            {
                patterns.notFound.Remove(x.Key);
                patterns.founds.Add(x.Key, x.Value);
            }


            var newOne = patterns.BackScan(addresses);



            foreach (var x in newOne.founds)
            {
                patterns.notFound.Remove(x.Key);
                patterns.founds.Add(x.Key, x.Value);
                //Console.WriteLine($" {x.Key} - {x.Value.Signature}");
            }



            return patterns;

        }



        static void Main(string[] args)
        {
            string binaryPath1 = "PaintCE.exe";


            string binaryPath2 = "PaintOriginal.exe";
            bool isSecond = false;
            string jsonNamed = "";
            jsonNamed = "pattern_try.json";
            isSecond = true;

            if (!File.Exists(binaryPath1) || !File.Exists(binaryPath2))
            {
                Console.WriteLine("Certifique-se de que os binários estão disponíveis.");
                return;
            }
            var addresses = ReadAddress();
            var jsonContent = File.ReadAllText(jsonNamed);
            var jsonNotFoundFile = JsonSerializer.Deserialize<PatternData>(jsonContent);

            dest = File.ReadAllBytes(binaryPath2);

            source = File.ReadAllBytes(binaryPath1).Select(b => (byte?)b).ToArray();

            if (addresses.Count == 0)
            {
                Console.WriteLine("Nenhum endereço encontrado no arquivo JSON.");
                return;
            }

            if (jsonNotFoundFile == null)
            {
                return;
            }


            Console.WriteLine("\nItens não encontrados (notFound):");
            foreach (var notFound in jsonNotFoundFile.notFound)
            {
                Console.WriteLine($"- {notFound.Key}: {notFound.Value.Signature}");
            }


            var newData = jsonNotFoundFile.Scan( addresses );



            var options = new JsonSerializerOptions { WriteIndented = true, AllowTrailingCommas = true};
            var data = JsonSerializer.Serialize(newData, options: options);
            File.WriteAllText("pattern22.json", data);
            Console.WriteLine($"DONE! pattern.json created with {losses} addresses not found.");
        }
    }
}
