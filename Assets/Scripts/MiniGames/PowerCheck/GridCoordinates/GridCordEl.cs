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
        public Vector2 GlobalCenter { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public TypeOfCordEl TypeOfEl { get; private set; }
        public GridCordEl PreviousEl = null;
        public GameObject GridElGameObject = null;

        public GridCordEl (float width, float height, TypeOfCordEl typeofel, int row, int column)
        {
            this.Width = width;
            this.Height = height;
            this.TypeOfEl = typeofel;
            this.Row = row;
            this.Column = column;
        }

        public void SetCenter(Vector3 center)
        {
            Center = new Vector2(center.x, center.z); // Переводим из Vector3 в Vector2
        }
        public void SetGlobalCenter(Vector3 center)
        {
            GlobalCenter = new Vector2(center.x, center.z); // Переводим из Vector3 в Vector2
        }
        public void ChangeColour(Color colour)
        {
            var cellGameObjectRenderer = this.GridElGameObject.GetComponent<Renderer>();
            if (cellGameObjectRenderer != null)
            {
                // Создаем экземпляр материала
                cellGameObjectRenderer.material = new Material(cellGameObjectRenderer.material);
                cellGameObjectRenderer.material.color = colour;
            }
            else
            {
                Debug.LogWarning("cellGameObjectRenderer is null");
            }
        }
    }
}
