using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoTiler
{
    class TextureContainer
    {
        public static Texture2D BlackSquare;
        public static Texture2D WhiteSquare;
        public static Texture2D TileShape;
        public static Texture2D Grid;
        public static SpriteFont Font;
        public static Texture2D YellowSquare;
        public static Texture2D RedSquare;

        public static void LoadTextures(ContentManager content)
        {
            BlackSquare = content.Load <Texture2D>("blackBox");
            WhiteSquare = content.Load<Texture2D>("Cursor");
            TileShape = content.Load<Texture2D>("Tile");
            Grid = content.Load<Texture2D>("grid");
            Font = content.Load<SpriteFont>("font");
            YellowSquare = content.Load<Texture2D>("yellowBlock");
            RedSquare = content.Load<Texture2D>("redBlock");
        }
    }
}
