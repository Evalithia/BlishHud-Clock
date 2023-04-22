using System;
using System.Collections.Generic;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using NAudio.MediaFoundation;

namespace Manlaan.Clock.Control
{
    class DrawClock : Container
    {
        public bool ShowLocal = false;
        public bool ShowTyria = false;
        public bool ShowServer = false;
        public bool ShowDayNight = false;
        public bool Show24H = false;
        public bool HideLabel = false;
        public bool Drag = false;
        public bool FlatClock = false;
        public ContentService.FontSize Font_Size = ContentService.FontSize.Size11;
        public ContentService.FontSize Font_Size_Small = ContentService.FontSize.Size11;
        public DateTime LocalTime = DateTime.Now;
        public DateTime TyriaTime;
        public DateTime ServerTime;
        public string DayNightTime;
        public HorizontalAlignment LabelAlign = HorizontalAlignment.Right;
        public HorizontalAlignment TimeAlign = HorizontalAlignment.Right;

        private static BitmapFont _font;
        private static BitmapFont _fontSmall;
        private Point _dragStart = Point.Zero;
        private bool _dragging;

        public DrawClock()
        {
            this.Location = new Point(0, 0);
            this.Size = new Point(0, 0);
            this.Visible = true;
            this.Padding = Thickness.Zero;
        }

        protected override CaptureType CapturesInput()
        {
            if (this.Drag)
                return CaptureType.Mouse;
            else 
                return CaptureType.Filter;
        }

        protected override void OnLeftMouseButtonPressed(MouseEventArgs e) {
            if (Drag) {
                _dragging = true;
                _dragStart = Input.Mouse.Position;
            }
            base.OnLeftMouseButtonPressed(e);
        }
        protected override void OnLeftMouseButtonReleased(MouseEventArgs e) {
            if (Drag) {
                _dragging = false;
                Module._settingClockLoc.Value = this.Location;
            }
            base.OnLeftMouseButtonPressed(e);
        }

        private Boolean IsPointInBounds(Point point) {
            Point windowSize = GameService.Graphics.SpriteScreen.Size;

            return point.X > 0 &&
                    point.Y > 0 &&
                    point.X < windowSize.X &&
                    point.Y < windowSize.Y;
        }

        public override void UpdateContainer(GameTime gameTime) {
            if (_dragging) {
                if(IsPointInBounds(Input.Mouse.Position)) {
                    var nOffset = Input.Mouse.Position - _dragStart;
                    Location += nOffset;
                } else {
                    _dragging = false;
                    Module._settingClockLoc.Value = this.Location;
                }
                _dragStart = Input.Mouse.Position;
            }
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            List<string> labels = new List<string>();
            List<string> times = new List<string>();
            List<string> ampm = new List<string>();                 //all this AMPM stuff is to help align a horribly nonmonospaced font
            List<string> placeholderTimes = new List<string>();     //so when a minute changes from for example :20 to :21, the width of the time column doesn't shift slightly over
            List<string> placeholderFlatClock = new List<string>(); 
            string flClockS = "";
            string flClockL = "";
            string flClockT = "";
            string flClockDN = "";

            string flClockSp = "";
            string flClockLp = "";
            string flClockTp = "";

            string ampmcutS = "";
            string ampmcutL = "";
            string ampmcutT = "";
            string ampmcutSa = "";
            string ampmcutLa = "";
            string ampmcutTa = "";
            string ampmcutSp = "";
            string ampmcutLp = "";
            string ampmcutTp = "";


            string flTimeFixS = "";
            string flTimeFixL = "";
            string flTimeFixT = "";

            List<string> daynightspecial = new List<string>();

            Point LabelSize = new Point(0, 0);
            Point TimeSize = new Point(0, 0);
            Point AMPMSize = new Point(0, 0);
            Point DayNightSize = new Point(0, 0);

            Point flClockSizePlaceholder = new Point(0, 0);
            Point flClockSizeS = new Point(0, 0);
            Point flClockSizeL = new Point(0, 0);
            Point flClockSizeT = new Point(0, 0);
            Point flAMPMSizeS = new Point(0, 0);
            Point flAMPMSizeL = new Point(0, 0);
            Point flAMPMSizeT = new Point(0, 0);
            Point flFixSizeS = new Point(0, 0);
            Point flFixSizeL = new Point(0, 0);
            Point flFixSizeT = new Point(0, 0);
            Point flDNSize = new Point(0, 0);
            _font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, Font_Size, ContentService.FontStyle.Regular);
            _fontSmall = GameService.Content.GetFont(ContentService.FontFace.Menomonia, Font_Size_Small, ContentService.FontStyle.Regular);

