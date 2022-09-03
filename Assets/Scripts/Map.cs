using System;
using System.Collections.Generic;
using UnityEngine;
using Orkan;
using System.Linq;
using System.Threading.Tasks;

namespace Game
{
    public class Map
    {
        public HashSet<Invader> invaders;

        private Grid<Damageable> buildingGrid;
        private HashSet<Flag> flags;
        public HashSet<Mine> mines;
        public HashSet<Resistor> resistors;

        public Damageable this[int x, int y]
        {
            get => buildingGrid[x, y];
            set 
            {
                if (buildingGrid[x, y] != null) return;

                if (!flags.Any(flag => flag.faction == value.faction && Vector2.Distance(new Vector2Int(x, y), flag.transform.position) <= Flag.RADIUS)) return;

                switch (value)
                {
                    case Flag:
                        return;
                    case Invader invader:
                        invaders.Add(invader); return;
                    case Resistor resistor:
                        resistors.Add(resistor); break;
                    case Mine mine:
                        mines.Add(mine); break;
                }
                value.transform.position = buildingGrid.ToWorldPos(x, y);
                buildingGrid[x, y] = value;
            }
        }

        public int this[Flag flag] 
        {
            set 
            {
                if (flags.Contains(flag))
                    flag.faction = value;
            }
        }
    }
}
