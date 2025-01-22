using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.MiniGames.PowerCheck.GridCoordinates
{
    public class GridCordEl
    {
        public Vector2 Center { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public TypeOfCordEl TypeOfEl { get; private set; }
        public GridCordEl PreviousEl = null;

        public GridCordEl (Vector2 center, float width, float height, TypeOfCordEl typeofel)
        {
            this.Center = center;
            this.Width = width;
            this.Height = height;
            this.TypeOfEl = typeofel;
        }

        public void SetCenter(Vector3 center)
        {
            Center = new Vector2(center.x, center.z); // Переводим из Vector3 в Vector2
        }
    }
}
