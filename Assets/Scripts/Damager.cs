using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Orkan;

namespace Game
{
    public abstract class Damager : Damageable
    {
        public static ParticleSystem muzzleFlash;
        public static ParticleSystem hitSpark;

        //Rotate Sys.
        public float angularSpeed = 300.0f;
        public float angle
        {
            get => transform.eulerAngles.z.Mod(360.0f);
            set => transform.eulerAngles = new Vector3(0.0f, 0.0f, value.Mod(360.0f));
        }

        protected object rotation;
        protected bool rotating;
        public IEnumerator RotateTo(float angle)
        {
            rotating = true;
            object rotation = this.rotation = new ();
            angle = angle.Mod(360.0f);
            float angularVelocity = angularSpeed * MathUtils.RotationSign(this.angle, angle);
            float prevAngle;
            do
            {
                prevAngle = this.angle;
                this.angle += angularVelocity * Time.deltaTime;
                yield return null;
                if (this.rotation != rotation)
                    break;
            } while (Mathf.Sign(angle - prevAngle) == Mathf.Sign(angle - this.angle));
            rotating = false;
        }
        public IEnumerator Rotate(float dAngle) { yield return StartCoroutine(RotateTo(angle + dAngle)); }

        //Damage Sys.
        public int damage = 5;
        [Range(0.0f, 360.0f)]
        public float maxInaccuracyAngle = 1;
        public IEnumerator Damage(Damageable damageable) 
        {
            Vector2 targetOffset = damageable.transform.position - transform.position;
            float targetAngle = Mathf.Atan(targetOffset.y / targetOffset.x);
            yield return StartCoroutine(RotateTo(targetAngle));

            int ammo = (int)(firingSpeed / targetOffset.magnitude);
            if (ammo < 0)
                ammo = 0;

            while(this.ammo > 0 && ammo > 0)
            {
                StartCoroutine(Shoot());
                yield return new WaitForSeconds(1.0f / firingSpeed);
            }
        }

        protected bool shooting;
        public IEnumerator Shoot() 
        {
            if (shooting || ammo <= 0) yield break;
            shooting = true;

            float inaccurateAngle = angle + UnityEngine.Random.Range(-maxInaccuracyAngle, maxInaccuracyAngle) / 2;
            Vector3 dir = MathUtils.UnitCircle(inaccurateAngle);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + dir, dir);
            ammo--;

            muzzleFlash.transform.eulerAngles = new Vector3(0.0f, 0.0f, inaccurateAngle);
            if(!muzzleFlash.isPlaying) muzzleFlash.Emit(1);
            if (hit.transform != null && !hitSpark.isPlaying)
            {
                hitSpark.transform.position = hit.point;
                hitSpark.transform.eulerAngles = new Vector3(0.0f, 0.0f, Vector2.SignedAngle(Vector2.right, hit.normal) + 180.0f);
                hitSpark.Play();
            }
            yield return new WaitForSeconds(1/firingSpeed);

            shooting = false;
            if (hit.transform == null) yield break;
            bool hasHit = hit.transform.TryGetComponent(out Damageable hitTarget);
            if (hasHit && ((faction != hitTarget.faction && damage >= 0) || (faction == hitTarget.faction && damage <= 0)))
            {
                hitTarget.health -= damage;
                if(hitTarget.health <= 0) OnDeath(this);
                hitTarget.health = Mathf.Clamp(hitTarget.health, 0, maxHealth);
            }
        }

        //Reload Sys.
        public float firingSpeed = 1.0f;
        public float reloadSpeed = 3.0f;
        public int maxAmmo = 100;
        private int ammo;

        private float sinceLastReload;
        protected void Awake()
        {
            ammo = maxAmmo;
            (muzzleFlash = Instantiate(Resources.Load<ParticleSystem>("MuzzleFlash"), transform)).Stop();
            (hitSpark = Instantiate(Resources.Load<ParticleSystem>("HitSpark"))).Stop();
            
            ParticleSystem.MainModule mfMain = muzzleFlash.main;
            ParticleSystem.MainModule hsMain = hitSpark.main;
            mfMain.duration = hsMain.duration = 0.1f / Mathf.Exp(firingSpeed) + 0.2f;
            mfMain.startSize = hsMain.startSize = 0.5f / Mathf.Exp(firingSpeed / 10.0f) + 0.05f;
        }

        protected void FixedUpdate()
        {
            if (sinceLastReload >= 1.0f / reloadSpeed)
            {
                ammo = Mathf.Min(ammo + 1, maxAmmo);
                sinceLastReload = 0;
            }
            sinceLastReload += Time.fixedDeltaTime;
        }
    }
}
