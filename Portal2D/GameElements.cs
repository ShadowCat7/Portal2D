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
    //  Summary:
    //      This class holds the size of the screen.
    public static class Screen
    { static public int X = 800, Y = 480; }
    //  Summary:
    //      This class holds the size of the screen when it is split in half x-wise.
    public static class SplitScreen
    { static public int X = 400, Y = 480; }

    // Summary:
    //      The parent for all rooms.
    public abstract class Room
    {
        //  Summary:
        //      The image for the background of the room.
        public Texture2D background;
        //  Summary:
        //      The size of the room.
        public int sizeX, sizeY;
        //  Summary:
        //      The variable size of the screen. The X value will change when the screen is split.
        public int screenX, screenY;
        //  Summary:
        //      The top left position of the view that fits on the screen.
        public int onScreenX, onScreenY;
        //  Summary:
        //      The top left position of the viewthat fits on the right side of the split screen.
        public int portedOnScreenX, portedOnScreenY;
        //  Summary:
        //      The list of boxes in the room.
        public List<Box> boxList;
        //  Summary:
        //      The user's character in the room.
        public Player player;
        //  Summary:
        //      The room's portals. There are two: the blue and orange portals.
        public Portal bluePortal, orangePortal;
        //  Summary:
        //      The image that splits the screen when teleporting.
        public Texture2D screenSplitter;

        //  Summary:
        //      Initializes the room. Only provided as the default constructor for the Room's children.
        public Room() { }
        //  Summary:
        //      Draws the background of the room.
        //  
        //  Parameters:
        //      spriteBatch:
        //          The SpriteBatch for the game.
        public void DrawBackground(SpriteBatch spriteBatch)
        { spriteBatch.Draw(background, new Vector2(0, 0), Color.White); }

        //Virtual methods
        public virtual void Update(KeyboardState newKeyboardState, KeyboardState oldKeyboardstate, MouseState newMouseState, 
            MouseState oldMouseState) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        //Virtual methods for GameRoom
        public virtual void setBoxSprites(Texture2D normalWallSprite, Texture2D portalWallSprite, Texture2D emptyFloorSprite) 
        { }
        public virtual void drawBoxes(SpriteBatch spriteBatch) { }
        public virtual void AddBoxesAround() { }
    }

    //  Summary:
    //      The parent class for rooms that will have game play.
    public class GameRoom : Room
    {
        //  Summary:
        //      Initializes the GameRoom.
        //      Provides initialization for the room size, the list of boxes, the player,
        //      the boxes surroundingElement the room, the screen size, and the two portals.
        public GameRoom()
        { }

        //  Summary:
        //      Adds boxes to the list of boxes that will surround the room.
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

        //  Summary:
        //      Updates the logic of the GameRoom.
        //
        //  Parameters:
        //
        //      newKeyboardState:
        //          The current KeyboardState.
        //
        //      oldKeyboardState:
        //          The previous KeyboardState.
        //
        //      newMouseState:
        //          The current MouseState.
        //
        //      oldMouseState:
        //          The previous MouseState.
        public override void Update(KeyboardState newKeyboardState, KeyboardState oldKeyboardstate, MouseState newMouseState, 
            MouseState oldMouseState)
        {
            player.Update(newKeyboardState, oldKeyboardstate, newMouseState, oldMouseState, boxList, ref bluePortal, 
                ref orangePortal);

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

            // Puts the boxes on the screen in their screen position with regard to their room position.
            for (int i = 0; i < boxList.Count; i++)
            {
                boxList[i].screenX = boxList[i].roomX - onScreenX;
                boxList[i].screenY = boxList[i].roomY - onScreenY;
            }

            // Puts the bullets on the screen in their screen position with regard to their room position.
            player.blueBullet.screenX = player.blueBullet.roomX - onScreenX;
            player.blueBullet.screenY = player.blueBullet.roomY - onScreenY;
            player.orangeBullet.screenX = player.orangeBullet.roomX - onScreenX;
            player.orangeBullet.screenY = player.orangeBullet.roomY - onScreenY;

            // Puts the portals on the screen in their screen position with regard to their room position.
            if (bluePortal.exists)
            {
                bluePortal.screenX = bluePortal.roomX - onScreenX;
                bluePortal.screenY = bluePortal.roomY - onScreenY;
            }
            if (orangePortal.exists)
            {
                orangePortal.screenX = orangePortal.roomX - onScreenX;
                orangePortal.screenY = orangePortal.roomY - onScreenY;
            }

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

                // For the boxes.
                for (int i = 0; i < boxList.Count; i++)
                {
                    boxList[i].portedScreenX = boxList[i].roomX - portedOnScreenX + 400;
                    boxList[i].portedScreenY = boxList[i].roomY - portedOnScreenY;
                }

                // For the bullets.
                player.blueBullet.portedScreenX = player.blueBullet.roomX - portedOnScreenX + 400;
                player.blueBullet.portedScreenY = player.blueBullet.roomY - portedOnScreenY;
                player.orangeBullet.portedScreenX = player.orangeBullet.roomX - portedOnScreenX + 400;
                player.orangeBullet.portedScreenY = player.orangeBullet.roomY - portedOnScreenY;

                // For the portals.
                if (bluePortal.exists)
                {
                    bluePortal.portedScreenX = bluePortal.roomX - portedOnScreenX + 400;
                    bluePortal.portedScreenY = bluePortal.roomY - portedOnScreenY;
                }
                if (orangePortal.exists)
                {
                    orangePortal.portedScreenX = orangePortal.roomX - portedOnScreenX + 400;
                    orangePortal.portedScreenY = orangePortal.roomY - portedOnScreenY;
                }
            }
        }

        //  Summary:
        //      Draws the images in the room.
        //      Draws images in this order:
        //      Background, empty boxes, player, blue bullet, orange bullet, the rest of the boxes,
        //      blue portal, and the orange portal. Then it does the same for the other viewport
        //      if it exists.
        //
        //  Parameters:
        //
        //      spriteBatch:
        //          The SpriteBatch for the game.
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(0, 0), new Rectangle(onScreenX, onScreenY, Screen.X, Screen.Y), 
                Color.White);
            for (int i = 0; i < boxList.Count; i++)
            {
                if (boxList[i].empty)
                { spriteBatch.Draw(boxList[i].sprite, new Vector2(boxList[i].screenX, boxList[i].screenY), Color.White); }
            }
            spriteBatch.Draw(player.sprite, new Rectangle(player.screenX + player.sprite.Bounds.Center.X, 
                player.screenY + player.sprite.Bounds.Center.Y, player.sprite.Width, player.sprite.Height),
                null, Color.White, (float)player.direction, new Vector2(player.sprite.Bounds.Center.X, 
                    player.sprite.Bounds.Center.Y), new SpriteEffects(), 0);

            player.blueBullet.Draw(spriteBatch);
            player.orangeBullet.Draw(spriteBatch);

            drawBoxes(spriteBatch);
            bluePortal.Draw(spriteBatch);
            orangePortal.Draw(spriteBatch);

            if (player.ported)
            {
                spriteBatch.Draw(background, new Vector2(400, 0), new Rectangle(portedOnScreenX, portedOnScreenY, 
                    SplitScreen.X, SplitScreen.Y), Color.White);
                spriteBatch.Draw(player.sprite, new Rectangle(player.portedScreenX + player.sprite.Bounds.Center.X, 
                    player.portedScreenY + player.sprite.Bounds.Center.Y, player.sprite.Width, player.sprite.Height),
                null, Color.White, (float)player.direction, new Vector2(player.sprite.Bounds.Center.X, 
                    player.sprite.Bounds.Center.Y), new SpriteEffects(), 0);

                for (int i = 0; i < boxList.Count; i++)
                {
                    if (boxList[i].portedScreenX < SplitScreen.X)
                    {
                        if (boxList[i].portedScreenX + boxList[i].sprite.Width < SplitScreen.X)
                        { }
                        else
                        { spriteBatch.Draw(boxList[i].sprite, new Vector2(SplitScreen.X, boxList[i].portedScreenY), 
                            new Rectangle(SplitScreen.X - boxList[i].portedScreenX, 0, 
                                boxList[i].sprite.Width - (SplitScreen.X - boxList[i].portedScreenX), 
                                boxList[i].sprite.Height), Color.White); }
                    }
                    else
                    { spriteBatch.Draw(boxList[i].sprite, new Vector2(boxList[i].portedScreenX, boxList[i].portedScreenY), 
                        Color.White); }
                }

                if (player.blueBullet.exists)
                { spriteBatch.Draw(player.blueBullet.sprite, new Vector2(player.blueBullet.portedScreenX, 
                    player.blueBullet.portedScreenY), Color.White); }
                if (player.orangeBullet.exists)
                { spriteBatch.Draw(player.orangeBullet.sprite, new Vector2(player.orangeBullet.portedScreenX, 
                    player.orangeBullet.portedScreenY), Color.White); }

                if (bluePortal.portedScreenX < SplitScreen.X)
                {
                    if (bluePortal.portedScreenX + bluePortal.sprite.Width < SplitScreen.X)
                    { }
                    else
                    {
                        spriteBatch.Draw(bluePortal.sprite, new Vector2(SplitScreen.X, bluePortal.portedScreenY),
                                  new Rectangle(SplitScreen.X - bluePortal.portedScreenX, 0,
                                      bluePortal.sprite.Width - (SplitScreen.X - bluePortal.portedScreenX),
                                      bluePortal.sprite.Height), Color.White);
                    }
                }
                else
                {
                    spriteBatch.Draw(bluePortal.sprite, new Vector2(bluePortal.portedScreenX, bluePortal.portedScreenY),
                    Color.White);
                }

                if (orangePortal.portedScreenX < SplitScreen.X)
                {
                    if (orangePortal.portedScreenX + orangePortal.sprite.Width < SplitScreen.X)
                    { }
                    else
                    {
                        spriteBatch.Draw(orangePortal.sprite, new Vector2(SplitScreen.X, orangePortal.portedScreenY),
                                  new Rectangle(SplitScreen.X - orangePortal.portedScreenX, 0,
                                      orangePortal.sprite.Width - (SplitScreen.X - orangePortal.portedScreenX),
                                      orangePortal.sprite.Height), Color.White);
                    }
                }
                else
                {
                    spriteBatch.Draw(orangePortal.sprite, new Vector2(orangePortal.portedScreenX, orangePortal.portedScreenY),
                    Color.White);
                }

                spriteBatch.Draw(screenSplitter, new Vector2(SplitScreen.X - 5, 0), Color.White);
            }
        }

        //  Summary:
        //      Sets the sprites for every box in the list of boxes.
        //
        //  Parameters:
        //
        //      normalWallSprite:
        //          The image of the normal wall.
        //
        //      portalWallSprite:
        //          The image of the wall that can hold portals.
        //
        //      emptyFloorSprite:
        //          The image of the empty pit.
        public override void setBoxSprites(Texture2D normalWallSprite, Texture2D portalWallSprite, 
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

        //  Summary:
        //      Draws all the boxes that are not empty boxes.
        //
        //  Parameters:
        //      
        //      spriteBatch
        //          The SpriteBatch for the game.
        public override void drawBoxes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < boxList.Count; i++)
            {
                if (!boxList[i].empty)
                { spriteBatch.Draw(boxList[i].sprite, new Vector2(boxList[i].screenX, boxList[i].screenY), Color.White); }
            }
        }
    }

    //  Summary:
    //      The parent class for everything in a room.
    public class Element
    {
        //  Summary:
        //      The position the element has on the screen.
        public int screenX, screenY;
        //  Summary:
        //      The position the element has in the room.
        public int roomX, roomY;
        //  Summary:
        //      The position the element has on the right viewport.
        public int portedScreenX, portedScreenY;
        //  Summary:
        //      The flag that, if true, says that the mouse is currently hovering over the element.
        public bool mouseOver;
        //  Summary:
        //      The flag that, if true, says that the element currently exists in the game.
        public bool exists;
        //  Summary:
        //      The image of the element.
        public Texture2D sprite;
        //  Summary:
        //      The images that the element can use as its image.
        public List<Texture2D> sprites;
        //  Summary:
        //      Initializes the element. This is only provided as the default constructor
        //      for the element's children.
        public Element() { }
        //  Summary:
        //      Initializes the element.
        //      Provides initialization for the room position, the mouse detection, the list of
        //      sprites, and the flag for being on the screen.
        //
        //  Parameters:
        //
        //      argX, argY:
        //          The integers for the position of the element.
        public Element(int argX, int argY)
        {
            roomX = argX;
            roomY = argY;
            mouseOver = false;
            sprites = new List<Texture2D>(0);
        }
        //  Summary:
        //      Updates the logic for the element.
        public virtual void Update() { }
        //  Summary:
        //      Updates the logic for the element.
        //      This is specifically for the inherited member of Player.
        public virtual void Update(KeyboardState newKeyboardState, KeyboardState oldKeyboardState, MouseState newMouseState,
            MouseState oldMouseState, List<Box> boxList, ref Portal bluePortal, ref Portal orangePortal) { }
        //  Summary:
        //      Updates the logic for the element.
        //      This is specifically for the inherited member of Bullet.
        public virtual void Update(List<Element> boxList, ref Portal portal, ref Portal otherPortal) { }
        //  Summary:
        //      Draws the image of the element.
        //
        //  Parameters:
        //
        //      spriteBatch:
        //          The SpriteBatch for the game.
        public void Draw(SpriteBatch spriteBatch)
        {
            if (exists)
            { spriteBatch.Draw(sprite, new Vector2(screenX, screenY), Color.White); }
        }
    }

    //  Summary:
    //      The class for boxes.
    //      Boxes are used as walls and empty pits.
    public class Box : Element
    {
        //  Summary:
        //      The flag that, if true, says that the box can hold portals.
        public bool portalUse;
        //  Summary:
        //      The flag that, if true, says that the box is an empty pit.
        public bool empty;
        //  Summary:
        //      Initializes the box.
        //      Initializes the box's position in the room, the mouse detection, the list of
        //      sprites, the flag for holding portals, and the flag for being an empty pit.
        //
        //  Parameters:
        //
        //      argX, argY:
        //          The integers for the position of the box.
        //
        //      portalBox:
        //          A bool that, if true, makes the box able to hold portals.
        //
        //      emptyBox:
        //          A bool that, if true, makes the box an empty pit.
        public Box(int argX, int argY, bool portalBox, bool emptyBox)
        {
            roomX = argX;
            roomY = argY;
            mouseOver = false;
            sprites = new List<Texture2D>(0);
            portalUse = portalBox;
            empty = emptyBox;
        }
    }

    //  Summary:
    //      The class for the user's input.
    //      The player is the character.
    public class Player : Element
    {
        //  Summary:
        //      The amount of pixels moved in one unit of gametime.
        private int speed;
        //  Summary:
        //      The flag that, if true, says that the player is currently in a portal.
        //      How this is different from the bool porting:
        //          Porting:
        //          The flag that, if true, says that the player is currently moving through
        //          a portal.
        public bool ported;
        //  Summary:
        //      The flag that, if true, says that the player is currently moving through a portal.
        //      How this is different from the bool ported:
        //          Ported:
        //          The flag that, if true, says that the player is currently in a portal.
        private bool porting;
        //  Summary:
        //      The position of the player's image on the other viewport.
        public int portedRoomX, portedRoomY;

        //  Summary:
        //      The direction that the player is currently facing.
        public double direction;
        //  Summary:
        //      The bullets of the player.
        //      There are two bullets, the blue bullet and the orange bullet, that
        //      correspond to the two portals.
        public Bullet blueBullet, orangeBullet;
        //  Summary:
        //      The counter for the amount of time it takes to shoot another bullet.
        private int rateOfFire;
        //  Summary:
        //      Initializes the player.
        //      Initializes the player's position in the room, the player's speed, the
        //      list of sprites, the bullets, the flag for existence, the flags for
        //      porting and touching a portal, and the rate of fire counter.
        //
        //  Parameters:
        //
        //      argX, argY:
        //          The integers for the position of the player.
        //
        //      argSpeed:
        //          The integer that represents the speed of the player.
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
            rateOfFire = -1;
        }

        //  Summary:
        //      Updates the logic for the player.
        //      Determines collisions, movement, and portal logic.
        //      This is by far the largest update method in the game. As such it has been broken
        //      down and summarized as well.
        //
        //  Parameters:
        //
        //      newKeyboardState:
        //          The current KeyboardState.
        //
        //      oldKeyboardState:
        //          The previous KeyboardState.
        //
        //      newMouseState:
        //          The current MouseState.
        //
        //      oldMouseState:
        //          The previous MouseState.
        //
        //      boxList:
        //          The list of boxes in the current room.
        //
        //      bluePortal:
        //          The blue portal in the room.
        //
        //      orangePortal:
        //          The orange portal in the room.
        public override void Update(KeyboardState newKeyboardState, KeyboardState oldKeyboardState, MouseState newMouseState, 
            MouseState oldMouseState, List<Box> boxList, ref Portal bluePortal, ref Portal orangePortal)
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
                    //      TODO: Not currently set up. Will eventually say if the player
                    //      has fallen into a hole, and the consequences of that.
                    else
                    {
                        if (Collision.TestCompletelyInside(this, boxList[i]))
                        { }
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
    }

    public class Sentry : Element
    {
        public double direction;

        public Sentry(int argX, int argY, double facing)
        {
            roomX = argX;
            roomY = argY;
            direction = facing;
            mouseOver = false;
            sprites = new List<Texture2D>(0);
        }

        public void Update(Player player, List<Box> boxList)
        {

        }
    }

    public class Laser : Element
    {
        public double direction;

        public Laser(LaserShooter laserShooter)
        {
            direction = laserShooter.direction;
            if (direction == 0 || direction == Math.PI)
            {
                roomX = laserShooter.roomX + laserShooter.sprite.Bounds.Center.X;
            }
        }
    }

    public class LaserShooter : Element
    {
        public double direction;

        public LaserShooter(int argX, int argY, double facing)
        {

        }
    }

    public class LaserCatcher : Element
    {

    }

    //  Summary:
    //      The class for the bullets that the player shoots. These create portals.
    public class Bullet : Element
    {
        //  Summary:
        //      The direction the bullet is travelling, in radians.
        //      This is also the direction that the player is or was facing when the bullet was fired.
        private double direction;
        //  Summary:
        //      The position of the bullet as a decimal.
        //      This information is required due to the truncation caused by using integers.
        private double doubleX, doubleY;
        //  Summary:
        //      Initializes the bullet.
        //      Initializes the bullet's position and its existence.
        public Bullet()
        {
            roomX = 0;
            roomY = 0;
            exists = false;
        }
        //  Summary:
        //      Updates the logic for the bullet.
        //      Uses similar logic for collisions as the Player class. Checks for collisions
        //      with boxes and portals. Makes a portal if the bullet collides with a box that
        //      can hold a portal.
        //
        //  Parameters:
        //      
        //      boxList:
        //          The list of boxes in the current room.
        //
        //      portal:
        //          The portal that the bullet will create if it collides with a box that
        //          can hold a portal.
        //
        //      otherPortal:
        //          The portal that the bullet cannot create.
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

        //  Summary:
        //      Finds the part of the box the bullet collided with.
        //
        //  Parameters:
        //
        //      portalBox:
        //          The box the bullet collided with. This box must be able to hold portals.
        //
        //      portal:
        //          The portal that will be made.
        //
        //      bulletExists:
        //          The bool that says whether the bullet exists.
        public void portalMade(Box portalBox, ref Portal portal, ref bool bulletExists)
        {
            double tempDouble = Collision.TestVertical(this, portalBox);
            if (tempDouble == 10)
            { tempDouble = Collision.TestHorizontal(this, portalBox); }
            portal.Created(portalBox, tempDouble, ref bulletExists);
        }

        //  Summary:
        //      Instantiates the bullet's position and direction when fired, and
        //      makes the bullet exist.
        //
        //  Parameters:
        //
        //      positionX, positionY:
        //          The position the bullet is fired from.
        //
        //      argDirection:
        //          The direction the bullet will travel, in radians.
        public void Fired(int positionX, int positionY, double argDirection)
        {
            roomX = positionX;
            roomY = positionY;
            direction = argDirection;
            exists = true;
        }
    }

    //  Summary:
    //      The class for portals. These will transport the player to a different location.
    public class Portal : Element
    {
        //  Summary:
        //      The direction the portal is facing, in radians.
        //      The number 10 was arbitrarily chosen as a null value.
        public double portalDirection;
        //  Summary:
        //      Instantiates the portal.
        //      Instantiates the portals non-existence, its list of sprites, and
        //      its direction as null (10).
        public Portal()
        {
            exists = false;
            sprites = new List<Texture2D>(0);
            portalDirection = 10;
        }
        //  Summary:
        //      Instantiates the portal when it is created by a bullet.
        //      Instantiates the portal's position, using the box's position and the
        //      facing of the portal, the portal's sprite, and then destroys the bullet.
        //
        //  Parameters:
        //
        //      portalBox:
        //          The box that the bullet hit. Must be able to hold portals.
        //
        //      facing:
        //          The direction that the portal will be facing, in radians.
        //
        //      bulletExists:
        //          The flag for the existence of the bullet.
        public void Created(Box portalBox, double facing, ref bool bulletExists)
        {
            if (facing == 0)
            {
                sprite = sprites[1];
                roomX = portalBox.roomX;
                roomY = portalBox.roomY + portalBox.sprite.Height - 5;
                portalDirection = facing;
            }
            if (facing == Math.PI)
            {
                sprite = sprites[1];
                roomX = portalBox.roomX;
                roomY = portalBox.roomY - 5;
                portalDirection = facing;
            }
            if (facing == -Math.PI / 2)
            {
                sprite = sprites[0];
                roomX = portalBox.roomX + portalBox.sprite.Width - 5;
                roomY = portalBox.roomY;
                portalDirection = -facing;
            }
            if (facing == Math.PI / 2)
            {
                sprite = sprites[0];
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
        //  Summary:
        //      Checks the collision between the player and the portal, and keeps the player from
        //      moving through the rest of the box.
        //
        //  Parameters:
        //
        //      player:
        //          The Player in the game.
        //
        //      playerRoomX, playerRoomY:
        //          The position of the player in the room.
        //
        //      ported:
        //          The flag that says if there is a collision between the portal and the player.
        //
        //      up, down, left, right:
        //          The directions that the player can go.
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
    }

    //  Summary:
    //      The class that tests collisions.
    public static class Collision
    {
        //  Summary:
        //      Tests the corners of the first element insideElement the corners of the second element,
        //      then switches the elements and does the same thing.
        //
        //  Parameters:
        //
        //      element1, element2:
        //          An element.
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
        //  Summary:
        //      Tests the coordinate insideElement the corneres of the element.
        //
        //  Parameters:
        //
        //      argX, argY:
        //          The coordinate that will be tested.
        //
        //      element:
        //          The element that will be tested.
        public static bool TestCoordinate(int argX, int argY, Element element)
        {
            if (argX >= element.roomX && argX <= element.roomX + element.sprite.Width)
            {
                if (argY >= element.roomY && argY <= element.roomY + element.sprite.Height)
                { return true; }
            }
            return false;
        }
        //  Summary:
        //      Tests for a collision on the top and bottom of element1 with element2.
        //      The double returned is in radians.
        //
        //  Parameters:
        //
        //      element1:
        //          The element that wil have its top and bottom tested.
        //
        //      element2:
        //          The element that will be tested.
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
        //  Summary:
        //      Tests for a collision on the left and right of element1 with element2.
        //      The double returned is in radians.
        //
        //  Parameters:
        //
        //      element1:
        //          The element that wil have its left and right tested.
        //
        //      element2:
        //          The element that will be tested.
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
        //  Summary:
        //      Tests the corners of the inside element inside the surrounding element.
        //      It will only return true if all the corners are inside the surrounding element.
        //
        //  Parameters:
        //
        //      insideElement:
        //          The element that will be tested if inside the surrounding element.
        //
        //      surroundingElement:
        //          The element that will be tested if surrounding the inside element.
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
    }
}