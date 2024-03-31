using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {

    public enum MapSize {
        Duel,
        Tiny,
        Small,
        Standard,
        Large,
        Huge
    }

    // based on https://civilization.fandom.com/wiki/Map_(Civ5)
    Dictionary<MapSize, Vector2Int> mapSizeWidthHeight = new() {
        {MapSize.Duel, new Vector2Int(40, 24)},
        {MapSize.Tiny, new Vector2Int(56, 36)},
        {MapSize.Small, new Vector2Int(66, 42)},
        {MapSize.Standard, new Vector2Int(80, 52)},
        {MapSize.Large, new Vector2Int(104, 64)},
        {MapSize.Huge, new Vector2Int(128, 80)}
    };

    private readonly MapSize mapSize;
    private HexGrid<Tile> hexGrid;

    public Map(MapSize mapSize) {
        this.mapSize = mapSize;
        int mapSizeWidth = mapSizeWidthHeight[mapSize].x;
        int mapSizeHeight = mapSizeWidthHeight[mapSize].y;
        hexGrid = new(mapSizeWidth, mapSizeHeight, Vector3.zero, (Grid<Tile> g, int x, int y) => new Tile(), 5f);
    }

    public MapSize GetMapSize() {
        return mapSize;
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
