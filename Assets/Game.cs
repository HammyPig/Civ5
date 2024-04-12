using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    [SerializeField] private MapVisual mapVisual;
    [SerializeField] private MapDebugger mapDebugger;

    private Map map;
    private GridDebugVisual<Map.Tile> gridDebugVisual;

    private Unit selectedUnit;

    void Start() {
        map = new Map(Map.MapSize.Standard);
        mapVisual.SetMap(map);
        mapDebugger.SetMap(map);
        gridDebugVisual = new(map.GetGrid());

        Unit unit = new();
        unit.SetName("Settler");
        map.GetTile(0, 0).SetUnit(unit);
        unit.SetTile(map.GetTile(0, 0));
        map.UpdateTileVisual(0, 0);

        Unit unit2 = new();
        unit2.SetName("Warrior");
        map.GetTile(1, 0).SetUnit(unit2);
        unit2.SetTile(map.GetTile(1, 0));
        map.UpdateTileVisual(1, 0);
    }

    void Update() {
        if (Input.GetMouseButtonUp(0)) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Map.Tile tile = map.GetTile(mousePosition);

            if (tile == null) return;

            Unit unit = tile.GetUnit();

            if (unit == null || unit == selectedUnit) return;

            selectedUnit = unit;

            Debug.Log("Selected: " + selectedUnit);
        }

        if (Input.GetMouseButtonDown(1)) {
            if (selectedUnit == null) return;
            
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Map.Tile targetTile = map.GetTile(mousePosition);

            if (targetTile == null) return;

            Map.Tile originTile = selectedUnit.GetTile();

            selectedUnit.SetTile(targetTile);
            originTile.SetUnit(null);
            targetTile.SetUnit(selectedUnit);

            map.UpdateTileVisual(originTile);
            map.UpdateTileVisual(targetTile);
        }
    }
}
