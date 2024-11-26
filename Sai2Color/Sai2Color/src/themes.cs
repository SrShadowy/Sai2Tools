
using Microsoft.Windows.Themes;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Xml.XPath;

namespace Sai2Color.src
{
    class ThemeS2CE
    {
        public byte[] binary { get; set; }
        public ThemeS2CE()
        {

            binary = FileS2CE.GetFile();
        }

        public void SetElementColor(byte[] to_color, int color_address)
        {
            for (int i = 0; i < to_color.Length; i++) { binary[color_address + i] = to_color[i]; }
        }
        bool WrongSequence(byte[] bin, int index, byte[] color_to_detect)
        {
            // Detects bytes in sequence which doesn't equal certain color_to_detect/
            // Skips this sequence and goes to the next one.
            for (int cur_index = 0; cur_index < color_to_detect.Length; cur_index++)
            {
                if (bin[index + cur_index] != color_to_detect[cur_index]) { return true; }

            }
            return false;
        }
        /// <summary>
        /// Searches sequences in certain byte's range and replaces pack of the colors.
        /// Some parts of the SAI2 contains long byte sequences, which is hard to debug, so temporary solution is what you see.
        /// </summary>
        /// <param name="to_color">Custom theme color you want to replace to</param>
        /// <param name="start_index">Beggining of byte sequence</param>
        /// <param name="end_index">End of byte sequence</param>
        /// <param name="from_color">Color, which should be replaced</param>
        /// <param name="isArtifacted">
        /// Set this to true, if you want to check every single byte as independent sequence.
        /// Useful to fix different kind of artifacts. False as default.
        /// </param>
        public void SetElementColorComplicated(byte[] from_color, byte[] to_color, int start_index, int end_index, bool isArtifacted = false)
        {
            //if (!File.Exists("./ref/sai2.tmp.exe")) { return; }

            int value = isArtifacted ? 1 : to_color.Length;
            // Find certain sequence position and move on until the end
            for (int index = start_index; index < end_index; index += value)
            {
                if (WrongSequence(binary, index, from_color)) { continue; }
                // Change color in certain sequence
                for (int col_index = 0; col_index < to_color.Length; col_index++) { binary[index + col_index] = to_color[col_index]; }
            }

        }
        public void SetElementColorWithTotalReplacment(byte[] to_color, int start_index, int end_index)
        {
            //if (!File.Exists("./ref/sai2.tmp.exe")) { return; }

            int value = to_color.Length;
            // Find sequence position and move on until the end
            for (int index = start_index; index < end_index; index += value)
            {
                // Change color in sequence
                for (int col_index = 0; col_index < to_color.Length; col_index++) { binary[index + col_index] = to_color[col_index]; }
            }

        }
        // TODO: Color picker has white outline. Thinking on better solution right now.. 
        public void FixColorPicker(byte[] to_color, int start_index, int end_index)
        {
            string hexLets = "ABCDEF", hexNums = "0123456789", hexAll = "0123456789ABCDEF";
            byte[] cirCol;

            // For colors like letter + num in each channel
            foreach (var o in hexLets)
            {
                foreach (var i in hexNums)
                {
                    cirCol = $"#00{o}{i}{o}{i}{o}{i}".ToByteColor().RemoveAlpha();
                    for (int n = start_index; n < end_index; n++)
                    {
                        if (WrongSequence(binary, n, cirCol)) { continue; }
                        // Change color in certain sequence
                        for (int col_index = 0; col_index < to_color.Length; col_index++) { binary[n + col_index] = to_color[col_index]; }
                    }
                }
            }

            // For colors like letter + letter in each channel
            foreach (var o in hexLets)
            {
                foreach (var oo in hexLets)
                {
                    cirCol = $"#00{o}{oo}{o}{oo}{o}{oo}".ToByteColor().RemoveAlpha();
                    for (int n = start_index; n < end_index; n++)
                    {
                        if (WrongSequence(binary, n, cirCol)) { continue; }
                        // Change color in certain sequence
                        for (int col_index = 0; col_index < to_color.Length; col_index++) { binary[n + col_index] = to_color[col_index]; }
                    }
                }
            }

            // For colors like num + letter in each channel
            foreach (var i in hexNums)
            {
                foreach (var o in hexLets)
                {
                    cirCol = $"#00{i}{o}{i}{o}{i}{o}".ToByteColor().RemoveAlpha();
                    for (int n = start_index; n < end_index; n++)
                    {
                        if (WrongSequence(binary, n, cirCol)) { continue; }
                        // Change color in certain sequence
                        for (int col_index = 0; col_index < to_color.Length; col_index++) { binary[n + col_index] = to_color[col_index]; }
                    }
                }
            }

            // // CA -> F9 fixes

            // F9CAE2 - > F9F9CA
            for (int o = 2; o < hexLets.Length; o++)
            {
                for (int i = 9; i < hexAll.Length; i++)
                {
                    cirCol = $"#00F9{hexLets[o]}{hexAll[i]}E2".ToByteColor().RemoveAlpha();
                    for (int n = start_index; n < end_index; n++)
                    {
                        if (WrongSequence(binary, n, cirCol)) { continue; }
                        // Change color in certain sequence
                        for (int col_index = 0; col_index < to_color.Length; col_index++) { binary[n + col_index] = to_color[col_index]; }
                    }
                }
            }
            for (int o = 2; o < hexLets.Length; o++)
            {
                for (int i = 2; i < hexAll.Length; i++)
                {
                    cirCol = $"#00F9F9{hexLets[o]}{hexAll[i]}".ToByteColor().RemoveAlpha();
                    for (int n = start_index; n < end_index; n++)
                    {
                        if (WrongSequence(binary, n, cirCol)) { continue; }
                        // Change color in certain sequence
                        for (int col_index = 0; col_index < to_color.Length; col_index++) { binary[n + col_index] = to_color[col_index]; }
                    }
                }
            }

            // // F9F9CA - > CAF9E2
            for (int o = 2; o < hexLets.Length; o++)
            {
                for (int i = 0; i < hexAll.Length; i++)
                {
                    cirCol = $"#00{o}{i}F9CA".ToByteColor().RemoveAlpha();
                    for (int n = start_index; n < end_index; n++)
                    {
                        if (WrongSequence(binary, n, cirCol)) { continue; }
                        // Change color in certain sequence
                        for (int col_index = 0; col_index < to_color.Length; col_index++) { binary[n + col_index] = to_color[col_index]; }
                    }
                }
            }
            for (int o = 2; o < hexLets.Length; o++)
            {
                for (int i = 2; i < hexAll.Length; i++)
                {
                    cirCol = $"#00CAF9{hexLets[o]}{hexAll[i]}".ToByteColor().RemoveAlpha();
                    for (int n = start_index; n < end_index; n++)
                    {
                        if (WrongSequence(binary, n, cirCol)) { continue; }
                        // Change color in certain sequence
                        for (int col_index = 0; col_index < to_color.Length; col_index++) { binary[n + col_index] = to_color[col_index]; }
                    }
                }
            }

            // CAF9E2 -> CACAF9
            for (int o = 2; o < hexLets.Length; o++)
            {
                for (int i = 0; i < hexAll.Length; i++)
                {
                    cirCol = $"#00{o}{i}F9E2".ToByteColor().RemoveAlpha();
                    for (int n = start_index; n < end_index; n++)
                    {
                        if (WrongSequence(binary, n, cirCol)) { continue; }
                        // Change color in certain sequence
                        for (int col_index = 0; col_index < to_color.Length; col_index++) { binary[n + col_index] = to_color[col_index]; }
                    }
                }
            }
            for (int o = 4; o < hexLets.Length; o++)
            {
                for (int i = 2; i < hexAll.Length; i++)
                {
                    cirCol = $"#00CACA{hexLets[o]}{hexAll[i]}".ToByteColor().RemoveAlpha();
                    for (int n = start_index; n < end_index; n++)
                    {
                        if (WrongSequence(binary, n, cirCol)) { continue; }
                        // Change color in certain sequence
                        for (int col_index = 0; col_index < to_color.Length; col_index++) { binary[n + col_index] = to_color[col_index]; }
                    }
                }
            }
        }
        /// <summary>
        /// Saves current theme changes.
        /// </summary>
        public void SaveTheme()
        {
            File.WriteAllBytes(PathS2CE.Sai2 , binary);
        }
    }

    class themes
    {
        private static readonly ColorS2CE ColorS2 = new();
       // private ColorS2CE ColorS2 = colorS2CE;
        private static readonly ThemeS2CE theme = new();
        //ThemeS2CE theme = themeS2CE;
        private Dictionary<string, byte[]> SaiRGB { get; set; } = new Dictionary<string, byte[]>();
        private Dictionary<string, int> SaiAddress { get; set; } = new Dictionary<string, int>();

        public themes()
        { 
            SaiAddress = PathTheme.ReadAddress();
            SaiRGB = PathTheme.ReadSaiColors();

        }

