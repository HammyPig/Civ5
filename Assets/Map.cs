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

    [System.Serializable] public struct BiomeHeight {
        public Map.Tile.Biome biome;
        public float height;
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

    public void GenerateMap(int seed, float scale, int octaves, float persistence, float lacunarity, BiomeHeight[] biomeHeights) {
        float[,] noiseMap = new float[hexGrid.GetWidth(), hexGrid.GetHeight()];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaveOffsets.Length; i++) {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;
        for (int x = 0; x < hexGrid.GetWidth(); x++) {
            for (int y = 0; y < hexGrid.GetHeight(); y++) {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = x / scale * frequency + octaveOffsets[i].x;
                    float sampleY = y / scale * frequency + octaveOffsets[i].y;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int x = 0; x < hexGrid.GetWidth(); x++) {
            for (int y = 0; y < hexGrid.GetHeight(); y++) {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        for (int x = 0; x < hexGrid.GetWidth(); x++) {
            for (int y = 0; y < hexGrid.GetHeight(); y++) {
                Tile tile = hexGrid.GetObject(x, y);
                
                for (int i = 0; i < biomeHeights.Length; i++) {
                    if (noiseMap[x, y] <= biomeHeights[i].height) {
                        tile.SetBiome(biomeHeights[i].biome);
                        break;
                    }
                }
            }
        }

        hexGrid.OnCellUpdate(new Grid<Tile>.CellUpdateEventArgs { x = 0, y = 0 });
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
