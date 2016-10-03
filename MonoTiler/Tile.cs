using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoTiler
{
    class Tile
    {
        public int Index;
        // dependend on the left upper corner to the left upper corner of the tile (on the tilesheet)
        public int X;
        public int Y;

        public Tile(int index,int x,int y)
        {
            Index = index;
            X = x;
            Y = y;
        }
    }
}
