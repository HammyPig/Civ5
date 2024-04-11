using System;
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

    [System.Serializable] public struct ValueThreshold<T> {
        public T value;
        public float threshold;
    }

    [System.Serializable] public struct NoiseMapArgs {
        public float scale;
        public int octaves;
        public float persistence;
        public float lacunarity;
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
        
        NoiseMapArgs seaLevelNoiseMapArgs = new() {
            scale = 25,
            octaves = 4,
            persistence = 0.5f,
            lacunarity = 2
        };

        NoiseMapArgs elevationNoiseMapArgs = new() {
            scale = 3,
            octaves = 4,
            persistence = 0.5f,
            lacunarity = 2
        };

        NoiseMapArgs temperatureNoiseMapArgs = new() {
            scale = 20,
            octaves = 4,
            persistence = 0.5f,
            lacunarity = 2
        };

        NoiseMapArgs rainfallNoiseMapArgs = new() {
            scale = 5,
            octaves = 4,
            persistence = 0.5f,
            lacunarity = 2
        };

        float seaLevelThreshold = 0.6f;

        ValueThreshold<Map.Tile.Terrain.Elevation>[] elevationThresholds = {
            new() { value = Tile.Terrain.Elevation.Flat, threshold = 0.6f },
            new() { value = Tile.Terrain.Elevation.Hill, threshold = 0.8f },
            new() { value = Tile.Terrain.Elevation.Mountain, threshold = 1 }
        };

        ValueThreshold<Map.Tile.Terrain.Temperature>[] temperatureThresholds = {
            new() { value = Tile.Terrain.Temperature.Cold, threshold = 0.2f },
            new() { value = Tile.Terrain.Temperature.Warm, threshold = 0.6f },
            new() { value = Tile.Terrain.Temperature.Hot, threshold = 1 }
        };

        ValueThreshold<Map.Tile.Terrain.Rainfall>[] rainfallThresholds = {
            new() { value = Tile.Terrain.Rainfall.Dry, threshold = 0.4f },
            new() { value = Tile.Terrain.Rainfall.Moderate, threshold = 0.7f },
            new() { value = Tile.Terrain.Rainfall.Wet, threshold = 1 }
        };

        GenerateMap(
            0,
            seaLevelNoiseMapArgs,
            elevationNoiseMapArgs,
            temperatureNoiseMapArgs,
            rainfallNoiseMapArgs,
            seaLevelThreshold,
            elevationThresholds,
            temperatureThresholds,
            rainfallThresholds
        );
    }

    public MapSize GetMapSize() {
        return mapSize;
    }

    public Grid<Tile> GetGrid() {
        return hexGrid;
    }

    public Tile GetTile(int x, int y) {
        return hexGrid.GetObject(x, y);
    }

    public Tile GetTile(Vector3 position) {
        return hexGrid.GetObject(position);
    }

    public void GenerateMap(
        int seed, NoiseMapArgs seaLevelNoiseMapArgs, NoiseMapArgs elevationNoiseMapArgs,
        NoiseMapArgs temperatureNoiseMapArgs, NoiseMapArgs rainfallNoiseMapArgs,
        float seaLevelThreshold, ValueThreshold<Map.Tile.Terrain.Elevation>[] elevationThresholds,
        ValueThreshold<Map.Tile.Terrain.Temperature>[] temperatureThresholds,
        ValueThreshold<Map.Tile.Terrain.Rainfall>[] rainfallThresholds
    ) {
        float[,] seaLevelNoiseMap = GenerateNoiseMap(seed, seaLevelNoiseMapArgs);
        float[,] elevationNoiseMap = GenerateNoiseMap(seed * 2, elevationNoiseMapArgs);
        float[,] temperatureNoiseMap = GenerateNoiseMap(seed * 3, temperatureNoiseMapArgs);
        float[,] rainfallNoiseMap = GenerateNoiseMap(seed * 5, rainfallNoiseMapArgs);

        for (int x = 0; x < hexGrid.GetWidth(); x++) {
            for (int y = 0; y < hexGrid.GetHeight(); y++) {
                Tile tile = hexGrid.GetObject(x, y);
                tile.GetTerrain().SetBiome(Tile.Terrain.Biome.None);
                tile.GetTerrain().SetVegetation(Tile.Terrain.Vegetation.None);
            }
        }

        for (int x = 0; x < hexGrid.GetWidth(); x++) {
            for (int y = 0; y < hexGrid.GetHeight(); y++) {
                Tile tile = hexGrid.GetObject(x, y);
                
                if (seaLevelNoiseMap[x, y] <= seaLevelThreshold) {
                    tile.GetTerrain().SetBiome(Tile.Terrain.Biome.Water);
                }

                if (tile.GetTerrain().GetBiome() == Tile.Terrain.Biome.Water) continue;
                
                Map.Tile.Terrain.Elevation elevation = Tile.Terrain.Elevation.None;
                Map.Tile.Terrain.Temperature temperature = Tile.Terrain.Temperature.None;
                Map.Tile.Terrain.Rainfall rainfall = Tile.Terrain.Rainfall.None;

                for (int i = 0; i < elevationThresholds.Length; i++) {
                    if (elevationNoiseMap[x, y] <= elevationThresholds[i].threshold) {
                        elevation = elevationThresholds[i].value;
                        break;
                    }
                }

                for (int i = 0; i < temperatureThresholds.Length; i++) {
                    if (temperatureNoiseMap[x, y] <= temperatureThresholds[i].threshold) {
                        temperature = temperatureThresholds[i].value;
                        break;
                    }
                }

                for (int i = 0; i < rainfallThresholds.Length; i++) {
                    if (rainfallNoiseMap[x, y] <= rainfallThresholds[i].threshold) {
                        rainfall = rainfallThresholds[i].value;
                        break;
                    }
                }

                tile.GetTerrain().SetBiome(elevation, temperature, rainfall, seed);
                tile.GetTerrain().SetVegetation(temperature, rainfall, seed);
            }
        }

        hexGrid.OnCellUpdate(new Grid<Tile>.CellUpdateEventArgs { x = 0, y = 0 });
    }

    private float[,] GenerateNoiseMap(int seed, NoiseMapArgs noiseMapArgs) {
        float scale = noiseMapArgs.scale;
        int octaves = noiseMapArgs.octaves;
        float persistence = noiseMapArgs.persistence;
        float lacunarity = noiseMapArgs.lacunarity;

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

        return noiseMap;
    }

    public class Tile {

        private Terrain terrain;
        private Unit unit;

        public Tile() {
            terrain = new();
        }

        public override string ToString() {
            return terrain.ToString() + " " + unit?.ToString();
        }

        public Terrain GetTerrain() {
            return terrain;
        }

        public void SetUnit(Unit unit) {
            this.unit = unit;
        }

        public class Terrain {

            public enum SeaLevel {
                Below,
                Above
            }

            public enum Elevation {
                None,
                Flat,
                Hill,
                Mountain
            }

            public enum Temperature {
                None,
                Cold,
                Warm,
                Hot
            }

            public enum Rainfall {
                None,
                Dry,
                Moderate,
                Wet
            }

            //             cold            warm        hot
            // dry         tundra          plains      desert
            // normal      tundra-forest   grassland   plains
            // wet         snow            p/g-forest  grassland-jungle
            public enum Biome {
                None,
                Desert,
                Grassland,
                Plains,
                Snow,
                Tundra,
                Water,
                DesertHill,
                GrasslandHill,
                PlainsHill,
                SnowHill,
                TundraHill,
                DesertMountain,
                GrasslandMountain,
                PlainsMountain,
                SnowMountain,
                TundraMountain
            }

            public enum Vegetation {
                None,
                Forest,
                Jungle
            }

            private Biome biome;
            private Vegetation vegetation;

            public Terrain() {
                biome = Biome.None;
                vegetation = Vegetation.None;
            }

            public override string ToString() {
                return biome.ToString() + " " + vegetation.ToString();
            }

            public Biome GetBiome() {
                return biome;
            }

            public Vegetation GetVegetation() {
                return vegetation;
            }

            public void SetBiome(Biome biome) {
                this.biome = biome;
            }

            public void SetBiome(Elevation elevation, Temperature temperature, Rainfall rainfall, int seed) {
                System.Random prng = new(seed);
                Biome biome = Biome.None;
                
                if (temperature == Temperature.Cold) {
                    if (rainfall == Rainfall.Dry || rainfall == Rainfall.Moderate) biome = Biome.Tundra;
                    else if (rainfall == Rainfall.Wet) biome = Biome.Snow;
                } else if (temperature == Temperature.Warm) {
                    if (rainfall == Rainfall.Dry) biome = Biome.Plains;
                    else if (rainfall == Rainfall.Moderate) biome = Biome.Grassland;
                    else if (rainfall == Rainfall.Wet) {
                        if (prng.Next(1) < 0.5) biome = Biome.Plains;
                        else biome = Biome.Grassland;
                    }
                } else if (temperature == Temperature.Hot) {
                    if (rainfall == Rainfall.Dry) biome = Biome.Desert;
                    else if (rainfall == Rainfall.Moderate) biome = Biome.Plains;
                    else if (rainfall == Rainfall.Wet) biome = Biome.Grassland;
                }

                if (elevation == Elevation.Hill) {
                    biome += 6;
                } else if (elevation == Elevation.Mountain) {
                    biome += 11;
                }

                SetBiome(biome);
            }

            public void SetVegetation(Vegetation vegetation) {
                this.vegetation = vegetation;
            }

            public void SetVegetation(Temperature temperature, Rainfall rainfall, int seed) {
                System.Random prng = new(seed);
                Vegetation vegetation = Vegetation.None;

                if (temperature == Temperature.Cold) {
                    if (rainfall == Rainfall.Moderate) vegetation = Vegetation.Forest;
                } else if (temperature == Temperature.Warm) {
                    if (rainfall == Rainfall.Wet) vegetation = Vegetation.Forest;
                } else if (temperature == Temperature.Hot) {
                    if (rainfall == Rainfall.Wet) vegetation = Vegetation.Jungle;
                }

                SetVegetation(vegetation);
            }
        }
    }   
}
