using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//TODO:
//  2. get and set for each variable,
//  3. public and private for each variable.

namespace GameSpace
{
    /// <summary>
    /// This class holds the size of the screen.
    /// </summary>
    public static class Screen
    { static public int X = 800, Y = 480; }
    /// <summary>
    /// This class holds the size of the screen when it is split in half x-wise.
    /// </summary>
    public static class SplitScreen
    { static public int X = 400, Y = 480; }

    /// <summary>
    /// The parent for all rooms in the game.
    /// </summary>
    public abstract class Room
    {
        /// <summary>
        /// The image for the background of the room.
        /// </summary>
        public Texture2D background;
        /// <summary>
        /// The size of the room.
        /// </summary>
        public int sizeX, sizeY;
        /// <summary>
        /// The variable size of the screen. The X value will change when the screen is split.
        /// </summary>
        public int screenX, screenY;
        public int cutSceneTimer;
        /// <summary>
        /// The top left position of the view that fits on the screen.
        /// </summary>
        public int onScreenX, onScreenY;
        /// <summary>
        /// The top left position of the view that fits on the right side of the split screen.
        /// </summary>
        public int portedOnScreenX, portedOnScreenY;
        /// <summary>
        /// The list of boxes in the room.
        /// </summary>
        public List<Box> boxList;
        /// <summary>
        /// The list of laser shooters in the room.
        /// </summary>
        public List<LaserShooter> laserShooters;
        /// <summary>
        /// The list of laser catchers in the room.
        /// </summary>
        public List<LaserCatcher> laserCatchers;
        /// <summary>
        /// The user's character in the room.
        /// </summary>
        public Player player;
        /// <summary>
        /// The room's portals. There are two: the blue and orange portals.
        /// </summary>
        public Portal bluePortal, orangePortal;
        /// <summary>
        /// The image that splits the screen when teleporting.
        /// </summary>
        public Texture2D screenSplitter;

        /// <summary>
        /// Initializes the room. Only provided as the default constructor for the Room's children.
        /// </summary>
        public Room() { }
        /// <summary>
        /// Initializes the room. Specifically initializes the images in the room.
        /// </summary>
        /// <param name="images">The global ImageHandler that holds the images.</param>
        public Room(ImageHandler images)
        { cutSceneTimer = 0; }
        /// <summary>
        /// Draws the background of the room.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch for the game.</param>
        public void DrawBackground(SpriteBatch spriteBatch)
        { spriteBatch.Draw(background, new Vector2(0, 0), Color.White); }
        /// <summary>
        /// Sets the sprites for every box in the list of boxes.
        /// </summary>
        /// <param name="normalWallSprite">The image of the normal wall.</param>
        /// <param name="portalWallSprite">The image of the wall that can hold portals.</param>
        /// <param name="emptyFloorSprite">The image of the empty pit.</param>
        public void setBoxSprites(Texture2D normalWallSprite, Texture2D portalWallSprite, 
            Texture2D emptyFloorSprite)
        {
            for (int i = 0; i < boxList.Count; i++)
            {
                if (boxList[i].portalUse)
                { boxList[i].sprite = portalWallSprite; }
                else if (boxList[i].empty)
                { boxList[i].sprite = emptyFloorSprite; }
                else
                { boxList[i].sprite = normalWallSprite; }
            }
        }
        /// <summary>
        /// Sets the sprites for every laser related instance in the room.
        /// </summary>
        /// <param name="textures">The Dictionary of sprites that has the laser shooter sprites.
        /// </param>
        /// <param name="catcherTextures">The Dictionary of sprites that has the laser catcher 
        /// sprites.</param>
        public void setAllLaserSprites(Dictionary<string, Texture2D> shooterTextures, 
            Dictionary<string, Texture2D> catcherTextures)
        {
            for (int i = 0; i < laserShooters.Count; i++)
            {
                if (laserShooters[i].direction == 0)
                { laserShooters[i].sprite = shooterTextures["UP"]; }
                if (laserShooters[i].direction == Math.PI)
                { laserShooters[i].sprite = shooterTextures["DOWN"]; }
                if (laserShooters[i].direction == -Math.PI / 2)
                { laserShooters[i].sprite = shooterTextures["LEFT"]; }
                if (laserShooters[i].direction == Math.PI / 2)
                { laserShooters[i].sprite = shooterTextures["RIGHT"]; }
            }

            for (int i = 0; i < laserCatchers.Count; i++)
            {
                if (laserCatchers[i].direction == 0)
                { laserCatchers[i].sprite = catcherTextures["UP"]; }
                if (laserCatchers[i].direction == Math.PI)
                { laserCatchers[i].sprite = catcherTextures["DOWN"]; }
                if (laserCatchers[i].direction == -Math.PI / 2)
                { laserCatchers[i].sprite = catcherTextures["LEFT"]; }
                if (laserCatchers[i].direction == Math.PI / 2)
                { laserCatchers[i].sprite = catcherTextures["RIGHT"]; }
            }
        }

        //Virtual methods
        public virtual void Update(KeyboardState newKeyboardState, KeyboardState oldKeyboardstate, 
            MouseState newMouseState, 
            MouseState oldMouseState) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        //Virtual methods for GameRoom
        public virtual void setBoxSprites() 
        { }
        public virtual void drawBoxes(SpriteBatch spriteBatch) { }
        public virtual void AddBoxesAround() { }
        public virtual void playerDied() { }
    }

    /// <summary>
    /// The parent class for rooms that will have game play.
    /// </summary>
    public class GameRoom : Room
    {
        /// <summary>
        /// Initializes the GameRoom. Provides initialization for the room size, the list of boxes, 
        /// the player,the boxes surroundingElement the room, the screen size, and the two portals.
        /// </summary>
        public GameRoom()
        { }

        /// <summary>
        /// Adds boxes to the list of boxes that will surround the room.
        /// </summary>
        public override void AddBoxesAround()
        {
            for (int i = 0; i < sizeX; i += 100)
            { boxList.Add(new Box(i, 0, false, false)); }
            for (int i = 0; i < sizeX; i += 100)
            { boxList.Add(new Box(i, sizeY - 100, false, false)); }
            for (int i = 100; i < sizeY - 100; i += 100)
            { boxList.Add(new Box(sizeX - 100, i, false, false)); }
            for (int i = 100; i < sizeY - 100; i += 100)
            { boxList.Add(new Box(0, i, false, false)); }
        }

