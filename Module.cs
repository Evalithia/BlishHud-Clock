﻿using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD.Graphics.UI;
using static Blish_HUD.GameService;

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
        public static SettingEntry<bool> _settingFlatClock;
        public static SettingEntry<bool> _settingClockLocal;
        public static SettingEntry<bool> _settingClockTyria;
        public static SettingEntry<bool> _settingClockServer;
        public static SettingEntry<bool> _settingClockDayNight;
        public static SettingEntry<bool> _settingClock24H;
        public static SettingEntry<bool> _settingClockHideLabel; 
        public static SettingEntry<string> _settingClockFontSize;
        public static SettingEntry<string> _settingClockLabelAlign;
        public static SettingEntry<string> _settingClockTimeAlign;
        public static SettingEntry<bool> _settingClockDrag;
        public static SettingEntry<Point> _settingClockLoc;
        public int settingFontSizeSmall;
        private Control.DrawClock _clockImg;

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingFlatClock = settings.DefineSetting("FlatClock", false, "Flat Clock (FFXIV Style)", "");
            _settingClockLocal = settings.DefineSetting("ClockLocal", true, "Local", "");
            _settingClockTyria = settings.DefineSetting("ClockTyria", true, "Tyria", "");
            _settingClockServer = settings.DefineSetting("ClockServer", false, "Server", "");
            _settingClockDayNight = settings.DefineSetting("ClockDay", false, "Day/Night", "");
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
            _settingClockDayNight.SettingChanged += UpdateClockSettings_Show;
            _settingClockFontSize.SettingChanged += UpdateClockSettings_Font;
            _settingClockLoc.SettingChanged += UpdateClockSettings_Location;
            _settingClockLabelAlign.SettingChanged += UpdateClockSettings_Font;
            _settingClockTimeAlign.SettingChanged += UpdateClockSettings_Font;
            _settingClock24H.SettingChanged += UpdateClockSettings_Show;
            _settingClockHideLabel.SettingChanged += UpdateClockSettings_Show;
            _settingFlatClock.SettingChanged += UpdateClockSettings_Show;
        }
        public override IView GetSettingsView() {
            return new Clock.Views.SettingsView();
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
            _clockImg.DayNightTime = CalcDayNightTime();
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            _settingClockDrag.SettingChanged -= UpdateClockSettings_Show;
            _settingClockLocal.SettingChanged -= UpdateClockSettings_Show;
            _settingClockTyria.SettingChanged -= UpdateClockSettings_Show;
            _settingClockServer.SettingChanged -= UpdateClockSettings_Show;
            _settingClockDayNight.SettingChanged -= UpdateClockSettings_Show;
            _settingClockFontSize.SettingChanged -= UpdateClockSettings_Font;
            _settingClockLoc.SettingChanged -= UpdateClockSettings_Location;
            _settingClockLabelAlign.SettingChanged -= UpdateClockSettings_Font;
            _settingClockTimeAlign.SettingChanged -= UpdateClockSettings_Font;
            _settingClock24H.SettingChanged -= UpdateClockSettings_Show;
            _settingClockHideLabel.SettingChanged -= UpdateClockSettings_Show;
            _settingFlatClock.SettingChanged -= UpdateClockSettings_Show;
            _clockImg?.Dispose();
        }

        private void UpdateClockSettings_Show(object sender = null, ValueChangedEventArgs<bool> e = null)
        {
            _clockImg.ShowLocal = _settingClockLocal.Value;
            _clockImg.ShowTyria = _settingClockTyria.Value;
            _clockImg.ShowServer = _settingClockServer.Value;
            _clockImg.ShowDayNight = _settingClockDayNight.Value;
            _clockImg.Show24H = _settingClock24H.Value;
            _clockImg.HideLabel = _settingClockHideLabel.Value;
            _clockImg.FlatClock = _settingFlatClock.Value;
            _clockImg.Drag = _settingClockDrag.Value;
        }
        
        private void UpdateClockSettings_Font(object sender = null, ValueChangedEventArgs<string> e = null)
        {

            if (Int32.Parse(_settingClockFontSize.Value) >= 14)
            {
                settingFontSizeSmall = Int32.Parse(_settingClockFontSize.Value) - 2;
            }
            else {
                settingFontSizeSmall = Int32.Parse(_settingClockFontSize.Value);
            }
            _clockImg.Font_Size = (ContentService.FontSize) Enum.Parse(typeof(ContentService.FontSize), "Size" + _settingClockFontSize.Value);
            _clockImg.Font_Size_Small = (ContentService.FontSize)Enum.Parse(typeof(ContentService.FontSize), "Size" + settingFontSizeSmall.ToString());
            _clockImg.LabelAlign = (HorizontalAlignment) Enum.Parse(typeof(HorizontalAlignment), _settingClockLabelAlign.Value);
            _clockImg.TimeAlign = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), _settingClockTimeAlign.Value);
        }
        private void UpdateClockSettings_Location(object sender = null, ValueChangedEventArgs<Point> e = null)
        {
            if (_settingClockLoc.Value.X < 1)
                _settingClockLoc.Value = new Point(1, _settingClockLoc.Value.Y);
            if (_settingClockLoc.Value.Y < 1)
                _settingClockLoc.Value = new Point(_settingClockLoc.Value.X, 1);
            _clockImg.Location = _settingClockLoc.Value;
        }
        private DateTime CalcServerTime()
        {
            return DateTime.UtcNow;
        }
        private DateTime CalcTyriaTime() {
            try {
                DateTime UTC = DateTime.UtcNow;
                int utcsec = utcsec = (UTC.Hour * 3600) + (UTC.Minute * 60) + UTC.Second;
                int tyriasec = (utcsec * 12) - 60;
                tyriasec = tyriasec % (3600 * 24);
                int tyrianhour = (int)(tyriasec / 3600);
                tyriasec = tyriasec % 3600;
                int tyrianmin = (int)(tyriasec / 60);
                tyriasec = tyriasec % 60;
                return new DateTime(2000, 1, 1, tyrianhour, tyrianmin, tyriasec);
            }
            catch {
                return new DateTime(2000, 1, 1, 0, 0, 0);
            }

        }
        private string CalcDayNightTime() {
            DateTime TyriaTime = CalcTyriaTime();
            int Map = Gw2Mumble.CurrentMap.Id;

            if (Map == 1452 /*Echovald*/ || Map == 1442 /*Seitung*/ || Map == 1438 /*Kaineng*/ || Map == 1422 /*Dragon's End*/ || Map == 1462 /*Guild Hall*/ ) {   //Cantha Maps
                if (TyriaTime >= new DateTime(2000, 1, 1, 8, 0, 0) && TyriaTime < new DateTime(2000, 1, 1, 18, 0, 0)) {
                    return "Day";
                }
                if (TyriaTime >= new DateTime(2000, 1, 1, 18, 0, 0) && TyriaTime < new DateTime(2000, 1, 1, 19, 0, 0)) {
                    return "Day";
                }
                if (TyriaTime >= new DateTime(2000, 1, 1, 19, 0, 0) && TyriaTime < new DateTime(2000, 1, 1, 20, 0, 0)) {
                    return "Dusk";
                }
                if (TyriaTime >= new DateTime(2000, 1, 1, 6, 0, 0) && TyriaTime < new DateTime(2000, 1, 1, 7, 0, 0)) {
                    return "Night";
                }
                if (TyriaTime >= new DateTime(2000, 1, 1, 7, 0, 0) && TyriaTime < new DateTime(2000, 1, 1, 8, 0, 0)) {
                    return "Dawn";
                }
                return "Night";
            }
            else {
                if (TyriaTime >= new DateTime(2000, 1, 1, 6, 0, 0) && TyriaTime < new DateTime(2000, 1, 1, 19, 0, 0)) {
                    return "Day";
                }
                if (TyriaTime >= new DateTime(2000, 1, 1, 19, 0, 0) && TyriaTime < new DateTime(2000, 1, 1, 20, 0, 0)) {
                    return "Day";
                }
                if (TyriaTime >= new DateTime(2000, 1, 1, 20, 0, 0) && TyriaTime < new DateTime(2000, 1, 1, 21, 0, 0)) {
                    return "Dusk";
                }
                if (TyriaTime >= new DateTime(2000, 1, 1, 4, 0, 0) && TyriaTime < new DateTime(2000, 1, 1, 5, 0, 0)) {
                    return "Night";
                }
                if (TyriaTime >= new DateTime(2000, 1, 1, 5, 0, 0) && TyriaTime < new DateTime(2000, 1, 1, 6, 0, 0)) {
                    return "Dawn";
                }
                return "Night";
            }
        }

    }

}
