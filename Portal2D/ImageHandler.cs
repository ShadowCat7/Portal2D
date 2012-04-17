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
    public class ImageHandler
    {
        public Texture2D background;
        public Texture2D splitScreen;
        
        public Texture2D playerSprite;

        public Texture2D emptyBox;
        public Texture2D portalWall;
        public Texture2D wall;

        public Texture2D blueBullet;
        public Texture2D orangeBullet;

        public Dictionary<string, Texture2D> laserCatcherSprites;
        public Dictionary<string, Texture2D> laserShooterSprites;
        public List<Texture2D> laserSprites;

        public Dictionary<string, Texture2D> bluePortalSprites;
        public Dictionary<string, Texture2D> orangePortalSprites;

        public ImageHandler()
        {
            laserCatcherSprites = new Dictionary<string, Texture2D>(4);
            laserShooterSprites = new Dictionary<string, Texture2D>(4);
            laserSprites = new List<Texture2D>(2);

            bluePortalSprites = new Dictionary<string, Texture2D>(4);
            orangePortalSprites = new Dictionary<string, Texture2D>(4);
        }
    }
}
