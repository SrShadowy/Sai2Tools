using System.IO;
using System.Windows;
using System.Text.Json;


namespace Sai2Color.src
{
    public class PathS2CE {
        private const string ExecutableFile = "./sai2.exe";
        private const string BackUpFile = "./sai2.old.exe";
        private const string ClientOriginal = "./sai2.original.exe";


        public static string OriginalSai2 { get; set; } = ClientOriginal;
        public static string OldSai2 { get; set; } = BackUpFile;
        public static string Sai2 { get; set; } = ExecutableFile;
    }

    public readonly struct THEME
    {
        public string Name { get; }
        public Dictionary<string, string> Color { get; }

        public THEME(string name, Dictionary<string, string> color)
        {
            Name = name;
            Color = color ?? new Dictionary<string, string>();
        }
    }

    public class PathTheme {
        private static string Path { get; set; } = "./data";


        public static Dictionary<string, int> ReadAddress( )
        {
            if (string.IsNullOrWhiteSpace(Path)) return new Dictionary<string, int>();
            var fileAddr = Path + "/s2/Address.json";
            if(!File.Exists(fileAddr)) return new Dictionary<string, int>();

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(fileAddr));

            if (data == null) return new Dictionary<string, int>();

            return ColorsExtensions.ToDecimalAddressDictionary(data);
        }

        public static Dictionary<string, byte[]> ReadSaiColors()
        {
            if (string.IsNullOrWhiteSpace(Path)) return new Dictionary<string, byte[]>();
            var fileAddr = Path + "/s2/ColorRGB.json";
            if (!File.Exists(fileAddr)) return new Dictionary<string, byte[]>();

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(fileAddr));

            if (data == null) return new Dictionary<string, byte[]>();

            return ColorsExtensions.ToByteColorDictionaryWithoutAlpha(data);
        }

        public static List<THEME> GetThemes()
        {
            var list = new List<THEME>
            {
                new("Default", new Dictionary<string, string>
                {
                        { "Primary","#C0C0C0" },
                        { "Secondary","#FFFFFF" },
                        { "Ternary","#CCCCCC" },
                        { "Text","#000000" },
                        { "SelectablePrimary","#BBBBBB" },
                        { "SelectableSecondary","#8E8E8E" },
                })
            };


            if (string.IsNullOrWhiteSpace(Path)) return list;
            var path = Path + "/theme/";
            if (!Directory.Exists(path)) return list;
            string[] files = Directory.GetFiles(path, "*.json");

            foreach (string file in files)
            {

                list.Add(
                    new(
                        file, 
                        GetThemeColorAsHex(file)
                    )
                );

            }

            return list;
        }
        public static Dictionary<string, string> GetThemeColorAsHex(string theme)
        {
            var list = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(Path)) return list;

            if (!File.Exists(theme)) return list;

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(theme));
            return data ?? list;
        }
        public static Dictionary<string, byte[]> GetThemeColorRGB(string theme)
        {
            var data = GetThemeColorAsHex(theme);

            var list = new Dictionary<string, byte[]>();
            if (data == null) return  list;
            list = ColorsExtensions.ToByteColorDictionaryWithoutAlpha(data);

            return list;
     
        }
        public static Dictionary<string, byte[]> GetThemeColorARGB(string theme)
        {
            var data = GetThemeColorAsHex(theme);

            var list = new Dictionary<string, byte[]>();
            if (data == null) return list;
            list = ColorsExtensions.ToByteColorDictionary(data);

            return list;

        }

        public static string GetThemePath()
        {
            return Path + "/theme/";
        }


    }


    public class FileS2CE
    {
        public static void SaveTheme(string theme, Dictionary<string, string> context)
        {
            var path = PathTheme.GetThemePath() + theme + ".json";
            if (File.Exists(path ))
            {
               var responce = MessageBox.Show("This file already exist, wanna replace?", "This file exist", MessageBoxButton.YesNo);
                if (responce == MessageBoxResult.No)
                {
                    return;
                }
            }

            try
            {
                var jsonString = JsonSerializer.Serialize(context, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, jsonString);

                MessageBox.Show("Theme saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Failed to save theme: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }



        /// <summary>
        /// Update old one
        /// </summary>
        public static void UpdateOriginalFile()
        {
 

            if (IsOldFileExists())
            {
     
                File.Delete(PathS2CE.Sai2);
                File.Copy(PathS2CE.OldSai2, PathS2CE.Sai2);
            }
        }

        /// <summary>
        /// create Copy for old one
        /// </summary>
        public static void CreateOldFile()
        {
            if (!IsOriginalFileExists())
                File.Copy(PathS2CE.Sai2, PathS2CE.OriginalSai2);
            

            if (IsOldFileExists()) return;

            File.Copy(PathS2CE.Sai2, PathS2CE.OldSai2, overwrite: true);
        }

        /// <summary>
        /// Remove Old one
        /// </summary>
        public static void DeleteOldFile()
        {
            if (IsOldFileExists())
            {
                File.Delete(PathS2CE.OldSai2);
            }
        }

        public static bool PutOriginalOn()
        {
            if (IsOriginalFileExists())
            {
                //File.Delete(PathS2CE.Sai2);
  
                File.Copy(PathS2CE.OriginalSai2, PathS2CE.Sai2, true);
                Thread.Sleep(100);
                return true;
            }

            return false;
        }

        public static bool IsOriginalFileExists()
        {
            return File.Exists(PathS2CE.OriginalSai2);
        }

        /// <summary>
        /// check if old file exist
        /// </summary>
        public static bool IsOldFileExists()
        {
             return File.Exists(PathS2CE.OldSai2);
            
        }

        public static byte[] GetFile()
        {
            var file_name = IsOldFileExists()? PathS2CE.OldSai2 : PathS2CE.Sai2;

            return File.ReadAllBytes(file_name);
        }

        /// <summary>
        /// checkout if is file busy
        /// </summary>
        public static bool IsFileBusy()
        {
            if (!IsOriginalFileExists()) return false;

            try
            {
                using var stream = File.Open(PathS2CE.Sai2, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                if(stream == null) return false; 
                if(stream.Length == 0) return false;
                
                return false;
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
                return true;
            }

        }
    }

}
