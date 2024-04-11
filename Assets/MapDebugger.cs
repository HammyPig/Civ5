using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDebugger : MonoBehaviour {
    
    public int seed;
    
    public Map.NoiseMapArgs seaLevelNoiseMapArgs;
    public Map.NoiseMapArgs elevationNoiseMapArgs;
    public Map.NoiseMapArgs temperatureNoiseMapArgs;
    public Map.NoiseMapArgs rainfallNoiseMapArgs;

    public float seaLevelThreshold;
    public Map.ValueThreshold<Map.Tile.Terrain.Elevation>[] elevationThresholds;
    public Map.ValueThreshold<Map.Tile.Terrain.Temperature>[] temperatureThresholds;
    public Map.ValueThreshold<Map.Tile.Terrain.Rainfall>[] rainfallThresholds;

    private Map map;

    void OnValidate() {
        seed = Mathf.Max(seed, 0);

        Map.NoiseMapArgs[] noiseMapArgs = {
            seaLevelNoiseMapArgs,
            elevationNoiseMapArgs,
            temperatureNoiseMapArgs,
            rainfallNoiseMapArgs
        };

        for (int i = 0; i < noiseMapArgs.Length; i++) {
            noiseMapArgs[i].scale = Mathf.Max(noiseMapArgs[i].scale, 0.0001f);
            noiseMapArgs[i].octaves = Mathf.Max(noiseMapArgs[i].octaves, 0);
            noiseMapArgs[i].persistence = Mathf.Clamp(noiseMapArgs[i].persistence, 0, 1);
            noiseMapArgs[i].lacunarity = Mathf.Max(noiseMapArgs[i].lacunarity, 1);
        }
    }

    public void SetMap(Map map) {
        this.map = map;
    }

    public void GenerateMap() {
        map.GenerateMap(seed, seaLevelNoiseMapArgs, elevationNoiseMapArgs, temperatureNoiseMapArgs, rainfallNoiseMapArgs, seaLevelThreshold, elevationThresholds, temperatureThresholds, rainfallThresholds);
    }
}
