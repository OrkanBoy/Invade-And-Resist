using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Flag : Damageable
    {
        public const int RADIUS = 5;
        public Flag()
        {
            Death += (sender, e) => 
            {
                faction = e.killer.faction;
                health = maxHealth;
            };
        }
        //Implement Area Control Sys.
    }
}