        /// <summary>
        /// Updates the logic of the GameRoom.
        /// </summary>
        /// <param name="newKeyboardState">The current KeyboardState.</param>
        /// <param name="oldKeyboardstate">The previous KeyboardState.</param>
        /// <param name="newMouseState">The current MouseState.</param>
        /// <param name="oldMouseState">The previous MouseState.</param>
        public override void Update(KeyboardState newKeyboardState, KeyboardState oldKeyboardstate, 
            MouseState newMouseState, MouseState oldMouseState)
        {
            if (!player.dead)
            {
                player.Update(newKeyboardState, oldKeyboardstate, newMouseState, oldMouseState, boxList, laserShooters,
                    ref bluePortal, ref orangePortal);
            }
            else
            {
                playerDied();
                player.blueBullet.Update();
                player.orangeBullet.Update();
            }

            if (player.ported)
            {
                screenX = SplitScreen.X;
                screenY = SplitScreen.Y;
            }
            else
            {
                screenX = Screen.X;
                screenY = Screen.Y;
            }

            // Finds the position of the player on the screen with regard to its position in the room
            // for the purpose of scrolling.
            if (player.roomX + player.sprite.Bounds.Center.X <= screenX / 2)
            {
                player.screenX = player.roomX;
                onScreenX = 0;
            }
            else if (player.roomX + player.sprite.Bounds.Center.X >= sizeX - screenX / 2)
            {
                player.screenX = screenX - (sizeX - player.roomX);
                onScreenX = sizeX - screenX;
            }
            else
            {
                player.screenX = screenX / 2 - player.sprite.Bounds.Center.X;
                onScreenX = player.roomX + player.sprite.Bounds.Center.X - screenX / 2;
            }

            if (player.roomY + player.sprite.Bounds.Center.Y <= screenY / 2)
            {
                player.screenY = player.roomY;
                onScreenY = 0;
            }
            else if (player.roomY + player.sprite.Bounds.Center.Y >= sizeY - screenY / 2)
            {
                player.screenY = screenY - (sizeY - player.roomY);
                onScreenY = sizeY - screenY;
            }
            else
            {
                player.screenY = screenY / 2 - player.sprite.Bounds.Center.Y;
                onScreenY = player.roomY + player.sprite.Bounds.Center.Y - screenY / 2;
            }

            for (int i = 0; i < laserShooters.Count; i++)
            {
                laserShooters[i].Update(boxList, bluePortal, orangePortal);
                laserShooters[i].setScreenPosition(onScreenX, onScreenY);
                laserShooters[i].laser.Update(boxList, bluePortal, orangePortal);
                laserShooters[i].laser.setScreenPosition(onScreenX, onScreenY);
            }

            // Puts the boxes on the screen in their screen position with regard to their room position.
            for (int i = 0; i < boxList.Count; i++)
            { boxList[i].setScreenPosition(onScreenX, onScreenY); }

            // Puts the bullets on the screen in their screen position with regard to their room position.
            player.blueBullet.setScreenPosition(onScreenX, onScreenY);
            player.orangeBullet.setScreenPosition(onScreenX, onScreenY);

            // Puts the portals on the screen in their screen position with regard to their room position.
            if (bluePortal.exists)
            { bluePortal.setScreenPosition(onScreenX, onScreenY); }
            if (orangePortal.exists)
            { orangePortal.setScreenPosition(onScreenX, onScreenY); }

            // Puts everything that is on the viewport of the players ported position in its screen position.
            if (player.ported)
            {
                // For the player.
                if (player.portedRoomX + player.sprite.Bounds.Center.X <= screenX / 2)
                {
                    player.portedScreenX = player.portedRoomX + screenX;
                    portedOnScreenX = 0;
                }
                else if (player.portedRoomX + player.sprite.Bounds.Center.X >= sizeX - screenX / 2)
                {
                    player.portedScreenX = 2 * screenX - (sizeX - player.portedRoomX);
                    portedOnScreenX = sizeX - screenX;
                }
                else
                {
                    player.portedScreenX = screenX / 2 - player.sprite.Bounds.Center.X + screenX;
                    portedOnScreenX = player.portedRoomX + player.sprite.Bounds.Center.X - screenX / 2;
                }

                if (player.portedRoomY + player.sprite.Bounds.Center.Y <= screenY / 2)
                {
                    player.portedScreenY = player.portedRoomY;
                    portedOnScreenY = 0;
                }
                else if (player.portedRoomY + player.sprite.Bounds.Center.Y >= sizeY - screenY / 2)
                {
                    player.portedScreenY = screenY - (sizeY - player.portedRoomY);
                    portedOnScreenY = sizeY - screenY;
                }
                else
                {
                    player.portedScreenY = screenY / 2 - player.sprite.Bounds.Center.Y;
                    portedOnScreenY = player.portedRoomY + player.sprite.Bounds.Center.Y - screenY / 2;
                }

                player.setPortedScreenPosition(portedOnScreenX, portedOnScreenY);

                //For the laser shooters.
                for (int i = 0; i < laserShooters.Count; i++)
                {
                    laserShooters[i].setPortedScreenPosition(portedOnScreenX, portedOnScreenY);
                    laserShooters[i].laser.setPortedScreenPosition(portedOnScreenX, portedOnScreenY);
                }

                // For the boxes.
                for (int i = 0; i < boxList.Count; i++)
                { boxList[i].setPortedScreenPosition(portedOnScreenX, portedOnScreenY); }

                // For the bullets.
                player.blueBullet.setPortedScreenPosition(portedOnScreenX, portedOnScreenY);
                player.orangeBullet.setPortedScreenPosition(portedOnScreenX, portedOnScreenY);

                // For the portals.
                if (bluePortal.exists)
                { bluePortal.setPortedScreenPosition(portedOnScreenX, portedOnScreenY); }
                if (orangePortal.exists)
                { orangePortal.setPortedScreenPosition(portedOnScreenX, portedOnScreenY); }
            }
        }

        /// <summary>
        /// Draws the images in the room.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch for the game.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(0, 0), new Rectangle(onScreenX, onScreenY, Screen.X, Screen.Y), 
                Color.White);

            for (int i = 0; i < boxList.Count; i++)
            {
                if (boxList[i].empty)
                { spriteBatch.Draw(boxList[i].sprite, new Vector2(boxList[i].screenX, boxList[i].screenY), Color.White); }
            }

            bluePortal.DrawBottom(spriteBatch, false);
            orangePortal.DrawBottom(spriteBatch, false);
            
            spriteBatch.Draw(player.sprite, new Rectangle(player.screenX + player.sprite.Bounds.Center.X, 
                player.screenY + player.sprite.Bounds.Center.Y, player.sprite.Width, player.sprite.Height),
                null, Color.White, (float)player.direction, new Vector2(player.sprite.Bounds.Center.X, 
                    player.sprite.Bounds.Center.Y), new SpriteEffects(), 0);
            //TODO
            //if (player.ported)
            //    spriteBatch.Draw(player.sprite, new Rectangle(player.portedScreenX - 400 + player.sprite.Bounds.Center.X,
            //        player.portedScreenY + player.sprite.Bounds.Center.Y, player.sprite.Width, player.sprite.Height),
            //    null, Color.White, (float)player.direction, new Vector2(player.sprite.Bounds.Center.X,
            //        player.sprite.Bounds.Center.Y), new SpriteEffects(), 0);

            player.blueBullet.Draw(spriteBatch);
            player.orangeBullet.Draw(spriteBatch);

            for (int i = 0; i < laserShooters.Count; i++)
            {
                laserShooters[i].Draw(spriteBatch);
                laserShooters[i].laser.Draw(spriteBatch);
            }

            drawBoxes(spriteBatch);
            bluePortal.Draw(spriteBatch);
            orangePortal.Draw(spriteBatch);

