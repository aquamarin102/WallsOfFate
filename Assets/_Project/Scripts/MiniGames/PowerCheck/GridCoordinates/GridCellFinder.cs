using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MiniGames.PowerCheck.GridCoordinates
{
    public class GridCellFinder
    {
        private GridCordinates gridCordinates;

        public GridCellFinder(GridCordinates gridCordinates)
        {
            this.gridCordinates = gridCordinates;
        }

        public HashSet<GridCordEl> GetAdjacentCells(Vector2 pointPosition, float width, float height, int row, int col)
        {
            HashSet<GridCordEl> adjacentCells = new HashSet<GridCordEl>();

            GridCordEl currentCell = gridCordinates.CordMatrix[row][col];
            adjacentCells.Add(currentCell);
            int delCellsWidth = Mathf.FloorToInt(width * 2 / currentCell.Width) + 1;
            int delCellsHeight = Mathf.FloorToInt(height * 2 / currentCell.Height) + 1;

            for (int delRow = -1 * delCellsWidth; delRow <= delCellsWidth; delRow++)
            {
                int nearRow = row + delRow;
                if (nearRow < 0 || nearRow >= gridCordinates.CordMatrix.Count) continue;

                for (int delCol = -1 * delCellsHeight; delCol <= delCellsHeight; delCol++)
                {
                    int nearCol = col + delCol;
                    if (nearCol < 0 || nearCol >= gridCordinates.CordMatrix[nearRow].Count || (nearRow == row && nearCol == col)) continue;

                    for (float delWidth = -1; delWidth <= 1; delWidth += 0.1f)
                    {
                        for (float delHeight = -1; delHeight <= 1; delHeight += 0.1f)
                        {
                            float nearWidth = delWidth * width;
                            float nearHeight = delHeight * height;
                            Vector2 newBorderPoint = new Vector2(pointPosition.x + nearWidth, pointPosition.y + nearHeight);

                            if (IfPointInCell(newBorderPoint, nearRow, nearCol, true) == 1)
                            {
                                adjacentCells.Add(gridCordinates.CordMatrix[nearRow][nearCol]);
                            }
                        }
                    }
                }
            }

            return adjacentCells;
        }

        public GridCordEl FindCellsByPosition(Vector2 position2d)
        {
            //Vector2 position2d = new Vector2(position.x, position.z);

            int maxRow = Mathf.RoundToInt(Mathf.Log(gridCordinates.CordMatrix.Count) / Mathf.Log(2));

            int rowIndex = gridCordinates.CordMatrix.Count / 2, ifincell = -1;
            int colIndex = 0;
            BinaryCycle(position2d, gridCordinates.CordMatrix.Count, ref rowIndex, ref colIndex, ref ifincell, false);

            if (ifincell == 1) return gridCordinates.CordMatrix[rowIndex][colIndex];
            else if (ifincell == 0)
            {
                colIndex = gridCordinates.CordMatrix[rowIndex].Count / 2;
                int maxCol = Mathf.RoundToInt(Mathf.Log(gridCordinates.CordMatrix[rowIndex].Count) / Mathf.Log(2));
                BinaryCycle(position2d, gridCordinates.CordMatrix[rowIndex].Count, ref colIndex, ref rowIndex, ref ifincell, true);
                if (ifincell == 1) return gridCordinates.CordMatrix[rowIndex][colIndex];
            }

            return null;
        }

        public void BinaryCycle(Vector2 position2d, int limit, ref int index, ref int secondaryIndex,
            ref int ifInCell, bool searchByX)
        {
            if (ifInCell == 0 || ifInCell == 1) ifInCell = -1;
            int prevIndex = 0, numOfSteps = 0;
            while (index >= 0 && index < limit /*numOfSteps <= limit + 1*/)
            {
                if (ifInCell == 0 || ifInCell == 1) break;

                int step = Mathf.RoundToInt(Mathf.Abs(index - prevIndex) / 2);
                if (step == 0) step = 1;
                prevIndex = index;
                if (searchByX)
                {
                    float comparisonValue = gridCordinates.CordMatrix[secondaryIndex][index].Center.x;
                    index += comparisonValue < position2d.x ? step : -step;
                    ifInCell = IfPointInCell(position2d, secondaryIndex, index, searchByX);
                }
                else
                {
                    float comparisonValue = gridCordinates.CordMatrix[index][secondaryIndex].Center.y;
                    index += comparisonValue < position2d.y ? step : -step;
                    ifInCell = IfPointInCell(position2d, index, secondaryIndex, searchByX);
                }
                numOfSteps++;
            }
        }

        private int IfPointInCell(Vector2 pointPosition, int rowIndex, int colIndex, bool findRow)
        {
            if (IsOutOfBounds(rowIndex, colIndex)) return -1;

            float leftBorder = gridCordinates.CordMatrix[rowIndex][colIndex].Center.x - gridCordinates.CordMatrix[rowIndex][colIndex].Width / 2;
            float rightBorder = gridCordinates.CordMatrix[rowIndex][colIndex].Center.x + gridCordinates.CordMatrix[rowIndex][colIndex].Width / 2;
            float bottomBorder = gridCordinates.CordMatrix[rowIndex][colIndex].Center.y - gridCordinates.CordMatrix[rowIndex][colIndex].Height / 2;
            float topBorder = gridCordinates.CordMatrix[rowIndex][colIndex].Center.y + gridCordinates.CordMatrix[rowIndex][colIndex].Height / 2;

            bool xInRange = leftBorder <= pointPosition.x && pointPosition.x <= rightBorder;
            bool yInRange = bottomBorder <= pointPosition.y && pointPosition.y <= topBorder;

            if (xInRange && yInRange && findRow) return 1;
            if ((xInRange || yInRange) && !findRow) return 0;

            return -1;
        }

        private bool IsOutOfBounds(int row, int col)
        {
            return row < 0 || row >= gridCordinates.CordMatrix.Count || col < 0 || col >= gridCordinates.CordMatrix[row].Count;
        }
    }
}
