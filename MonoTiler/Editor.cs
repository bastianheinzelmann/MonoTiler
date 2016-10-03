using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoTiler
{
    public enum EditorStates
    {
        Layer1 = 0,
        Layer2 = 1,
        Layer3 = 2,
        Layer4 = 3,
        CollisionLayer,
        DeleteMode
    }

    class Editor
    {
        ContentManager content;

        TileSheet[] tileSheets;
        Camera2D editorCamera;
        TileSelectionCamera tileSelectionCamera;

        Viewport editorView;
        Viewport tileSelectionView;

        MouseState mouState;
        MouseState mouState_old;
        int mouseX;
        int mouseY;

        //UI_Stuff
        TextButton Layer1Button;
        TextButton Layer2Button;
        TextButton Layer3Button;
        TextButton Layer4Button;
        TextButton CollisionLayerButton;

        //Map-Stuff
        EditorStates currentState = EditorStates.Layer1;
        int mapOffsetX = 0;
        int mapOffsetY = 0;
        float scale = 1f;

        //tileSelectionStuff
        int tileSelected;           //
        int currentTileSheet = 0;   // currentTileSheet and tileSelected is an important couple, its needed to specify the tile
        int offset = 35;

        TileMap map;
        GraphicsDeviceManager graphics;


        public Editor(int mapWidth, int mapHeight,int tileSize,Viewport view,ContentManager content,GraphicsDeviceManager graphics,params string[] tileSheets)
        {
            this.content = content;
            this.graphics = graphics;
            //---------------------------------------------------------
            editorView = view;
            editorView.Width = ((editorView.Width / 4) * 3) - 100;
            tileSelectionView = view;
            tileSelectionView.Width = tileSelectionView.Width / 4;
            tileSelectionView.X = (view.Width / 4) * 3;
            //Viewportshit---------------------------------------------
            editorCamera = new Camera2D(editorView);
            tileSelectionCamera = new TileSelectionCamera(tileSelectionView);

            editorStartUp();
        }

        void editorStartUp()
        {
            //Load tileSheets
            //the first tileSheet in tileSheets has the index 0 the second 1 and so on (importan for the map and drawing)
            tileSheets = new TileSheet[1];
            tileSheets[0] = new TileSheet(0, 32, 0, 0, content.Load<Texture2D>("Liberty_A4_1"));

            //------------initialization of the map---------------------
            map = new TileMap(20, 8, editorCamera, tileSheets);
            // initialize Buttons
            Layer1Button = new TextButton("Layer 1", TextureContainer.Font, editorView.Width + 10, 20 ,0 ,Color.White, TextureContainer.YellowSquare);
            Layer2Button = new TextButton("Layer 2", TextureContainer.Font, editorView.Width + 10, 50, 0, Color.White, TextureContainer.YellowSquare);
            Layer3Button = new TextButton("Layer 3", TextureContainer.Font, editorView.Width + 10, 80, 0, Color.White, TextureContainer.YellowSquare);
            Layer4Button = new TextButton("Layer 4", TextureContainer.Font, editorView.Width + 10, 110, 0, Color.White, TextureContainer.YellowSquare);
        }

        public void Update(GameTime gameTime)
        {
            mouState = Mouse.GetState();
            mouseX = mouState.X; mouseY = mouState.Y;

            updateUI();

            if (mouseX >= tileSelectionView.X)
            {
                tileSelectionCamera.Update();
                if ((mouState.LeftButton == ButtonState.Pressed) && (mouState_old.LeftButton == ButtonState.Released))
                    updateTileSelection(mouseX, mouseY);
                offset = (int)tileSelectionCamera.Pos.Y + 35;
            }
            else
            {                
                UpdateMap();
            }           


            mouState_old = mouState;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //einmal den editor screen
            Viewport original = graphics.GraphicsDevice.Viewport;           
            drawUI(spriteBatch);
            graphics.GraphicsDevice.Viewport = tileSelectionView;
            drawTileSelection(spriteBatch);
            graphics.GraphicsDevice.Viewport = editorView;
            map.Draw(spriteBatch);
            graphics.GraphicsDevice.Viewport = original;
        }

        #region UI-Stuff

        void drawUI(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(TextureContainer.WhiteSquare, new Rectangle(editorView.Width, 0, 100, editorView.Height), Color.LightSlateGray);
            spriteBatch.Draw(TextureContainer.WhiteSquare, new Rectangle(tileSelectionView.X, 0, tileSelectionView.Width, tileSelectionView.Height), Color.WhiteSmoke);
            Layer1Button.Draw(spriteBatch);
            Layer2Button.Draw(spriteBatch);
            Layer3Button.Draw(spriteBatch);
            Layer4Button.Draw(spriteBatch);
            spriteBatch.End();
        }

        void updateUI()
        {
            Layer1Button.Update();
            if (Layer1Button.ButtonPressed)
                currentState = EditorStates.Layer1;
            Layer2Button.Update();
            if (Layer2Button.ButtonPressed)
                currentState = EditorStates.Layer2;
            Layer3Button.Update();
            if (Layer3Button.ButtonPressed)
                currentState = EditorStates.Layer3;
            Layer4Button.Update();
            if (Layer4Button.ButtonPressed)
                currentState = EditorStates.Layer4;

            //Highlight Button
            if (currentState == EditorStates.Layer1)
                Layer1Button.ButtonImage = TextureContainer.RedSquare;
            else Layer1Button.ButtonImage = TextureContainer.YellowSquare;
            if (currentState == EditorStates.Layer2)
                Layer2Button.ButtonImage = TextureContainer.RedSquare;
            else Layer2Button.ButtonImage = TextureContainer.YellowSquare;
            if (currentState == EditorStates.Layer3)
                Layer3Button.ButtonImage = TextureContainer.RedSquare;
            else Layer3Button.ButtonImage = TextureContainer.YellowSquare;
            if (currentState == EditorStates.Layer4)
                Layer4Button.ButtonImage = TextureContainer.RedSquare;
            else Layer4Button.ButtonImage = TextureContainer.YellowSquare;
        }

        #endregion

        #region Tile Selection

        void updateTileSelection(int x, int y)
        {
            int row = 0; int column = 0;

            for (int i = 0; i <= tileSheets[currentTileSheet].Tiles.Length / 8; i++)
            {
                if (y >= ((35 * i) + offset) && y < ((i + 1) * 35) + offset)
                {
                    row = i;
                    //Console.WriteLine("Rows: " + row);
                    break;
                }
            }

            for (int i = 0; i <= 8; i++)
            {
                if(x >= ((35 * i) + tileSelectionView.X) && x < ((i + 1) * 35) + tileSelectionView.X)
                {
                    column = i;
                    //Console.WriteLine("Columns: " + column);
                    break;
                }
            }

            int result = row * 8 + column;
            if (result < tileSheets[currentTileSheet].Tiles.Length)
                tileSelected = result;
                Console.WriteLine("Tileindex: " + result);

        }

        void drawTileSelection(SpriteBatch spriteBatch)
        {
            Rectangle destinationRect = new Rectangle(0, 0, 32, 32);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, tileSelectionCamera.Transform);
                for (int i = 0; i < tileSheets[currentTileSheet].Tiles.Length; i++)
                {
                    if (i % 8 == 0) { destinationRect.X = 0; destinationRect.Y += 35; }
                    spriteBatch.Draw(tileSheets[currentTileSheet].TileSheetImage, destinationRect, new Rectangle(tileSheets[currentTileSheet].Tiles[i].X, tileSheets[currentTileSheet].Tiles[i].Y, tileSheets[currentTileSheet].TileSize, tileSheets[currentTileSheet].TileSize), Color.White);
                    destinationRect.X += 35;
                }          
            spriteBatch.End();
        }
        #endregion

        #region Map

        void UpdateMap()
        {
            editorCamera.Update();
            mapOffsetX = (int)editorCamera.Pos.X;
            mapOffsetY = (int)editorCamera.Pos.Y;
            scale = editorCamera.Zoom;
            mapPreview();

            switch (currentState)
            {
                case EditorStates.Layer1:
                    {
                        if ((mouState.LeftButton == ButtonState.Pressed) && (mouState_old.LeftButton == ButtonState.Released))
                        {
                            mapPlacement(0, tileSelected, currentTileSheet);
                        }
                        if ((mouState.RightButton == ButtonState.Pressed) == (mouState_old.RightButton == ButtonState.Released))
                        {
                            deleteTile(0);
                        }
                        break;
                    }
                case EditorStates.Layer2:
                    {
                        if ((mouState.LeftButton == ButtonState.Pressed) && (mouState_old.LeftButton == ButtonState.Released))
                        {
                            mapPlacement(1, tileSelected, currentTileSheet);
                        }
                        if ((mouState.RightButton == ButtonState.Pressed) == (mouState_old.RightButton == ButtonState.Released))
                        {
                            deleteTile(1);
                        }
                    }
                    break;
                case EditorStates.Layer3:
                    {
                        if ((mouState.LeftButton == ButtonState.Pressed) && (mouState_old.LeftButton == ButtonState.Released))
                        {
                            mapPlacement(2, tileSelected, currentTileSheet);
                        }
                        if ((mouState.RightButton == ButtonState.Pressed) == (mouState_old.RightButton == ButtonState.Released))
                        {
                            deleteTile(2);
                        }
                    }
                    break;
                case EditorStates.Layer4:
                    {
                        if ((mouState.LeftButton == ButtonState.Pressed) && (mouState_old.LeftButton == ButtonState.Released))
                        {
                            mapPlacement(3, tileSelected, currentTileSheet);
                        }
                        if ((mouState.RightButton == ButtonState.Pressed) == (mouState_old.RightButton == ButtonState.Released))
                        {
                            deleteTile(3);
                        }
                    }
                    break;
                case EditorStates.DeleteMode:
                    break;
                case EditorStates.CollisionLayer:
                    break;
            }
        }

        void mapPlacement(int layer, int tileIndex, int tileSheetIndex)
        {
            //converts mouseposition to Map position
            int xPos = (mouseX - mapOffsetX) / (int)(32 * scale);
            int yPos = (mouseY - mapOffsetY) / (int)(32 * scale);
            Console.WriteLine("Posti: " + "(" + xPos + "|" + yPos + ")");
            if (xPos >= map.MapSizeX || yPos >= map.MapSizeY || xPos < 0 || yPos < 0)
                return;
            map.SetTile(xPos, yPos, layer, tileIndex, tileSheetIndex);
        }

        void deleteTile(int layer)
        {
            Vector2 pos = mouseToMapPosition();
            if (pos.X >= map.MapSizeX || pos.Y >= map.MapSizeY || pos.X < 0 || pos.Y < 0)
                return;
            map.DeleteTile((int)pos.X, (int)pos.Y, layer);
        }

        Vector2 mouseToMapPosition()
        {
            int xPos = (mouseX - mapOffsetX) / (int)(32 * scale);
            int yPos = (mouseY - mapOffsetY) / (int)(32 * scale);
            return new Vector2(xPos, yPos);
        }

        void mapPreview()
        {
            Vector2 pos = mouseToMapPosition();
            if (pos.X >= map.MapSizeX || pos.Y >= map.MapSizeY || pos.X < 0 || pos.Y < 0)
                return;
            map.SetPreview((int)pos.X, (int)pos.Y, currentTileSheet, tileSelected);
        }

        #endregion
    }
}
