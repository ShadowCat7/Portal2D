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

        private ImageHandler images;

        private Room currentRoom;

        private Element mouseElement;

        private SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            oldKeyboardState = new KeyboardState();

            mouseElement = new Element();
            mouseElement.exists = true;

            images = new ImageHandler();
        }

        protected override void Initialize()
        { base.Initialize(); }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mouseElement.sprite = Content.Load<Texture2D>("Sprites/cursor");

            images.background = Content.Load<Texture2D>("Sprites/Super Kills TF2");
            images.splitScreen = Content.Load<Texture2D>("Sprites/splitscreen");

            images.playerSprite = Content.Load<Texture2D>("Sprites/personBack");

            images.emptyBox = Content.Load<Texture2D>("Sprites/emptyBox");
            images.portalWall = Content.Load<Texture2D>("Sprites/portalWall");
            images.wall = Content.Load<Texture2D>("Sprites/wallBox");

            images.blueBullet = Content.Load<Texture2D>("Sprites/blueBullet");
            images.orangeBullet = Content.Load<Texture2D>("Sprites/orangeBullet");

            images.laserCatcherSprites.Add("UP", Content.Load<Texture2D>("Sprites/Lasers/LaserCatcherUp"));
            images.laserCatcherSprites.Add("DOWN", Content.Load<Texture2D>("Sprites/Lasers/LaserCatcherDown"));
            images.laserCatcherSprites.Add("LEFT", Content.Load<Texture2D>("Sprites/Lasers/LaserCatcherLeft"));
            images.laserCatcherSprites.Add("RIGHT", Content.Load<Texture2D>("Sprites/Lasers/LaserCatcherRight"));

            images.laserShooterSprites.Add("UP", Content.Load<Texture2D>("Sprites/Lasers/LaserShooterUp"));
            images.laserShooterSprites.Add("DOWN", Content.Load<Texture2D>("Sprites/Lasers/LaserShooterDown"));
            images.laserShooterSprites.Add("LEFT", Content.Load<Texture2D>("Sprites/Lasers/LaserShooterLeft"));
            images.laserShooterSprites.Add("RIGHT", Content.Load<Texture2D>("Sprites/Lasers/LaserShooterRight"));

            images.laserSprites.Add(Content.Load<Texture2D>("Sprites/Lasers/LaserUD"));
            images.laserSprites.Add(Content.Load<Texture2D>("Sprites/Lasers/LaserLR"));

            images.bluePortalSprites.Add("LEFT", Content.Load<Texture2D>("Sprites/Portals/bluePortalTop"));
            images.bluePortalSprites.Add("RIGHT", Content.Load<Texture2D>("Sprites/Portals/bluePortalBottom"));
            images.bluePortalSprites.Add("UP", Content.Load<Texture2D>("Sprites/Portals/bluePortalSideTop"));
            images.bluePortalSprites.Add("DOWN", Content.Load<Texture2D>("Sprites/Portals/bluePortalSideBottom"));

            images.orangePortalSprites.Add("LEFT", Content.Load<Texture2D>("Sprites/Portals/orangePortalTop"));
            images.orangePortalSprites.Add("RIGHT", Content.Load<Texture2D>("Sprites/Portals/orangePortalBottom"));
            images.orangePortalSprites.Add("UP", Content.Load<Texture2D>("Sprites/Portals/orangePortalSideTop"));
            images.orangePortalSprites.Add("DOWN", Content.Load<Texture2D>("Sprites/Portals/orangePortalSideBottom"));

            currentRoom = new Room1(images);

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
            if (currentRoom.player.dead)

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

            //spriteBatch.DrawString();

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
