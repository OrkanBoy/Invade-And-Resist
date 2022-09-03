using UnityEngine;
using Orkan;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class ControllableInvader : Invader
    {
        PathFinding<GameObject> pathFinding;
        
        void Start() 
        {
            string map = 
                "-###-##-#\n"+
                "#-##----#\n"+
                "-----###-\n"+
                "-###-#-#-\n"+
                "---#---#-\n"+
                "#--#####-";

            Grid<GameObject> grid = new (9, 6, (x, y) => {
                if (map[(5 - y) * 10 + x] == '-') return null;
                return Instantiate(Resources.Load<GameObject>("Square"), new Vector2(x, y), Quaternion.identity);
            });
            //5,2 => 1, 4

            pathFinding = new (grid);
            Vector2[] path = pathFinding.FindPath(new Vector2(1, 1), new Vector2(8, 0)).ToArray();
            for (int i = 0; i < path.Length - 1; i++)
                Debug.DrawLine(path[i], path[i + 1], Color.red, 100.0f);
            StartCoroutine(FollowPath(path));

            
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.W))
                StartCoroutine(Move(0.01f));
            if (Input.GetKey(KeyCode.S))
                StartCoroutine(Move(-0.01f));
            if (Input.GetKey(KeyCode.D))
                StartCoroutine(Rotate(-1.0f));
            if (Input.GetKey(KeyCode.A))
                StartCoroutine(Rotate(1.0f));
            if (Input.GetKey(KeyCode.Mouse0))
                StartCoroutine(Shoot());
        }
    }

}
