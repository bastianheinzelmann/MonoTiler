using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoTiler
{
    class TileMap
    {
        Camera2D camera;
        
        public MapTile[,] Grid;
        public int MapSizeX;
        public int MapSizeY;

        //preview
        int previewTileSheetIndex = 0;
        int previewTileIndex = 0;
        int previewPosX = 0;
        int previewPosY = 0;

        EditorStates currentState = EditorStates.Layer1;

        TileSheet[] tileSheets;

        public TileMap(int mapSizeX, int mapSizeY, Camera2D camera, TileSheet[] tileSheets)
        {
            MapSizeX = mapSizeX;
            MapSizeY = mapSizeY;
            this.tileSheets = tileSheets;
            this.camera = camera;
            initializeGrid();
        }

        void initializeGrid()
        {
            Grid = new MapTile[MapSizeX,MapSizeY];

            for (int y = 0; y < MapSizeY; y++)
            {
                for (int x = 0; x < MapSizeX; x++)
                {
                    Grid[x, y] = new MapTile();
                    Grid[x, y].X = x * 32;
                    Grid[x, y].Y = y * 32;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            camera.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draws the grid
            spriteBatch.Begin(SpriteSortMode.Deferred ,BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transform);
            foreach(MapTile tile in Grid)
            {
                spriteBatch.Draw(TextureContainer.Grid, new Vector2(tile.X, tile.Y), Color.White);
                for (int i = 0; i < 4; i++)
                {
                    if (tile.Layers[i].TileIndex >= 0 && tile.Layers[i].TileSheetIndex >= 0) // if it's below 0 the layer is empty
                    {
                        spriteBatch.Draw(tileSheets[tile.Layers[i].TileSheetIndex].TileSheetImage, new Vector2(tile.X, tile.Y), new Rectangle(tileSheets[tile.Layers[i].TileSheetIndex].Tiles[tile.Layers[i].TileIndex].X, tileSheets[tile.Layers[i].TileSheetIndex].Tiles[tile.Layers[i].TileIndex].Y, 32, 32), Color.White);
                    }
                }               
            }
            //draw preview on top of everything else
            spriteBatch.Draw(tileSheets[previewTileSheetIndex].TileSheetImage, new Vector2(Grid[previewPosX, previewPosY].X, Grid[previewPosX, previewPosY].Y), new Rectangle(tileSheets[previewTileSheetIndex].Tiles[previewTileIndex].X, tileSheets[previewTileSheetIndex].Tiles[previewTileIndex].Y, 32, 32), Color.White * 0.5f);
            spriteBatch.End();
        }

        public void SetState(EditorStates state)
        {
            currentState = state;
        }

        public void SetTile(int x, int y, int layer, int tileIndex, int tileSheetIndex)
        {
            Grid[x, y].Layers[layer].TileIndex = tileIndex;
            Grid[x, y].Layers[layer].TileSheetIndex = tileSheetIndex;
        }

        public void DeleteTile(int x, int y, int layer)
        {
            Grid[x, y].Layers[layer].TileIndex = -1;
            Grid[x, y].Layers[layer].TileSheetIndex = -1;
            Console.WriteLine("Deleted Tile: ({0}|{1})", x, y);
        }

        public void SetPreview(int mapX, int mapY, int tileSheetIndex, int tileIndex)
        {
            previewPosX = mapX;
            previewPosY = mapY;
            previewTileIndex = tileIndex;
            previewTileSheetIndex = tileSheetIndex;
        }

        public void SetCollision(int mapX, int mapY, CollisionType collision)
        {
            Grid[mapX, mapY].Collision = collision;
        }
    }
}
