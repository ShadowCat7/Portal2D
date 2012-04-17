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
    public class Room1 : GameRoom
    {
        public Room1() { }

        public Room1(ImageHandler images)
        {
            sizeX = 1600;
            sizeY = 1000;

            boxList = new List<Box>(0);
            laserShooters = new List<LaserShooter>();
            laserCatchers = new List<LaserCatcher>();

            player = new Player(100, 100, 4);

            AddBoxesAround();

            screenX = Screen.X;
            screenY = Screen.Y;

            bluePortal = new Portal();
            orangePortal = new Portal();

            boxList.Add(new Box(300, 100, true, false));
            boxList.Add(new Box(300, 200, true, false));
            boxList.Add(new Box(300, 300, false, true));
            boxList.Add(new Box(300, 400, false, true));
            boxList.Add(new Box(300, 500, false, true));
            boxList.Add(new Box(300, 600, false, true));
            boxList.Add(new Box(300, 700, false, true));
            boxList.Add(new Box(300, 800, false, true));

            boxList.Add(new Box(100, 800, true, false));
            boxList.Add(new Box(200, 800, true, false));

            boxList.Add(new Box(600, 100, true, false));
            boxList.Add(new Box(600, 200, true, false));
            boxList.Add(new Box(600, 300, true, false));
            boxList.Add(new Box(600, 400, true, false));
            boxList.Add(new Box(600, 500, true, false));
            boxList.Add(new Box(600, 600, true, false));
            boxList.Add(new Box(600, 700, true, false));

            laserShooters.Add(new LaserShooter(100, 500, Math.PI / 2, images.laserSprites));
            laserShooters.Add(new LaserShooter(100, 800, 0, images.laserSprites));

            player.sprite = images.playerSprite;
            player.blueBullet.sprite = images.blueBullet;
            player.orangeBullet.sprite = images.orangeBullet;

            background = images.background;
            screenSplitter = images.splitScreen;

            bluePortal.sprites = images.bluePortalSprites;
            orangePortal.sprites = images.orangePortalSprites;

            setBoxSprites(images.wall, images.portalWall, images.emptyBox);
            setAllLaserSprites(images.laserShooterSprites, images.laserCatcherSprites);
        }
    }
}
