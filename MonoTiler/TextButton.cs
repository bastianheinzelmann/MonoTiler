using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoTiler
{
    class TextButton
    {
        SpriteFont font;
        public Texture2D ButtonImage;

        Rectangle buttonBox;
        Vector2 pos;
        int textWidth, textHeight;

        public Color color;
        Color mainColor;
        public string Text;

        MouseState mouseState;
        MouseState oldMouseState;
        public bool ButtonPressed { get; set; }


        // bezieht sich auf linke untere Ecke und rechte obere
        public TextButton (string text, SpriteFont font, int x, int y, int pivot,Color color)
        {
            this.Text = text;
            this.font = font;
            textWidth = (int)this.font.MeasureString(Text).X;
            textHeight = (int)this.font.MeasureString(Text).Y;
            buttonBox = new Rectangle(x, y, textWidth, textHeight);
            ButtonImage = null;
            this.color = color;
            this.mainColor = color;
            setPivot(pivot, x, y);
        }
        
        public TextButton(string text, SpriteFont font,int x, int y ,int pivot,Color color, Texture2D buttonImage)
        {
            this.Text = text;
            this.font = font;
            textWidth = (int)font.MeasureString(text).X;
            textHeight = (int)font.MeasureString(text).Y;
            buttonBox = new Rectangle(x, y, textWidth, textHeight);
            this.ButtonImage = buttonImage;
            this.color = color;
            this.mainColor = color;
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

        int mouseX;
        int mouseY;

        public void Update()
        {
            mouseState = Mouse.GetState();
            mouseX = mouseState.X;
            mouseY = mouseState.Y;

            if (mouseX > pos.X && mouseX < pos.X + textWidth && mouseY > pos.Y && mouseY < pos.Y + textHeight)
            {
                color = Color.Red;
                if ((mouseState.LeftButton == ButtonState.Pressed) && (oldMouseState.LeftButton == ButtonState.Released))
                {
                    ButtonPressed = true;
                }
                else
                    ButtonPressed = false;
            }
            else
                color = mainColor;

            oldMouseState = mouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (ButtonImage != null)
                spriteBatch.Draw(ButtonImage, buttonBox, Color.White);
            spriteBatch.DrawString(font, Text, pos, color);
        }         
    }
}
