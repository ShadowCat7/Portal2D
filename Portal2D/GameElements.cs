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

namespace GameSpace
{
    public static class Screen
    { static public int X = 800, Y = 480; }
    public static class SplitScreen
    { static public int X = 400, Y = 480; }

    public abstract class Room
    {
        public Texture2D background;
        public int sizeX, sizeY;
        public int screenX, screenY;
        public int onScreenX, onScreenY;
        public int portedOnScreenX, portedOnScreenY;
        public List<Box> boxList;
        public Player player;
        public Portal bluePortal;
        public Portal orangePortal;

        public Room()
        {
            boxList = new List<Box>(0);
            player = new Player(100, 100, 4);
            bluePortal = new Portal();
            orangePortal = new Portal();
        }
        public void DrawBackground(SpriteBatch spriteBatch)
        { spriteBatch.Draw(background, new Vector2(0, 0), Color.White); }

        //virtual methods
        public virtual void Update(KeyboardState newKeyboardState, KeyboardState oldKeyboardstate, MouseState newMouseState, MouseState oldMouseState, List<Box> boxList) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        //virtual methods for GameRoom
        public virtual void setBoxSprites(Texture2D normalWallSprite, Texture2D portalWallSprite, Texture2D emptyFloorSprite) { }
        public virtual void drawBoxes(SpriteBatch spriteBatch) { }
        public virtual void AddBoxesAround() { }
    }

    public class GameRoom : Room
    {
        public GameRoom()
        {
            sizeX = 1600;
            sizeY = 1000;

            AddBoxesAround();

            screenX = Screen.X;
            screenY = Screen.Y;

            bluePortal = new Portal();
            orangePortal = new Portal();

            boxList.Add(new Box(600, 600, false, true));
            boxList.Add(new Box(100, 400, true, false));
            boxList.Add(new Box(100, 800, true, false));
            boxList.Add(new Box(1400, 400, true, false));
        }

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

        public override void Update(KeyboardState newKeyboardState, KeyboardState oldKeyboardstate, MouseState newMouseState, MouseState oldMouseState, List<Box> boxList)
        {
            player.Update(newKeyboardState, oldKeyboardstate, newMouseState, oldMouseState, boxList, ref bluePortal, ref orangePortal);

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

            for (int i = 0; i < boxList.Count; i++)
            {
                boxList[i].screenX = boxList[i].roomX - onScreenX;
                boxList[i].screenY = boxList[i].roomY - onScreenY;
            }

            player.blueBullet.screenX = player.blueBullet.roomX - onScreenX;
            player.blueBullet.screenY = player.blueBullet.roomY - onScreenY;
            player.orangeBullet.screenX = player.orangeBullet.roomX - onScreenX;
            player.orangeBullet.screenY = player.orangeBullet.roomY - onScreenY;

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

            if (player.ported)
            {
                if (player.portedRoomX + player.sprite.Bounds.Center.X <= screenX / 2)
                {
                    player.portedScreenX = player.portedRoomX + screenX;
                    portedOnScreenX = 0;
                }
                else if (player.roomX + player.sprite.Bounds.Center.X >= sizeX - screenX / 2)
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

                for (int i = 0; i < boxList.Count; i++)
                {
                    boxList[i].portedScreenX = boxList[i].roomX - portedOnScreenX + 400;
                    boxList[i].portedScreenY = boxList[i].roomY - portedOnScreenY;
                }

                player.blueBullet.portedScreenX = player.blueBullet.roomX - portedOnScreenX + 400;
                player.blueBullet.portedScreenY = player.blueBullet.roomY - portedOnScreenY;
                player.orangeBullet.portedScreenX = player.orangeBullet.roomX - portedOnScreenX + 400;
                player.orangeBullet.portedScreenY = player.orangeBullet.roomY - portedOnScreenY;

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
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(0, 0), new Rectangle(onScreenX, onScreenY, Screen.X, Screen.Y), Color.White);
            for (int i = 0; i < boxList.Count; i++)
            {
                if (boxList[i].empty)
                { spriteBatch.Draw(boxList[i].sprite, new Vector2(boxList[i].screenX, boxList[i].screenY), Color.White); }
            }
            spriteBatch.Draw(player.sprite, new Rectangle(player.screenX + player.sprite.Bounds.Center.X, player.screenY + player.sprite.Bounds.Center.Y, player.sprite.Width, player.sprite.Height), 
                null, Color.White, (float)player.direction, new Vector2(player.sprite.Bounds.Center.X, player.sprite.Bounds.Center.Y), new SpriteEffects(), 0);
            
            player.blueBullet.Draw(spriteBatch);
            player.orangeBullet.Draw(spriteBatch);

            drawBoxes(spriteBatch);
            bluePortal.Draw(spriteBatch);
            orangePortal.Draw(spriteBatch);

            if (player.ported)
            {
                spriteBatch.Draw(background, new Vector2(400, 0), new Rectangle(portedOnScreenX, portedOnScreenY, SplitScreen.X, SplitScreen.Y),Color.White);
                spriteBatch.Draw(player.sprite, new Rectangle(player.portedScreenX + player.sprite.Bounds.Center.X, player.portedScreenY + player.sprite.Bounds.Center.Y, player.sprite.Width, player.sprite.Height),
                null, Color.White, (float)player.direction, new Vector2(player.sprite.Bounds.Center.X, player.sprite.Bounds.Center.Y), new SpriteEffects(), 0);

                for (int i = 0; i < boxList.Count; i++)
                { spriteBatch.Draw(boxList[i].sprite, new Vector2(boxList[i].portedScreenX, boxList[i].portedScreenY), Color.White); }

                if (player.blueBullet.exists)
                { spriteBatch.Draw(player.blueBullet.sprite, new Vector2(player.blueBullet.portedScreenX, player.blueBullet.portedScreenY), Color.White); }
                if (player.orangeBullet.exists)
                { spriteBatch.Draw(player.orangeBullet.sprite, new Vector2(player.orangeBullet.portedScreenX, player.orangeBullet.portedScreenY), Color.White); }

                spriteBatch.Draw(bluePortal.sprite, new Vector2(bluePortal.portedScreenX, bluePortal.portedScreenY), Color.White);
                spriteBatch.Draw(orangePortal.sprite, new Vector2(orangePortal.portedScreenX, orangePortal.portedScreenY), Color.White);
            }
        }

