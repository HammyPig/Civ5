using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid<T> : Grid<T> {

    private float circumradius;
    private float inradius;
    
    public HexGrid(int width, int height, Vector3 originPosition, float cellSize) : base(width, height, originPosition) {
        circumradius = cellSize / 2f;
        inradius = Mathf.Sqrt(3) / 2 * circumradius;
    }

    public override void GetXY(Vector3 position, out int x, out int y) {
        Vector3 relativePosition = position - base.GetOriginPosition();
        int chunkX = Mathf.FloorToInt(relativePosition.x / (inradius * 2f));
        int chunkY = Mathf.FloorToInt(relativePosition.y / (circumradius * 1.5f));
        bool evenChunkRow = (chunkY % 2) == 0;

        int[,] objectXYsInChunk = {
            {chunkX, chunkY},
            {chunkX, chunkY - 1},
            {chunkX - 1, chunkY - (evenChunkRow ? 1 : 0)}
        };

        Vector3 closestObject = Vector3.positiveInfinity;
        x = -1;
        y = -1;

        for (int i = 0; i < objectXYsInChunk.GetLength(0); i++) {
            int objectX = objectXYsInChunk[i, 0];
            int objectY = objectXYsInChunk[i, 1];

            if (objectX < 0 || objectY < 0 || objectX >= base.GetWidth() || objectY >= base.GetHeight()) continue;

            Vector3 objectCentre = GetCentrePosition(objectX, objectY);

            if (Vector3.Distance(position, objectCentre) < Vector3.Distance(position, closestObject)) {
                closestObject = objectCentre;
                x = objectX;
                y = objectY;
            }
        }
    }

    public override Vector3 GetPosition(int x, int y) {
        float xOffset = 0;
        if (y % 2 == 1) xOffset = inradius;

        return new Vector3(x * inradius * 2f + xOffset, y * circumradius * 1.5f) + base.GetOriginPosition();
    }

    public override Vector3 GetCentrePosition(int x, int y) {
        return GetPosition(x, y) + new Vector3(inradius, circumradius);
    }

    public override Vector3[] GetVertexPositions(int x, int y) {
        Vector3[] vertexPositions = {
            GetPosition(x, y) + new Vector3(0, circumradius / 2f),
            GetPosition(x, y) + new Vector3(0, circumradius * 1.5f),
            GetPosition(x, y) + new Vector3(inradius, circumradius * 2f),
            GetPosition(x, y) + new Vector3(inradius * 2f, circumradius * 1.5f),
            GetPosition(x, y) + new Vector3(inradius * 2f, circumradius / 2f),
            GetPosition(x, y) + new Vector3(inradius, 0)
        };

        return vertexPositions;
    }

    public override T GetObject(int x, int y) {
        if (x < 0 || x >= base.GetWidth() || y < 0 || y >= base.GetHeight()) return default(T);

        return base.gridArray[x, y];
    }

    public override T GetObject(Vector3 position) {
        GetXY(position, out int x, out int y);
        return GetObject(x, y);
    }

    public override void SetObject(int x, int y, T value) {
        if (x < 0 || x >= base.GetWidth() || y < 0 || y >= base.GetHeight()) return;

        base.gridArray[x, y] = value;
        base.OnCellUpdate(new Grid<T>.CellUpdateEventArgs { x = x, y = y });
    }

    public override void SetObject(Vector3 position, T value) {
        GetXY(position, out int x, out int y);
        SetObject(x, y, value);
    }
}
