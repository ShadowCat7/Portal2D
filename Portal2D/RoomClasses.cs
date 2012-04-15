using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameSpace
{
    public class Room1 : GameRoom
    {
        public Room1()
        {
            sizeX = 1600;
            sizeY = 1000;

            boxList = new List<Box>(0);
            player = new Player(100, 100, 4);

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
    }
}
