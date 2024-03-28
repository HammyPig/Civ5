using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDebugVisual<T> {

    private Grid<T> grid;
    private TextMesh[,] debugTextArray;

    public GridDebugVisual(Grid<T> grid) {
        this.grid = grid;

        grid.CellUpdate += (object sender, Grid<T>.CellUpdateEventArgs eventArgs) => UpdateDebugTextCell(eventArgs.x, eventArgs.y);

        debugTextArray = new TextMesh[grid.GetWidth(), grid.GetHeight()];
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                GameObject gameObject = new("World_Text", typeof(TextMesh));

                Transform transform = gameObject.transform;
                transform.localPosition = grid.GetCentrePosition(x, y);

                TextMesh textMesh = gameObject.GetComponent<TextMesh>();
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.text = grid.GetObject(x, y)?.ToString();

                debugTextArray[x, y] = textMesh;

                Vector3[] vertexPositions = grid.GetVertexPositions(x, y);
                DrawShape(vertexPositions);
            }
        }
    }

    private void DrawShape(Vector3[] vertexPositions) {
        for (int i = 0; i < vertexPositions.Length; i++) {
            Debug.DrawLine(vertexPositions[i], vertexPositions[(i + 1) % vertexPositions.Length], Color.white, 100f);
        }
    }

    private void UpdateDebugTextCell(int x, int y) {
        debugTextArray[x, y].text = grid.GetObject(x, y)?.ToString();
    }
}
