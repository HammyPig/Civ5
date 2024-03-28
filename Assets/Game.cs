using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    private Grid<int> grid;
    private GridDebugVisual<int> gridDebugVisual;

    void Start() {
        grid = new HexGrid<int>(5, 3, Vector3.zero, 5f);
        gridDebugVisual = new GridDebugVisual<int>(grid);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            grid.SetObject(mousePosition, grid.GetObject(mousePosition) + 1);
        }

        if (Input.GetMouseButtonDown(1)) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(grid.GetObject(mousePosition));
        }
    }
}