            string format = "h:mm";
            string afterformat = "h:mm tt";
            if (this.Show24H)
                format = "HH:mm";

            if (this.ShowServer && !FlatClock)
            {
                if (!HideLabel) labels.Add(" Server: ");
                times.Add(" " + ServerTime.ToString(format));
                placeholderTimes.Add(" 00:00");
                daynightspecial.Add("");
                if (!this.Show24H)
                {
                    ampmcutS = ServerTime.ToString(afterformat);
                    ampm.Add(" " + ampmcutS.Substring(ampmcutS.Length - 2));
                }
            }
            else if (this.ShowServer && FlatClock)
            {
                flClockS = "  " + ServerTime.ToString(format) + " ";
                if (!this.Show24H)
                {
                    ampmcutS = ServerTime.ToString(afterformat);
                    ampmcutSa = ampmcutS.Substring(ampmcutS.Length - 2);
                    ampmcutSp = "MM";
                    placeholderFlatClock.Add(ampmcutSp);
                }
                flClockSp = "00:00";
                flTimeFixS = "ST ";
                placeholderFlatClock.Add(flClockSp);
                placeholderFlatClock.Add(flTimeFixS);
            }

            if (this.ShowLocal && !FlatClock)
            {
                if (!HideLabel) labels.Add(" Local: ");
                times.Add(" " + LocalTime.ToString(format));
                placeholderTimes.Add(" 00:00");
                daynightspecial.Add("");
                if (!this.Show24H)
                {
                    ampmcutL = LocalTime.ToString(afterformat);
                    ampm.Add(" " + ampmcutL.Substring(ampmcutL.Length - 2));
                }
            }
            else if (this.ShowLocal && FlatClock)
            {
                flClockL = "  " + LocalTime.ToString(format) + " ";
                if (!this.Show24H)
                {
                    ampmcutL = ServerTime.ToString(afterformat);
                    ampmcutLa = ampmcutL.Substring(ampmcutL.Length - 2);
                    ampmcutLp = "MM";
                    placeholderFlatClock.Add(ampmcutLp);
                }
                flClockLp = "00:00";
                flTimeFixL = "LT ";
                placeholderFlatClock.Add(flClockLp);
                placeholderFlatClock.Add(flTimeFixL);
            }
            // puts the daynight label next to the Tyrian clock when both are enabled
            if (this.ShowTyria && this.ShowDayNight && !FlatClock)
            {
                if (!HideLabel) labels.Add(" Tyria: ");
                times.Add(" " + TyriaTime.ToString(format));
                placeholderTimes.Add(" 00:00");
                if (!this.Show24H)
                {
                    ampmcutT = TyriaTime.ToString(afterformat);
                    ampm.Add(" " + ampmcutT.Substring(ampmcutT.Length - 2));
                }
                daynightspecial.Add(" " + DayNightTime);
            }
            else if (this.ShowTyria && !FlatClock)
            {
                if (!HideLabel) labels.Add(" Tyria: ");
                times.Add(" " + TyriaTime.ToString(format));
                placeholderTimes.Add(" 00:00");
                if (!this.Show24H)
                {
                    ampmcutT = TyriaTime.ToString(afterformat);
                    ampm.Add(" " + ampmcutT.Substring(ampmcutT.Length - 2));
                }
            }
            else if (this.ShowTyria && FlatClock)
            {
                flClockT = "  " + TyriaTime.ToString(format) + " ";
                if (!this.Show24H)
                {
                    ampmcutT = TyriaTime.ToString(afterformat);
                    ampmcutTa = ampmcutT.Substring(ampmcutT.Length - 2);
                    ampmcutTp = "MM";
                    placeholderFlatClock.Add(ampmcutTp);
                }
                flClockTp = "00:00";
                flTimeFixT = "TT ";
                placeholderFlatClock.Add(flClockTp);
                placeholderFlatClock.Add(flTimeFixT);
            }
            if (this.ShowDayNight)
            {
                if (!HideLabel) labels.Add(" ");
                times.Add(" " + DayNightTime);
                flClockDN = " " + DayNightTime;
                placeholderFlatClock.Add("  " + DayNightTime);
            }

