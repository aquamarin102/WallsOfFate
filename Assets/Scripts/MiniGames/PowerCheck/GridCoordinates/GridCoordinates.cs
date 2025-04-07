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
        public List<Transform> ObjectsToCheck; // Список трансформов объектов для проверки


        private GridCellFinder finder;

        private void Awake()
        {
            InitializeMatrix();
            DrawMatrix();

            Vector3 vector2 = new Vector2(4.75f, -3.25f);

            finder = new GridCellFinder(this.GetComponent<GridCordinates>());

            //GridCordEl gridCordEl = finder.FindCellsByPosition(vector2);
            //Debug.Log(gridCordEl.Center);

            //HashSet<GridCordEl> allelms = finder.GetAdjacentCells(vector2, 0.5f, 0.5f, gridCordEl.Row, gridCordEl.Column);
            //foreach(GridCordEl cell in allelms)
            //{
            //    cell.ChangeColour(Color.grey);
            //    //ChanegeCellColor(cell);
            //    Debug.Log(cell.Column + " " + cell.Row);
            //}
            //Debug.Log(gridCordEl.Center);
        }

        //private void FixedUpdate()
        //{

        //    Vector2 gridCenterPosition = new Vector2(GridCenter.localPosition.x, GridCenter.localPosition.z);


        //    foreach (Transform obj in ObjectsToCheck)
        //    {
        //        Vector2 objLocPOsition = new Vector2(obj.localPosition.x, obj.localPosition.z);
        //        Vector2 objGlobPOsition = new Vector2(obj.position.x, obj.position.z);
        //        float distance = Vector2.Distance(gridCenterPosition, objGlobPOsition);

        //        // Проверяем условия расстояния
        //        if (distance <= MatrixWidth / 2 && distance <= MatrixHeight / 2)
        //        {
        //            GridCordEl gridCordEl = finder.FindCellsByPosition(objLocPOsition);
        //            if (gridCordEl != null)
        //            {
        //                HashSet<GridCordEl> allelms = finder.GetAdjacentCells(objLocPOsition, obj.localScale.x / 2, obj.localScale.z / 2, gridCordEl.Row, gridCordEl.Column);

        //                if (allelms.Count > 0)
        //                {
        //                    Debug.Log('\n');
        //                }
        //                foreach (GridCordEl cell in allelms)
        //                {
        //                    cell.ChangeColour(Color.grey);
        //                    Debug.Log(cell.Column + " " + cell.Row);
        //                }
        //            }
        //        }
        //    }
        //}

        private static void ChanegeCellColor(GridCordEl cell)
        {
            if(cell.TypeOfEl == TypeOfCordEl.Border)
                cell.ChangeColour(Color.grey);
            else if(cell.TypeOfEl == TypeOfCordEl.Heal)
                cell.ChangeColour(Color.green);
            else if(cell.TypeOfEl == TypeOfCordEl.Damage)
                cell.ChangeColour(Color.red);
            else if(cell.TypeOfEl == TypeOfCordEl.SpeedBuff)
                cell.ChangeColour(Color.yellow);
            else if(cell.TypeOfEl == TypeOfCordEl.Path)
                cell.ChangeColour(Color.blue);
        }

        /// <summary>
        /// Инициализация матрицы координат
        /// </summary>
        public void InitializeMatrix()
        {
            CordMatrix = new List<List<GridCordEl>>();

            for (int i = 0; i < MatrixHeight; i++)
            {
                var row = new List<GridCordEl>();
                for (int j = 0; j < MatrixWidth; j++)
                {
                    Vector2 pos = Vector2.zero;
                    GridCordEl gridel = new GridCordEl(CellWidth, CellHeight, TypeOfCordEl.NotOccupied, i, j);
                    row.Add(gridel); 
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

                    //GameObject cellGameObject = DrawCell(cellPosition, i, j, ParentForAllCells);
                    //CordMatrix[i][j].GridElGameObject = cellGameObject;


                    CordMatrix[i][j].SetGlobalCenter(cellPosition);
                    Vector3 localPosition = ParentForAllCells.InverseTransformPoint(cellPosition);
                    CordMatrix[i][j].SetCenter(localPosition);
                }
            }
        }
         
        private GameObject DrawCell(Vector3 cellPosition, int i, int j, Transform parent)
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
            return cell;
        }
    }
}
