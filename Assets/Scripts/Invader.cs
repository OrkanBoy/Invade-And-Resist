using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orkan;

namespace Game
{
    public class Invader : Damager
    {
        public float speed = 10.0f;


        public IEnumerator FollowPath(IEnumerable<Vector2> path)
        {
            foreach (Vector2 destination in path)
            {
                yield return StartCoroutine(MoveTo(destination));
                while (moving)
                    yield return null;
            }
        }

        public IEnumerator MoveTo(Vector2 destination)
        {
            Vector2 offset = destination - (Vector2)transform.position;
            Debug.Log(Vector2.SignedAngle(Vector2.right, offset));
            StartCoroutine(RotateTo(Vector2.SignedAngle(Vector2.right, offset)));
            while (rotating)
                yield return null;
            yield return StartCoroutine(Move(offset.magnitude)); 
        }

        protected bool moving;
        public IEnumerator Move(float distance)
        {
            moving = true;
            GetComponent<Rigidbody2D>().velocity = speed * Mathf.Sign(distance) * MathUtils.UnitCircle(angle);
            distance = Mathf.Abs(distance);
            Vector3 originalPosition = transform.position;
            while (Vector2.Distance(transform.position, originalPosition) < distance)
                yield return null;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            moving = false;
        }
        //Implement Pathfinding Sys.

        //Implement Player Control Sys.
    }
}
