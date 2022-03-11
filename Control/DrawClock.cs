using System;
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
        public bool Show24H = false;
        public bool HideLabel = false;
        public bool Drag = false;
        public ContentService.FontSize Font_Size = ContentService.FontSize.Size11;
        public DateTime LocalTime = DateTime.Now;
        public DateTime TyriaTime;
        public DateTime ServerTime;
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
                EnsureLocationIsInBounds();
                _dragging = false;
                Module._settingClockLoc.Value = this.Location;
            }
            base.OnLeftMouseButtonPressed(e);
        }

        public void EnsureLocationIsInBounds() {
            if(Location.X < 1) {
                Location = new Point(1, Location.Y);
            } else if(Location.X + Size.X > Parent.Size.X) {
                Location = new Point(Parent.Size.X - Size.X, Location.Y);
            }

            if(Location.Y < 1) {
                Location = new Point(Location.X, 1);
            } else if(Location.Y + Size.Y > Parent.Size.Y) {
                Location = new Point(Location.X, Parent.Size.Y - Size.Y);
            }
        }

        public override void UpdateContainer(GameTime gameTime) {
            if (_dragging) {
                var nOffset = Input.Mouse.Position - _dragStart;
                Location += nOffset;

                _dragStart = Input.Mouse.Position;
            }
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            string labels = "";
            string times = "";
            Point LabelSize;
            Point TimeSize;
            _font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, Font_Size, ContentService.FontStyle.Regular);

            string format = "h:mm tt";
            if (this.Show24H)
                format = "HH:mm";

            if (this.ShowLocal)
            {
                if (!HideLabel) labels += " Local: \n";
                times += " " + LocalTime.ToString(format) + " \n";
            }
            if (this.ShowTyria) 
            {
                if (!HideLabel) labels += " Tyria: \n";
                times += " " + TyriaTime.ToString(format) + " \n";
            }
            if (this.ShowServer)
            {
                if (!HideLabel) labels += " Server: \n";
                times += " " + ServerTime.ToString(format) + " \n";
            }
            LabelSize = new Point(
                (int)_font.MeasureString(labels).Width,
                (int)_font.MeasureString(labels).Height
                );
            TimeSize = new Point(
                (int)_font.MeasureString(times).Width,
                (int)_font.MeasureString(times).Height
                );
            this.Size = LabelSize + TimeSize;

            if (!HideLabel) 
                spriteBatch.DrawStringOnCtrl(this,
                    labels,
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
                times,
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
