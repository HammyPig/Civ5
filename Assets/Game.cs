using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    private Grid<int> grid;
    private GridDebugVisual<int> gridDebugVisual;

    void Start() {
        grid = new Grid<int>(5, 3, 5f, Vector3.zero);
        gridDebugVisual = new GridDebugVisual<int>(grid);
    }
}
