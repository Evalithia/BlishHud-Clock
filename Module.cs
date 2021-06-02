using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Clock
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

        private SettingEntry<bool> _settingClockLocal;
        private SettingEntry<bool> _settingClockTyria;
        private SettingEntry<bool> _settingClockServer;
        private SettingEntry<ContentService.FontSize> _settingClockFontSize;
        private SettingEntry<string> _settingClockX;
        private SettingEntry<string> _settingClockY;
        private DrawClock _clockImg;

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void DefineSettings(SettingCollection settings)
        {
            
            _settingClockLocal = settings.DefineSetting("ClockLocal", true, "Local", "");
            _settingClockTyria = settings.DefineSetting("ClockTyria", true, "Tyria", "");
            _settingClockServer = settings.DefineSetting("ClockServer", false, "Server", "");
            _settingClockFontSize = settings.DefineSetting("ClockFont", ContentService.FontSize.Size12, "Font Size", "");
            _settingClockX = settings.DefineSetting("ClockX", "100", "X", "");
            _settingClockY = settings.DefineSetting("ClockY", "100", "Y", "");
            //_settingFontSize.SetRange(1, 20);
        }

        protected override void Initialize()
        {
            _settingClockLocal.SettingChanged += UpdateClockSettings_Show;
            _settingClockTyria.SettingChanged += UpdateClockSettings_Show;
            _settingClockServer.SettingChanged += UpdateClockSettings_Show;
            _settingClockFontSize.SettingChanged += UpdateClockSettings_Font;
            _settingClockX.SettingChanged += UpdateClockSettings_Location;
            _settingClockY.SettingChanged += UpdateClockSettings_Location;

            _clockImg = new DrawClock();
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
            _clockImg.LocalTime = DateTime.Now;
            _clockImg.TyriaTime = CalcTyriaTime();
            _clockImg.ServerTime = CalcServerTime();
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            _settingClockLocal.SettingChanged -= UpdateClockSettings_Show;
            _settingClockTyria.SettingChanged -= UpdateClockSettings_Show;
            _settingClockServer.SettingChanged -= UpdateClockSettings_Show;
            _settingClockFontSize.SettingChanged -= UpdateClockSettings_Font;
            _settingClockX.SettingChanged -= UpdateClockSettings_Location;
            _settingClockY.SettingChanged -= UpdateClockSettings_Location;
            _clockImg?.Dispose();
        }

        private void UpdateClockSettings_Show(object sender = null, ValueChangedEventArgs<bool> e = null)
        {
            _clockImg.ShowLocal = _settingClockLocal.Value;
            _clockImg.ShowTyria = _settingClockTyria.Value;
            _clockImg.ShowServer = _settingClockServer.Value;
        }
        private void UpdateClockSettings_Font(object sender = null, ValueChangedEventArgs<ContentService.FontSize> e = null)
        {
            _clockImg.Font_Size = _settingClockFontSize.Value;
        }
        private void UpdateClockSettings_Location(object sender = null, ValueChangedEventArgs<string> e = null)
        {
            if (int.Parse(_settingClockX.Value) <= 0)
                _settingClockX.Value = "1";
            if (int.Parse(_settingClockY.Value) <= 0)
                _settingClockY.Value = "1";

            _clockImg.Location = new Point(int.Parse(_settingClockX.Value), int.Parse(_settingClockY.Value));
        }
        private DateTime CalcServerTime()
        {
            
            return DateTime.UtcNow;
        }
        private DateTime CalcTyriaTime()
        {
            DateTime UTC = DateTime.UtcNow;
            int utcsec = utcsec = (UTC.Hour * 3600) + (UTC.Minute * 60) + UTC.Second; 
            int tyriasec = (utcsec *  12)-60;
            tyriasec = tyriasec % (3600*24);
            int tyrianhour = (int)(tyriasec / 3600);
            tyriasec = tyriasec % 3600;
            int tyrianmin = (int)(tyriasec / 60);
            tyriasec = tyriasec % 60;
            return new DateTime (2000,1,1,tyrianhour,tyrianmin,tyriasec);
        }

    }

}
