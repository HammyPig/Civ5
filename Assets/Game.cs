using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    [SerializeField] private MapVisual mapVisual;
    [SerializeField] private MapDebugger mapDebugger;

    private Map map;

    void Start() {
        map = new Map(Map.MapSize.Standard);
        mapVisual.SetMap(map);
        mapDebugger.SetMap(map);

        Unit unit = new();
        unit.SetName("Worker");
        map.GetTile(0, 0).SetUnit(unit);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(map.GetTile(mousePosition));
        }
    }
}
