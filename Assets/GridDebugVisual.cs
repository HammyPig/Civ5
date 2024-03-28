using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDebugVisual<T> {

    private Grid<T> grid;
    private TextMesh[,] debugTextArray;

    public GridDebugVisual(Grid<T> grid) {
        this.grid = grid;

        debugTextArray = new TextMesh[grid.GetWidth(), grid.GetHeight()];
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                GameObject gameObject = new("World_Text", typeof(TextMesh));

                Transform transform = gameObject.transform;
                transform.localPosition = grid.GetWorldPosition(x, y) + new Vector3(grid.GetCellSize(), grid.GetCellSize()) * 0.5f;

                TextMesh textMesh = gameObject.GetComponent<TextMesh>();
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.text = grid.GetObject(x, y)?.ToString();

                debugTextArray[x, y] = textMesh;

                Debug.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(grid.GetWorldPosition(x, y + 1), grid.GetWorldPosition(x + 1, y + 1), Color.white, 100f);
                Debug.DrawLine(grid.GetWorldPosition(x + 1, y + 1), grid.GetWorldPosition(x + 1, y), Color.white, 100f);
                Debug.DrawLine(grid.GetWorldPosition(x + 1, y), grid.GetWorldPosition(x, y), Color.white, 100f);
            }
        }
    }
}
