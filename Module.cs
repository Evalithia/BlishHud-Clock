using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD.Graphics.UI;

namespace Manlaan.Clock
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        //Doing this because using ContentServices.FontSize in SettingEntry shows "0" on settings page.
        public static string[] _fontSizes = new string[] { "8", "11", "12", "14", "16", "18", "20", "22", "24", "32", "34", "36" };
        public static string[] _fontAlign = new string[] { "Left", "Center", "Right" };
        public static SettingEntry<bool> _settingClockLocal;
        public static SettingEntry<bool> _settingClockTyria;
        public static SettingEntry<bool> _settingClockServer;
        public static SettingEntry<bool> _settingClock24H;
        public static SettingEntry<bool> _settingClockHideLabel;
        public static SettingEntry<string> _settingClockFontSize;
        public static SettingEntry<string> _settingClockLabelAlign;
        public static SettingEntry<string> _settingClockTimeAlign;
        public static SettingEntry<bool> _settingClockDrag;
        public static SettingEntry<Point> _settingClockLoc;
        private Control.DrawClock _clockImg; 

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void DefineSettings(SettingCollection settings)
        {
            
            _settingClockLocal = settings.DefineSetting("ClockLocal", true, "Local", "");
            _settingClockTyria = settings.DefineSetting("ClockTyria", true, "Tyria", "");
            _settingClockServer = settings.DefineSetting("ClockServer", false, "Server", "");
            _settingClock24H = settings.DefineSetting("Clock24H", false, "24 Hour Time", "");
            _settingClockHideLabel = settings.DefineSetting("ClockHideLabel", false, "Hide Labels", "");
            _settingClockFontSize = settings.DefineSetting("ClockFont2", "12", "Font Size", "");
            _settingClockLabelAlign = settings.DefineSetting("ClockLebelAlign2", "Right", "Label Align", "");
            _settingClockTimeAlign = settings.DefineSetting("ClockTimeAlign2", "Right", "Time Align", "");
            _settingClockLoc = settings.DefineSetting("ClockLoc", new Point(100,100), "Location", "");
            _settingClockDrag = settings.DefineSetting("ClockDrag", false, "Enable Dragging", "");

            _settingClockDrag.SettingChanged += UpdateClockSettings_Show;
            _settingClockLocal.SettingChanged += UpdateClockSettings_Show;
            _settingClockTyria.SettingChanged += UpdateClockSettings_Show;
            _settingClockServer.SettingChanged += UpdateClockSettings_Show;
            _settingClockFontSize.SettingChanged += UpdateClockSettings_Font;
            _settingClockLoc.SettingChanged += UpdateClockSettings_Location;
            _settingClockLabelAlign.SettingChanged += UpdateClockSettings_Font;
            _settingClockTimeAlign.SettingChanged += UpdateClockSettings_Font;
            _settingClock24H.SettingChanged += UpdateClockSettings_Show;
            _settingClockHideLabel.SettingChanged += UpdateClockSettings_Show;
        }
        public override IView GetSettingsView() {
            return new Clock.Views.SettingsView();
            //return new SettingsView( (this.ModuleParameters.SettingsManager.ModuleSettings);
        }

        protected override void Initialize()
        {
            _clockImg = new Control.DrawClock();
            _clockImg.Parent = GameService.Graphics.SpriteScreen;
            UpdateClockSettings_Show();
            UpdateClockSettings_Font();
            UpdateClockSettings_Location();
        }

        protected override async Task LoadAsync()
        {

        }

        protected override void OnModuleLoaded(EventArgs e)
        {
            base.OnModuleLoaded(e);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GameService.GameIntegration.Gw2Instance.IsInGame && !GameService.Gw2Mumble.UI.IsMapOpen) {
                _clockImg.Show();
            }
            else {
                _clockImg.Hide();
            }

            _clockImg.LocalTime = DateTime.Now;
            _clockImg.TyriaTime = CalcTyriaTime();
            _clockImg.ServerTime = CalcServerTime();
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            _settingClockDrag.SettingChanged -= UpdateClockSettings_Show;
            _settingClockLocal.SettingChanged -= UpdateClockSettings_Show;
            _settingClockTyria.SettingChanged -= UpdateClockSettings_Show;
            _settingClockServer.SettingChanged -= UpdateClockSettings_Show;
            _settingClockFontSize.SettingChanged -= UpdateClockSettings_Font;
            _settingClockLoc.SettingChanged -= UpdateClockSettings_Location;
            _settingClockLabelAlign.SettingChanged -= UpdateClockSettings_Font;
            _settingClockTimeAlign.SettingChanged -= UpdateClockSettings_Font;
            _settingClock24H.SettingChanged -= UpdateClockSettings_Show;
            _settingClockHideLabel.SettingChanged -= UpdateClockSettings_Show;
            _clockImg?.Dispose();
        }

        private void UpdateClockSettings_Show(object sender = null, ValueChangedEventArgs<bool> e = null)
        {
            _clockImg.ShowLocal = _settingClockLocal.Value;
            _clockImg.ShowTyria = _settingClockTyria.Value;
            _clockImg.ShowServer = _settingClockServer.Value;
            _clockImg.Show24H = _settingClock24H.Value;
            _clockImg.HideLabel = _settingClockHideLabel.Value;
            _clockImg.Drag = _settingClockDrag.Value;
        }
        private void UpdateClockSettings_Font(object sender = null, ValueChangedEventArgs<string> e = null)
        {
            _clockImg.Font_Size = (ContentService.FontSize) Enum.Parse(typeof(ContentService.FontSize), "Size" + _settingClockFontSize.Value);
            _clockImg.LabelAlign = (HorizontalAlignment) Enum.Parse(typeof(HorizontalAlignment), _settingClockLabelAlign.Value);
            _clockImg.TimeAlign = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), _settingClockTimeAlign.Value);
        }
        private void UpdateClockSettings_Location(object sender = null, ValueChangedEventArgs<Point> e = null)
        {
            _clockImg.Location = _settingClockLoc.Value;
        }
        private DateTime CalcServerTime()
        {
            
            return DateTime.UtcNow;
        }
        private DateTime CalcTyriaTime()
        {
            try
            {
                DateTime UTC = DateTime.UtcNow;
                int utcsec = utcsec = (UTC.Hour * 3600) + (UTC.Minute * 60) + UTC.Second;
                int tyriasec = (utcsec * 12) - 60;
                tyriasec = tyriasec % (3600 * 24);
                int tyrianhour = (int)(tyriasec / 3600);
                tyriasec = tyriasec % 3600;
                int tyrianmin = (int)(tyriasec / 60);
                tyriasec = tyriasec % 60;
                return new DateTime(2000, 1, 1, tyrianhour, tyrianmin, tyriasec);
            } catch
            {
                return new DateTime(2000, 1, 1, 0, 0, 0);
            }

        }
    
    }

}
