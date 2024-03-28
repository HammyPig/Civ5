using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<T> {

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;

    private T[,] gridArray;
    
    public Grid(int width, int height, float cellSize, Vector3 originPosition) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new T[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = default(T);
            }
        }
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public T GetObject(int x, int y) {
        if (x < 0 || x >= width || y < 0 || y >= height) return default(T);

        return gridArray[x, y];
    }

    public T GetObject(Vector3 worldPosition) {
        GetXY(worldPosition, out int x, out int y);

        return GetObject(x, y);
    }
}