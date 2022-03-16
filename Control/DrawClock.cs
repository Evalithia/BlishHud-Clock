using System;
using System.Collections.Generic;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

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
        public ContentService.FontSize Font_Size = ContentService.FontSize.Size11;
        public DateTime LocalTime = DateTime.Now;
        public DateTime TyriaTime;
        public DateTime ServerTime;
        public string DayNightTime;
        public HorizontalAlignment LabelAlign = HorizontalAlignment.Right;
        public HorizontalAlignment TimeAlign = HorizontalAlignment.Right;

        private static BitmapFont _font;
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
            Point LabelSize;
            Point TimeSize;
            _font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, Font_Size, ContentService.FontStyle.Regular);

            string format = "h:mm tt";
            if (this.Show24H)
                format = "HH:mm";

            if (this.ShowLocal)
            {
                if (!HideLabel) labels.Add(" Local: ");
                times.Add(" " + LocalTime.ToString(format));
            }
            if (this.ShowTyria) 
            {
                if (!HideLabel) labels.Add(" Tyria: ");
                times.Add(" " + TyriaTime.ToString(format));
            }
            if (this.ShowServer) {
                if (!HideLabel) labels.Add(" Server: ");
                times.Add(" " + ServerTime.ToString(format));
            }
            if (this.ShowDayNight) {
                if (!HideLabel) labels.Add(" ");
                times.Add(" " + DayNightTime);
            }
            LabelSize = new Point(
                (int)_font.MeasureString(String.Join("\n",labels)).Width,
                (int)_font.MeasureString(String.Join("\n", labels)).Height
                );
            TimeSize = new Point(
                (int)_font.MeasureString(String.Join("\n", times)).Width,
                (int)_font.MeasureString(String.Join("\n", times)).Height
                );
            int maxHeight = Math.Max(LabelSize.Y, TimeSize.Y);
            this.Size = new Point(LabelSize.X + TimeSize.X, maxHeight);

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
        }

    }
}
