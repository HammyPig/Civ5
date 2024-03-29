using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Grid<T> {

    public event EventHandler<CellUpdateEventArgs> CellUpdate;
    public class CellUpdateEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private Vector3 originPosition;
    
    protected T[,] gridArray;
    
    public Grid(int width, int height, Vector3 originPosition, Func<Grid<T>, int, int, T> newGridObject) {
        this.width = width;
        this.height = height;
        this.originPosition = originPosition;

        gridArray = new T[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = newGridObject(this, x, y);
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

    public virtual void OnCellUpdate(CellUpdateEventArgs e) {
        CellUpdate?.Invoke(this, e);
    }

    public abstract void GetXY(Vector3 position, out int x, out int y);

    public abstract Vector3 GetPosition(int x, int y);

    public abstract Vector3 GetCentrePosition(int x, int y);

    public abstract Vector3[] GetVertexPositions(int x, int y);

    public abstract T GetObject(int x, int y);

    public abstract T GetObject(Vector3 position);

    public abstract void SetObject(int x, int y, T value);

    public abstract void SetObject(Vector3 position, T value);
}