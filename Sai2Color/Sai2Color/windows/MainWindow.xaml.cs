using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Sai2Color.src;
using Sai2Color.windows;

namespace Sai2Color
{
    public partial class MainWindow : Window
    {
        ColorEditor colorEditorWindow = new();
        List<THEME> themes = new();
        
        private void LoadItems()
        {

            themes.Clear();
            appList.Items.Clear();
            themes = PathTheme.GetThemes();

            foreach (var theme in themes)
            {

                AddItem(theme.Name, theme.Color);
            }

        }

        public MainWindow()
        {
            InitializeComponent();

            LoadItems();
        }

        static StackPanel CreateItemPanel(string fileName, Dictionary<string, string> colors)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

    
            if (colors != null)
            {
                foreach (var colorHex in colors.Values)
                {
                    panel.Children.Add(CreateColorRectangle(colorHex));
                }
            }

            panel.Children.Add(new TextBlock
            {
                Text = fileName.GetFileNameWithoutExtension(),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            });

            return panel;
        }


        static Rectangle CreateColorRectangle(string colorHex)
        {
            var rgb = colorHex.ToByteColor(); 

            return new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromRgb(rgb[2], rgb[1], rgb[0])),
                Width = 12,
                Height = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        void AddItem(string fileName, Dictionary<string, string> colors)
        {
            try
            {

                var item = new ListBoxItem
                {
                    Name = fileName.GetFileNameWithoutExtension(),
                    IsSelected = true
                };


                var panel = CreateItemPanel(fileName, colors);


                item.Content = panel;


                appList.Items.Add(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar item: {ex.Message}");
            }
        }

        private void Editor_Click(object sender, RoutedEventArgs e)
        {
            colorEditorWindow = new();
            colorEditorWindow.GetThemes(themes);
            colorEditorWindow.ShowDialog();

            LoadItems();

        }


        private void MainWindow__Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private async void SaveAndRun_ClickAsync(object sender, RoutedEventArgs e)
        {
            themes fn = new();
            var p = appList.SelectedIndex;
            if (p == -1)
            {
                p = 0;
            }
            MainTheme mainTheme = new();
            await Task.Run(() =>
            {
                mainTheme.Apply(themes[p].Name, themes[p].Color.ToByteColorDictionary());
                //_ = fn.ApplyTheme(themes[p]);
            });
       
        }
    }
}