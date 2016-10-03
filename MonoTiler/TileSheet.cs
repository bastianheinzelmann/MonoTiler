using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoTiler
{
    class TileSheet
    {
        public int Index;

        public Tile[] Tiles;
        public Texture2D TileSheetImage;
        int distanceX;
        int distanceY;
        public int TileSize;

        public TileSheet(int index, int tileSize, int distanceX, int distanceY, Texture2D tileSheetImage)
        {
            Index = index;
            this.TileSheetImage = tileSheetImage;
            TileSize = tileSize;
            this.distanceX = distanceX;
            this.distanceY = distanceY;
            tilesSetup();
        }

        void tilesSetup()
        {
            int blubbY = TileSheetImage.Bounds.Height / (TileSize + distanceY);
            int blubbX = TileSheetImage.Bounds.Width / (TileSize + distanceX);
            Tiles = new Tile[blubbX * blubbY];
            Console.WriteLine(Tiles.Length);

            int counter = 0;

            for (int j = 0; j < blubbY; j++)
            {
                for (int i = 0; i < blubbX; i++)
                {
                    Tiles[counter] = new Tile(i, i * TileSize + distanceX, j * TileSize + distanceY);
                    counter++;
                }                
            }
        }
    }
}
