using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoTiler
{
    class MapTile
    {
        public int X;
        public int Y;

        public Tuple[] Layers = new Tuple[4];

        public CollisionType Collision;
        
        public MapTile(int x, int y, Tuple tileIndex1, Tuple tileIndex2, Tuple tileIndex3, Tuple tileIndex4, CollisionType collision)
        {
            Layers[0] = tileIndex1;
            Layers[1] = tileIndex2;
            Layers[2] = tileIndex3;
            Layers[3] = tileIndex4;
            Collision = collision;
        }
        
        public MapTile()
        {
            for (int i = 0; i < 4; i++)
            {
                Layers[i].TileIndex = -1;
                Layers[i].TileSheetIndex = -1;
            }
        }     
        
        public void Draw(SpriteBatch spriteBatch)
        {

        }  
    }
}
