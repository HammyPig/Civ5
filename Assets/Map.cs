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

    public void GenerateMap() {
        float scale = 0.3f;

        for (int x = 0; x < hexGrid.GetWidth(); x++) {
            for (int y = 0; y < hexGrid.GetHeight(); y++) {
                Tile tile = hexGrid.GetObject(x, y);

                float sampleX = x * scale;
                float sampleY = y * scale;
                float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY);

                Debug.Log(perlinNoise + " " + sampleX);

                if (perlinNoise <= 0.5) tile.SetBiome(Tile.Biome.Water);
                else if (perlinNoise <= 0.65) tile.SetBiome(Tile.Biome.Desert);
                else if (perlinNoise <= 1) tile.SetBiome(Tile.Biome.Plains);

                hexGrid.OnCellUpdate(new Grid<Tile>.CellUpdateEventArgs { x = x, y = y });
            }
        }
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
