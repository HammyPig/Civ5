using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDebugger : MonoBehaviour {
    
    public int seed;
    public float scale = 25;
    public int octaves = 4;
    [Range(0, 1)] public float persistence = 0.5f;
    public float lacunarity = 2;
    public Map.BiomeHeight[] biomeHeights;

    private Map map;

    void OnValidate() {
        seed = Mathf.Max(seed, 0);
        scale = Mathf.Max(scale, 0.0001f);
        octaves = Mathf.Max(octaves, 0);
        lacunarity = Mathf.Max(lacunarity, 1);
    }

    public void SetMap(Map map) {
        this.map = map;
    }

    public void GenerateMap() {
        map.GenerateMap(seed, scale, octaves, persistence, lacunarity, biomeHeights);
    }
}
