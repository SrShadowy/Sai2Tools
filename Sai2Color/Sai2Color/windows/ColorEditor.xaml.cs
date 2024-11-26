using Sai2Color.src;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Sai2Color.windows
{
    /// <summary>
    /// Lógica interna para ColorEditor.xaml
    /// </summary>
    public partial class ColorEditor : Window
    {
        public ColorEditor()
        {
            InitializeComponent();
        }
        List<THEME> themeList = new();
        Rectangle SelectedRectangle { get; set; }

        public void GetThemes(List<THEME> list)
        {
            themeList.Clear();

            themeList = list;

            foreach (THEME theme in themeList)
            {
                cb_themes.Items.Add(theme.Name.GetFileNameWithoutExtension());
            }
        }

        static void SetColor(Dictionary<string, byte[]> list, string key, Shape target)
        {
            if (list.TryGetValue(key, out var rgb))
            {
                target.Fill = new SolidColorBrush(Color.FromRgb(rgb[2], rgb[1], rgb[0]));
            }
            else
            {
                Console.WriteLine($"Chave '{key}' não encontrada no tema.");
            }
        }

        static string GetColor(Shape target)
        {
            if (target.Fill is SolidColorBrush brush)
            {
                var color = brush.Color;
                string hexColor = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                return hexColor;
            }
            return "#FFFFFF";

        }



        private void Btn_load_theme_Click(object sender, RoutedEventArgs e)
        {
            var index = cb_themes.SelectedIndex;

            if (index < 0 || index >= themeList.Count)
            {
                Console.WriteLine("Índice de tema inválido.");
                return;
            }

            var listColors = themeList[index].Color.ToByteColorDictionaryWithoutAlpha();

            SetColor(listColors, "Primary", TheColorPrimary);
            SetColor(listColors, "Secondary", TheColorSecond);
            SetColor(listColors, "Ternary", TheColorTerna);
            SetColor(listColors, "Text", TheColorText);
            SetColor(listColors, "SelectablePrimary", TheColorSelect);
            SetColor(listColors, "SelectableSecondary", TheColorSelect2);
        }



        private void TheColorPrimary_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Color_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectedRectangle = sender as Rectangle;
            if (SelectedRectangle != null && SelectedRectangle.Fill is SolidColorBrush brush)
            {
                var color = brush.Color;
                standard_picker.SelectedColor = color;
            }
            else
            {
                MessageBox.Show("Nenhuma cor foi encontrada ou o tipo de pincel não é SolidColorBrush.");

            }
        }

        private void ApplyColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRectangle != null)
            {
                SelectedRectangle.Fill = new SolidColorBrush(standard_picker.SelectedColor);
            }


        }

        private void Btn_save_theme_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> newTheme = new()
            {
                { "Primary", GetColor(TheColorPrimary) },
                { "Secondary", GetColor(TheColorSecond) },
                { "Ternary", GetColor(TheColorTerna) },
                { "Text", GetColor(TheColorText) },
                { "SelectablePrimary", GetColor(TheColorSelect) },
                { "SelectableSecondary", GetColor(TheColorSelect2) }
            };

            FileS2CE.SaveTheme(name_file.Text, newTheme);


        }
    }
}