            if (!FlatClock)
            {
            LabelSize = new Point(
                (int)_font.MeasureString(String.Join("\n",labels)).Width,
                (int)_font.MeasureString(String.Join("\n", labels)).Height
                );
            TimeSize = new Point(
                (int)_font.MeasureString(String.Join("\n", placeholderTimes)).Width,
                (int)_font.MeasureString(String.Join("\n", placeholderTimes)).Height
                );
            AMPMSize = new Point(
                (int)_font.MeasureString(String.Join("\n", ampm)).Width,
                (int)_font.MeasureString(String.Join("\n", ampm)).Height
                );
            DayNightSize = new Point(
                (int)_font.MeasureString(String.Join("\n", daynightspecial)).Width,
                (int)_font.MeasureString(String.Join("\n", daynightspecial)).Height
                );
            int maxHeight = Math.Max(LabelSize.Y, TimeSize.Y);
            this.Size = new Point(LabelSize.X + TimeSize.X + AMPMSize.X + AMPMSize.X + DayNightSize.X, maxHeight);
                if (!HideLabel)
                    spriteBatch.DrawStringOnCtrl(this,
                        String.Join("\n", labels),
                        _font,
                        new Rectangle(0, 0, LabelSize.X, this.Size.Y),
                        Color.White,
                        false,
                        true,
                        1,
                        LabelAlign,
                        VerticalAlignment.Top
                        );
                spriteBatch.DrawStringOnCtrl(this,
                    String.Join("\n", times),
                    _font,
                    new Rectangle(LabelSize.X, 0, TimeSize.X, this.Size.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    TimeAlign,
                    VerticalAlignment.Top
                    );
                spriteBatch.DrawStringOnCtrl(this,
                    String.Join("\n", ampm),
                    _font,
                    new Rectangle(LabelSize.X + TimeSize.X, 0, AMPMSize.X, this.Size.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Right,
                    VerticalAlignment.Top
                    );
                spriteBatch.DrawStringOnCtrl(this,
                    String.Join("\n", daynightspecial),
                    _font,
                    new Rectangle(LabelSize.X + TimeSize.X + AMPMSize.X, 0, DayNightSize.X, this.Size.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Right,
                    VerticalAlignment.Top
                    );
            }
            else if (FlatClock)
            {
                flClockSizePlaceholder = new Point(
                     (int)_font.MeasureString(String.Join(" ", placeholderFlatClock)).Width,
                     (int)_font.MeasureString(String.Join("", placeholderFlatClock)).Height
                    );

                flClockSizeS = new Point(
                    (int)_font.MeasureString(flClockSp).Width,
                    (int)_font.MeasureString(flClockSp).Height
                    );
                flAMPMSizeS = new Point(
                    (int)_font.MeasureString(ampmcutSp).Width,
                    (int)_font.MeasureString(ampmcutSp).Height
                    );
                flFixSizeS = new Point(
                    (int)_fontSmall.MeasureString(flTimeFixS).Width,
                    (int)_fontSmall.MeasureString(flTimeFixS).Height
                    );

                flClockSizeL = new Point(
                    (int)_font.MeasureString(flClockLp).Width,
                    (int)_font.MeasureString(flClockLp).Height
                    );
                flAMPMSizeL = new Point(
                    (int)_font.MeasureString(ampmcutLp).Width,
                    (int)_font.MeasureString(ampmcutLp).Height
                    );
                flFixSizeL = new Point(
                    (int)_fontSmall.MeasureString(flTimeFixL).Width,
                    (int)_fontSmall.MeasureString(flTimeFixL).Height
                    );

                flClockSizeT = new Point(
                    (int)_font.MeasureString(flClockTp).Width,
                    (int)_font.MeasureString(flClockTp).Height
                    );
                flAMPMSizeT = new Point(
                    (int)_font.MeasureString(ampmcutTp).Width,
                    (int)_font.MeasureString(ampmcutTp).Height
                    );
                flFixSizeT = new Point(
                    (int)_fontSmall.MeasureString(flTimeFixT).Width,
                    (int)_fontSmall.MeasureString(flTimeFixT).Height
                    );

                flDNSize = new Point(
                    (int)_font.MeasureString(flClockDN).Width,
                    (int)_font.MeasureString(flClockDN).Height
                    );
                int firstDistance = flClockSizeS.X + flAMPMSizeS.X + flFixSizeS.X;
                int secondDistance = firstDistance + flClockSizeL.X + flAMPMSizeL.X + flFixSizeL.X;


                this.Size = new Point(flClockSizePlaceholder.X, flClockSizePlaceholder.Y);

                spriteBatch.DrawStringOnCtrl(this,
                    flClockS,
                    _font,
                    new Rectangle(0,0, flClockSizeS.X, flClockSizePlaceholder.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Bottom
                    );
                spriteBatch.DrawStringOnCtrl(this,
                    ampmcutSa,
                    _font,
                    new Rectangle(flClockSizeS.X, 0, flClockSizeS.X + flAMPMSizeS.X, flClockSizePlaceholder.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Bottom
                    );
                spriteBatch.DrawStringOnCtrl(this,
                    flTimeFixS,
                    _fontSmall,
                    new Rectangle(flClockSizeS.X + flAMPMSizeS.X, 0, flClockSizeS.X + flAMPMSizeS.X + flFixSizeS.X, flClockSizePlaceholder.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Middle
                    );

                spriteBatch.DrawStringOnCtrl(this,
                    flClockL,
                    _font,
                    new Rectangle(firstDistance, 0, firstDistance + flClockSizeL.X, flClockSizePlaceholder.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Bottom
                    );
                spriteBatch.DrawStringOnCtrl(this,
                    ampmcutLa,
                    _font,
                    new Rectangle(firstDistance + flClockSizeL.X, 0, firstDistance + flClockSizeL.X + flAMPMSizeL.X, flClockSizePlaceholder.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Bottom
                    );
                spriteBatch.DrawStringOnCtrl(this,
                    flTimeFixL,
                    _fontSmall,
                    new Rectangle(firstDistance + flClockSizeL.X + flAMPMSizeL.X, 0, firstDistance + flClockSizeL.X + flAMPMSizeL.X + flFixSizeL.X, flClockSizePlaceholder.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Middle
                    );

                spriteBatch.DrawStringOnCtrl(this,
                    flClockT,
                    _font,
                    new Rectangle(secondDistance, 0, secondDistance + flClockSizeT.X, flClockSizePlaceholder.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Bottom
                    );
                spriteBatch.DrawStringOnCtrl(this,
                    ampmcutTa,
                    _font,
                    new Rectangle(secondDistance + flClockSizeT.X, 0, secondDistance + flClockSizeT.X + flAMPMSizeT.X, flClockSizePlaceholder.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Bottom
                    );
                spriteBatch.DrawStringOnCtrl(this,
                    flTimeFixT,
                    _fontSmall,
                    new Rectangle(secondDistance + flClockSizeT.X + flAMPMSizeT.X, 0, secondDistance + flClockSizeT.X + flAMPMSizeT.X + flFixSizeT.X, flClockSizePlaceholder.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Middle
                    );
                spriteBatch.DrawStringOnCtrl(this,
                    flClockDN,
                    _font,
                    new Rectangle(secondDistance + flClockSizeT.X + flAMPMSizeT.X + flFixSizeT.X, 0, secondDistance + flClockSizeT.X + flAMPMSizeT.X + flFixSizeT.X + flDNSize.X, flClockSizePlaceholder.Y),
                    Color.White,
                    false,
                    true,
                    1,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Bottom
                    );
            }
        }


    }
}
