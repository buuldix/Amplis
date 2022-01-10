using System;
using System.Collections.Generic;
using System.Text;

namespace Amplis
{
    public class Personnage
    {
        
        private String[,] anim = new String[,] { {"idle", "walkSouth", "walkWest", "walkEast", "walkNorth" }, { "idle2", "walkSouth2", "walkWest2", "walkEast2", "walkNorth2" } };
        public const int NBPERS = 2;
        private double persDelay;
        int pers;
        public Personnage()
        {
            Pers = 0;
            PersDelay = 0;
        }

        public string[,] Anim { get => anim; set => anim = value; }
        public int Pers { get => pers; set => pers = value; }
        public double PersDelay { get => persDelay; set => persDelay = value; }

        public void ChangePers()
        {
            if (this.Pers == NBPERS - 1)
                this.Pers = 0;
            else
                this.Pers++;
        }
    }
}
