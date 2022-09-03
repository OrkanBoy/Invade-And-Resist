using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Orkan
{
    public class Grid<T>
    {
        private Vector2 position;
        private readonly T[,] cells;

        public readonly int width, height;
        public float xSpacing = 1.0f, ySpacing = 1.0f;

        public event EventHandler<ChangedEventArgs> Changed;
        public class ChangedEventArgs : EventArgs 
        {
            public int x, y;
        }

        public T this[in int x, in int y]
        {
            get
            {
                if (IsValidArrayPos(x, y))
                    return cells[x, y];
                return default;
            }
            set
            {
                if (IsValidArrayPos(x, y))
                {
                    cells[x, y] = value;
                    Changed?.Invoke(this, new ChangedEventArgs() { x = x, y = y});
                }
            }
        }
        public T this[in Vector3 v]
        {
            get
            {
                ToArrayPos(v, out int x, out int y);
                return this[x, y];
            }
            set
            {
                ToArrayPos(v, out int x, out int y);
                this[x, y] = value;
            }
        }

        public Grid(in int width, in int height, in Func<int, int, T> func = null)
        {
            cells = new T[width, height];
            this.width = width;
            this.height = height;

            if (func != null)
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        cells[x, y] = func(x, y);
        }
        public Grid(in T[,] cells)
        {
            width = cells.GetLength(0);
            height = cells.GetLength(1);
            this.cells = cells;
        }

        public bool IsValidArrayPos(in int x, in int y) => x >= 0 && x < width && y >= 0 && y < height;

        public Vector2 ToWorldPos(int x, int y) => new Vector2(x * xSpacing, y * ySpacing) + position;
        public void ToArrayPos(Vector2 v, out int x, out int y)
        {
            x = (int)((v.x - position.x) / xSpacing);
            y = (int)((v.y - position.y) / ySpacing);
        }
    }
}

