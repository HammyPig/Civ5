using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareGrid<T> : Grid<T> {

    private float cellSize;
    
    public SquareGrid(int width, int height, Vector3 originPosition, float cellSize) : base(width, height, originPosition) {
        this.cellSize = cellSize;
    }

    public override Vector3 GetPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + base.GetOriginPosition();
    }

    public override Vector3 GetCentrePosition(int x, int y) {
        return GetPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f;
    }

    public override Vector3[] GetVertexPositions(int x, int y) {
        Vector3[] vertexPositions = {
            GetPosition(x, y) + new Vector3(0, 0, 0),
            GetPosition(x, y) + new Vector3(0, cellSize, 0),
            GetPosition(x, y) + new Vector3(cellSize, cellSize, 0),
            GetPosition(x, y) + new Vector3(cellSize, 0, 0)
        };

        return vertexPositions;
    }

    public override T GetObject(int x, int y) {
        if (x < 0 || x >= base.GetWidth() || y < 0 || y >= base.GetHeight()) return default(T);

        return base.gridArray[x, y];
    }

    public override T GetObject(Vector3 Position) {
        int x = Mathf.FloorToInt((Position - base.GetOriginPosition()).x / cellSize);
        int y = Mathf.FloorToInt((Position - base.GetOriginPosition()).y / cellSize);

        return GetObject(x, y);
    }
}
