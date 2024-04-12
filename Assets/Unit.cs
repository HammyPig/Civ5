using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {

    private Map.Tile tile;
    private string name;
    private int moves;
    private int maxMoves;

    public override string ToString() {
        return name;
    }

    public Map.Tile GetTile() {
        return tile;
    }

    public string GetName() {
        return name;
    }

    public int GetMoves() {
        return moves;
    }

    public int GetMaxMoves() {
        return maxMoves;
    }

    public void SetTile(Map.Tile tile) {
        this.tile = tile;
    }

    public void SetName(string name) {
        this.name = name;
    }

    public void SetMoves(int moves) {
        this.moves = moves;
    }

    public void SetMaxMoves(int maxMoves) {
        this.maxMoves = maxMoves;
    }

    public void RefreshMoves() {
        moves = maxMoves;
    }
}
