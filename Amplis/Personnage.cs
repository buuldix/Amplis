using System;
using System.Collections.Generic;
using System.Text;

namespace Amplis
{
    public class Personnage
    {
        public String[,] anim = new String[,] { {"idle", "walkSouth", "walkWest", "walkEast", "walkNorth" }, { "idle2", "walkSouth2", "walkWest2", "walkEast2", "walkNorth2" } };

        public Personnage()
        { 
        }

        public string[,] Anim { get => anim; set => anim = value; }

        public String Animation(int perso,int anim)
        {
            return this.Anim[perso, anim];
        }
    }
}