        public static void Run(bool adm)
        {
            string arquivo = PathS2CE.Sai2;

            string? caminho = Path.GetDirectoryName(arquivo);
            string nome = Path.GetFileName(arquivo);
            var processInfo = new System.Diagnostics.ProcessStartInfo();
            if (adm)
            {
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";
            }
            processInfo.WorkingDirectory = caminho;
            processInfo.FileName = nome;
            try
            {
                Process.Start(processInfo);
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message, "ERROR ON STARTUP THE PROGRAM", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ColorPick(Dictionary<string, string> applyTheme)
        {
            var colorRanges = new[]
            {
                new { From = SaiAddress["ColorCircleFrom"], To = SaiAddress["ColorCircleTo"] },
                new { From = SaiAddress["ColorCircleFrom2"], To = SaiAddress["ColorCircleTo2"] },
                new { From = SaiAddress["ColorCircleFrom3"], To = SaiAddress["ColorCircleTo3"] },
                new { From = SaiAddress["ColorCircleFrom4"], To = SaiAddress["ColorCircleTo4"] }
            };

            var color = applyTheme["Ternary"].ToByteColor().RemoveAlpha();

            foreach (var range in colorRanges)
            {
                Thread thread = new(() =>
                {
                    theme.FixColorPicker(color, range.From, range.To);
                })
                { IsBackground = true };
                thread.Start();

            }
        }

        
        private void ApplyColor(string colorHex, string[] keys)
        {
            var color = colorHex.ToByteColor().RemoveAlpha();

            foreach (var key in keys)
            {
                theme.SetElementColor(color, SaiAddress[key]);
            }
        }

        private void ApplyComplicatedColor(string colorHex, string[] keys, int fromAddress, int toAddress, bool appskin)
        {
            var color = colorHex.ToByteColor().RemoveAlpha();

            foreach (var key in keys)
            {
                theme.SetElementColorComplicated(SaiRGB[key], color, fromAddress, toAddress, appskin);
            }
        }

        public void RegionPrimary(Dictionary<string, string> applyTheme )
        {
            var PrimaryItems = new string[]
            {
                  "ActiveCanvasBackground", "ActiveCanvasBackground2", "ActiveCanvasBackground3", "ActiveCanvasBackground4",
                  "InActiveCanvasBackground", "BehindLayersUIBackground", "BehindLayersUIBackground2", "BehindLayersUIBackground3", "BehindLayersUIBackground4",
                  "BrushBorders", "SlidersVertical", "SlidersHorizontal", "InActiveText", "SliderBarHovered",
                  "ScrollBarArrowUp", "ScrollBarArrowDown", "ScrollBarArrowLeft", "ScrollBarArrowRight", "ClosedListArrow",
                  "GlobalTopBar", "GlobalTopBar2", "GlobalTopBar3", "GlobalTopBar4", "GlobalTopBar5", "GlobalTopBar6",
                  "GlobalTopBar7", "GlobalTopBar8", "ScaleAngleArrow", "ColorSlidersArrows", "GreyNoteText",
                  "SettingsListBackground", "SettingsListBackground2", "SettingsListBackground3", "SettingsListBackground4",
                  "OkCancelButtonsTextInActive", "AssetManagerLeftBackground", "ScaleAngleSliders2", "ScaleAngleSliders3",
                  "ScaleAngleSliders4", "ScaleAngleSliders5", "ScaleAngleSliders6", "ScaleAngleSliders7", "ScaleAngleSliders8",
                  "ScaleAngleSliders9", "ScaleAngleSliders10", "ScaleAngleSliders11", "ScaleAngleSliders12", "ScaleAngleSliders13",
                  "ScaleAngleSliders14", "ScaleAngleSliders15"
            };

            var PrimaryRGBComplicatedItemsAppskinTrue = new string[]
            {
                 "BordersFix13", "BordersFix15", "BordersFix16", "LayerOutline", "FileMenuBackground", "FolderBehindBackground1",
                 "FolderBehindBackground2"
            };


            ApplyColor(applyTheme["Primary"], PrimaryItems);

            ApplyComplicatedColor(applyTheme["Primary"], 
                PrimaryRGBComplicatedItemsAppskinTrue, 
                SaiAddress["GlobalSectionAppskinFrom"],
                SaiAddress["GlobalSectionAppskinTo"], true);
        }

        public void SecundaryColorRegion(Dictionary<string, string> applyTheme)
        {
            var SecondaryItems = new string[]
            {
                "Separator","TopBar","TopBar2","TopBar3","TopBar4","ContextMenu",
                "ContextMenu2","ContextMenu3","ContextMenu4","SlidersInActiveBackground",
                "BookmarkBackgroundAndOutlinesSomewhere","GlobalTopBarInActive","GlobalTopBarInActive2",
                "GlobalTopBarInActive3","GlobalTopBarInActive4","TopBarTextFocused","TopBarTextFocused2",
                "TopBarTextFocused3","TopBarTextFocused4","SomeMinimizedListsBackground","CheckBoxesBackground",
                "StabilizerBackground", "PathLineInFileMenuBackground"
            };
            var SecondaryRGBComplicatedItemsAppskinTrue = new string[]
            {
                "FolderBackgroundHovered", "BordersFix17", "BordersFix18"
            };

            var SecondaryRGBComplicatedItemsSrclibsTrue = new string[] {
                "SelectedElementBackgroundIdle","SelectedElementBackgroundActive",
                "SelectedElementBackgroundHovered","FileMenuListElementsBackgroundHovered",
                "BurgerButtonsOutlineFix"
            };



            ApplyColor(applyTheme["Secondary"], SecondaryItems);

            ApplyComplicatedColor(applyTheme["Secondary"],
            SecondaryRGBComplicatedItemsAppskinTrue,
            SaiAddress["GlobalSectionAppskinFrom"],
            SaiAddress["GlobalSectionAppskinTo"], true);

            ApplyComplicatedColor(applyTheme["Secondary"],
            SecondaryRGBComplicatedItemsSrclibsTrue,
             SaiAddress["GlobalSectionSrclibsFrom"], 
             SaiAddress["GlobalSectionSrclibsTo"], true);

          
        }

        public void TextColorRegion(Dictionary<string, string> applyTheme)
        {
            var TextItems = new string[]
            {
            "BrushesBlueText","BrushesBlueText2","BrushesBlueText3",
            "TopBarText","TopBarText2","TopBarText3",
            "TopBarText4","TopBarTextHovered","TopBarTextHovered2","TopBarTextHovered3",
            "TopBarTextHovered4","FileMenuScrollableText","FileMenuScrollableText2","FileMenuScrollableText3","FileMenuScrollableText4",
            "FileMenuTilesText","FileMenuTilesText2","FileMenuTilesText3","FileMenuTilesText4",
            "BrushesText","BrushesText2","BrushesText3","BrushesText4","BrushesText5","BrushesText6","BrushesText7","BrushesText8","BrushesText9","BrushesText10","BrushesText11","BrushesText12","BrushesText13","BrushesText14","BrushesText15","BrushesText16","BrushesText17",
            "ShitTextInWindows","FolderOverlayText","BrushCircles","WindowTitles",
            "OkCancelButtonsText","OkCancelButtonsTextHovered","OkCancelButtonsTextFocused1","OkCancelButtonsTextFocused2","OkCancelButtonsTextFocused3","OkCancelButtonsTextDisfocused1","OkCancelButtonsTextDisfocused2","OkCancelButtonsTextDisfocused3",
            "ContextMenuContent","ContextMenuContent2","ContextMenuContent3","ContextMenuContent4","ContextMenuContent5","ContextMenuContent6","ContextMenuContent7","ContextMenuContent8","ContextMenuContent9","ContextMenuContent10","ContextMenuContent11",
            "ContextMenuContent12","ContextMenuContent13","ContextMenuContent14","ContextMenuContent15","ContextMenuContent16","ContextMenuContent17","ContextMenuContent18","ContextMenuContent19","ContextMenuContent20","ContextMenuContent21","ContextMenuContent22",
            "ContextMenuContent23","ContextMenuContent24","ContextMenuContent25","ContextMenuContent26","ContextMenuContent27","ContextMenuContent28","ContextMenuContent29","ContextMenuContent30","ContextMenuContent31","ContextMenuContent32","ContextMenuContent33",
            "ContextMenuContent34","ContextMenuContent35","ContextMenuContent36","ContextMenuContent37","ContextMenuContent38","ContextMenuContent39","ContextMenuContent40","ContextMenuContent41","ContextMenuContent42","ContextMenuContent43","ContextMenuContent44",
            "ContextMenuContent45","ContextMenuContent46","ContextMenuContent47","ContextMenuContent48","ContextMenuContent49","ContextMenuContent50","ContextMenuContent51","ContextMenuContent52","ContextMenuContent53","ContextMenuContent54","ContextMenuContent55",
            "ContextMenuContent56","ContextMenuContent57","ContextMenuContent58","ContextMenuContent59","ContextMenuContent60","ContextMenuContent61","ContextMenuContent62","ContextMenuContent63","ContextMenuContent64","ContextMenuContent65","ContextMenuContent66",
            "ContextMenuContent67","ContextMenuContent68","ContextMenuContent69","ContextMenuContent70","ContextMenuContent71","ContextMenuContent72","ContextMenuContent73","ContextMenuContent74","ContextMenuContent75","ContextMenuContent76","ContextMenuContent77",
            "ContextMenuContent78","ContextMenuContent79","ContextMenuContent80","ContextMenuContent81","ContextMenuContent82","ContextMenuContent83","ContextMenuContent84","ContextMenuContent85","ContextMenuContent86","ContextMenuContent87","ContextMenuContent88",
            "ContextMenuContent89","ContextMenuContent90","ContextMenuContent91","ContextMenuContent92","ContextMenuContent93","ContextMenuContent94","ContextMenuContent95","ContextMenuContent96","ContextMenuContent97","ContextMenuContent98","ContextMenuContent99",
            "ContextMenuContent100","ContextMenuContent101","ContextMenuContent102","ContextMenuContent103","ContextMenuContent104","ContextMenuContent105","ContextMenuContent106","ContextMenuContent107","ContextMenuContent108","ContextMenuContent109","ContextMenuContent110",
            "ContextMenuContent111","ContextMenuContent112","ContextMenuContent113","ContextMenuContent114","ContextMenuContent115","ContextMenuContent116","ContextMenuContent117","ContextMenuContent118","ContextMenuContent119","ContextMenuContent120"
            };

            var TextItemSrcLib = new string[]
            {
                "ShitColoredText", "FileMenuTreeText"
            };


            ApplyColor(applyTheme["Text"], TextItems);

            ApplyComplicatedColor(applyTheme["Text"], TextItemSrcLib, 
                SaiAddress["GlobalSectionSrclibsFrom"],
                SaiAddress["GlobalSectionSrclibsTo"], false);
      

        }

        public void SelectedOne(Dictionary<string, string> applyTheme)
        {
            var SelectablePrimaryItems = new string[]
            {
                "SlidersColor","saiFileInMenuBelowText","ButtonsInLayersFill","BlueNoteText"
            };

            var SelectablePrimaryComplicatedItemsTextTrue = new string[]
            {
                "BlueSelectableElementsText"
            };
            var SelectablePrimaryRGBComplicatedItemsSrclibsTrue = new string[]
            {
                "SelectedElementOutlineActive","SelectedElementOutlineHovered","SelectedElementOutlineIdle","SelectedElementBackgroundFocused",
                "SelectedElementOutlineFix1","SelectedElementOutlineFix2","SelectedElementOutlineFix3",
                "SelectedElementOutlineFix5","SelectedElementOutlineFix6","SelectedElementOutlineFix7",
                "SelectedElementOutlineFix8","SelectedElementOutlineFix9","SelectedElementOutlineFix10",
                "SelectedElementOutlineFix11","ScrollBarFillHovered","YesNoButtonsBackground",
                "ScrollBarAndServiceButtonsFill","saiFileInMenuBelowBackgroundHovered",
                "FileMenuListElementsOutlineDefault","FileMenuTreeTextFocused","BlueSelectableElements",
                "ServiceButtonsOutline", "ServiceButtonsOutlineFix1","ServiceButtonsOutlineFix2","ServiceButtonsOutlineFocused",
                "ServiceButtonsOutlineFocusedFix1","ServiceButtonsOutlineFocusedFix2",
                "ServiceButtonsBackgroundAndOutlineFocused","BrushesBackgroundGrabbed","BrushesOutlineGrabbed",
                "ServiceButtonsBackground2","ServiceButtonsOutline2","ServiceButtonsOutline2Fix1","ServiceButtonsOutline2Fix2",
                "ServiceButtonsOutline2Fix3","ServiceButtonsOutline2Fix4","ServiceButtonsBackground3",
                "ServiceButtonsOutline3","ServiceButtonsOutline3Fix1","ServiceButtonsOutline3Fix2","ServiceButtonsOutline3Fix3",
                "AnotherSelectableOutlineFocused","AnotherSelectableOutlineFocusedFix1","AnotherSelectableOutlineFocusedFix2",
                "AnotherSelectableInnerOutlineFocused","AnotherSelectableInnerOutlineFocusedFix","SelectableBackgroundRightClicked",
                "BurgerButtonsOutlineRightClicked1","BurgerButtonsOutlineRightClicked2","BurgerButtonsOutlineRightClicked3",
                "BurgerButtonsOutlineRightClicked4","BurgerButtonsOutlineRightClicked4" 
            };

            var SelectablePrimaryRGBComplicatedItemsAppskinTrue = new string[]
            {
                "BlueFixesHovered","GreyFixesClosed","SelectedLayerOutlineActiveHovered",
                "SelectedLayerOutlineFocused","SelectedLayerInnerOutlineActive","SelectedLayerInnerOutlineHovered","SelectedLayerInnerOutlineFocused",
                "SelectedLayerInnerOutlineGrabbed","LayerBackgroundFocused","SelectedLayerBackgroundGrabbed","LayerBackgroundGrabbed",
                "FileMenuListCategoryArrows","FolderOutlineSelected","FolderInnerOutlineSelected","FolderBackgroundSelectedFocused2","FolderOutlineSelectedFocused",
                "FolderInnerOutlineSelectedHovered","FolderInnerOutlineSelectedFocused1","FolderInnerOutlineSelectedFocused2","FolderServiceButtonsSelectedFocused"
            };

            ApplyColor(applyTheme["SelectablePrimary"], SelectablePrimaryItems);

            ApplyComplicatedColor(applyTheme["SelectablePrimary"], SelectablePrimaryComplicatedItemsTextTrue,
                SaiAddress["GlobalSectionTextFrom"],
                SaiAddress["GlobalSectionTextTo"], true);

            ApplyComplicatedColor(applyTheme["SelectablePrimary"], SelectablePrimaryRGBComplicatedItemsSrclibsTrue,
                SaiAddress["GlobalSectionSrclibsFrom"],
                SaiAddress["GlobalSectionSrclibsTo"], true);

            ApplyComplicatedColor(applyTheme["SelectablePrimary"], SelectablePrimaryRGBComplicatedItemsAppskinTrue,
                SaiAddress["GlobalSectionAppskinFrom"], 
                SaiAddress["GlobalSectionAppskinTo"], true);

        }

        public void SelectTwo(Dictionary<string, string> applyTheme)
        {
            var SelectableSecondaryItems = new string[] {
                "SlidersActiveBackground", "SlidersActiveBackgroundHoveredFocused", "saiFileInMenuBelowPercents"
            };
            var SelectableSecondaryRGBComplicatedItemsAppskinTrue = new string[] {
                "SelectedLayerBackgroundActive","SelectedLayerBackgroundHovered","SelectedLayerBackgroundFocused",
                "FolderBackgroundFocused1","FolderBackgroundFocused2",
                "FolderBackgroundSelectedFocused1","FolderBackgroundSelected1","FolderBackgroundSelected2"
            };

            var SelectableSecondaryRGBComplicatedItemsSrclibsTrue = new string[] {
                "ScrollBarFillFocused","ScrollBarOutlineHovered","ScrollBarOutlineHoveredFix1","ScrollBarOutlineHoveredFix2",
                "ScrollBarOutlineFocused","ScrollBarOutlineFocusedFix1","ScrollBarOutlineFocusedFix2",
                "YesNoButtonsOutline","YesNoButtonsOutlineFix1","YesNoButtonsOutlineFix2","YesNoButtonsOutlineFix3",
                "YesNoButtonsOutlineFix4","YesNoButtonsOutlineFix5","YesNoButtonsOutlineFix6","YesNoButtonsOutlineFix7","YesNoButtonsOutlineFix8","YesNoButtonsOutlineFix9",
                "YesNoButtonsOutlineFix10","YesNoButtonsOutlineFix11","YesNoButtonsOutlineFix12","YesNoButtonsOutlineFix13","YesNoButtonsOutlineFix14","YesNoButtonsOutlineFix15",
                "ScrollBarAndServiceButtonsOutline","ScrollBarAndServiceButtonsOutlineFix1","ScrollBarAndServiceButtonsOutlineFix2",
                "saiFileInMenuBelowOutlineHovered","FileMenuTreeTextHovered","BrushesButtonsBackgroundRightClicked","BurgerButtonsBackgroundRightClicked",
                "TopbarOutlineFix","TopbarOutlineFix2","TopbarOutlineFix3","TopbarOutlineFix4"
            };


            ApplyColor(applyTheme["SelectableSecondary"], SelectableSecondaryItems);

            ApplyComplicatedColor(applyTheme["SelectableSecondary"], SelectableSecondaryRGBComplicatedItemsAppskinTrue,
            SaiAddress["GlobalSectionAppskinFrom"], SaiAddress["GlobalSectionAppskinTo"], true);

            ApplyComplicatedColor(applyTheme["SelectableSecondary"], SelectableSecondaryRGBComplicatedItemsSrclibsTrue,
            SaiAddress["GlobalSectionSrclibsFrom"], SaiAddress["GlobalSectionSrclibsTo"], true);


        }



        public bool ApplyTheme(THEME applyTheme)
        {
            if (!FileS2CE.IsOriginalFileExists())
            {
                File.Copy(PathS2CE.Sai2, PathS2CE.OriginalSai2);
            }
            //File.Copy(PathS2CE.Sai2, PathS2CE.OldSai2);

            if (applyTheme.Name == "Default")
            {
                FileS2CE.PutOriginalOn();
                Run(false);
                Application.Current.Shutdown();
                return true;
            }


            if (!FileS2CE.IsOldFileExists()) { FileS2CE.CreateOldFile(); }


            if (applyTheme.Color is null || SaiAddress is null || SaiRGB is null)
                return false;

            ColorS2.ConfigureArtifactsColors(applyTheme.Color["Secondary"].ToByteColor().RemoveAlpha(), applyTheme.Color["Ternary"].ToByteColor().RemoveAlpha());


            //colorRGB.ConfigureArtifactsColors(themeColor["Secondary"].NoAlpha(), themeColor["Ternary"].NoAlpha());

            // Color picker. Little bit chunky, but not bad at all:
            Thread partOne = new Thread(() =>
            {
                theme.FixColorPicker(applyTheme.Color["Ternary"].ToByteColor().RemoveAlpha(), SaiAddress["ColorCircleFrom"], SaiAddress["ColorCircleTo"]);
            })
            { IsBackground = true };

            Thread partTwo = new Thread(() =>
            {
                theme.FixColorPicker(applyTheme.Color["Ternary"].ToByteColor().RemoveAlpha(), SaiAddress["ColorCircleFrom2"], SaiAddress["ColorCircleTo2"]);
            })
            { IsBackground = true };

            Thread partThree = new Thread(() =>
            {
                theme.FixColorPicker(applyTheme.Color["Ternary"].ToByteColor().RemoveAlpha(), SaiAddress["ColorCircleFrom3"], SaiAddress["ColorCircleTo3"]);
            })
            { IsBackground = true };

            Thread partFour = new Thread(() =>
            {
                theme.FixColorPicker(applyTheme.Color["Ternary"].ToByteColor().RemoveAlpha(), SaiAddress["ColorCircleFrom4"], SaiAddress["ColorCircleTo4"]);
            })
            { IsBackground = true };

            partOne.Start();
            partTwo.Start();
            partThree.Start();
            partFour.Start();

            //ColorPick(applyTheme.Color);
            /*
             * 
            RegionPrimary(applyTheme.Color);
            SecundaryColorRegion(applyTheme.Color);
            TextColorRegion(applyTheme.Color);
            SelectedOne(applyTheme.Color);
            SelectTwo(applyTheme.Color);
            */


            #region 0 PRIMARY COLOR
            theme.SetElementColorComplicated(SaiRGB["SlidersBackgroundTransparent2"].RemoveAlpha(), applyTheme.Color["Primary"].ToByteColor().RemoveAlpha(), SaiAddress["SlidersBackgroundTransparentFrom"], SaiAddress["SlidersBackgroundTransparentTo"]);
            theme.SetElementColorComplicated(SaiRGB["ActiveCanvasBackgroundFix"], applyTheme.Color["Primary"].ToByteColor(), SaiAddress["GlobalSectionTextFrom"], SaiAddress["GlobalSectionTextTo"], true);
            #endregion

            #region 0 SECONDARY COLOR
            theme.SetElementColorWithTotalReplacment(applyTheme.Color["Secondary"].ToByteColor().RemoveAlpha(), SaiAddress["HoveredEmptyBrushesBackgroundFrom"], SaiAddress["HoveredEmptyBrushesBackgroundTo"]);
            theme.SetElementColorWithTotalReplacment(applyTheme.Color["Secondary"].ToByteColor().RemoveAlpha(), SaiAddress["HoveredLayersBackgroundFrom"], SaiAddress["HoveredLayersBackgroundTo"]);
            theme.SetElementColorComplicated(SaiRGB["SelectedElementBackgroundHovered"].RemoveAlpha(), applyTheme.Color["Secondary"].ToByteColor().RemoveAlpha(), SaiAddress["HoveredLayersBackgroundFrom2"], SaiAddress["HoveredLayersBackgroundTo2"], true);
            theme.SetElementColorComplicated(SaiRGB["SlidersBackgroundTransparent1"].RemoveAlpha(), applyTheme.Color["Secondary"].ToByteColor().RemoveAlpha(), SaiAddress["SlidersBackgroundTransparentFrom"], SaiAddress["SlidersBackgroundTransparentTo"]);
            byte[][] SlidersPrivelegySelectableTernaryTrue = new byte[][]
            {
                SaiRGB["SlidersBackgroundTransparentLineFix1"],
                SaiRGB["SlidersBackgroundTransparentLineFix2"]
            };

            foreach (var n in SlidersPrivelegySelectableTernaryTrue)
            {
                theme.SetElementColorComplicated(
                    n.RemoveAlpha(),
                    applyTheme.Color["Secondary"].ToByteColor().RemoveAlpha(),
                    SaiAddress["SlidersBackgroundTransparentFrom"],
                    SaiAddress["SlidersBackgroundTransparentTo"]
                );
            }

            #endregion

            #region 0 TERNARY COLOR
            theme.SetElementColorComplicated(SaiRGB["LayerBackground"], applyTheme.Color["Ternary"].ToByteColor().RemoveAlpha(), SaiAddress["LayerBackgroundFrom"], SaiAddress["LayerBackgroundTo"], true);
            theme.SetElementColorComplicated(SaiRGB["BrushesBackgroundFileMenuBackgroundScrollBlockBackground"], applyTheme.Color["Ternary"].ToByteColor().RemoveAlpha(), SaiAddress["BrushesFileMenuTilesScrollableListsBackgroundFrom"], SaiAddress["BrushesFileMenuTilesScrollableListsBackgroundTo"]);
            #endregion

            #region 0 SELECTABLE PRIMARY COLOR
            byte[][] SlidersPrivelegySelectablePrimaryTrue = new byte[][]{
                SaiRGB["SlidersBarBackgroundTransparent1"],
                SaiRGB["SlidersBarBackgroundTransparentLineFix1"]
            };
            foreach (byte[] n in SlidersPrivelegySelectablePrimaryTrue)
            {
                theme.SetElementColorComplicated(n.RemoveAlpha(), applyTheme.Color["SelectablePrimary"].ToByteColor().RemoveAlpha(), SaiAddress["SlidersBackgroundTransparentFrom"], SaiAddress["SlidersBackgroundTransparentTo"]);
            }
            #endregion

            #region 0 SELECTABLE SECONDARY COLOR
            theme.SetElementColorComplicated(SaiRGB["SlidersBarBackgroundTransparent2"].RemoveAlpha(), applyTheme.Color["SelectableSecondary"].ToByteColor().RemoveAlpha(), SaiAddress["SlidersBackgroundTransparentFrom"], SaiAddress["SlidersBackgroundTransparentTo"]);
            #endregion

            // Main regions, which contains basic coloring operations:
            #region PRIMARY COLOR
            int[] PrimaryItems = new int[] {
                SaiAddress["ActiveCanvasBackground"],
                SaiAddress["ActiveCanvasBackground2"],
                SaiAddress["ActiveCanvasBackground3"],
                SaiAddress["ActiveCanvasBackground4"],
                SaiAddress["InActiveCanvasBackground"],
                SaiAddress["BehindLayersUIBackground"],
                SaiAddress["BehindLayersUIBackground2"],
                SaiAddress["BehindLayersUIBackground3"],
                SaiAddress["BehindLayersUIBackground4"],
                SaiAddress["BrushBorders"],
                SaiAddress["SlidersVertical"],
                SaiAddress["SlidersHorizontal"],
                SaiAddress["InActiveText"],
                SaiAddress["SliderBarHovered"],
                SaiAddress["ScrollBarArrowUp"],
                SaiAddress["ScrollBarArrowDown"],
                SaiAddress["ScrollBarArrowLeft"],
                SaiAddress["ScrollBarArrowRight"],
                SaiAddress["ClosedListArrow"],
                SaiAddress["GlobalTopBar"],
                SaiAddress["GlobalTopBar2"],
                SaiAddress["GlobalTopBar3"],
                SaiAddress["GlobalTopBar4"],
                SaiAddress["GlobalTopBar5"],
                SaiAddress["GlobalTopBar6"],
                SaiAddress["GlobalTopBar7"],
                SaiAddress["GlobalTopBar8"],
                SaiAddress["ScaleAngleArrow"],
                SaiAddress["ColorSlidersArrows"],
                SaiAddress["GreyNoteText"],
                SaiAddress["SettingsListBackground"],
                SaiAddress["SettingsListBackground2"],
                SaiAddress["SettingsListBackground3"],
                SaiAddress["SettingsListBackground4"],
                SaiAddress["OkCancelButtonsTextInActive"],
                SaiAddress["AssetManagerLeftBackground"],
                SaiAddress["ScaleAngleSliders2"],
                SaiAddress["ScaleAngleSliders3"],
                SaiAddress["ScaleAngleSliders4"],
                SaiAddress["ScaleAngleSliders5"],
                SaiAddress["ScaleAngleSliders6"],
                SaiAddress["ScaleAngleSliders7"],
                SaiAddress["ScaleAngleSliders8"],
                SaiAddress["ScaleAngleSliders9"],
                SaiAddress["ScaleAngleSliders10"],
                SaiAddress["ScaleAngleSliders11"],
                SaiAddress["ScaleAngleSliders12"],
                SaiAddress["ScaleAngleSliders13"],
                SaiAddress["ScaleAngleSliders14"],
                SaiAddress["ScaleAngleSliders15"]
            };
            foreach (int n in PrimaryItems)
            {
                theme.SetElementColor(applyTheme.Color["Primary"].ToByteColor().RemoveAlpha(), n);
            }

            byte[][] PrimaryRGBComplicatedItemsSrclibsTrue = new byte[][]{
                SaiRGB["FileMenuTreeTabsFix2"],
                SaiRGB["BurgerButtonsOutline1"],
                SaiRGB["BurgerButtonsOutline3"],
                SaiRGB["BordersFix9"],
                SaiRGB["saiFileInMenuBelowOutline"],
                SaiRGB["saiFileInMenuBelowOutlineFix"],
                SaiRGB["saiFileInMenuBelowInnerOutline"],
                SaiRGB["BurgerButtonsOutlineSlidersOutline"],
                SaiRGB["BurgerButtonsOutline4"],
                SaiRGB["BurgerButtonsOutline2"],
                SaiRGB["BordersFix1"],
                SaiRGB["BordersFix2"],
                SaiRGB["BordersFix3"],
                SaiRGB["BordersFix4"],
                SaiRGB["BordersFix5"],
                SaiRGB["BordersFix6"],
                SaiRGB["BordersFix7"],
                SaiRGB["BordersFix8"],
                SaiRGB["BordersFix10"],
                SaiRGB["BordersFix11"],
                SaiRGB["BordersFix12"],
                SaiRGB["BordersFix14"],
                SaiRGB["Separators"],
                SaiRGB["FileMenuListElementsBackgroundDefault"],
                SaiRGB["InActiveBurgerButtonsOutline"],
                SaiRGB["InActiveBurgerButtonsOutlineFix1"],
                SaiRGB["InActiveBurgerButtonsOutlineFix2"],
            };
            foreach (byte[] n in PrimaryRGBComplicatedItemsSrclibsTrue)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["Primary"].ToByteColor().RemoveAlpha(), SaiAddress["GlobalSectionSrclibsFrom"], SaiAddress["GlobalSectionSrclibsTo"], true);
            }

            byte[][] PrimaryRGBComplicatedItemsAppskinTrue = new byte[][]{
                SaiRGB["BordersFix13"],
                SaiRGB["BordersFix15"],
                SaiRGB["BordersFix16"],
                SaiRGB["LayerOutline"],
                SaiRGB["FileMenuBackground"],
                SaiRGB["FolderBehindBackground1"],
                SaiRGB["FolderBehindBackground2"],
            };
            foreach (byte[] n in PrimaryRGBComplicatedItemsAppskinTrue)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["Primary"].ToByteColor().RemoveAlpha(), SaiAddress["GlobalSectionAppskinFrom"], SaiAddress["GlobalSectionAppskinTo"], true);
            }
            #endregion

