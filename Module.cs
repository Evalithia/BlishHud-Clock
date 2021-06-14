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

        //Doing this because using ContentServices.FontSize in SettingEntry shows "0" on settings page.
        private enum FontSizes
        {
            Size8, Size11, Size12, Size14, Size16, Size18, Size20, Size22, Size24, Size32, Size34, Size36
        };
        private SettingEntry<bool> _settingClockLocal;
        private SettingEntry<bool> _settingClockTyria;
        private SettingEntry<bool> _settingClockServer;
        private SettingEntry<FontSizes> _settingClockFontSize;
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
            _settingClockFontSize = settings.DefineSetting("ClockFont", FontSizes.Size12, "Font Size", "");
            _settingClockX = settings.DefineSetting("ClockX", "100", "X", "");
            _settingClockY = settings.DefineSetting("ClockY", "100", "Y", "");
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
        private void UpdateClockSettings_Font(object sender = null, ValueChangedEventArgs<FontSizes> e = null)
        {
            _clockImg.Font_Size = GetFontSize(_settingClockFontSize.Value);
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

        private ContentService.FontSize GetFontSize(FontSizes fontSize)
        {
            switch (fontSize)
            {
                default:
                case FontSizes.Size8:
                    return ContentService.FontSize.Size8;
                case FontSizes.Size11:
                    return ContentService.FontSize.Size11;
                case FontSizes.Size12:
                    return ContentService.FontSize.Size12;
                case FontSizes.Size14:
                    return ContentService.FontSize.Size14;
                case FontSizes.Size16:
                    return ContentService.FontSize.Size16;
                case FontSizes.Size18:
                    return ContentService.FontSize.Size18;
                case FontSizes.Size20:
                    return ContentService.FontSize.Size20;
                case FontSizes.Size22:
                    return ContentService.FontSize.Size22;
                case FontSizes.Size24:
                    return ContentService.FontSize.Size24;
                case FontSizes.Size32:
                    return ContentService.FontSize.Size32;
                case FontSizes.Size34:
                    return ContentService.FontSize.Size34;
                case FontSizes.Size36:
                    return ContentService.FontSize.Size36;
            }
        }
    
    }

}
