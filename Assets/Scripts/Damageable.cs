using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Damageable : MonoBehaviour
    {
        public int faction = 0;
        
        public readonly int maxHealth = 100;
        public int health = 100;

        public class DeathEventArgs : EventArgs 
        {
            public Damager killer;
        }
        public event EventHandler<DeathEventArgs> Death;
        protected virtual void OnDeath(Damager damager) 
        {
            Death?.Invoke(this, new DeathEventArgs { killer = damager });
        }
    }
}
