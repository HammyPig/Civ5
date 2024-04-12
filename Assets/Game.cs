using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    [SerializeField] private MapVisual mapVisual;
    [SerializeField] private MapDebugger mapDebugger;

    private Map map;
    private GridDebugVisual<Map.Tile> gridDebugVisual;

    private Unit selectedUnit;
    private List<Unit> units;

    void Start() {
        map = new Map(Map.MapSize.Standard);
        mapVisual.SetMap(map);
        mapDebugger.SetMap(map);
        gridDebugVisual = new(map.GetGrid());

        units = new();

        Unit unit1 = new();
        unit1.SetName("Settler");
        unit1.SetMaxMoves(2);
        unit1.RefreshMoves();
        map.GetTile(0, 0).SetUnit(unit1);
        unit1.SetTile(map.GetTile(0, 0));
        map.UpdateTileVisual(0, 0);

        Unit unit2 = new();
        unit2.SetName("Warrior");
        unit2.SetMaxMoves(2);
        unit2.RefreshMoves();
        map.GetTile(1, 0).SetUnit(unit2);
        unit2.SetTile(map.GetTile(1, 0));
        map.UpdateTileVisual(1, 0);

        units.Add(unit1);
        units.Add(unit2);
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
            if (selectedUnit.GetMoves() <= 0) return;
            
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Map.Tile targetTile = map.GetTile(mousePosition);

            if (targetTile == null) return;

            Map.Tile originTile = selectedUnit.GetTile();

            selectedUnit.SetTile(targetTile);
            originTile.SetUnit(null);
            targetTile.SetUnit(selectedUnit);
            selectedUnit.SetMoves(0);

            map.UpdateTileVisual(originTile);
            map.UpdateTileVisual(targetTile);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            foreach (Unit unit in units) {
                unit.RefreshMoves();
            }
        }
    }
}
