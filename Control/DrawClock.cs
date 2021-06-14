using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Clock
{
    class DrawClock : Container
    {
        public bool ShowLocal = false;
        public bool ShowTyria = false;
        public bool ShowServer = false;
        public ContentService.FontSize Font_Size;
        public DateTime LocalTime;
        public DateTime TyriaTime;
        public DateTime ServerTime;

        private static BitmapFont _font; 

        public DrawClock()
        {
            this.ShowLocal = false;
            this.ShowTyria = false;
            this.ShowServer = false;
            this.Font_Size = ContentService.FontSize.Size11;
            this.LocalTime = DateTime.Now;
            this.TyriaTime = DateTime.Now;
            this.ServerTime = DateTime.Now;
            this.Location = new Point(0, 0);

            this.Size = new Point(0, 0);
            this.Visible = true;
            this.ZIndex = 0;
            this.Padding = Thickness.Zero;
        }

        protected override CaptureType CapturesInput()
        {
            return CaptureType.ForceNone;
        }


        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            string text = "";
            int height;
            int width;
            _font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, Font_Size, ContentService.FontStyle.Regular);

            if (this.ShowLocal)
                text += " Local: " + LocalTime.ToString("h:mm tt") + " \n";
            if (this.ShowTyria)
                text += " Tyria: " + TyriaTime.ToString("h:mm tt") + " \n";
            if (this.ShowServer)
                text += " Server: " + ServerTime.ToString("h:mm tt") + "  \n";
            width = (int)_font.MeasureString(text).Width;
            height = (int)_font.MeasureString(text).Height;
            this.Size = new Point(width, height);

            spriteBatch.DrawStringOnCtrl(this, 
                text, 
                _font,
                new Rectangle(0, 0, width, height), 
                Color.White, 
                false, 
                true, 
                1, 
                HorizontalAlignment.Right, 
                VerticalAlignment.Top
                );

        }

    }
}
