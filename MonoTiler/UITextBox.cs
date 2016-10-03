using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoTiler
{
    class UITextBox
    {
        TextBox textBox;
        KeyboardDispatcher kd;
        GameWindow gameWindow;

        public UITextBox (GameWindow window, Texture2D textImage, Texture2D textBoxImage, SpriteFont font, int x, int y, int width)
        {
            gameWindow = window;
            textBox = new TextBox(textImage, textBoxImage, font);
            textBox.X = x;
            textBox.Y = y;
            textBox.Width = width;
            kd = new KeyboardDispatcher(window);
            kd.Subscriber = textBox;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            textBox.Draw(spriteBatch, gameTime);
        }

        public void Update(GameTime gameTime)
        {
            textBox.Update(gameTime);
        }

        public string GetText()
        {
            return textBox.Text;
        }

    }
}
