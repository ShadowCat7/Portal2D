using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameSpace
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private KeyboardState oldKeyboardState;
        private MouseState oldMouseState;

        private Room currentRoom;

        private Element mouseElement;

        private SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            oldKeyboardState = new KeyboardState();

            currentRoom = new Room1();

            mouseElement = new Element();
            mouseElement.exists = true;
        }

        protected override void Initialize()
        { base.Initialize(); }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mouseElement.sprite = Content.Load<Texture2D>("Sprites/cursor");

            currentRoom.player.sprite = Content.Load<Texture2D>("Sprites/personBack");

            currentRoom.player.blueBullet.sprite = Content.Load<Texture2D>("Sprites/blueBullet");
            currentRoom.player.orangeBullet.sprite = Content.Load<Texture2D>("Sprites/orangeBullet");

            currentRoom.setBoxSprites(Content.Load<Texture2D>("Sprites/wallBox"), Content.Load<Texture2D>("Sprites/portalWall"), Content.Load<Texture2D>("Sprites/emptyBox"));
            currentRoom.background = Content.Load<Texture2D>("Sprites/Super Kills TF2");

            currentRoom.bluePortal.sprites.Add(Content.Load<Texture2D>("Sprites/bluePortal"));
            currentRoom.bluePortal.sprites.Add(Content.Load<Texture2D>("Sprites/bluePortalSide"));

            currentRoom.orangePortal.sprites.Add(Content.Load<Texture2D>("Sprites/orangePortal"));
            currentRoom.orangePortal.sprites.Add(Content.Load<Texture2D>("Sprites/orangePortalSide"));

            currentRoom.screenSplitter = Content.Load<Texture2D>("Sprites/splitscreen");

            font = Content.Load<SpriteFont>("SpriteFont1");
        }


        protected override void UnloadContent()
        { }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState newKeyboardState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();
            mouseElement.screenX = newMouseState.X - mouseElement.sprite.Bounds.Center.X;
            mouseElement.screenY = newMouseState.Y - mouseElement.sprite.Bounds.Center.Y;

            currentRoom.Update(newKeyboardState, oldKeyboardState, newMouseState, oldMouseState);

            oldKeyboardState = newKeyboardState;
            oldMouseState = newMouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            currentRoom.Draw(spriteBatch);

            mouseElement.Draw(spriteBatch);

            spriteBatch.DrawString(font, mouseElement.screenX.ToString(), new Vector2(10, 10), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