            if (player.ported)
            {
                spriteBatch.Draw(background, new Vector2(400, 0), new Rectangle(portedOnScreenX, portedOnScreenY, 
                    SplitScreen.X, SplitScreen.Y), Color.White);

                for (int i = 0; i < boxList.Count; i++)
                {
                    if (boxList[i].empty)
                    { boxList[i].DrawPorted(spriteBatch); }
                }

                bluePortal.DrawBottom(spriteBatch, true);
                orangePortal.DrawBottom(spriteBatch, true);

                spriteBatch.Draw(player.sprite, new Rectangle(player.portedScreenX + player.sprite.Bounds.Center.X,
                    player.portedScreenY + player.sprite.Bounds.Center.Y, player.sprite.Width, player.sprite.Height),
                null, Color.White, (float)player.direction, new Vector2(player.sprite.Bounds.Center.X,
                    player.sprite.Bounds.Center.Y), new SpriteEffects(), 0);
                player.DrawPorted(spriteBatch);

                for (int i = 0; i < laserShooters.Count; i++)
                {
                    laserShooters[i].DrawPorted(spriteBatch);
                    laserShooters[i].laser.DrawPorted(spriteBatch);
                }

                for (int i = 0; i < boxList.Count; i++)
                {
                    if (!boxList[i].empty)
                    { boxList[i].DrawPorted(spriteBatch); }
                }

                if (player.blueBullet.exists)
                { player.blueBullet.DrawPorted(spriteBatch); }
                if (player.orangeBullet.exists)
                { player.orangeBullet.DrawPorted(spriteBatch); }

                bluePortal.DrawPorted(spriteBatch);
                orangePortal.DrawPorted(spriteBatch);

                spriteBatch.Draw(screenSplitter, new Vector2(SplitScreen.X - 5, 0), Color.White);
            }
        }
        /// <summary>
        /// Draws all the boxes that are not empty boxes.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch for the game.</param>
        public override void drawBoxes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < boxList.Count; i++)
            {
                if (!boxList[i].empty)
                { spriteBatch.Draw(boxList[i].sprite, new Vector2(boxList[i].screenX, boxList[i].screenY), Color.White); }
            }
        }
    }

    /// <summary>
    /// The parent class for everything in a room.
    /// </summary>
    public class Element
    {
        /// <summary>
        /// The position the element has on the screen.
        /// </summary>
        public int screenX, screenY;
        /// <summary>
        /// The position the element has in the room.
        /// </summary>
        public int roomX, roomY;
        /// <summary>
        /// The position the element has on the right viewport.
        /// </summary>
        public int portedScreenX, portedScreenY;
        /// <summary>
        /// The flag that, if true, says that the mouse is currently hovering over the element.
        /// </summary>
        public bool mouseOver;
        /// <summary>
        /// The flag that, if true, says that the element currently exists in the game.
        /// </summary>
        public bool exists;
        /// <summary>
        /// The image of the element.
        /// </summary>
        public Texture2D sprite;
        /// <summary>
        /// The images that the element can use as its image.
        /// </summary>
        public Dictionary<string, Texture2D> sprites;
        /// <summary>
        /// Initializes the element. This is only provided as the default constructor for the element's children.
        /// </summary>
        public Element() { }
        /// <summary>
        /// Initializes the element. Provides initialization for the room position, the mouse detection, 
        /// the list of sprites, and the flag for being on the screen.
        /// </summary>
        /// <param name="argX">An integer for the position of the element.</param>
        /// <param name="argY">An integer for the position of the element.</param>
        public Element(int argX, int argY)
        {
            roomX = argX;
            roomY = argY;
            mouseOver = false;
            sprites = new Dictionary<string,Texture2D>();
        }
        /// <summary>
        /// Updates the logic for the element.
        /// </summary>
        public virtual void Update() { }
        /// <summary>
        /// Updates the logic for the element. This is specifically for the inherited member of Player.
        /// </summary>
        /// <param name="newKeyboardState">The current KeyboardState.</param>
        /// <param name="oldKeyboardState">The previous KeyboardState.</param>
        /// <param name="newMouseState">The current MouseState.</param>
        /// <param name="oldMouseState">The previous MouseState.</param>
        /// <param name="boxList">The list of boxes in the current room.</param>
        /// <param name="bluePortal">The blue portal in the room.</param>
        /// <param name="orangePortal">The orange portal in the room.</param>
        public virtual void Update(KeyboardState newKeyboardState, KeyboardState oldKeyboardState, 
            MouseState newMouseState, MouseState oldMouseState, List<Box> boxList, List<LaserShooter> laserList,
            ref Portal bluePortal, ref Portal orangePortal) { }
        /// <summary>
        /// Updates the logic for the element. This is specifically for the inherited member of Bullet.
        /// </summary>
        /// <param name="boxList">The list of boxes in the current room.</param>
        /// <param name="portal">The portal that the bullet will create if it collides with a box that 
        /// can hold a portal.</param>
        /// <param name="otherPortal">The portal that the bullet cannot create.</param>
        public virtual void Update(List<Element> boxList, ref Portal portal, ref Portal otherPortal) { }
        /// <summary>
        /// Draws the image of the element.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch for the game.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (exists)
            { spriteBatch.Draw(sprite, new Vector2(screenX, screenY), Color.White); }
        }
        /// <summary>
        /// Draws the image of the element on the ported side of the screen.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch for the game.</param>
        public virtual void DrawPorted(SpriteBatch spriteBatch)
        {
            if (portedScreenX < SplitScreen.X)
            {
                if (portedScreenX + sprite.Width < SplitScreen.X)
                { }
                else
                {
                    spriteBatch.Draw(sprite, new Vector2(SplitScreen.X, portedScreenY),
                                     new Rectangle(SplitScreen.X - portedScreenX, 0,
                                     sprite.Width - (SplitScreen.X - portedScreenX), sprite.Height), Color.White);
                }
            }
            else
            { spriteBatch.Draw(sprite, new Vector2(portedScreenX, portedScreenY), Color.White); }
        }
        /// <summary>
        /// Sets the position of the element on the screen.
        /// </summary>
        /// <param name="onScreenX">The top left position of the screen.</param>
        /// <param name="onScreenY">The top left position of the screen.</param>
        public virtual void setScreenPosition(int onScreenX, int onScreenY)
        {
            screenX = roomX - onScreenX;
            screenY = roomY - onScreenY;
        }
        /// <summary>
        /// Sets the position of the element on the ported screen.
        /// </summary>
        /// <param name="portedOnScreenX">The top left position of the ported screen.</param>
        /// <param name="portedOnScreenY">The top left position of the ported screen.</param>
        public virtual void setPortedScreenPosition(int portedOnScreenX, int portedOnScreenY)
        {
            portedScreenX = roomX - portedOnScreenX + 400;
            portedScreenY = roomY - portedOnScreenY;
        }
    }

    /// <summary>
    /// The class for boxes. Boxes are used as walls and empty pits.
    /// </summary>
    public class Box : Element
    {
        /// <summary>
        /// The flag that, if true, says that the box can hold portals.
        /// </summary>
        public bool portalUse;
        /// <summary>
        /// The flag that, if true, says that the box is an empty pit.
        /// </summary>
        public bool empty;
        
        /// <summary>
        /// Initializes the box. Initializes the box's position in the room, the mouse detection, 
        /// the list of sprites, the flag for holding portals, and the flag for being an empty pit.
        /// </summary>
        /// <param name="argX">The integer for the x-position of the box.</param>
        /// <param name="argY">The integer for the y-position of the box.</param>
        /// <param name="portalBox">A bool that, if true, makes the box able to hold portals.</param>
        /// <param name="emptyBox">A bool that, if true, makes the box an empty pit.</param>
        public Box(int argX, int argY, bool portalBox, bool emptyBox)
        {
            roomX = argX;
            roomY = argY;
            mouseOver = false;
            sprites = new Dictionary<string,Texture2D>(0);
            portalUse = portalBox;
            empty = emptyBox;
        }
    }

    /// <summary>
    /// The class for the user's input. The player is the character.
    /// </summary>
    public class Player : Element
    {
        /// <summary>
        /// The amount of pixels moved in one unit of gametime.
        /// </summary>
        private int speed;
        /// <summary>
        /// The flag that, if true, says that the player is currently in a portal.
        ///     How this is different from the bool porting:
        ///         Porting is the flag that, if true, says that the player is currently moving through
        ///         a portal.
        /// </summary>
        public bool ported;
        /// <summary>
        /// The flag that, if true, says that the player is currently moving through a portal.
        ///     How this is different from the bool ported:
        ///         Ported is the flag that, if true, says that the player is currently in a portal.
        /// </summary>
        private bool porting;
        public bool dead;
        /// <summary>
        /// The position of the player's image on the other viewport.
        /// </summary>
        public int portedRoomX, portedRoomY;
        public int tempPortedScreenX, tempPortedScreenY;

        /// <summary>
        /// The direction that the player is currently facing.
        /// </summary>
        public double direction;
        /// <summary>
        /// The bullets of the player. There are two bullets, the blue bullet and the orange bullet, 
        /// that correspond to the two portals.
        /// </summary>
        public Bullet blueBullet, orangeBullet;
        /// <summary>
        /// The counter for the amount of time it takes to shoot another bullet.
        /// </summary>
        private int rateOfFire;

        /// <summary>
        /// Initializes the player. Initializes the player's position in the room, the player's speed, the
        /// list of sprites, the bullets, the flag for existence, the flags for porting and touching a 
        /// portal, and the rate of fire counter.
        /// </summary>
        /// <param name="argX">The beginning room position of the player.</param>
        /// <param name="argY">The beginning room position of the player.</param>
        /// <param name="argSpeed">The beginning speed of the player.</param>
        public Player(int argX, int argY, int argSpeed)
        {
            roomX = argX;
            roomY = argY;
            speed = argSpeed;
            blueBullet = new Bullet();
            orangeBullet = new Bullet();
            exists = true;
            ported = false;
            porting = false;
            dead = false;
            rateOfFire = -1;
        }

        /// <summary>
        /// Updates the logic for the player. Determines collisions, movement, and portal logic. 
        /// This is by far the largest update method in the game. As such it has been broken down and 
        /// summarized as well.
        /// </summary>
        /// <param name="newKeyboardState">The current KeyboardState.</param>
        /// <param name="oldKeyboardState">The previous KeyboardState.</param>
        /// <param name="newMouseState">The current MouseState.</param>
        /// <param name="oldMouseState">The previous MouseState.</param>
        /// <param name="boxList">The list of boxes in the current room.</param>
        /// <param name="bluePortal">The blue portal in the room.</param>
        /// <param name="orangePortal">The orange portal in the room.</param>
        public override void Update(KeyboardState newKeyboardState, KeyboardState oldKeyboardState, 
            MouseState newMouseState, MouseState oldMouseState, List<Box> boxList, List<LaserShooter> laserList, 
            ref Portal bluePortal, ref Portal orangePortal)
        {
            //  Summary:
            //      These four bools determine the directions possible.
            bool up = true,
                 down = true,
                 left = true,
                 right = true;
            //  Summary:
            //      The programming behind collision testing is to test for a collision
            //      after each individual X and Y change.
            for (int count = 0; count < speed; count++)
            {
                for (int i = 0; i < boxList.Count; i++)
                {
                    //  Summary:
                    //      The collision testing for detecting where the collision
                    //      happened is split into vertical and horizontal tests.
                    if (!boxList[i].empty)
                    {
                        // The direction is represented in radians.
                        double tempDouble = Collision.TestVertical(this, boxList[i]);
                        if (tempDouble == 0)
                        { up = false; }
                        if (tempDouble == Math.PI)
                        { down = false; }
                        tempDouble = Collision.TestHorizontal(this, boxList[i]);
                        if (tempDouble == -Math.PI / 2)
                        { left = false; }
                        if (tempDouble == Math.PI / 2)
                        { right = false; }
                    }
                    //  Summary:
                    //      When a collision with a hole happens, the player dies.
                    else
                    {
                        if (Collision.TestCompletelyInsideForAll(this, boxList))
                        { dead = true; }
                    }
                }

                for (int i = 0; i < laserList.Count && laserList[i].laser != null; i++)
                {
                    for (int j = 0; j < Math.Abs(laserList[i].laser.xReach) / 10; j++)
                    {
                        if (Collision.TestCoordinate(laserList[i].laser.roomX + 10 * j * 
                            Math.Abs(laserList[i].laser.xReach) / laserList[i].laser.xReach,
                            laserList[i].laser.roomY, this))
                        { dead = true; }
                    }
                    for (int j = 0; j < Math.Abs(laserList[i].laser.yReach) / 10; j++)
                    {
                        if (Collision.TestCoordinate(laserList[i].roomX, laserList[i].roomY +
                            10 * j * Math.Abs(laserList[i].laser.yReach) / laserList[i].laser.yReach, this))
                        { dead = true; }
                    }
                    for (int j = 0; j < Math.Abs(laserList[i].laser.xReachPorted) / 10; j++)
                    {
                        if (Collision.TestCoordinate(laserList[i].laser.portedRoomX + 10 * j * 
                            Math.Abs(laserList[i].laser.xReachPorted) / laserList[i].laser.xReachPorted,
                            laserList[i].laser.portedRoomY, this))
                        { dead = true; }
                    }
                    for (int j = 0; j < Math.Abs(laserList[i].laser.yReachPorted) / 10; j++)
                    {
                        if (Collision.TestCoordinate(laserList[i].laser.portedRoomX, laserList[i].laser.portedRoomY
                            + 10 * j * Math.Abs(laserList[i].laser.yReachPorted) / laserList[i].laser.yReachPorted, 
                            this))
                        { dead = true; }
                    }
                }

                // Temporary bools to detect collisions with portals.
                bool bluePortalporting = false;
                bool orangePortalporting = false;

                // Both portals must exist for there to be a collision.
                if (bluePortal.exists && orangePortal.exists)
                { bluePortal.Porting(this, ref roomX, ref roomY, ref bluePortalporting, ref up, ref down, ref left, 
                    ref right); }
                if (bluePortal.exists && orangePortal.exists)
                { orangePortal.Porting(this, ref roomX, ref roomY, ref orangePortalporting, ref up, ref down, ref left, 
                    ref right); }
                
                // The temporary bools are set up to tell which portal the player collided with.
                if (bluePortalporting || orangePortalporting)
                { ported = true; }
                else
                { ported = false; }

                //  Summary:
                //      The otherPortal is used to abstractly refer to the portal not currently being collided with.
                Portal otherPortal = new Portal();
                if (bluePortalporting)
                { otherPortal = orangePortal; }
                else
                { otherPortal = bluePortal; }

                //  Summary:
                //      Gets the position of the player at the other portal.
                //      Remember: ported is for collisions, porting is for moving through portals.
                if (ported && !porting)
                {
                    if (otherPortal.portalDirection == 0)
                    {
                        portedRoomX = otherPortal.roomX + otherPortal.sprite.Bounds.Center.X - sprite.Bounds.Center.X;
                        portedRoomY = otherPortal.roomY - sprite.Height + otherPortal.sprite.Height;
                    }
                    if (otherPortal.portalDirection == Math.PI)
                    {
                        portedRoomX = otherPortal.roomX + otherPortal.sprite.Bounds.Center.X - sprite.Bounds.Center.X;
                        portedRoomY = otherPortal.roomY;
                    }
                    if (otherPortal.portalDirection == Math.PI / 2)
                    {
                        portedRoomX = otherPortal.roomX - sprite.Width + otherPortal.sprite.Width;
                        portedRoomY = otherPortal.roomY + otherPortal.sprite.Bounds.Center.Y - sprite.Bounds.Center.Y;
                    }
                    if (otherPortal.portalDirection == -Math.PI / 2)
                    {
                        portedRoomX = otherPortal.roomX;
                        portedRoomY = otherPortal.roomY + otherPortal.sprite.Bounds.Center.Y - sprite.Bounds.Center.Y;
                    }
                }

                // When the spacebar is pressed, the player will be moving through portals.
                if (ported && !porting && newKeyboardState.IsKeyDown(Keys.Space) && !oldKeyboardState.IsKeyDown(Keys.Space))
                { porting = true; }

                // If there is no collision with a portal, then the player is not moving through a portal.
                if (!ported)
                { porting = false; }

                //  Summary:
                //      Sets up the player's direction while moving through the portal, and
                //      moves it through the portal far enough to no longer be colliding with
                //      the other portal.

                if (porting)
                {
                    if (otherPortal.portalDirection == 0)
                    {
                        portedRoomY += 1;
                        direction = otherPortal.portalDirection + Math.PI;
                        if (portedRoomY > otherPortal.roomY + otherPortal.sprite.Height)
                        {
                            roomX = portedRoomX;
                            roomY = portedRoomY;
                        }
                    }
                    if (otherPortal.portalDirection == Math.PI)
                    {
                        portedRoomY -= 1;
                        direction = otherPortal.portalDirection + Math.PI;
                        if (portedRoomY + sprite.Height < otherPortal.roomY)
                        {
                            roomX = portedRoomX;
                            roomY = portedRoomY;
                        }
                    }
                    if (otherPortal.portalDirection == Math.PI / 2)
                    {
                        portedRoomX += 1;
                        direction = otherPortal.portalDirection;
                        if (portedRoomX > otherPortal.roomX + otherPortal.sprite.Width + 1)
                        {
                            roomX = portedRoomX;
                            roomY = portedRoomY;
                        }
                    }
                    if (otherPortal.portalDirection == -Math.PI / 2)
                    {
                        portedRoomX -= 1;
                        direction = otherPortal.portalDirection;
                        if (portedRoomX + sprite.Width < otherPortal.roomX - 1)
                        {
                            roomX = portedRoomX;
                            roomY = portedRoomY;
                        }
                    }
                }
                // If the player is not moving through a portal.
                else
                {
                    //  Summary:
                    //      The key being pressed and corresponding directional bool must both be true.
                    if (newKeyboardState.IsKeyDown(Keys.W) && up)
                    { roomY -= 1; }
                    if (newKeyboardState.IsKeyDown(Keys.S) && down)
                    { roomY += 1; }
                    if (newKeyboardState.IsKeyDown(Keys.A) && left)
                    { roomX -= 1; }
                    if (newKeyboardState.IsKeyDown(Keys.D) && right)
                    { roomX += 1; }

                    // Prevents the sprite from moving when opposite directions are pressed.
                    if (newKeyboardState.IsKeyDown(Keys.A) && newKeyboardState.IsKeyDown(Keys.D))
                    {
                        if (left)
                        { roomX += 1; }
                        if (right)
                        { roomX -= 1; }
                    }
                    if (newKeyboardState.IsKeyDown(Keys.W) && newKeyboardState.IsKeyDown(Keys.S))
                    {
                        if (up)
                        { roomY += 1; }
                        if (down)
                        { roomY -= 1; }
                    }

                    // Finds the slope of the line from the screen positions of the player and the mouse.
                    direction = Math.Atan2(newMouseState.Y - screenY, newMouseState.X - screenX) + Math.PI / 2;
                }
            }

            //  Summary:
            //      Fires bullets if the rate of fire counter has caught up.
            //      The two MouseStates prevent rapid fire from holding down the button.
            if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton != ButtonState.Pressed
                && !blueBullet.exists && !ported && rateOfFire == -1)
            {
                blueBullet.Fired(roomX + sprite.Bounds.Center.X, roomY + sprite.Bounds.Center.Y, direction);
                rateOfFire = 0;
            }
            if (newMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton != ButtonState.Pressed && 
                !orangeBullet.exists && !ported && rateOfFire == -1)
            {
                orangeBullet.Fired(roomX + sprite.Bounds.Center.X, roomY + sprite.Bounds.Center.Y, direction);
                rateOfFire = 0;
            }
            // Adds to the rate of fire after a bullet has been fired.
            if (rateOfFire > -1)
            {
                rateOfFire += 1;
                if (rateOfFire == 20)
                { rateOfFire = -1; }
            }

            // Bullet logic can only happen if the bullets exist.
            if (blueBullet.exists)
            { blueBullet.Update(boxList, ref bluePortal, ref orangePortal); }
            if (orangeBullet.exists)
            { orangeBullet.Update(boxList, ref orangePortal, ref bluePortal); }
        }

        public override void setPortedScreenPosition(int portedOnScreenX, int portedOnScreenY)
        {
            tempPortedScreenX = roomX - portedOnScreenX + 400;
            tempPortedScreenY = roomY - portedOnScreenY;
        }
        public override void DrawPorted(SpriteBatch spriteBatch)
        {
            //TODO
            if (portedScreenX < SplitScreen.X)
            {
                if (portedScreenX + sprite.Width < SplitScreen.X)
                { }
                else
                {
                    spriteBatch.Draw(sprite, new Vector2(SplitScreen.X, portedScreenY),
                                     new Rectangle(SplitScreen.X - portedScreenX, 0,
                                     sprite.Width - (SplitScreen.X - portedScreenX), sprite.Height), Color.White);
                }
            }
            else
            { spriteBatch.Draw(sprite, new Vector2(portedScreenX, portedScreenY), Color.White); }

            spriteBatch.Draw(sprite, new Rectangle(tempPortedScreenX + sprite.Bounds.Center.X,
                    tempPortedScreenY + sprite.Bounds.Center.Y, sprite.Width, sprite.Height),
                null, Color.White, (float)direction, new Vector2(sprite.Bounds.Center.X,
                    sprite.Bounds.Center.Y), new SpriteEffects(), 0);
        }
    }

    /// <summary>
    /// The class of small turrets that will shoot the player.
    /// </summary>
    public class Sentry : Element
    {
        /// <summary>
        /// The direction the sentry is facing.
        /// </summary>
        public double direction;
        /// <summary>
        /// Initializes the sentry. Initializes the sentry's position, direction, and sprites.
        /// </summary>
        /// <param name="argX">The room position of the sentry.</param>
        /// <param name="argY">The room position of the sentry.</param>
        /// <param name="facing">The direction that the sentry will face.</param>
        public Sentry(int argX, int argY, double facing)
        {
            roomX = argX;
            roomY = argY;
            direction = facing;
            mouseOver = false;
            sprites = new Dictionary<string,Texture2D>();
        }
        /// <summary>
        /// Updates the logic for the sentry.
        /// </summary>
        /// <param name="player">The player in the current room.</param>
        /// <param name="boxList">The list of boxes in the current room.</param>
        public void Update(Player player, List<Box> boxList)
        {

        }
    }

    /// <summary>
    /// The class for lasers.
    /// </summary>
    public class Laser : Element
    {
        /// <summary>
        /// The direction the laser is moving towards.
        /// </summary>
        public double direction;
        public double portedDirection;
        public int portedRoomX, portedRoomY;
        public int otherScreenX, otherScreenY;
        public int xReach, yReach;
        public int xReachPorted, yReachPorted;
        public bool ported;
        public Texture2D portedSprite;
        /// <summary>
        /// The two images of the laser.
        /// </summary>
        private List<Texture2D> textures;
        /// <summary>
        /// Initializes the laser. Initializes the laser's direction.
        /// </summary>
        /// <param name="laserShooter">The laserShooter that shot this laser.</param>
        /// <param name="argTextures">The two images of the laser.</param>
        public Laser(LaserShooter laserShooter, List<Texture2D> argTextures)
        {
            exists = true;
            direction = laserShooter.direction;

            textures = argTextures;
            xReachPorted = 0;
            yReachPorted = 0;
            ported = false;

            if (direction == 0)
            {
                sprite = textures[0];
                roomX = laserShooter.roomX + laserShooter.sprite.Bounds.Center.X - sprite.Bounds.Center.X;
                roomY = laserShooter.roomY - 2; //2 to get it in place.
            }

            if (direction == Math.PI)
            {
                sprite = textures[0];
                roomX = laserShooter.roomX + laserShooter.sprite.Bounds.Center.X - sprite.Bounds.Center.X;
                roomY = laserShooter.roomY + 12; //12 to get it in place.
            }

            if (direction == Math.PI / 2)
            {
                sprite = textures[1];
                roomX = laserShooter.roomX + 12; //12 to get it in place.
                roomY = laserShooter.roomY + laserShooter.sprite.Bounds.Center.Y - sprite.Bounds.Center.Y;
            }

            if (direction == -Math.PI / 2)
            {
                sprite = textures[1];
                roomX = laserShooter.roomX + 8; //8 to get it in place.
                roomY = laserShooter.roomY + laserShooter.sprite.Bounds.Center.Y - sprite.Bounds.Center.Y;
            }

            xReach = sprite.Width * (int)Math.Sin(direction);
            yReach = sprite.Height * (int)Math.Cos(direction);
        }

        public void Update(List<Box> boxList, Portal bluePortal, Portal orangePortal)
        {
            bool collided = false;
            for (int i = 0; i < boxList.Count; i++)
            {
                if (!boxList[i].empty)
                if (Collision.TestCoordinate(roomX + xReach, roomY + yReach, boxList[i]))
                { collided = true; }
            }

            if (!collided)
            {
                xReach += sprite.Width * (int)Math.Sin(direction);
                yReach -= sprite.Height * (int)Math.Cos(direction);
            }

            Portal tempPortal = null;
            if (bluePortal.exists && Collision.TestCoordinate(roomX + xReach, roomY + yReach, bluePortal))
            { tempPortal = orangePortal; }
            else if (orangePortal.exists && Collision.TestCoordinate(roomX + xReach, roomY + yReach, orangePortal))
            { tempPortal = bluePortal; }
            else
            {
                ported = false;
                xReachPorted = 0;
                yReachPorted = 0;
            }

            if (tempPortal != null)
            {
                ported = true;
                portedDirection = tempPortal.portalDirection;

                if (portedDirection == 0 || portedDirection == Math.PI)
                {
                    portedSprite = textures[0];

                    if (portedDirection == 0)
                    {
                        if (yReachPorted <= 0 || !Collision.TestCoordinate(portedRoomX, portedRoomY, tempPortal))
                        { yReachPorted = portedSprite.Height * (int)Math.Cos(portedDirection); }
                        portedRoomY = tempPortal.roomY + tempPortal.sprite.Bounds.Center.Y;
                    }
                    if (portedDirection == Math.PI)
                    {
                        if (yReachPorted > 0 || !Collision.TestCoordinate(portedRoomX, portedRoomY + 
                            portedSprite.Height, tempPortal))
                        { yReachPorted = portedSprite.Height * (int)Math.Cos(portedDirection); }
                        portedRoomY = tempPortal.roomY + tempPortal.sprite.Bounds.Center.Y - portedSprite.Height;
                    }

                    portedRoomX = tempPortal.roomX + tempPortal.sprite.Bounds.Center.X - portedSprite.Bounds.Center.X;
                    if (xReachPorted != 0)
                    { xReachPorted = 0; }
                }
                if (portedDirection == Math.PI / 2 || portedDirection == -Math.PI / 2)
                {
                    portedSprite = textures[1];

                    if (portedDirection == Math.PI / 2)
                    {
                        if (xReachPorted < 0 || !Collision.TestCoordinate(portedRoomX, portedRoomY, tempPortal))
                        { xReachPorted = 0; }
                        portedRoomX = tempPortal.roomX + tempPortal.sprite.Bounds.Center.X + 1;
                    }
                    if (portedDirection == -Math.PI / 2)
                    {
                        if (xReachPorted > 0 || !Collision.TestCoordinate(portedRoomX + portedSprite.Width, 
                            portedRoomY, tempPortal))
                        { xReachPorted = 0; }
                        portedRoomX = tempPortal.roomX + tempPortal.sprite.Bounds.Center.X - portedSprite.Width;
                    }

                    portedRoomY = tempPortal.roomY + tempPortal.sprite.Bounds.Center.Y - portedSprite.Bounds.Center.Y;
                    if (yReachPorted != 0)
                    { yReachPorted = 0; }
                }

                bool portCollided = false;
                for (int i = 0; i < boxList.Count; i++)
                {
                    if (!boxList[i].empty)
                        if (Collision.TestCoordinate(portedRoomX + xReachPorted, portedRoomY + yReachPorted, boxList[i]))
                        { portCollided = true; }
                }

                if (!portCollided)
                {
                    try
                    {
                        xReachPorted += portedSprite.Width * (int)Math.Sin(portedDirection);
                        yReachPorted += portedSprite.Height * (int)Math.Cos(portedDirection);
                    }
                    catch (Exception NullReferenceException)
                    { }
                }
            }
        }

        public override void setScreenPosition(int onScreenX, int onScreenY)
        {
            base.setScreenPosition(onScreenX, onScreenY);

            if (ported)
            {
                otherScreenX = portedRoomX - onScreenX;
                otherScreenY = portedRoomY - onScreenY;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Vector2(screenX, screenY), Color.White);

            for (int i = 1; i < Math.Abs(xReach / sprite.Width) + 1; i++)
            {
                spriteBatch.Draw(sprite, new Vector2(screenX + i * sprite.Width * Math.Abs(xReach) / xReach, screenY), 
                Color.White);
            }

            for (int i = 1; i < Math.Abs(yReach / sprite.Height) + 1; i++)
            {
                spriteBatch.Draw(sprite, new Vector2(screenX, screenY + i * sprite.Height * Math.Abs(yReach) / yReach),
                  Color.White);
            }

            if (ported)
            {
                try
                {
                    spriteBatch.Draw(portedSprite, new Vector2(otherScreenX, otherScreenY), Color.White);

                    for (int i = 1; i < Math.Abs(xReachPorted / portedSprite.Width) + 1; i++)
                    {
                        spriteBatch.Draw(portedSprite, new Vector2(otherScreenX + i * portedSprite.Width *
                            Math.Abs(xReachPorted) / xReachPorted, otherScreenY), Color.White);
                    }

                    for (int i = 1; i < Math.Abs(yReachPorted / portedSprite.Height) + 1; i++)
                    {
                        spriteBatch.Draw(portedSprite, new Vector2(otherScreenX, otherScreenY + i *
                            portedSprite.Height * Math.Abs(yReachPorted) / yReachPorted), Color.White);
                    }
                }
                catch (Exception NullReferenceException)
                { }
            }
        }
    }

    /// <summary>
    /// The class for laser shooters.
    /// </summary>
    public class LaserShooter : Element
    {
        /// <summary>
        /// The direction the laser shooter is facing.
        /// </summary>
        public double direction;
        public Laser laser;
        public List<Texture2D> laserSprites;
        /// <summary>
        /// Initializies the laser shooter. Initializes the laser shooter's position, direction, and sprite.
        /// </summary>
        /// <param name="argX">The position of the laser shooter in the room.</param>
        /// <param name="argY">The position of the laser shooter in the room.</param>
        /// <param name="facing">The direction the laser shooter will face.</param>
        /// <param name="laserTextures">The two sprites for a laser.</param>
        public LaserShooter(int argX, int argY, double facing, List<Texture2D> laserTextures)
        {
            direction = facing;
            exists = true;
            
            laserSprites = laserTextures;

            if (facing == 0)
            {
                roomX = argX;
                roomY = argY - 20;
            }

            if (facing == Math.PI)
            {
                roomX = argX;
                roomY = argY + 100;
            }

            if (facing == Math.PI / 2)
            {
                roomX = argX - 2;
                roomY = argY;
            }

            if (facing == -Math.PI / 2)
            {
                roomX = argX - 20;
                roomY = argY;
            }
        }

        public void Update(List<Box> boxList, Portal bluePortal, Portal orangePortal)
        {
            if (laser == null)
            { laser = new Laser(this, laserSprites); }
        }
    }

    /// <summary>
    /// The class for laser catchers.
    /// </summary>
    public class LaserCatcher : Element
    {
        /// <summary>
        /// The direction the laserCatcher is facing.
        /// </summary>
        public double direction;
        /// <summary>
        /// Initializies the laser shooter. Initializes the laser shooter's position, direction, and sprite.
        /// </summary>
        /// <param name="argX">The position of the laser shooter in the room.</param>
        /// <param name="argY">The position of the laser shooter in the room.</param>
        /// <param name="facing">The direction the laser shooter will face.</param>
        public LaserCatcher(int argX, int argY, double facing)
        {
            exists = true;
            if (facing == 0)
            {
                roomX = argX;
                roomY = argY + sprite.Bounds.Height;
            }

            if (facing == Math.PI)
            {
                roomX = argX;
                roomY = argY + 100;
            }

            if (facing == Math.PI / 2)
            {
                roomX = argX + 100;
                roomY = argY;
            }

            if (facing == -Math.PI / 2)
            {
                roomX = argX - sprite.Bounds.Width;
                roomY = argY;
            }
        }
    }

    /// <summary>
    /// The class for the bullets that the player shoots. These create portals.
    /// </summary>
    public class Bullet : Element
    {
        /// <summary>
        /// The direction the bullet is travelling, in radians. This is also the direction 
        /// that the player was facing when the bullet was fired.
        /// </summary>
        private double direction;
        /// <summary>
        /// The position of the bullet as a decimal. This information is required due to the 
        /// truncation caused by using integers.
        /// </summary>
        private double doubleX, doubleY;
        /// <summary>
        /// Initializes the bullet. Initializes the bullet's position and its existence.
        /// </summary>
        public Bullet()
        {
            roomX = 0;
            roomY = 0;
            exists = false;
        }
        /// <summary>
        /// Updates the logic for the bullet. Uses similar logic for collisions as the Player class. 
        /// Checks for collisions with boxes and portals. Makes a portal if the bullet collides with 
        /// a box that can hold a portal.
        /// </summary>
        /// <param name="boxList">The list of boxes in the current room.</param>
        /// <param name="portal">The portal that the bullet will create if it collides with a box that 
        /// can hold a portal.</param>
        /// <param name="otherPortal">The portal that the bullet cannot create.</param>
        public void Update(List<Box> boxList, ref Portal portal, ref Portal otherPortal)
        {
            doubleX = roomX;
            doubleY = roomY;

            double XSpeed = 10 * Math.Cos(direction - Math.PI / 2);
            double YSpeed = 10 * Math.Sin(direction - Math.PI / 2);

            for (int count = 0; count < 10; count++)
            {
                for (int i = 0; i < boxList.Count && exists; i++)
                {
                    if (otherPortal.exists && Collision.Test(this, otherPortal))
                    { exists = false; }

                    if (Collision.Test(this, boxList[i]) && exists)
                    {
                        if (boxList[i].portalUse)
                        { portalMade(boxList[i], ref portal, ref exists); }
                        else if (!boxList[i].empty)
                        { exists = false; }
                    }
                }

                doubleX += XSpeed / 10.0;
                doubleY += YSpeed / 10.0;

                roomX = (int)doubleX;
                roomY = (int)doubleY;
            }
        }

        /// <summary>
        /// Finds the part of the box the bullet collided with.
        /// </summary>
        /// <param name="portalBox">The box the bullet collided with. This box must be able to hold 
        /// portals.</param>
        /// <param name="portal">The portal that will be made.</param>
        /// <param name="bulletExists">The bool that says whether the bullet exists.</param>
        private void portalMade(Box portalBox, ref Portal portal, ref bool bulletExists)
        {
            double tempDouble = Collision.TestVertical(this, portalBox);
            if (tempDouble == 10)
            { tempDouble = Collision.TestHorizontal(this, portalBox); }
            portal.Created(portalBox, tempDouble, ref bulletExists);
        }

        /// <summary>
        /// Instantiates the bullet's position and direction when fired, and makes the bullet exist.
        /// </summary>
        /// <param name="positionX">The x-position the bullet is fired from.</param>
        /// <param name="positionY">The y-position the bullet is fired from.</param>
        /// <param name="argDirection">The direction the bullet will travel, in radians.</param>
        public void Fired(int positionX, int positionY, double argDirection)
        {
            roomX = positionX;
            roomY = positionY;
            direction = argDirection;
            exists = true;
        }
    }

    /// <summary>
    /// The class for portals. These will transport the player to a different location.
    /// </summary>
    public class Portal : Element
    {
        /// <summary>
        /// The direction the portal is facing, in radians. The number 10 was arbitrarily chosen as 
        /// a null value.
        /// </summary>
        public double portalDirection;
        /// <summary>
        /// Instantiates the portal. Instantiates the portals non-existence, its list of sprites, and 
        /// its direction as null (10).
        /// </summary>
        public Portal()
        {
            exists = false;
            sprites = new Dictionary<string,Texture2D>();
            portalDirection = 10;
        }

        /// <summary>
        /// Instantiates the portal when it is created by a bullet. Instantiates the portal's position, 
        /// using the box's position and the facing of the portal, the portal's sprite, and then 
        /// destroys the bullet.
        /// </summary>
        /// <param name="portalBox">The box that the bullet hit. Must be able to hold portals.</param>
        /// <param name="facing">The direction that the portal will be facing, in radians.</param>
        /// <param name="bulletExists">The flag for the existence of the bullet.</param>
        public void Created(Box portalBox, double facing, ref bool bulletExists)
        {
            if (facing == 0)
            {
                sprite = sprites["UP"];
                roomX = portalBox.roomX;
                roomY = portalBox.roomY + portalBox.sprite.Height - 5;
                portalDirection = facing;
            }
            if (facing == Math.PI)
            {
                sprite = sprites["DOWN"];
                roomX = portalBox.roomX;
                roomY = portalBox.roomY - 5;
                portalDirection = facing;
            }
            if (facing == -Math.PI / 2)
            {
                sprite = sprites["LEFT"];
                roomX = portalBox.roomX + portalBox.sprite.Width - 5;
                roomY = portalBox.roomY;
                portalDirection = -facing;
            }
            if (facing == Math.PI / 2)
            {
                sprite = sprites["RIGHT"];
                roomX = portalBox.roomX - 5;
                roomY = portalBox.roomY;
                portalDirection = -facing;
            }

            exists = true;
            bulletExists = false;

            if (facing == 10)
            {
                exists = false;
                bulletExists = true;
            }
        }

        /// <summary>
        /// Checks the collision between the player and the portal, and keeps the player from 
        /// moving through the rest of the box.
        /// </summary>
        /// <param name="player">The Player in the game.</param>
        /// <param name="playerRoomX">The x-position of the player in the room.</param>
        /// <param name="playerRoomY">The y-position of the player in the room.</param>
        /// <param name="ported">The flag that says if there is a collision between the portal 
        /// and the player.</param>
        /// <param name="up">The direction that the player can go.</param>
        /// <param name="down">The direction that the player can go.</param>
        /// <param name="left">The direction that the player can go.</param>
        /// <param name="right">The direction that the player can go.</param>
        public void Porting(Player player, ref int playerRoomX, ref int playerRoomY, ref bool ported, ref bool up, 
            ref bool down, ref bool left, ref bool right)
        {
            if (portalDirection == 0)
            {
                if (Collision.TestCoordinate(roomX + sprite.Bounds.Center.X, roomY + sprite.Height, player))
                {
                    ported = true;
                    playerRoomX = roomX + sprite.Bounds.Center.X - player.sprite.Bounds.Center.X;
                    up = true;
                    if (roomY + sprite.Height == playerRoomY + player.sprite.Width)
                    { up = false; }

                    right = false;
                    left = false;
                }
                else
                { ported = false; }
            }

            if (portalDirection == Math.PI)
            {
                if (Collision.TestCoordinate(roomX + sprite.Bounds.Center.X, roomY, player))
                {
                    ported = true;
                    playerRoomX = roomX + sprite.Bounds.Center.X - player.sprite.Bounds.Center.X;
                    down = true;
                    if (roomY == playerRoomY)
                    { down = false; }

                    right = false;
                    left = false;
                }
                else
                { ported = false; }
            }

            if (portalDirection == -Math.PI / 2)
            {
                if (Collision.TestCoordinate(roomX, roomY + sprite.Bounds.Center.Y, player))
                {
                    ported = true;
                    playerRoomY = roomY + sprite.Bounds.Center.Y - player.sprite.Bounds.Center.Y;
                    right = true;
                    if (roomX == playerRoomX)
                    { right = false; }

                    up = false;
                    down = false;
                }
                else
                { ported = false; }
            }

            if (portalDirection == Math.PI / 2)
            {
                if (Collision.TestCoordinate(roomX + sprite.Width, roomY + sprite.Bounds.Center.Y, player))
                {
                    ported = true;
                    playerRoomY = roomY + sprite.Bounds.Center.Y - player.sprite.Bounds.Center.Y;
                    left = true;
                    if (roomX + sprite.Width == playerRoomX + player.sprite.Width)
                    { left = false; }

                    up = false;
                    down = false;
                }
                else
                { ported = false; }
            }
        }

        public void DrawBottom(SpriteBatch spriteBatch, bool ported)
        {
            if (exists)
            {
                Texture2D tempSprite = sprite;

                if (portalDirection == 0)
                { tempSprite = sprites["DOWN"]; }
                if (portalDirection == Math.PI)
                { tempSprite = sprites["UP"]; }
                if (portalDirection == Math.PI / 2)
                { tempSprite = sprites["RIGHT"]; }
                if (portalDirection == -Math.PI / 2)
                { tempSprite = sprites["LEFT"]; }

                if (!ported)
                { spriteBatch.Draw(tempSprite, new Vector2(screenX, screenY), Color.White); }
                else
                {
                    if (portedScreenX < SplitScreen.X)
                    {
                        if (portedScreenX + sprite.Width < SplitScreen.X)
                        { }
                        else
                        {
                            spriteBatch.Draw(tempSprite, new Vector2(SplitScreen.X, portedScreenY),
                                             new Rectangle(SplitScreen.X - portedScreenX, 0,
                                             tempSprite.Width - (SplitScreen.X - portedScreenX), tempSprite.Height), 
                                             Color.White);
                        }
                    }
                    else
                    { spriteBatch.Draw(tempSprite, new Vector2(portedScreenX, portedScreenY), Color.White); }
                }
            }
        }
    }

    /// <summary>
    /// The class that tests collisions.
    /// </summary>
    public static class Collision
    {
        /// <summary>
        /// Tests the corners of the first element insideElement the corners of the second element,
        /// then switches the elements and does the same thing.
        /// </summary>
        /// <param name="element1">An element that is or could be colliding.</param>
        /// <param name="element2">An element that is or could be colliding.</param>
        /// <returns></returns>
        public static bool Test(Element element1, Element element2)
        {
            if (element1.roomX >= element2.roomX && element1.roomX <= element2.roomX + element2.sprite.Width)
            {
                if (element1.roomY >= element2.roomY && element1.roomY <= element2.roomY + element2.sprite.Height)
                { return true; }
                if (element1.roomY + element1.sprite.Height >= element2.roomY && element1.roomY + element1.sprite.Height <= 
                    element2.roomY + element2.sprite.Height)
                { return true; }
            }
            if (element1.roomX + element1.sprite.Width >= element2.roomX && element1.roomX + element1.sprite.Width <= 
                element2.roomX + element2.sprite.Width)
            {
                if (element1.roomY >= element2.roomY && element1.roomY <= element2.roomY + element2.sprite.Height)
                { return true; }
                if (element1.roomY + element1.sprite.Height >= element2.roomY && element1.roomY + element1.sprite.Height <= 
                    element2.roomY + element2.sprite.Height)
                { return true; }
            }

            Element tempElement = element1;
            element1 = element2;
            element2 = tempElement;

            if (element1.roomX >= element2.roomX && element1.roomX <= element2.roomX + element2.sprite.Width)
            {
                if (element1.roomY >= element2.roomY && element1.roomY <= element2.roomY + element2.sprite.Height)
                { return true; }
                if (element1.roomY + element1.sprite.Height >= element2.roomY && element1.roomY + element1.sprite.Height <= 
                    element2.roomY + element2.sprite.Height)
                { return true; }
            }
            if (element1.roomX + element1.sprite.Width >= element2.roomX && element1.roomX + element1.sprite.Width <= 
                element2.roomX + element2.sprite.Width)
            {
                if (element1.roomY >= element2.roomY && element1.roomY <= element2.roomY + element2.sprite.Height)
                { return true; }
                if (element1.roomY + element1.sprite.Height >= element2.roomY && element1.roomY + element1.sprite.Height <= 
                    element2.roomY + element2.sprite.Height)
                { return true; }
            }

            return false;
        }
        /// <summary>
        /// Tests if a coordinate is inside the corners of an element.
        /// </summary>
        /// <param name="argX">The x-coordinate that will be tested.</param>
        /// <param name="argY">The y-coordinate that will be tested.</param>
        /// <param name="element">The element that will be tested.</param>
        /// <returns></returns>
        public static bool TestCoordinate(int argX, int argY, Element element)
        {
            if (argX >= element.roomX && argX <= element.roomX + element.sprite.Width)
            {
                if (argY >= element.roomY && argY <= element.roomY + element.sprite.Height)
                { return true; }
            }
            return false;
        }
        /// <summary>
        /// Tests for a collision on the top and bottom of element1 with element2. The double 
        /// returned is in radians.
        /// </summary>
        /// <param name="element1">The element that wil have its top and bottom tested.</param>
        /// <param name="element2">The element that will be tested.</param>
        /// <returns></returns>
        public static double TestVertical(Element element1, Element element2)
        {
            if ((element1.roomX >= element2.roomX && element1.roomX <= element2.roomX + element2.sprite.Width - 1) || 
                (element1.roomX + element1.sprite.Width >= element2.roomX + 1 && element1.roomX + element1.sprite.Width <= 
                element2.roomX + element2.sprite.Width))
            {
                if (element1.roomY <= element2.roomY + element2.sprite.Height && element1.roomY >= element2.roomY)
                { return 0; }
                if (element1.roomY + element1.sprite.Height >= element2.roomY && element1.roomY + element1.sprite.Height <= 
                    element2.roomY + element2.sprite.Height)
                { return Math.PI; }
            }
            return 10;
        }
        /// <summary>
        /// Tests for a collision on the left and right of element1 with element2. The 
        /// double returned is in radians.
        /// </summary>
        /// <param name="element1">The element that wil have its left and right tested.</param>
        /// <param name="element2">The element that will be tested.</param>
        /// <returns></returns>
        public static double TestHorizontal(Element element1, Element element2)
        {
            if ((element1.roomY >= element2.roomY && element1.roomY <= element2.roomY + element2.sprite.Height - 1) || 
                (element1.roomY + element1.sprite.Height >= element2.roomY + 1 && element1.roomY + element1.sprite.Height <= 
                element2.roomY + element2.sprite.Height))
            {
                if (element1.roomX <= element2.roomX + element2.sprite.Width && element1.roomX >= element2.roomX)
                { return -Math.PI / 2; }
                if (element1.roomX + element1.sprite.Width >= element2.roomX && element1.roomX + element1.sprite.Width <= 
                    element2.roomX)
                { return Math.PI / 2; }
            }
            return 10;
        }
        /// <summary>
        /// Tests the corners of the inside element inside the surrounding element. It will 
        /// only return true if all the corners are inside the surrounding element.
        /// </summary>
        /// <param name="insideElement">The element that will be tested if inside the 
        /// surrounding element.</param>
        /// <param name="surroundingElement">The element that will be tested if surrounding 
        /// the inside element.</param>
        /// <returns></returns>
        public static bool TestCompletelyInside(Element insideElement, Element surroundingElement)
        {
            if (insideElement.roomX >= surroundingElement.roomX && insideElement.roomX + insideElement.sprite.Width <= 
                surroundingElement.roomX + surroundingElement.sprite.Width)
            {
                if (insideElement.roomY >= surroundingElement.roomY && insideElement.roomY + insideElement.sprite.Height <= 
                    surroundingElement.roomY + surroundingElement.sprite.Height)
                { return true; }
            }
            return false;
        }

        public static bool TestCompletelyInsideForAll(Element insideElement, List<Box> surroundingElements)
        {
            bool topLeft, topRight, bottomLeft, bottomRight;
            topLeft = topRight = bottomLeft = bottomRight = false;

            for (int i = 0; i < surroundingElements.Count; i++)
            {
                if (!topLeft)
                {
                    if (TestCoordinate(insideElement.roomX, insideElement.roomY, surroundingElements[i]))
                    { topLeft = true; }
                }
                if (!topRight)
                {
                    if (TestCoordinate(insideElement.roomX + insideElement.sprite.Width, insideElement.roomY, 
                        surroundingElements[i]))
                    { topRight = true; }
                }
                if (!bottomLeft)
                {
                    if (TestCoordinate(insideElement.roomX, insideElement.roomY + insideElement.sprite.Height, 
                        surroundingElements[i]))
                    { bottomLeft = true; }
                }
                if (!bottomRight)
                {
                    if (TestCoordinate(insideElement.roomX + insideElement.sprite.Width, insideElement.roomY + 
                        insideElement.sprite.Height, surroundingElements[i]))
                    { bottomRight = true; }
                }

                if (topLeft && topRight && bottomLeft && bottomRight)
                { return true; }
            }

            return false;
        }
    }
}