            #region SECONDARY COLOR
            int[] SecondaryItems = new int[] {
                SaiAddress["Separator"],
                SaiAddress["TopBar"],
                SaiAddress["TopBar2"],
                SaiAddress["TopBar3"],
                SaiAddress["TopBar4"],
                SaiAddress["ContextMenu"],
                SaiAddress["ContextMenu2"],
                SaiAddress["ContextMenu3"],
                SaiAddress["ContextMenu4"],
                SaiAddress["SlidersInActiveBackground"],
                SaiAddress["BookmarkBackgroundAndOutlinesSomewhere"],
                SaiAddress["GlobalTopBarInActive"],
                SaiAddress["GlobalTopBarInActive2"],
                SaiAddress["GlobalTopBarInActive3"],
                SaiAddress["GlobalTopBarInActive4"],
                SaiAddress["TopBarTextFocused"],
                SaiAddress["TopBarTextFocused2"],
                SaiAddress["TopBarTextFocused3"],
                SaiAddress["TopBarTextFocused4"],
                SaiAddress["SomeMinimizedListsBackground"],
                SaiAddress["CheckBoxesBackground"],
                SaiAddress["StabilizerBackground"],
                SaiAddress["PathLineInFileMenuBackground"],
            };
            foreach (int n in SecondaryItems)
            {
                theme.SetElementColor(applyTheme.Color["Secondary"].ToByteColor().RemoveAlpha(), n);
            }


