using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {

    private HexGrid<Tile> hexGrid;

    public Map(int size) {
        hexGrid = new(size, size, Vector3.zero, (Grid<Tile> g, int x, int y) => new Tile(), 5f);
    }

    public Grid<Tile> GetGrid() {
        return hexGrid;
    }

    public Tile GetTile(Vector3 position) {
        return hexGrid.GetObject(position);
    }

    public class Tile {

        public enum Biome {
            Desert,
            Grassland,
            Plains,
            Snow,
            Tundra,
            Water
        }

        private Biome biome = Biome.Plains;

        public override string ToString() {
            return biome.ToString();
        }

        public Biome GetBiome() {
            return biome;
        }

        public void SetBiome(Biome biome) {
            this.biome = biome;
        }
    }
}