        public override void setBoxSprites(Texture2D normalWallSprite, Texture2D portalWallSprite, Texture2D emptyFloorSprite)
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
        public override void drawBoxes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < boxList.Count; i++)
            {
                if (!boxList[i].empty)
                { spriteBatch.Draw(boxList[i].sprite, new Vector2(boxList[i].screenX, boxList[i].screenY), Color.White); }
            }
        }
    }

    public class Element
    {
        public int screenX, screenY;
        public int roomX, roomY;
        public int centerX, centerY;
        public int portedScreenX, portedScreenY;
        public bool mouseOver;
        public bool onScreen;
        public bool exists;
        public Texture2D sprite;
        public List<Texture2D> sprites;
        public Element() { }
        public Element(int argX, int argY)
        {
            roomX = argX;
            roomY = argY;
            mouseOver = false;
            sprites = new List<Texture2D>(0);
            onScreen = true;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (exists)
            { spriteBatch.Draw(sprite, new Vector2(screenX, screenY), Color.White); }
        }
    }

    public class Box : Element
    {
        public bool portalUse;
        public bool empty;
        public Box(int argX, int argY, bool portalBox, bool emptyBox)
        {
            roomX = argX;
            roomY = argY;
            mouseOver = false;
            sprites = new List<Texture2D>(0);
            onScreen = true;
            portalUse = portalBox;
            empty = emptyBox;
        }
    }

    public class Player : Element
    {
        public int Speed;
        public bool ported;
        public bool porting;
        public int portedRoomX, portedRoomY;
        public double portedDirection;

        public double direction;
        public Bullet blueBullet;
        public Bullet orangeBullet;
        public int rateOfFire;
        public Player(int argX, int argY, int argSpeed)
        {
            roomX = argX;
            roomY = argY;
            Speed = argSpeed;
            sprites = new List<Texture2D>(0);
            blueBullet = new Bullet();
            orangeBullet = new Bullet();
            exists = true;
            ported = false;
            porting = false;
            rateOfFire = -1;
        }
        public void Update(KeyboardState newKeyboardState, KeyboardState oldKeyboardState, MouseState newMouseState, MouseState oldMouseState, List<Box> boxList,
                      ref Portal bluePortal, ref Portal orangePortal)
        {
            bool up = true,
                 down = true,
                 left = true,
                 right = true;
            for (int count = 0; count < Speed; count++)
            {
                for (int i = 0; i < boxList.Count; i++)
                {
                    if (!boxList[i].empty)
                    {
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
                    else
                    {
                        if (Collision.TestCompletelyInside(this, boxList[i]))
                        { }
                    }
                }

                bool bluePortalporting = false;
                bool orangePortalporting = false;

                if (bluePortal.exists && orangePortal.exists)
                { bluePortal.Porting(this, ref roomX, ref roomY, ref bluePortalporting, orangePortal, ref portedRoomX, ref portedRoomY, ref portedDirection, ref up, ref down, ref left, ref right); }
                if (bluePortal.exists && orangePortal.exists)
                { orangePortal.Porting(this, ref roomX, ref roomY, ref orangePortalporting, bluePortal, ref portedRoomX, ref portedRoomY, ref portedDirection, ref up, ref down, ref left, ref right); }
                if (bluePortalporting || orangePortalporting)
                { ported = true; }
                else
                { ported = false; }

                Portal otherPortal = new Portal();
                if (bluePortalporting)
                { otherPortal = orangePortal; }
                else
                { otherPortal = bluePortal; }

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

                if (ported && !porting && newKeyboardState.IsKeyDown(Keys.Space) && !oldKeyboardState.IsKeyDown(Keys.Space))
                { porting = true; }

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
                            ported = false;
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
                            ported = false;
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
                            ported = false;
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
                            ported = false;
                        }
                    }
                }

                else
                {
                    if (newKeyboardState.IsKeyDown(Keys.W) && up)
                    { roomY -= 1; }
                    if (newKeyboardState.IsKeyDown(Keys.S) && down)
                    { roomY += 1; }
                    if (newKeyboardState.IsKeyDown(Keys.A) && left)
                    { roomX -= 1; }
                    if (newKeyboardState.IsKeyDown(Keys.D) && right)
                    { roomX += 1; }

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

                    direction = Math.Atan2(newMouseState.Y - screenY, newMouseState.X - screenX) + Math.PI / 2;
                }
            }

            if (!ported)
            { porting = false; }

            if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton != ButtonState.Pressed && !blueBullet.exists && !ported && rateOfFire == -1)
            {
                blueBullet.Fired(roomX + sprite.Bounds.Center.X, roomY + sprite.Bounds.Center.Y, direction);
                rateOfFire = 0;
            }

            if (newMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton != ButtonState.Pressed && !orangeBullet.exists && !ported && rateOfFire == -1)
            {
                orangeBullet.Fired(roomX + sprite.Bounds.Center.X, roomY + sprite.Bounds.Center.Y, direction);
                rateOfFire = 0;
            }
            if (rateOfFire > -1)
            {
                rateOfFire += 1;
                if (rateOfFire == 20)
                { rateOfFire = -1; }
            }

            if (blueBullet.exists)
            { blueBullet.Update(boxList, ref bluePortal, ref orangePortal); }
            if (orangeBullet.exists)
            { orangeBullet.Update(boxList, ref orangePortal, ref bluePortal); }
        }
    }

    public class Bullet : Element
    {
        public double direction;
        public double doubleX, doubleY;
        public Bullet()
        {
            roomX = 0;
            roomY = 0;
            exists = false;
        }
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
                        { portalMade(boxList[i], ref portal, ref this.exists); }
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

        public void portalMade(Box portalBox, ref Portal portal, ref bool bulletExists)
        {
            double tempDouble = Collision.TestVertical(this, portalBox);
            if (tempDouble == 10)
            { tempDouble = Collision.TestHorizontal(this, portalBox); }
            portal.Created(portalBox, tempDouble, ref bulletExists);
        }

        public void Fired(int positionX, int positionY, double argDirection)
        {
            roomX = positionX;
            roomY = positionY;
            direction = argDirection;
            exists = true;
        }
    }

    public class Portal : Element
    {
        public double portalDirection;
        public Portal()
        {
            exists = false;
            sprites = new List<Texture2D>(0);
            portalDirection = 10;
        }
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

        public void Porting(Player player, ref int playerRoomX, ref int playerRoomY, ref bool ported, Portal otherPortal, 
            ref int portedToX, ref int portedToY, ref double portedDirection, ref bool up, ref bool down, ref bool left, ref bool right)
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

    public static class Collision
    {
        public static bool Test(Element element1, Element element2)
        {
            if (element1.roomX >= element2.roomX && element1.roomX <= element2.roomX + element2.sprite.Width)
            {
                if (element1.roomY >= element2.roomY && element1.roomY <= element2.roomY + element2.sprite.Height)
                { return true; }
                if (element1.roomY + element1.sprite.Height >= element2.roomY && element1.roomY + element1.sprite.Height <= element2.roomY + element2.sprite.Height)
                { return true; }
            }
            if (element1.roomX + element1.sprite.Width >= element2.roomX && element1.roomX + element1.sprite.Width <= element2.roomX + element2.sprite.Width)
            {
                if (element1.roomY >= element2.roomY && element1.roomY <= element2.roomY + element2.sprite.Height)
                { return true; }
                if (element1.roomY + element1.sprite.Height >= element2.roomY && element1.roomY + element1.sprite.Height <= element2.roomY + element2.sprite.Height)
                { return true; }
            }

            Element tempElement = element1;
            element1 = element2;
            element2 = tempElement;

            if (element1.roomX >= element2.roomX && element1.roomX <= element2.roomX + element2.sprite.Width)
            {
                if (element1.roomY >= element2.roomY && element1.roomY <= element2.roomY + element2.sprite.Height)
                { return true; }
                if (element1.roomY + element1.sprite.Height >= element2.roomY && element1.roomY + element1.sprite.Height <= element2.roomY + element2.sprite.Height)
                { return true; }
            }
            if (element1.roomX + element1.sprite.Width >= element2.roomX && element1.roomX + element1.sprite.Width <= element2.roomX + element2.sprite.Width)
            {
                if (element1.roomY >= element2.roomY && element1.roomY <= element2.roomY + element2.sprite.Height)
                { return true; }
                if (element1.roomY + element1.sprite.Height >= element2.roomY && element1.roomY + element1.sprite.Height <= element2.roomY + element2.sprite.Height)
                { return true; }
            }

            return false;
        }

        public static bool TestCoordinate(int argX, int argY, Element element2)
        {
            if (argX >= element2.roomX && argX <= element2.roomX + element2.sprite.Width)
            {
                if (argY >= element2.roomY && argY <= element2.roomY + element2.sprite.Height)
                { return true; }
            }
            return false;
        }

        public static double TestVertical(Element element1, Element element2)
        {
            if ((element1.roomX >= element2.roomX && element1.roomX <= element2.roomX + element2.sprite.Width - 1) || (element1.roomX + element1.sprite.Width >= element2.roomX + 1 && element1.roomX + element1.sprite.Width <= element2.roomX + element2.sprite.Width))
            {
                if (element1.roomY <= element2.roomY + element2.sprite.Height && element1.roomY >= element2.roomY)
                { return 0; }
                if (element1.roomY + element1.sprite.Height >= element2.roomY && element1.roomY + element1.sprite.Height <= element2.roomY + element2.sprite.Height)
                { return Math.PI; }
            }
            return 10;
        }

        public static double TestHorizontal(Element element1, Element element2)
        {
            if ((element1.roomY >= element2.roomY && element1.roomY <= element2.roomY + element2.sprite.Height - 1) || (element1.roomY + element1.sprite.Height >= element2.roomY + 1 && element1.roomY + element1.sprite.Height <= element2.roomY + element2.sprite.Height))
            {
                if (element1.roomX <= element2.roomX + element2.sprite.Width && element1.roomX >= element2.roomX)
                { return -Math.PI / 2; }
                if (element1.roomX + element1.sprite.Width >= element2.roomX && element1.roomX + element1.sprite.Width <= element2.roomX)
                { return Math.PI / 2; }
            }
            return 10;
        }

        public static bool TestCompletelyInside(Element inside, Element surrounding)
        {
            if (inside.roomX >= surrounding.roomX && inside.roomX + inside.sprite.Width <= surrounding.roomX + surrounding.sprite.Width)
            {
                if (inside.roomY >= surrounding.roomY && inside.roomY + inside.sprite.Height <= surrounding.roomY + surrounding.sprite.Height)
                { return true; }
            }
            return false;
        }
    }

    public static class RoomChanger
    {
        static Room Change(Room roomChange, Player keepPlayer)
        {
            roomChange.player = keepPlayer;
            return roomChange;
        }
    }
}