            byte[][] SecondaryRGBComplicatedItemsAppskinTrue = new byte[][]{
                SaiRGB["FolderBackgroundHovered"],
                SaiRGB["BordersFix17"],
                SaiRGB["BordersFix18"],
            };
            foreach (byte[] n in SecondaryRGBComplicatedItemsAppskinTrue)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["Secondary"].ToByteColor().RemoveAlpha(), SaiAddress["GlobalSectionAppskinFrom"], SaiAddress["GlobalSectionAppskinTo"], true);
            }


            byte[][] SecondaryRGBComplicatedItemsSrclibsTrue = new byte[][]{
                SaiRGB["SelectedElementBackgroundIdle"],
                SaiRGB["SelectedElementBackgroundActive"],
                SaiRGB["SelectedElementBackgroundHovered"],
                SaiRGB["FileMenuListElementsBackgroundHovered"],
                SaiRGB["BurgerButtonsOutlineFix"],
                ColorS2.SecondaryArtifactsColor1,
                ColorS2.SecondaryArtifactsColor2,
                ColorS2.SecondaryArtifactsColor3,
                ColorS2.SecondaryArtifactsColor4,
            };
            foreach (byte[] n in SecondaryRGBComplicatedItemsSrclibsTrue)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["Secondary"].ToByteColor().RemoveAlpha(), SaiAddress["GlobalSectionSrclibsFrom"], SaiAddress["GlobalSectionSrclibsTo"], true);
            }
            #endregion

            #region TERNARY COLOR
            int[] TernaryItems = new int[] {
                SaiAddress["ScaleAngleSlidersBg"],
                SaiAddress["GlobalBorders"],
                SaiAddress["GlobalBorders2"],
                SaiAddress["TabsResizeGrabberVertical"],
                SaiAddress["ResizeWindowGrabber"],
                SaiAddress["ContextMenuListBackground"],
                SaiAddress["ContextMenuListSeparatorBackground"],
            };
            foreach (int n in TernaryItems)
            {
                theme.SetElementColor(applyTheme.Color["Ternary"].ToByteColor().RemoveAlpha(), n);
            }

            byte[][] TernaryRGBComplicatedItemsAppskinTrue = new byte[][] {
                SaiRGB["EmptyElementsInBrushesUI"],
                SaiRGB["FolderBackground"],
            };
            foreach (byte[] n in TernaryRGBComplicatedItemsAppskinTrue)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["Ternary"].ToByteColor().RemoveAlpha(), SaiAddress["GlobalSectionAppskinFrom"], SaiAddress["GlobalSectionAppskinTo"], true);
            }

            byte[][] TernaryRGBComplicatedItemsSrclibsTrue = new byte[][] {
SaiRGB["BordersFixBurgerButtonsBackgroundColorLinesBackground"],
SaiRGB["EmptyScrollBarBackground"],
SaiRGB["ScrollBarOutlineHoveredFix3"],
SaiRGB["saiFileInMenuBelowBackground"],
SaiRGB["FileMenuTreeTabsFix3"],
SaiRGB["FileMenuTreeTabsFix4"],
SaiRGB["FileMenuTreeTabsFix5"],
SaiRGB["FileMenuTreeTabsFix6"],
SaiRGB["FileMenuTreeTabsFix7"],
SaiRGB["FileMenuTreeTabsFix8"],
                ColorS2.TernaryArtifactsColor1,
                ColorS2.TernaryArtifactsColor2,
                ColorS2.TernaryArtifactsColor3,
                ColorS2.TernaryArtifactsColor4,
                ColorS2.TernaryArtifactsColor5,
                ColorS2.TernaryArtifactsColor6,
                ColorS2.TernaryArtifactsColor7,
                ColorS2.TernaryArtifactsColor8,
            };
            foreach (byte[] n in TernaryRGBComplicatedItemsSrclibsTrue)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["Ternary"].ToByteColor().RemoveAlpha(), SaiAddress["GlobalSectionSrclibsFrom"], SaiAddress["GlobalSectionSrclibsTo"], true);
            }
            #endregion

            #region TEXT COLOR
            int[] TextItems = new int[]{
                SaiAddress["BrushesBlueText"],
                SaiAddress["BrushesBlueText2"],
                SaiAddress["BrushesBlueText3"],
                SaiAddress["TopBarText"],
                SaiAddress["TopBarText2"],
                SaiAddress["TopBarText3"],
                SaiAddress["TopBarText4"],
                SaiAddress["TopBarTextHovered"],
                SaiAddress["TopBarTextHovered2"],
                SaiAddress["TopBarTextHovered3"],
                SaiAddress["TopBarTextHovered4"],
                SaiAddress["FileMenuScrollableText"],
                SaiAddress["FileMenuScrollableText2"],
                SaiAddress["FileMenuScrollableText3"],
                SaiAddress["FileMenuScrollableText4"],
                SaiAddress["FileMenuTilesText"],
                SaiAddress["FileMenuTilesText2"],
                SaiAddress["FileMenuTilesText3"],
                SaiAddress["FileMenuTilesText4"],
                SaiAddress["BrushesText"],
                SaiAddress["BrushesText2"],
                SaiAddress["BrushesText3"],
                SaiAddress["BrushesText4"],
                SaiAddress["BrushesText5"],
                SaiAddress["BrushesText6"],
                SaiAddress["BrushesText7"],
                SaiAddress["BrushesText8"],
                SaiAddress["BrushesText9"],
                SaiAddress["BrushesText10"],
                SaiAddress["BrushesText11"],
                SaiAddress["BrushesText12"],
                SaiAddress["BrushesText13"],
                SaiAddress["BrushesText14"],
                SaiAddress["BrushesText15"],
                SaiAddress["BrushesText16"],
                SaiAddress["BrushesText17"],
                SaiAddress["ShitTextInWindows"],
                SaiAddress["FolderOverlayText"],
                SaiAddress["BrushCircles"],
                SaiAddress["WindowTitles"],
                SaiAddress["OkCancelButtonsText"],
                SaiAddress["OkCancelButtonsTextHovered"],
                SaiAddress["OkCancelButtonsTextFocused1"],
                SaiAddress["OkCancelButtonsTextFocused2"],
                SaiAddress["OkCancelButtonsTextFocused3"],
                SaiAddress["OkCancelButtonsTextDisfocused1"],
                SaiAddress["OkCancelButtonsTextDisfocused2"],
                SaiAddress["OkCancelButtonsTextDisfocused3"],
                SaiAddress["ContextMenuContent"],
                SaiAddress["ContextMenuContent2"],
                SaiAddress["ContextMenuContent3"],
                SaiAddress["ContextMenuContent4"],
                SaiAddress["ContextMenuContent5"],
                SaiAddress["ContextMenuContent6"],
                SaiAddress["ContextMenuContent7"],
                SaiAddress["ContextMenuContent8"],
                SaiAddress["ContextMenuContent9"],
                SaiAddress["ContextMenuContent10"],
                SaiAddress["ContextMenuContent11"],
                SaiAddress["ContextMenuContent12"],
                SaiAddress["ContextMenuContent13"],
                SaiAddress["ContextMenuContent14"],
                SaiAddress["ContextMenuContent15"],
                SaiAddress["ContextMenuContent16"],
                SaiAddress["ContextMenuContent17"],
                SaiAddress["ContextMenuContent18"],
                SaiAddress["ContextMenuContent19"],
                SaiAddress["ContextMenuContent20"],
                SaiAddress["ContextMenuContent21"],
                SaiAddress["ContextMenuContent22"],
                SaiAddress["ContextMenuContent23"],
                SaiAddress["ContextMenuContent24"],
                SaiAddress["ContextMenuContent25"],
                SaiAddress["ContextMenuContent26"],
                SaiAddress["ContextMenuContent27"],
                SaiAddress["ContextMenuContent28"],
                SaiAddress["ContextMenuContent29"],
                SaiAddress["ContextMenuContent30"],
                SaiAddress["ContextMenuContent31"],
                SaiAddress["ContextMenuContent32"],
                SaiAddress["ContextMenuContent33"],
                SaiAddress["ContextMenuContent34"],
                SaiAddress["ContextMenuContent35"],
                SaiAddress["ContextMenuContent36"],
                SaiAddress["ContextMenuContent37"],
                SaiAddress["ContextMenuContent38"],
                SaiAddress["ContextMenuContent39"],
                SaiAddress["ContextMenuContent40"],
                SaiAddress["ContextMenuContent41"],
                SaiAddress["ContextMenuContent42"],
                SaiAddress["ContextMenuContent43"],
                SaiAddress["ContextMenuContent44"],
                SaiAddress["ContextMenuContent45"],
                SaiAddress["ContextMenuContent46"],
                SaiAddress["ContextMenuContent47"],
                SaiAddress["ContextMenuContent48"],
                SaiAddress["ContextMenuContent49"],
                SaiAddress["ContextMenuContent50"],
                SaiAddress["ContextMenuContent51"],
                SaiAddress["ContextMenuContent52"],
                SaiAddress["ContextMenuContent53"],
                SaiAddress["ContextMenuContent54"],
                SaiAddress["ContextMenuContent55"],
                SaiAddress["ContextMenuContent56"],
                SaiAddress["ContextMenuContent57"],
                SaiAddress["ContextMenuContent58"],
                SaiAddress["ContextMenuContent59"],
                SaiAddress["ContextMenuContent60"],
                SaiAddress["ContextMenuContent61"],
                SaiAddress["ContextMenuContent62"],
                SaiAddress["ContextMenuContent63"],
                SaiAddress["ContextMenuContent64"],
                SaiAddress["ContextMenuContent65"],
                SaiAddress["ContextMenuContent66"],
                SaiAddress["ContextMenuContent67"],
                SaiAddress["ContextMenuContent68"],
                SaiAddress["ContextMenuContent69"],
                SaiAddress["ContextMenuContent70"],
                SaiAddress["ContextMenuContent71"],
                SaiAddress["ContextMenuContent72"],
                SaiAddress["ContextMenuContent73"],
                SaiAddress["ContextMenuContent74"],
                SaiAddress["ContextMenuContent75"],
                SaiAddress["ContextMenuContent76"],
                SaiAddress["ContextMenuContent77"],
                SaiAddress["ContextMenuContent78"],
                SaiAddress["ContextMenuContent79"],
                SaiAddress["ContextMenuContent80"],
                SaiAddress["ContextMenuContent81"],
                SaiAddress["ContextMenuContent82"],
                SaiAddress["ContextMenuContent83"],
                SaiAddress["ContextMenuContent84"],
                SaiAddress["ContextMenuContent85"],
                SaiAddress["ContextMenuContent86"],
                SaiAddress["ContextMenuContent87"],
                SaiAddress["ContextMenuContent88"],
                SaiAddress["ContextMenuContent89"],
                SaiAddress["ContextMenuContent90"],
                SaiAddress["ContextMenuContent91"],
                SaiAddress["ContextMenuContent92"],
                SaiAddress["ContextMenuContent93"],
                SaiAddress["ContextMenuContent94"],
                SaiAddress["ContextMenuContent95"],
                SaiAddress["ContextMenuContent96"],
                SaiAddress["ContextMenuContent97"],
                SaiAddress["ContextMenuContent98"],
                SaiAddress["ContextMenuContent99"],
                SaiAddress["ContextMenuContent100"],
                SaiAddress["ContextMenuContent101"],
                SaiAddress["ContextMenuContent102"],
                SaiAddress["ContextMenuContent103"],
                SaiAddress["ContextMenuContent104"],
                SaiAddress["ContextMenuContent105"],
                SaiAddress["ContextMenuContent106"],
                SaiAddress["ContextMenuContent107"],
                SaiAddress["ContextMenuContent108"],
                SaiAddress["ContextMenuContent109"],
                SaiAddress["ContextMenuContent110"],
                SaiAddress["ContextMenuContent111"],
                SaiAddress["ContextMenuContent112"],
                SaiAddress["ContextMenuContent113"],
                SaiAddress["ContextMenuContent114"],
                SaiAddress["ContextMenuContent115"],
                SaiAddress["ContextMenuContent116"],
                SaiAddress["ContextMenuContent117"],
                SaiAddress["ContextMenuContent118"],
                SaiAddress["ContextMenuContent119"],
                SaiAddress["ContextMenuContent120"],
            };
            foreach (int n in TextItems)
            {
                theme.SetElementColor(applyTheme.Color["Text"].ToByteColor().RemoveAlpha(), n);
            }

            byte[][] TextComplicatedItemsSrclibs = new byte[][]{
                // This should has alpha channel to be replaced properly
SaiRGB["ShitColoredText"],
SaiRGB["FileMenuTreeText"],
            };
            foreach (byte[] n in TextComplicatedItemsSrclibs)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["Text"].ToByteColor(), SaiAddress["GlobalSectionSrclibsFrom"], SaiAddress["GlobalSectionSrclibsTo"]);
            }
            #endregion

            #region SELECTABLE PRIMARY COLOR
            int[] SelectablePrimaryItems = new int[]{
                SaiAddress["SlidersColor"],
                SaiAddress["saiFileInMenuBelowText"],
                SaiAddress["ButtonsInLayersFill"],
                SaiAddress["BlueNoteText"],
            };
            foreach (int n in SelectablePrimaryItems)
            {
                theme.SetElementColor(applyTheme.Color["SelectablePrimary"].ToByteColor().RemoveAlpha(), n);
            }

            byte[][] SelectablePrimaryComplicatedItemsTextTrue = new byte[][]{
                SaiRGB["BlueSelectableElementsText"],
            };
            foreach (byte[] n in SelectablePrimaryComplicatedItemsTextTrue)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["SelectablePrimary"].ToByteColor().RemoveAlpha(), SaiAddress["GlobalSectionTextFrom"], SaiAddress["GlobalSectionTextTo"], true);
            }

            byte[][] SelectablePrimaryRGBComplicatedItemsSrclibsTrue = new byte[][]{
                SaiRGB["SelectedElementOutlineActive"],
 SaiRGB["SelectedElementOutlineHovered"],
 SaiRGB["SelectedElementOutlineIdle"],
 SaiRGB["SelectedElementBackgroundFocused"],
 SaiRGB["SelectedElementOutlineFix1"],
 SaiRGB["SelectedElementOutlineFix2"],
 SaiRGB["SelectedElementOutlineFix3"],
 SaiRGB["SelectedElementOutlineFix5"],
 SaiRGB["SelectedElementOutlineFix6"],
 SaiRGB["SelectedElementOutlineFix7"],
 SaiRGB["SelectedElementOutlineFix8"],
 SaiRGB["SelectedElementOutlineFix9"],
 SaiRGB["SelectedElementOutlineFix10"],
 SaiRGB["SelectedElementOutlineFix11"],
 SaiRGB["ScrollBarFillHovered"],
 SaiRGB["YesNoButtonsBackground"],
 SaiRGB["ScrollBarAndServiceButtonsFill"],
 SaiRGB["saiFileInMenuBelowBackgroundHovered"],
 SaiRGB["FileMenuListElementsOutlineDefault"],
 SaiRGB["FileMenuTreeTextFocused"],
 SaiRGB["BlueSelectableElements"],
 SaiRGB["ServiceButtonsOutline"],
 SaiRGB["ServiceButtonsOutlineFix1"],
 SaiRGB["ServiceButtonsOutlineFix2"],
 SaiRGB["ServiceButtonsOutlineFocused"],
 SaiRGB["ServiceButtonsOutlineFocusedFix1"],
 SaiRGB["ServiceButtonsOutlineFocusedFix2"],
 SaiRGB["ServiceButtonsBackgroundAndOutlineFocused"],
 SaiRGB["BrushesBackgroundGrabbed"],
 SaiRGB["BrushesOutlineGrabbed"],
 SaiRGB["ServiceButtonsBackground2"],
 SaiRGB["ServiceButtonsOutline2"],
 SaiRGB["ServiceButtonsOutline2Fix1"],
 SaiRGB["ServiceButtonsOutline2Fix2"],
 SaiRGB["ServiceButtonsOutline2Fix3"],
 SaiRGB["ServiceButtonsOutline2Fix4"],
 SaiRGB["ServiceButtonsBackground3"],
 SaiRGB["ServiceButtonsOutline3"],
 SaiRGB["ServiceButtonsOutline3Fix1"],
 SaiRGB["ServiceButtonsOutline3Fix2"],
 SaiRGB["ServiceButtonsOutline3Fix3"],
 SaiRGB["AnotherSelectableOutlineFocused"],
 SaiRGB["AnotherSelectableOutlineFocusedFix1"],
 SaiRGB["AnotherSelectableOutlineFocusedFix2"],
 SaiRGB["AnotherSelectableInnerOutlineFocused"],
 SaiRGB["AnotherSelectableInnerOutlineFocusedFix"],
 SaiRGB["SelectableBackgroundRightClicked"],
 SaiRGB["BurgerButtonsOutlineRightClicked1"],
 SaiRGB["BurgerButtonsOutlineRightClicked2"],
 SaiRGB["BurgerButtonsOutlineRightClicked3"],
 SaiRGB["BurgerButtonsOutlineRightClicked4"],
 SaiRGB["BurgerButtonsOutlineRightClicked4"],
            };
            foreach (byte[] n in SelectablePrimaryRGBComplicatedItemsSrclibsTrue)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["SelectablePrimary"].ToByteColor().RemoveAlpha(), SaiAddress["GlobalSectionSrclibsFrom"], SaiAddress["GlobalSectionSrclibsTo"], true);
            }

            byte[][] SelectablePrimaryRGBComplicatedItemsAppskinTrue = new byte[][]{
                SaiRGB["BlueFixesHovered"],
                SaiRGB["GreyFixesClosed"],
                SaiRGB["SelectedLayerOutlineActiveHovered"],
                SaiRGB["SelectedLayerOutlineFocused"],
                SaiRGB["SelectedLayerInnerOutlineActive"],
                SaiRGB["SelectedLayerInnerOutlineHovered"],
                SaiRGB["SelectedLayerInnerOutlineFocused"],
                SaiRGB["SelectedLayerInnerOutlineGrabbed"],
                SaiRGB["LayerBackgroundFocused"],
                SaiRGB["SelectedLayerBackgroundGrabbed"],
                SaiRGB["LayerBackgroundGrabbed"],
                SaiRGB["FileMenuListCategoryArrows"],
                SaiRGB["FolderOutlineSelected"],
                SaiRGB["FolderInnerOutlineSelected"],
                SaiRGB["FolderBackgroundSelectedFocused2"],
                SaiRGB["FolderOutlineSelectedFocused"],
                SaiRGB["FolderInnerOutlineSelectedHovered"],
                SaiRGB["FolderInnerOutlineSelectedFocused1"],
                SaiRGB["FolderInnerOutlineSelectedFocused2"],
                SaiRGB["FolderServiceButtonsSelectedFocused"],
            };
            foreach (byte[] n in SelectablePrimaryRGBComplicatedItemsAppskinTrue)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["SelectablePrimary"].ToByteColor().RemoveAlpha(), SaiAddress["GlobalSectionAppskinFrom"], SaiAddress["GlobalSectionAppskinTo"], true);
            }
            #endregion

            #region SELECTABLE SECONDARY COLOR
            int[] SelectableSecondaryItems = new int[]{
                SaiAddress["SlidersActiveBackground"],
                SaiAddress["SlidersActiveBackgroundHoveredFocused"],
                SaiAddress["saiFileInMenuBelowPercents"],
            };
            foreach (int n in SelectableSecondaryItems)
            {
                theme.SetElementColor(applyTheme.Color["SelectableSecondary"].ToByteColor().RemoveAlpha(), n);
            }

            byte[][] SelectableSecondaryRGBComplicatedItemsAppskinTrue = new byte[][]{
                SaiRGB["SelectedLayerBackgroundActive"],
                SaiRGB["SelectedLayerBackgroundHovered"],
                SaiRGB["SelectedLayerBackgroundFocused"],
                SaiRGB["FolderBackgroundFocused1"],
                SaiRGB["FolderBackgroundFocused2"],
                SaiRGB["FolderBackgroundSelectedFocused1"],
                SaiRGB["FolderBackgroundSelected1"],
                SaiRGB["FolderBackgroundSelected2"],
            };
            foreach (byte[] n in SelectableSecondaryRGBComplicatedItemsAppskinTrue)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["SelectableSecondary"].ToByteColor().RemoveAlpha(), SaiAddress["GlobalSectionAppskinFrom"], SaiAddress["GlobalSectionAppskinTo"], true);
            }

            byte[][] SelectableSecondaryRGBComplicatedItemsSrclibsTrue = {
                SaiRGB["ScrollBarFillFocused"],
                SaiRGB["ScrollBarOutlineHovered"],
                SaiRGB["ScrollBarOutlineHoveredFix1"],
                SaiRGB["ScrollBarOutlineHoveredFix2"],
                SaiRGB["ScrollBarOutlineFocused"],
                SaiRGB["ScrollBarOutlineFocusedFix1"],
                SaiRGB["ScrollBarOutlineFocusedFix2"],
                SaiRGB["YesNoButtonsOutline"],
                SaiRGB["YesNoButtonsOutlineFix1"],
                SaiRGB["YesNoButtonsOutlineFix2"],
                SaiRGB["YesNoButtonsOutlineFix3"],
                SaiRGB["YesNoButtonsOutlineFix4"],
                SaiRGB["YesNoButtonsOutlineFix5"],
                SaiRGB["YesNoButtonsOutlineFix6"],
                SaiRGB["YesNoButtonsOutlineFix7"],
                SaiRGB["YesNoButtonsOutlineFix8"],
                SaiRGB["YesNoButtonsOutlineFix9"],
                SaiRGB["YesNoButtonsOutlineFix10"],
                SaiRGB["YesNoButtonsOutlineFix11"],
                SaiRGB["YesNoButtonsOutlineFix12"],
                SaiRGB["YesNoButtonsOutlineFix13"],
                SaiRGB["YesNoButtonsOutlineFix14"],
                SaiRGB["YesNoButtonsOutlineFix15"],
                SaiRGB["ScrollBarAndServiceButtonsOutline"],
                SaiRGB["ScrollBarAndServiceButtonsOutlineFix1"],
                SaiRGB["ScrollBarAndServiceButtonsOutlineFix2"],
                SaiRGB["saiFileInMenuBelowOutlineHovered"],
                SaiRGB["FileMenuTreeTextHovered"],
                SaiRGB["BrushesButtonsBackgroundRightClicked"],
                SaiRGB["BurgerButtonsBackgroundRightClicked"],
                SaiRGB["TopbarOutlineFix"],
                SaiRGB["TopbarOutlineFix2"],
                SaiRGB["TopbarOutlineFix3"],
                SaiRGB["TopbarOutlineFix4"],
            };
            foreach (byte[] n in SelectableSecondaryRGBComplicatedItemsSrclibsTrue)
            {
                theme.SetElementColorComplicated(n, applyTheme.Color["SelectableSecondary"].ToByteColor().RemoveAlpha(), SaiAddress["GlobalSectionSrclibsFrom"], SaiAddress["GlobalSectionSrclibsTo"], true);
            }

            #endregion



            theme.SaveTheme();

            Run(false);
            Application.Current.Shutdown();
            return true;
        }


    }
    class MainTheme
    {

        FileS2CE file = new();
        ThemeS2CE theme = new();
        ColorS2CE colorRGB = new();

        Dictionary<string, byte[]>? themeColor { get; set; }
        Dictionary<string, int>? saiAddress { get; set; }
        Dictionary<string, byte[]>? saiColorRGB { get; set; }

        public void Apply(string themeName, Dictionary<string, byte[]> colors)
        {

            FileS2CE.PutOriginalOn();

            if (themeName == "classic_sai2")
            {
                try
                {
                    //FileS2CE.PutOriginalOn();
                    themes.Run(false);
                    Application.Current.Shutdown();
  
                }
                catch { }
                return;
            }

            // Creating backup file to restore data or replace original file
            // with backup one to recolor it:
            if (!FileS2CE.IsOldFileExists()) { FileS2CE.CreateOldFile(); }



            // Getting theme vaules:
            themeColor = colors;

            // Getting replacment libraries:
            saiAddress = PathTheme.ReadAddress();
            saiColorRGB = PathTheme.ReadSaiColors();

            // Returning if nothing to replace to:
            if (themeColor is null || saiAddress is null || saiColorRGB is null) { return; }

            // colorRGB.ConfigureRGBColors(themeColor, saiColorRGB);
            colorRGB.ConfigureArtifactsColors(themeColor["Secondary"].RemoveAlpha(), themeColor["Ternary"].RemoveAlpha());

            // Color picker. Little bit chunky, but not bad at all:
            Thread partOne = new Thread(() =>
            {
                theme.FixColorPicker(themeColor["Ternary"].RemoveAlpha(), saiAddress["ColorCircleFrom"], saiAddress["ColorCircleTo"]);
            })
            { IsBackground = true };

            Thread partTwo = new Thread(() =>
            {
                theme.FixColorPicker(themeColor["Ternary"].RemoveAlpha(), saiAddress["ColorCircleFrom2"], saiAddress["ColorCircleTo2"]);
            })
            { IsBackground = true };

            Thread partThree = new Thread(() =>
            {
                theme.FixColorPicker(themeColor["Ternary"].RemoveAlpha(), saiAddress["ColorCircleFrom3"], saiAddress["ColorCircleTo3"]);
            })
            { IsBackground = true };

            Thread partFour = new Thread(() =>
            {
                theme.FixColorPicker(themeColor["Ternary"].RemoveAlpha(), saiAddress["ColorCircleFrom4"], saiAddress["ColorCircleTo4"]);
            })
            { IsBackground = true };


            partOne.Start();
            partTwo.Start();
            partThree.Start();
            partFour.Start();
            partOne.Join();
            partTwo.Join();
            partThree.Join();
            partFour.Join();


            // These regions is for elements, that has amount of artifacts under the main coloring,
            // so i found patterns, where it located and can be protectly colored, before main coloring processes:
            #region 0 PRIMARY COLOR
            theme.SetElementColorComplicated(saiColorRGB["SlidersBackgroundTransparent2"], themeColor["Primary"].RemoveAlpha(), saiAddress["SlidersBackgroundTransparentFrom"], saiAddress["SlidersBackgroundTransparentTo"]);
            theme.SetElementColorComplicated(saiColorRGB["ActiveCanvasBackgroundFix"], themeColor["Primary"].RemoveAlpha(), saiAddress["GlobalSectionTextFrom"], saiAddress["GlobalSectionTextTo"], true);
            #endregion

            #region 0 SECONDARY COLOR
            theme.SetElementColorWithTotalReplacment(themeColor["Secondary"].RemoveAlpha(), saiAddress["HoveredEmptyBrushesBackgroundFrom"], saiAddress["HoveredEmptyBrushesBackgroundTo"]);
            theme.SetElementColorWithTotalReplacment(themeColor["Secondary"].RemoveAlpha(), saiAddress["HoveredLayersBackgroundFrom"], saiAddress["HoveredLayersBackgroundTo"]);
            theme.SetElementColorComplicated(saiColorRGB["SelectedElementBackgroundHovered"], themeColor["Secondary"].RemoveAlpha(), saiAddress["HoveredLayersBackgroundFrom2"], saiAddress["HoveredLayersBackgroundTo2"], true);
            theme.SetElementColorComplicated(saiColorRGB["SlidersBackgroundTransparent1"], themeColor["Secondary"].RemoveAlpha(), saiAddress["SlidersBackgroundTransparentFrom"], saiAddress["SlidersBackgroundTransparentTo"]);
            byte[][] SlidersPrivelegySelectableTernaryTrue = new byte[][]{
                saiColorRGB["SlidersBackgroundTransparentLineFix1"],
                saiColorRGB["SlidersBackgroundTransparentLineFix2"]
            };
            foreach (byte[] n in SlidersPrivelegySelectableTernaryTrue)
            {
                theme.SetElementColorComplicated(n.RemoveAlpha(), themeColor["Secondary"].RemoveAlpha(), saiAddress["SlidersBackgroundTransparentFrom"], saiAddress["SlidersBackgroundTransparentTo"]);
            }
            #endregion

            #region 0 TERNARY COLOR
            theme.SetElementColorComplicated(saiColorRGB["LayerBackground"], themeColor["Ternary"].RemoveAlpha(), saiAddress["LayerBackgroundFrom"], saiAddress["LayerBackgroundTo"], true);
            theme.SetElementColorComplicated(saiColorRGB["BrushesBackgroundFileMenuBackgroundScrollBlockBackground"], themeColor["Ternary"].RemoveAlpha(), saiAddress["BrushesFileMenuTilesScrollableListsBackgroundFrom"], saiAddress["BrushesFileMenuTilesScrollableListsBackgroundTo"]);
            #endregion

            #region 0 SELECTABLE PRIMARY COLOR
            byte[][] SlidersPrivelegySelectablePrimaryTrue = new byte[][]{
                saiColorRGB["SlidersBarBackgroundTransparent1"],
              saiColorRGB["SlidersBarBackgroundTransparentLineFix1"]
            };
            foreach (byte[] n in SlidersPrivelegySelectablePrimaryTrue)
            {
                theme.SetElementColorComplicated(n.RemoveAlpha(), themeColor["SelectablePrimary"].RemoveAlpha(), saiAddress["SlidersBackgroundTransparentFrom"], saiAddress["SlidersBackgroundTransparentTo"]);
            }
            #endregion

            #region 0 SELECTABLE SECONDARY COLOR
            theme.SetElementColorComplicated(saiColorRGB["SlidersBarBackgroundTransparent2"], themeColor["SelectableSecondary"].RemoveAlpha(), saiAddress["SlidersBackgroundTransparentFrom"], saiAddress["SlidersBackgroundTransparentTo"]);
            #endregion

            // Main regions, which contains basic coloring operations:
            #region PRIMARY COLOR
            int[] PrimaryItems = new int[]{
                saiAddress["ActiveCanvasBackground"],
              saiAddress["ActiveCanvasBackground2"],
              saiAddress["ActiveCanvasBackground3"],
              saiAddress["ActiveCanvasBackground4"],
              saiAddress["InActiveCanvasBackground"],
              saiAddress["BehindLayersUIBackground"],
              saiAddress["BehindLayersUIBackground2"],
              saiAddress["BehindLayersUIBackground3"],
              saiAddress["BehindLayersUIBackground4"],
              saiAddress["BrushBorders"],
              saiAddress["SlidersVertical"],
              saiAddress["SlidersHorizontal"],
              saiAddress["InActiveText"],
              saiAddress["SliderBarHovered"],
              saiAddress["ScrollBarArrowUp"],
              saiAddress["ScrollBarArrowDown"],
              saiAddress["ScrollBarArrowLeft"],
              saiAddress["ScrollBarArrowRight"],
              saiAddress["ClosedListArrow"],
              saiAddress["GlobalTopBar"],
              saiAddress["GlobalTopBar2"],
              saiAddress["GlobalTopBar3"],
              saiAddress["GlobalTopBar4"],
              saiAddress["GlobalTopBar5"],
              saiAddress["GlobalTopBar6"],
              saiAddress["GlobalTopBar7"],
              saiAddress["GlobalTopBar8"],
              saiAddress["ScaleAngleArrow"],
              saiAddress["ColorSlidersArrows"],
              saiAddress["GreyNoteText"],
              saiAddress["SettingsListBackground"],
              saiAddress["SettingsListBackground2"],
              saiAddress["SettingsListBackground3"],
              saiAddress["SettingsListBackground4"],
              saiAddress["OkCancelButtonsTextInActive"],
              saiAddress["AssetManagerLeftBackground"],
              saiAddress["ScaleAngleSliders2"],
              saiAddress["ScaleAngleSliders3"],
              saiAddress["ScaleAngleSliders4"],
              saiAddress["ScaleAngleSliders5"],
              saiAddress["ScaleAngleSliders6"],
              saiAddress["ScaleAngleSliders7"],
              saiAddress["ScaleAngleSliders8"],
              saiAddress["ScaleAngleSliders9"],
              saiAddress["ScaleAngleSliders10"],
              saiAddress["ScaleAngleSliders11"],
              saiAddress["ScaleAngleSliders12"],
              saiAddress["ScaleAngleSliders13"],
              saiAddress["ScaleAngleSliders14"],
              saiAddress["ScaleAngleSliders15"],
            };
            foreach (int n in PrimaryItems)
            {
                theme.SetElementColor(themeColor["Primary"].RemoveAlpha(), n);
            }

            byte[][] PrimaryRGBComplicatedItemsSrclibsTrue = new byte[][]{
                saiColorRGB["FileMenuTreeTabsFix2"],
              saiColorRGB["BurgerButtonsOutline1"],
              saiColorRGB["BurgerButtonsOutline3"],
              saiColorRGB["BordersFix9"],
              saiColorRGB["saiFileInMenuBelowOutline"],
              saiColorRGB["saiFileInMenuBelowOutlineFix"],
              saiColorRGB["saiFileInMenuBelowInnerOutline"],
              saiColorRGB["BurgerButtonsOutlineSlidersOutline"],
              saiColorRGB["BurgerButtonsOutline4"],
              saiColorRGB["BurgerButtonsOutline2"],
              saiColorRGB["BordersFix1"],
              saiColorRGB["BordersFix2"],
              saiColorRGB["BordersFix3"],
              saiColorRGB["BordersFix4"],
              saiColorRGB["BordersFix5"],
              saiColorRGB["BordersFix6"],
              saiColorRGB["BordersFix7"],
              saiColorRGB["BordersFix8"],
              saiColorRGB["BordersFix10"],
              saiColorRGB["BordersFix11"],
              saiColorRGB["BordersFix12"],
              saiColorRGB["BordersFix14"],
              saiColorRGB["Separators"],
              saiColorRGB["FileMenuListElementsBackgroundDefault"],
              saiColorRGB["InActiveBurgerButtonsOutline"],
              saiColorRGB["InActiveBurgerButtonsOutlineFix1"],
              saiColorRGB["InActiveBurgerButtonsOutlineFix2"],
            };
            foreach (byte[] n in PrimaryRGBComplicatedItemsSrclibsTrue)
            {
                theme.SetElementColorComplicated(n, themeColor["Primary"].RemoveAlpha(), saiAddress["GlobalSectionSrclibsFrom"], saiAddress["GlobalSectionSrclibsTo"], true);
            }

            byte[][] PrimaryRGBComplicatedItemsAppskinTrue = new byte[][]{
                saiColorRGB["BordersFix13"],
              saiColorRGB["BordersFix15"],
              saiColorRGB["BordersFix16"],
              saiColorRGB["LayerOutline"],
              saiColorRGB["FileMenuBackground"],
              saiColorRGB["FolderBehindBackground1"],
              saiColorRGB["FolderBehindBackground2"],
            };
            foreach (byte[] n in PrimaryRGBComplicatedItemsAppskinTrue)
            {
                theme.SetElementColorComplicated(n, themeColor["Primary"].RemoveAlpha(), saiAddress["GlobalSectionAppskinFrom"], saiAddress["GlobalSectionAppskinTo"], true);
            }
            #endregion

            #region SECONDARY COLOR
            int[] SecondaryItems = new int[]{
                saiAddress["Separator"],
              saiAddress["TopBar"],
              saiAddress["TopBar2"],
              saiAddress["TopBar3"],
              saiAddress["TopBar4"],
              saiAddress["ContextMenu"],
              saiAddress["ContextMenu2"],
              saiAddress["ContextMenu3"],
              saiAddress["ContextMenu4"],
              saiAddress["SlidersInActiveBackground"],
              saiAddress["BookmarkBackgroundAndOutlinesSomewhere"],
              saiAddress["GlobalTopBarInActive"],
              saiAddress["GlobalTopBarInActive2"],
              saiAddress["GlobalTopBarInActive3"],
              saiAddress["GlobalTopBarInActive4"],
              saiAddress["TopBarTextFocused"],
              saiAddress["TopBarTextFocused2"],
              saiAddress["TopBarTextFocused3"],
              saiAddress["TopBarTextFocused4"],
              saiAddress["SomeMinimizedListsBackground"],
              saiAddress["CheckBoxesBackground"],
              saiAddress["StabilizerBackground"],
              saiAddress["PathLineInFileMenuBackground"],
            };
            foreach (int n in SecondaryItems)
            {
                theme.SetElementColor(themeColor["Secondary"].RemoveAlpha(), n);
            }


            byte[][] SecondaryRGBComplicatedItemsAppskinTrue = new byte[][]{
                saiColorRGB["FolderBackgroundHovered"],
              saiColorRGB["BordersFix17"],
              saiColorRGB["BordersFix18"],
            };
            foreach (byte[] n in SecondaryRGBComplicatedItemsAppskinTrue)
            {
                theme.SetElementColorComplicated(n, themeColor["Secondary"].RemoveAlpha(), saiAddress["GlobalSectionAppskinFrom"], saiAddress["GlobalSectionAppskinTo"], true);
            }


            byte[][] SecondaryRGBComplicatedItemsSrclibsTrue = new byte[][]{
                saiColorRGB["SelectedElementBackgroundIdle"],
              saiColorRGB["SelectedElementBackgroundActive"],
              saiColorRGB["SelectedElementBackgroundHovered"],
              saiColorRGB["FileMenuListElementsBackgroundHovered"],
              saiColorRGB["BurgerButtonsOutlineFix"],
              colorRGB.SecondaryArtifactsColor1,
              colorRGB.SecondaryArtifactsColor2,
              colorRGB.SecondaryArtifactsColor3,
              colorRGB.SecondaryArtifactsColor4,
            };
            foreach (byte[] n in SecondaryRGBComplicatedItemsSrclibsTrue)
            {
                theme.SetElementColorComplicated(n, themeColor["Secondary"].RemoveAlpha(), saiAddress["GlobalSectionSrclibsFrom"], saiAddress["GlobalSectionSrclibsTo"], true);
            }
            #endregion

            #region TERNARY COLOR
            int[] TernaryItems = new int[] {
                saiAddress["ScaleAngleSlidersBg"],
              saiAddress["GlobalBorders"],
              saiAddress["GlobalBorders2"],
              saiAddress["TabsResizeGrabberVertical"],
              saiAddress["ResizeWindowGrabber"],
              saiAddress["ContextMenuListBackground"],
              saiAddress["ContextMenuListSeparatorBackground"],
            };
            foreach (int n in TernaryItems)
            {
                theme.SetElementColor(themeColor["Ternary"].RemoveAlpha(), n);
            }

            byte[][] TernaryRGBComplicatedItemsAppskinTrue = new byte[][]{
                saiColorRGB["EmptyElementsInBrushesUI"],
              saiColorRGB["FolderBackground"],
            };
            foreach (byte[] n in TernaryRGBComplicatedItemsAppskinTrue)
            {
                theme.SetElementColorComplicated(n, themeColor["Ternary"].RemoveAlpha(), saiAddress["GlobalSectionAppskinFrom"], saiAddress["GlobalSectionAppskinTo"], true);
            }

            byte[][] TernaryRGBComplicatedItemsSrclibsTrue = new byte[][]{
                saiColorRGB["BordersFixBurgerButtonsBackgroundColorLinesBackground"],
              saiColorRGB["EmptyScrollBarBackground"],
              saiColorRGB["ScrollBarOutlineHoveredFix3"],
              saiColorRGB["saiFileInMenuBelowBackground"],
              saiColorRGB["FileMenuTreeTabsFix3"],
              saiColorRGB["FileMenuTreeTabsFix4"],
              saiColorRGB["FileMenuTreeTabsFix5"],
              saiColorRGB["FileMenuTreeTabsFix6"],
              saiColorRGB["FileMenuTreeTabsFix7"],
              saiColorRGB["FileMenuTreeTabsFix8"],
              colorRGB.TernaryArtifactsColor1,
              colorRGB.TernaryArtifactsColor2,
              colorRGB.TernaryArtifactsColor3,
              colorRGB.TernaryArtifactsColor4,
              colorRGB.TernaryArtifactsColor5,
              colorRGB.TernaryArtifactsColor6,
              colorRGB.TernaryArtifactsColor7,
              colorRGB.TernaryArtifactsColor8,
            };
            foreach (byte[] n in TernaryRGBComplicatedItemsSrclibsTrue)
            {
                theme.SetElementColorComplicated(n, themeColor["Ternary"].RemoveAlpha(), saiAddress["GlobalSectionSrclibsFrom"], saiAddress["GlobalSectionSrclibsTo"], true);
            }
            #endregion

            #region TEXT COLOR
            int[] TextItems = new int[] {
                saiAddress["BrushesBlueText"],
              saiAddress["BrushesBlueText2"],
              saiAddress["BrushesBlueText3"],
              saiAddress["TopBarText"],
              saiAddress["TopBarText2"],
              saiAddress["TopBarText3"],
              saiAddress["TopBarText4"],
              saiAddress["TopBarTextHovered"],
              saiAddress["TopBarTextHovered2"],
              saiAddress["TopBarTextHovered3"],
              saiAddress["TopBarTextHovered4"],
              saiAddress["FileMenuScrollableText"],
              saiAddress["FileMenuScrollableText2"],
              saiAddress["FileMenuScrollableText3"],
              saiAddress["FileMenuScrollableText4"],
              saiAddress["FileMenuTilesText"],
              saiAddress["FileMenuTilesText2"],
              saiAddress["FileMenuTilesText3"],
              saiAddress["FileMenuTilesText4"],
              saiAddress["BrushesText"],
              saiAddress["BrushesText2"],
              saiAddress["BrushesText3"],
              saiAddress["BrushesText4"],
              saiAddress["BrushesText5"],
              saiAddress["BrushesText6"],
              saiAddress["BrushesText7"],
              saiAddress["BrushesText8"],
              saiAddress["BrushesText9"],
              saiAddress["BrushesText10"],
              saiAddress["BrushesText11"],
              saiAddress["BrushesText12"],
              saiAddress["BrushesText13"],
              saiAddress["BrushesText14"],
              saiAddress["BrushesText15"],
              saiAddress["BrushesText16"],
              saiAddress["BrushesText17"],
              saiAddress["ShitTextInWindows"],
              saiAddress["FolderOverlayText"],
              saiAddress["BrushCircles"],
              saiAddress["WindowTitles"],
              saiAddress["OkCancelButtonsText"],
              saiAddress["OkCancelButtonsTextHovered"],
              saiAddress["OkCancelButtonsTextFocused1"],
              saiAddress["OkCancelButtonsTextFocused2"],
              saiAddress["OkCancelButtonsTextFocused3"],
              saiAddress["OkCancelButtonsTextDisfocused1"],
              saiAddress["OkCancelButtonsTextDisfocused2"],
              saiAddress["OkCancelButtonsTextDisfocused3"],
              saiAddress["ContextMenuContent"],
              saiAddress["ContextMenuContent2"],
              saiAddress["ContextMenuContent3"],
              saiAddress["ContextMenuContent4"],
              saiAddress["ContextMenuContent5"],
              saiAddress["ContextMenuContent6"],
              saiAddress["ContextMenuContent7"],
              saiAddress["ContextMenuContent8"],
              saiAddress["ContextMenuContent9"],
              saiAddress["ContextMenuContent10"],
              saiAddress["ContextMenuContent11"],
              saiAddress["ContextMenuContent12"],
              saiAddress["ContextMenuContent13"],
              saiAddress["ContextMenuContent14"],
              saiAddress["ContextMenuContent15"],
              saiAddress["ContextMenuContent16"],
              saiAddress["ContextMenuContent17"],
              saiAddress["ContextMenuContent18"],
              saiAddress["ContextMenuContent19"],
              saiAddress["ContextMenuContent20"],
              saiAddress["ContextMenuContent21"],
              saiAddress["ContextMenuContent22"],
              saiAddress["ContextMenuContent23"],
              saiAddress["ContextMenuContent24"],
              saiAddress["ContextMenuContent25"],
              saiAddress["ContextMenuContent26"],
              saiAddress["ContextMenuContent27"],
              saiAddress["ContextMenuContent28"],
              saiAddress["ContextMenuContent29"],
              saiAddress["ContextMenuContent30"],
              saiAddress["ContextMenuContent31"],
              saiAddress["ContextMenuContent32"],
              saiAddress["ContextMenuContent33"],
              saiAddress["ContextMenuContent34"],
              saiAddress["ContextMenuContent35"],
              saiAddress["ContextMenuContent36"],
              saiAddress["ContextMenuContent37"],
              saiAddress["ContextMenuContent38"],
              saiAddress["ContextMenuContent39"],
              saiAddress["ContextMenuContent40"],
              saiAddress["ContextMenuContent41"],
              saiAddress["ContextMenuContent42"],
              saiAddress["ContextMenuContent43"],
              saiAddress["ContextMenuContent44"],
              saiAddress["ContextMenuContent45"],
              saiAddress["ContextMenuContent46"],
              saiAddress["ContextMenuContent47"],
              saiAddress["ContextMenuContent48"],
              saiAddress["ContextMenuContent49"],
              saiAddress["ContextMenuContent50"],
              saiAddress["ContextMenuContent51"],
              saiAddress["ContextMenuContent52"],
              saiAddress["ContextMenuContent53"],
              saiAddress["ContextMenuContent54"],
              saiAddress["ContextMenuContent55"],
              saiAddress["ContextMenuContent56"],
              saiAddress["ContextMenuContent57"],
              saiAddress["ContextMenuContent58"],
              saiAddress["ContextMenuContent59"],
              saiAddress["ContextMenuContent60"],
              saiAddress["ContextMenuContent61"],
              saiAddress["ContextMenuContent62"],
              saiAddress["ContextMenuContent63"],
              saiAddress["ContextMenuContent64"],
              saiAddress["ContextMenuContent65"],
              saiAddress["ContextMenuContent66"],
              saiAddress["ContextMenuContent67"],
              saiAddress["ContextMenuContent68"],
              saiAddress["ContextMenuContent69"],
              saiAddress["ContextMenuContent70"],
              saiAddress["ContextMenuContent71"],
              saiAddress["ContextMenuContent72"],
              saiAddress["ContextMenuContent73"],
              saiAddress["ContextMenuContent74"],
              saiAddress["ContextMenuContent75"],
              saiAddress["ContextMenuContent76"],
              saiAddress["ContextMenuContent77"],
              saiAddress["ContextMenuContent78"],
              saiAddress["ContextMenuContent79"],
              saiAddress["ContextMenuContent80"],
              saiAddress["ContextMenuContent81"],
              saiAddress["ContextMenuContent82"],
              saiAddress["ContextMenuContent83"],
              saiAddress["ContextMenuContent84"],
              saiAddress["ContextMenuContent85"],
              saiAddress["ContextMenuContent86"],
              saiAddress["ContextMenuContent87"],
              saiAddress["ContextMenuContent88"],
              saiAddress["ContextMenuContent89"],
              saiAddress["ContextMenuContent90"],
              saiAddress["ContextMenuContent91"],
              saiAddress["ContextMenuContent92"],
              saiAddress["ContextMenuContent93"],
              saiAddress["ContextMenuContent94"],
              saiAddress["ContextMenuContent95"],
              saiAddress["ContextMenuContent96"],
              saiAddress["ContextMenuContent97"],
              saiAddress["ContextMenuContent98"],
              saiAddress["ContextMenuContent99"],
              saiAddress["ContextMenuContent100"],
              saiAddress["ContextMenuContent101"],
              saiAddress["ContextMenuContent102"],
              saiAddress["ContextMenuContent103"],
              saiAddress["ContextMenuContent104"],
              saiAddress["ContextMenuContent105"],
              saiAddress["ContextMenuContent106"],
              saiAddress["ContextMenuContent107"],
              saiAddress["ContextMenuContent108"],
              saiAddress["ContextMenuContent109"],
              saiAddress["ContextMenuContent110"],
              saiAddress["ContextMenuContent111"],
              saiAddress["ContextMenuContent112"],
              saiAddress["ContextMenuContent113"],
              saiAddress["ContextMenuContent114"],
              saiAddress["ContextMenuContent115"],
              saiAddress["ContextMenuContent116"],
              saiAddress["ContextMenuContent117"],
              saiAddress["ContextMenuContent118"],
              saiAddress["ContextMenuContent119"],
              saiAddress["ContextMenuContent120"],
            };
            foreach (int n in TextItems)
            {
                theme.SetElementColor(themeColor["Text"].RemoveAlpha(), n);
            }

            byte[][] TextComplicatedItemsSrclibs = new byte[][]{
                    saiColorRGB["ShitColoredText"],
                    saiColorRGB["FileMenuTreeText"],
            };

            foreach (byte[] n in TextComplicatedItemsSrclibs)
            {
                theme.SetElementColorComplicated(n, themeColor["Text"].RemoveAlpha(), saiAddress["GlobalSectionSrclibsFrom"], saiAddress["GlobalSectionSrclibsTo"]);
            }
            #endregion

            #region SELECTABLE PRIMARY COLOR
            int[] SelectablePrimaryItems = new int[] {
              saiAddress["SlidersColor"],
              saiAddress["saiFileInMenuBelowText"],
              saiAddress["ButtonsInLayersFill"],
              saiAddress["BlueNoteText"],
            };
            foreach (int n in SelectablePrimaryItems)
            {
                theme.SetElementColor(themeColor["SelectablePrimary"].RemoveAlpha(), n);
            }

            byte[][] SelectablePrimaryComplicatedItemsTextTrue = new byte[][] {
                saiColorRGB["BlueSelectableElementsText"],
            };
            foreach (byte[] n in SelectablePrimaryComplicatedItemsTextTrue)
            {
                theme.SetElementColorComplicated(n, themeColor["SelectablePrimary"].RemoveAlpha(), saiAddress["GlobalSectionTextFrom"], saiAddress["GlobalSectionTextTo"], true);
            }

            byte[][] SelectablePrimaryRGBComplicatedItemsSrclibsTrue = new byte[][]{
                saiColorRGB["SelectedElementOutlineActive"],
              saiColorRGB["SelectedElementOutlineHovered"],
              saiColorRGB["SelectedElementOutlineIdle"],
              saiColorRGB["SelectedElementBackgroundFocused"],
              saiColorRGB["SelectedElementOutlineFix1"],
              saiColorRGB["SelectedElementOutlineFix2"],
              saiColorRGB["SelectedElementOutlineFix3"],
              saiColorRGB["SelectedElementOutlineFix5"],
              saiColorRGB["SelectedElementOutlineFix6"],
              saiColorRGB["SelectedElementOutlineFix7"],
              saiColorRGB["SelectedElementOutlineFix8"],
              saiColorRGB["SelectedElementOutlineFix9"],
              saiColorRGB["SelectedElementOutlineFix10"],
              saiColorRGB["SelectedElementOutlineFix11"],
              saiColorRGB["ScrollBarFillHovered"],
              saiColorRGB["YesNoButtonsBackground"],
              saiColorRGB["ScrollBarAndServiceButtonsFill"],
              saiColorRGB["saiFileInMenuBelowBackgroundHovered"],
              saiColorRGB["FileMenuListElementsOutlineDefault"],
              saiColorRGB["FileMenuTreeTextFocused"],
              saiColorRGB["BlueSelectableElements"],
              saiColorRGB["ServiceButtonsOutline"],
              saiColorRGB["ServiceButtonsOutlineFix1"],
              saiColorRGB["ServiceButtonsOutlineFix2"],
              saiColorRGB["ServiceButtonsOutlineFocused"],
              saiColorRGB["ServiceButtonsOutlineFocusedFix1"],
              saiColorRGB["ServiceButtonsOutlineFocusedFix2"],
              saiColorRGB["ServiceButtonsBackgroundAndOutlineFocused"],
              saiColorRGB["BrushesBackgroundGrabbed"],
              saiColorRGB["BrushesOutlineGrabbed"],
              saiColorRGB["ServiceButtonsBackground2"],
              saiColorRGB["ServiceButtonsOutline2"],
              saiColorRGB["ServiceButtonsOutline2Fix1"],
              saiColorRGB["ServiceButtonsOutline2Fix2"],
              saiColorRGB["ServiceButtonsOutline2Fix3"],
              saiColorRGB["ServiceButtonsOutline2Fix4"],
              saiColorRGB["ServiceButtonsBackground3"],
              saiColorRGB["ServiceButtonsOutline3"],
              saiColorRGB["ServiceButtonsOutline3Fix1"],
              saiColorRGB["ServiceButtonsOutline3Fix2"],
              saiColorRGB["ServiceButtonsOutline3Fix3"],
              saiColorRGB["AnotherSelectableOutlineFocused"],
              saiColorRGB["AnotherSelectableOutlineFocusedFix1"],
              saiColorRGB["AnotherSelectableOutlineFocusedFix2"],
              saiColorRGB["AnotherSelectableInnerOutlineFocused"],
              saiColorRGB["AnotherSelectableInnerOutlineFocusedFix"],
              saiColorRGB["SelectableBackgroundRightClicked"],
              saiColorRGB["BurgerButtonsOutlineRightClicked1"],
              saiColorRGB["BurgerButtonsOutlineRightClicked2"],
              saiColorRGB["BurgerButtonsOutlineRightClicked3"],
              saiColorRGB["BurgerButtonsOutlineRightClicked4"],
              saiColorRGB["BurgerButtonsOutlineRightClicked4"],
            };
            foreach (byte[] n in SelectablePrimaryRGBComplicatedItemsSrclibsTrue)
            {
                theme.SetElementColorComplicated(n, themeColor["SelectablePrimary"].RemoveAlpha(), saiAddress["GlobalSectionSrclibsFrom"], saiAddress["GlobalSectionSrclibsTo"], true);
            }

            byte[][] SelectablePrimaryRGBComplicatedItemsAppskinTrue = new byte[][]{
                saiColorRGB["BlueFixesHovered"],
              saiColorRGB["GreyFixesClosed"],
              saiColorRGB["SelectedLayerOutlineActiveHovered"],
              saiColorRGB["SelectedLayerOutlineFocused"],
              saiColorRGB["SelectedLayerInnerOutlineActive"],
              saiColorRGB["SelectedLayerInnerOutlineHovered"],
              saiColorRGB["SelectedLayerInnerOutlineFocused"],
              saiColorRGB["SelectedLayerInnerOutlineGrabbed"],
              saiColorRGB["LayerBackgroundFocused"],
              saiColorRGB["SelectedLayerBackgroundGrabbed"],
              saiColorRGB["LayerBackgroundGrabbed"],
              saiColorRGB["FileMenuListCategoryArrows"],
              saiColorRGB["FolderOutlineSelected"],
              saiColorRGB["FolderInnerOutlineSelected"],
              saiColorRGB["FolderBackgroundSelectedFocused2"],
              saiColorRGB["FolderOutlineSelectedFocused"],
              saiColorRGB["FolderInnerOutlineSelectedHovered"],
              saiColorRGB["FolderInnerOutlineSelectedFocused1"],
              saiColorRGB["FolderInnerOutlineSelectedFocused2"],
              saiColorRGB["FolderServiceButtonsSelectedFocused"],
            };
            foreach (byte[] n in SelectablePrimaryRGBComplicatedItemsAppskinTrue)
            {
                theme.SetElementColorComplicated(n, themeColor["SelectablePrimary"].RemoveAlpha(), saiAddress["GlobalSectionAppskinFrom"], saiAddress["GlobalSectionAppskinTo"], true);
            }
            #endregion

            #region SELECTABLE SECONDARY COLOR
            int[] SelectableSecondaryItems = new int[] {
                saiAddress["SlidersActiveBackground"],
              saiAddress["SlidersActiveBackgroundHoveredFocused"],
              saiAddress["saiFileInMenuBelowPercents"],
            };

            foreach (int n in SelectableSecondaryItems)
            {
                theme.SetElementColor(themeColor["SelectableSecondary"].RemoveAlpha(), n);
            }

            byte[][] SelectableSecondaryRGBComplicatedItemsAppskinTrue = new byte[][]{
                saiColorRGB["SelectedLayerBackgroundActive"],
               saiColorRGB["SelectedLayerBackgroundHovered"],
               saiColorRGB["SelectedLayerBackgroundFocused"],
               saiColorRGB["FolderBackgroundFocused1"],
               saiColorRGB["FolderBackgroundFocused2"],
               saiColorRGB["FolderBackgroundSelectedFocused1"],
               saiColorRGB["FolderBackgroundSelected1"],
               saiColorRGB["FolderBackgroundSelected2"],
           };
            foreach (byte[] n in SelectableSecondaryRGBComplicatedItemsAppskinTrue)
            {
                theme.SetElementColorComplicated(n, themeColor["SelectableSecondary"].RemoveAlpha(), saiAddress["GlobalSectionAppskinFrom"], saiAddress["GlobalSectionAppskinTo"], true);
            }

            byte[][] SelectableSecondaryRGBComplicatedItemsSrclibsTrue = new byte[][]{
                saiColorRGB["ScrollBarFillFocused"],
              saiColorRGB["ScrollBarOutlineHovered"],
              saiColorRGB["ScrollBarOutlineHoveredFix1"],
              saiColorRGB["ScrollBarOutlineHoveredFix2"],
              saiColorRGB["ScrollBarOutlineFocused"],
              saiColorRGB["ScrollBarOutlineFocusedFix1"],
              saiColorRGB["ScrollBarOutlineFocusedFix2"],
              saiColorRGB["YesNoButtonsOutline"],
              saiColorRGB["YesNoButtonsOutlineFix1"],
              saiColorRGB["YesNoButtonsOutlineFix2"],
              saiColorRGB["YesNoButtonsOutlineFix3"],
              saiColorRGB["YesNoButtonsOutlineFix4"],
              saiColorRGB["YesNoButtonsOutlineFix5"],
              saiColorRGB["YesNoButtonsOutlineFix6"],
              saiColorRGB["YesNoButtonsOutlineFix7"],
              saiColorRGB["YesNoButtonsOutlineFix8"],
              saiColorRGB["YesNoButtonsOutlineFix9"],
              saiColorRGB["YesNoButtonsOutlineFix10"],
              saiColorRGB["YesNoButtonsOutlineFix11"],
              saiColorRGB["YesNoButtonsOutlineFix12"],
              saiColorRGB["YesNoButtonsOutlineFix13"],
              saiColorRGB["YesNoButtonsOutlineFix14"],
              saiColorRGB["YesNoButtonsOutlineFix15"],
              saiColorRGB["ScrollBarAndServiceButtonsOutline"],
              saiColorRGB["ScrollBarAndServiceButtonsOutlineFix1"],
              saiColorRGB["ScrollBarAndServiceButtonsOutlineFix2"],
              saiColorRGB["saiFileInMenuBelowOutlineHovered"],
              saiColorRGB["FileMenuTreeTextHovered"],
              saiColorRGB["BrushesButtonsBackgroundRightClicked"],
              saiColorRGB["BurgerButtonsBackgroundRightClicked"],
              saiColorRGB["TopbarOutlineFix"],
              saiColorRGB["TopbarOutlineFix2"],
              saiColorRGB["TopbarOutlineFix3"],
              saiColorRGB["TopbarOutlineFix4"],
            };
            foreach (byte[] n in SelectableSecondaryRGBComplicatedItemsSrclibsTrue)
            {
                theme.SetElementColorComplicated(n, themeColor["SelectableSecondary"].RemoveAlpha(), saiAddress["GlobalSectionSrclibsFrom"], saiAddress["GlobalSectionSrclibsTo"], true);
            }
            #endregion

            // Saving current theme changes:
            theme.SaveTheme();
            Process.Start(PathS2CE.Sai2);
            Environment.Exit(0);

        }
        public MainTheme() { }
    }

}
