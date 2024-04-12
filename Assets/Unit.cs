using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {

    private Map.Tile tile;
    private string name;

    public override string ToString() {
        return name;
    }

    public Map.Tile GetTile() {
        return tile;
    }

    public string GetName() {
        return name;
    }

    public void SetTile(Map.Tile tile) {
        this.tile = tile;
    }

    public void SetName(string name) {
        this.name = name;
    }
}
