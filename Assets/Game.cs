using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    private Grid<int> grid;
    private GridDebugVisual<int> gridDebugVisual;

    void Start() {
        grid = new SquareGrid<int>(5, 3, Vector3.zero, 5f);
        gridDebugVisual = new GridDebugVisual<int>(grid);
    }
}
