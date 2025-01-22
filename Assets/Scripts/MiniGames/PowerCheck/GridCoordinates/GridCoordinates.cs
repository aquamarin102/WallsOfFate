using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MiniGames.PowerCheck.GridCoordinates
{
    public class GridCordinates : MonoBehaviour
    {
        public int MatrixWidth = 20; // Количество колонок в матрице
        public int MatrixHeight = 20; // Количество строк в матрице
        public float CellWidth = 1; // Ширина одной клетки
        public float CellHeight = 1; // Высота одной клетки
        public Transform GridCenter; // Трансформ центра матрицы
        public Transform ParentForAllCells; // Родительский объект для всех клеток

        public List<List<GridCordEl>> CordMatrix { get; private set; } // Матрица координат

        private void Awake()
        {
            InitializeMatrix();
            DrawMatrix();
        }

        /// <summary>
        /// Инициализация матрицы координат
        /// </summary>
        private void InitializeMatrix()
        {
            CordMatrix = new List<List<GridCordEl>>();

            Vector2 pos = Vector2.zero;
            GridCordEl gridel = new GridCordEl(pos, CellWidth, CellWidth, TypeOfCordEl.NotOccupied);

            for (int i = 0; i < MatrixHeight; i++)
            {
                var row = new List<GridCordEl>();
                for (int j = 0; j < MatrixWidth; j++)
                {
                    row.Add(gridel); // По умолчанию все клетки незаняты
                }
                CordMatrix.Add(row);
            }
        }

        /// <summary>
        /// Отрисовка матрицы в игровом мире
        /// </summary>
        public void DrawMatrix()
        {
            if (GridCenter == null)
            {
                Debug.LogError("Центр матрицы не задан!");
                return;
            }

            Vector3 startPosition = GridCenter.position - new Vector3(
                (MatrixWidth * CellWidth) / 2f,
                0,
                (MatrixHeight * CellHeight) / 2f); // Верхний левый угол матрицы относительно центра

            for (int i = 0; i < MatrixHeight; i++)
            {
                for (int j = 0; j < MatrixWidth; j++)
                {
                    // Вычисляем позицию каждой клетки
                    Vector3 cellPosition = startPosition + new Vector3(
                        j * CellWidth + CellWidth / 2f,
                        -0.1f,
                        i * CellHeight + CellHeight / 2f);
                    
                    DrawCell(cellPosition, i, j, ParentForAllCells);

                    CordMatrix[i][j].SetCenter(cellPosition);
                }
            }
        }

        private void DrawCell(Vector3 cellPosition, int i, int j, Transform parent)
        {
            // Отрисовка клетки (например, создаем визуальный объект)
            GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cell.transform.position = cellPosition;
            cell.transform.localScale = new Vector3(CellWidth, 0.2f, CellHeight);
            cell.name = $"Cell[{i},{j}]";

            // Установка родительского объекта для клетки
            if (parent != null)
                cell.transform.SetParent(parent);

            // Настройка цвета в зависимости от состояния (по умолчанию все белые)
            var renderer = cell.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = Color.white;

            // Создание границ клетки
            float borderThickness = 0.1f; // Толщина границ

            // Верхняя граница
            GameObject topBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topBorder.transform.position = cellPosition + new Vector3(0, 0.1f, CellHeight / 2f - borderThickness / 2f);
            topBorder.transform.localScale = new Vector3(CellWidth, borderThickness, borderThickness);
            topBorder.name = $"TopBorder[{i},{j}]";

            // Нижняя граница
            GameObject bottomBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bottomBorder.transform.position = cellPosition + new Vector3(0, 0.1f, -(CellHeight / 2f - borderThickness / 2f));
            bottomBorder.transform.localScale = new Vector3(CellWidth, borderThickness, borderThickness);
            bottomBorder.name = $"BottomBorder[{i},{j}]";

            // Левая граница
            GameObject leftBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftBorder.transform.position = cellPosition + new Vector3(-(CellWidth / 2f - borderThickness / 2f), 0.1f, 0);
            leftBorder.transform.localScale = new Vector3(borderThickness, borderThickness, CellHeight);
            leftBorder.name = $"LeftBorder[{i},{j}]";

            // Правая граница
            GameObject rightBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightBorder.transform.position = cellPosition + new Vector3(CellWidth / 2f - borderThickness / 2f, 0.1f, 0);
            rightBorder.transform.localScale = new Vector3(borderThickness, borderThickness, CellHeight);
            rightBorder.name = $"RightBorder[{i},{j}]";

            // Установка родительского объекта для границ
            topBorder.transform.SetParent(cell.transform);
            bottomBorder.transform.SetParent(cell.transform);
            leftBorder.transform.SetParent(cell.transform);
            rightBorder.transform.SetParent(cell.transform);

            // Установка цвета для границ (например, чёрный)
            Color borderColor = Color.black;

            var topBorderRenderer = topBorder.GetComponent<Renderer>();
            if (topBorderRenderer != null)
                topBorderRenderer.material.color = borderColor;

            var bottomBorderRenderer = bottomBorder.GetComponent<Renderer>();
            if (bottomBorderRenderer != null)
                bottomBorderRenderer.material.color = borderColor;

            var leftBorderRenderer = leftBorder.GetComponent<Renderer>();
            if (leftBorderRenderer != null)
                leftBorderRenderer.material.color = borderColor;

            var rightBorderRenderer = rightBorder.GetComponent<Renderer>();
            if (rightBorderRenderer != null)
                rightBorderRenderer.material.color = borderColor;
        }

    }
}
