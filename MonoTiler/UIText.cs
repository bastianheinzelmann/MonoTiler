using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace MonoTiler
{
    class UIText
    {
        SpriteFont font;
        Texture2D buttonImage;

        Rectangle buttonBox;
        Vector2 pos;
        int textWidth, textHeight;

        public Color color;
        public string Text;

        public UIText(string text, SpriteFont font, int x, int y, int pivot, Color color)
        {
            this.Text = text;
            this.font = font;
            textWidth = (int)this.font.MeasureString(Text).X;
            textHeight = (int)this.font.MeasureString(Text).Y;
            buttonBox = new Rectangle(x, y, textWidth, textHeight);
            buttonImage = null;
            this.color = color;
            setPivot(pivot, x, y);
        }

        public UIText(string text, SpriteFont font, int x, int y, int pivot, Color color, Texture2D buttonImage)
        {
            this.Text = text;
            this.font = font;
            textWidth = (int)font.MeasureString(text).X;
            textHeight = (int)font.MeasureString(text).Y;
            buttonBox = new Rectangle(x, y, textWidth, textHeight);
            this.buttonImage = buttonImage;
            this.color = color;
            setPivot(pivot, x, y);
        }

        void setPivot(int pivot, int x, int y)
        {
            switch (pivot)
            {
                case 1:
                    {

                        int offx = (int)font.MeasureString(Text).X / 2;
                        int offy = (int)font.MeasureString(Text).Y / 2;
                        pos = new Vector2(x - offx, y - offy);
                        break;
                    }
                case 0:
                    {
                        pos = new Vector2(x, y);
                        break;
                    }

            }
        }

    }
}
