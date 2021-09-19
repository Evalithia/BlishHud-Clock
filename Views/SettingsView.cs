using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD.Settings.UI.Views;
using Blish_HUD.Graphics.UI;


namespace Manlaan.Clock.Views
{
    public class SettingsView : View
    {
        ColorPicker colorPicker;
        ColorBox colorBox;

        protected override void Build(Container buildPanel) {
            Panel parentPanel = new Panel() {
                CanScroll = false,
                Parent = buildPanel,
                Height = buildPanel.Height,
                HeightSizingMode = SizingMode.AutoSize,
                Width = 700,  //bug? with buildPanel.Width changing to 40 after loading a different module settings and coming back.,
            };

            IView settingClockLocal_View = SettingView.FromType(Module._settingClockLocal, buildPanel.Width);
            ViewContainer settingClockLocal_Container = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, 10),
                Parent = parentPanel
            };
            settingClockLocal_Container.Show(settingClockLocal_View);

            IView settingClockTyria_View = SettingView.FromType(Module._settingClockTyria, buildPanel.Width);
            ViewContainer settingClockTyria_Container = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(160, settingClockLocal_Container.Location.Y),
                Parent = parentPanel
            };
            settingClockTyria_Container.Show(settingClockTyria_View);

            IView settingClockServer_View = SettingView.FromType(Module._settingClockServer, buildPanel.Width);
            ViewContainer settingClockServer_Container = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(310, settingClockLocal_Container.Location.Y),
                Parent = parentPanel
            };
            settingClockServer_Container.Show(settingClockServer_View);

            IView settingClock24H_View = SettingView.FromType(Module._settingClock24H, buildPanel.Width);
            ViewContainer settingClock24H_Container = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, settingClockServer_Container.Bottom + 5),
                Parent = parentPanel
            };
            settingClock24H_Container.Show(settingClock24H_View);

            IView settingClockHideLabel_View = SettingView.FromType(Module._settingClockHideLabel, buildPanel.Width);
            ViewContainer settingClockHideLabel_Container = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(160, settingClock24H_Container.Location.Y),
                Parent = parentPanel
            };
            settingClockHideLabel_Container.Show(settingClockHideLabel_View);


            Label settingClockFontSize_Label = new Label() {
                Location = new Point(10, settingClock24H_Container.Bottom + 10),
                Width = 75,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Font Size: ",
            };
            Dropdown settingClockFontSize_Select = new Dropdown() {
                Location = new Point(settingClockFontSize_Label.Right + 8, settingClockFontSize_Label.Top - 4),
                Width = 50,
                Parent = parentPanel,
            };
            foreach (var s in Module._fontSizes) {
                settingClockFontSize_Select.Items.Add(s);
            }
            settingClockFontSize_Select.SelectedItem = Module._settingClockFontSize.Value;
            settingClockFontSize_Select.ValueChanged += delegate {
                Module._settingClockFontSize.Value = settingClockFontSize_Select.SelectedItem;
            };

            Label settingClockLabelAlign_Label = new Label() {
                Location = new Point(10, settingClockFontSize_Label.Bottom + 10),
                Width = 75,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Label Align: ",
            };
            Dropdown settingClockLabelAlign_Select = new Dropdown() {
                Location = new Point(settingClockLabelAlign_Label.Right + 8, settingClockLabelAlign_Label.Top - 4),
                Width = 75,
                Parent = parentPanel,
            };
            foreach (var s in Module._fontAlign) {
                settingClockLabelAlign_Select.Items.Add(s);
            }
            settingClockLabelAlign_Select.SelectedItem = Module._settingClockLabelAlign.Value;
            settingClockLabelAlign_Select.ValueChanged += delegate {
                Module._settingClockLabelAlign.Value = settingClockLabelAlign_Select.SelectedItem;
            };


            Label settingClockTimeAlign_Label = new Label() {
                Location = new Point(settingClockLabelAlign_Select.Right + 20, settingClockLabelAlign_Label.Top),
                Width = 75,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Time Align: ",
            };
            Dropdown settingClockTimeAlign_Select = new Dropdown() {
                Location = new Point(settingClockTimeAlign_Label.Right + 8, settingClockTimeAlign_Label.Top - 4),
                Width = 75,
                Parent = parentPanel,
            };
            foreach (var s in Module._fontAlign) {
                settingClockTimeAlign_Select.Items.Add(s);
            }
            settingClockTimeAlign_Select.SelectedItem = Module._settingClockTimeAlign.Value;
            settingClockTimeAlign_Select.ValueChanged += delegate {
                Module._settingClockTimeAlign.Value = settingClockTimeAlign_Select.SelectedItem;
            };

            IView settingClockDrag_View = SettingView.FromType(Module._settingClockDrag, buildPanel.Width);
            ViewContainer settingClockDrag_Container = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, settingClockTimeAlign_Label.Bottom+6),
                Parent = parentPanel
            };
            settingClockDrag_Container.Show(settingClockDrag_View);

            /*


                        Label cursorLabel = new Label() {
                            Location = new Point(10, 15),
                            Width = 60,
                            AutoSizeHeight = false,
                            WrapText = false,
                            Parent = _parentPanel,
                            Text = "Cursor: ",
                        };
                        Dropdown cursorSelect = new Dropdown() {
                            Location = new Point(cursorLabel.Right + 8, cursorLabel.Top - 5),
                            Width = 175,
                            Parent = _parentPanel,
                        };
                        foreach (var s in Module._mouseFiles) {
                            cursorSelect.Items.Add(s.Name);
                        }
                        cursorSelect.SelectedItem = Module._settingMouseCursorImage.Value;
                        cursorSelect.ValueChanged += delegate {
                            Module._settingMouseCursorImage.Value = cursorSelect.SelectedItem;
                        };

                        Label colorLabel = new Label() {
                            Location = new Point(10, cursorSelect.Bottom + 15),
                            Width = 60,
                            AutoSizeHeight = false,
                            WrapText = false,
                            Parent = _parentPanel,
                            Text = "Tint: ",
                        };
                        colorBox = new ColorBox() {
                            Location = new Point(colorLabel.Right + 8, colorLabel.Top - 10),
                            Parent = _parentPanel,
                            Color = Module._colors.Find(x => x.Name.Equals(Module._settingMouseCursorColor.Value)),
                        };
                        colorBox.Click += delegate { colorPicker.Visible = !colorPicker.Visible; };
                        colorPicker = new ColorPicker() {
                            Location = new Point(colorBox.Right + 10, colorBox.Top),
                            CanScroll = true,
                            Size = new Point(465, 255),
                            Visible = false,
                            AssociatedColorBox = colorBox,
                            ZIndex = 10,
                            Parent = _parentPanel,
                        };
                        colorPicker.SelectedColorChanged += delegate {
                            colorBox.Color = colorPicker.SelectedColor;
                            Module._settingMouseCursorColor.Value = colorPicker.SelectedColor.Name;
                            colorPicker.Visible = false;
                        };
                        colorPicker.LeftMouseButtonPressed += delegate {
                            colorPicker.Visible = false;
                        };
                        foreach (var color in Module._colors) {
                            colorPicker.Colors.Add(color);
                        }

                        Label sizeLabel = new Label() {
                            Location = new Point(10, colorBox.Bottom + 5),
                            Width = 60,
                            AutoSizeHeight = false,
                            WrapText = false,
                            Parent = _parentPanel,
                            Text = "Size: ",
                        };
                        TrackBar sizeSlider = new TrackBar() {
                            Location = new Point(sizeLabel.Right + 8, sizeLabel.Top),
                            Width = 250,
                            MaxValue = 250,
                            MinValue = 0,
                            Value = Module._settingMouseCursorSize.Value,
                            Parent = _parentPanel,
                        };
                        sizeSlider.ValueChanged += delegate { Module._settingMouseCursorSize.Value = (int)sizeSlider.Value; };

                        Label opacityLabel = new Label() {
                            Location = new Point(10, sizeSlider.Bottom + 7),
                            Width = 60,
                            AutoSizeHeight = false,
                            WrapText = false,
                            Parent = _parentPanel,
                            Text = "Opacity: ",
                        };
                        TrackBar opacitySlider = new TrackBar() {
                            Location = new Point(opacityLabel.Right + 8, opacityLabel.Top),
                            Width = 250,
                            MaxValue = 1,
                            MinValue = 0,
                            Value = Module._settingMouseCursorOpacity.Value,
                            Parent = _parentPanel,
                        };
                        sizeSlider.ValueChanged += delegate { Module._settingMouseCursorOpacity.Value = opacitySlider.Value; };

                        IView settingCameraDragView = SettingView.FromType(Module._settingMouseCursorCameraDrag, buildPanel.Width);
                        ViewContainer _settingCameraDragContainer = new ViewContainer() {
                            WidthSizingMode = SizingMode.Fill,
                            Location = new Point(10, opacityLabel.Bottom + 5),
                            Parent = _parentPanel
                        };
                        _settingCameraDragContainer.Show(settingCameraDragView);
                        IView settingAboveBlishView = SettingView.FromType(Module._settingMouseCursorAboveBlish, buildPanel.Width);
                        ViewContainer _settingAboveBlishContainer = new ViewContainer() {
                            WidthSizingMode = SizingMode.Fill,
                            Location = new Point(10, _settingCameraDragContainer.Bottom + 5),
                            Parent = _parentPanel
                        };
                        _settingAboveBlishContainer.Show(settingAboveBlishView);
            */
        }
    }
}
