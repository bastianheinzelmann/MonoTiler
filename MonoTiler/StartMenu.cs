using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoTiler
{
    class StartMenu
    {
        TextButton loadButton;
        TextButton newMapButton;
        UITextBox textBox;
        Game1 game;

        SpriteFont font;

        bool coordinateInput = false;
        bool loadFileInput = false;

        public StartMenu(SpriteFont _font, Game1 game)
        {
            font = _font;
            loadButton = new TextButton("LOAD MAP", font, Game1.SCREEN_WIDTH/4, 600, 1 ,Color.White);
            newMapButton = new TextButton("NEW MAP", font, 3*(Game1.SCREEN_WIDTH/4), 600, 1 ,Color.White);
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            if(coordinateInput)
                textBox.Update(gameTime);
            loadButton.Update();
            newMapButton.Update();
            if (newMapButton.ButtonPressed)
            {
                coordinateInput = true;
                textBox = new UITextBox(game.Window, TextureContainer.BlackSquare, TextureContainer.WhiteSquare, font, 0, 0, 200);
            }
            if(loadButton.ButtonPressed)
            {
                loadFileInput = true;
                textBox = new UITextBox(game.Window, TextureContainer.BlackSquare, TextureContainer.WhiteSquare, font, 0, 0, 200);
            }
            if((coordinateInput || loadFileInput) && Keyboard.GetState().IsKeyDown(Keys.Enter) )
            {
                game.CreateEditor("");
                Game1.EditMode = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            loadButton.Draw(spriteBatch);
            newMapButton.Draw(spriteBatch);
            if(coordinateInput || loadFileInput)
                textBox.Draw(spriteBatch, gameTime);
        }

    }
}
