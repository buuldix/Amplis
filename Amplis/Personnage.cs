using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amplis
{
    public class Personnage
    {

        private String[,] anim = new String[,] { { "idle", "walkSouth", "walkWest", "walkEast", "walkNorth" }, { "idle2", "walkSouth2", "walkWest2", "walkEast2", "walkNorth2" } };
        public const int NBPERS = 2;
        private double persDelay;
        private bool grounded;
        private Vector2 _persoPosition;
        private int _yVelocity;
        private int _xVelocity;
        private bool climbing;
        int pers;
        public Personnage()
        {
            Pers = 0;
            PersDelay = 0;
            Position = new Vector2(100, 500);
            YVelocity = 0;
            XVelocity = 4;
            Grounded = true;
            Climbing = false;
        }

        public string[,] Anim { get => anim; set => anim = value; }
        public int Pers { get => pers; set => pers = value; }
        public double PersDelay { get => persDelay; set => persDelay = value; }
        public bool Grounded { get => grounded; set => grounded = value; }
        public Vector2 Position { get => _persoPosition; set => _persoPosition = value; }
        public float X { get => _persoPosition.X; set => _persoPosition.X = value; }
        public float Y { get => _persoPosition.Y; set => _persoPosition.Y = value; }
        public int YVelocity { get => _yVelocity; set => _yVelocity = value; }
        public int XVelocity { get => _xVelocity; set => _xVelocity = value; }
        public bool Climbing { get => climbing; set => climbing = value; }

        public void ChangePers()
        {
            if (this.Pers == NBPERS - 1)
                this.Pers = 0;
            else
                this.Pers++;
        }
    }
}
