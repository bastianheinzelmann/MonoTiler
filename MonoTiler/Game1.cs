using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoTiler
{
    public class Game1 : Game
    {
        public const int SCREEN_WIDTH = 1280, SCREEN_HEIGHT = 720;
        const bool FULLSCREEN = false;
        public static bool EditMode = false;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;

        StartMenu startMenu;
        Editor editor;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = SCREEN_WIDTH,
                PreferredBackBufferHeight = SCREEN_HEIGHT,
                IsFullScreen = FULLSCREEN
            };
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            TextureContainer.LoadTextures(Content);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
            startMenu = new StartMenu(font, this);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(EditMode)
            {
                editor.Update(gameTime);
            }
            else
            {
                startMenu.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            if(EditMode)
            {
                editor.Draw(spriteBatch,gameTime);
            }
            else
            {
                spriteBatch.Begin();
                startMenu.Draw(spriteBatch, gameTime);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public void CreateEditor(string fileName, int mapWidth, int mapHeight,int tileSize)
        {
            editor = new Editor(fileName, mapWidth, mapHeight, 32, GraphicsDevice.Viewport, Content, graphics);
        }

        public void LoadEditor(string fileName)
        {
            editor = Editor.LoadMap(fileName, GraphicsDevice.Viewport, Content, graphics);
        }
    }
}
