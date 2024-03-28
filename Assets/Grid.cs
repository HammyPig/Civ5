using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Grid<T> {

    private int width;
    private int height;
    private Vector3 originPosition;
    
    protected T[,] gridArray;
    
    public Grid(int width, int height, Vector3 originPosition) {
        this.width = width;
        this.height = height;
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

    public Vector3 GetOriginPosition() {
        return originPosition;
    }

    public abstract Vector3 GetPosition(int x, int y);

    public abstract Vector3 GetCentrePosition(int x, int y);

    public abstract Vector3[] GetVertexPositions(int x, int y);

    public abstract T GetObject(int x, int y);

    public abstract T GetObject(Vector3 position);
}