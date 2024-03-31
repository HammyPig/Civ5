using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapVisual : MonoBehaviour {

    [System.Serializable] public struct TerrainSpritePixel {
        public Map.Tile.Terrain terrain;
        public Vector2Int uv00;
        public Vector2Int uv11;
    }

    [System.Serializable] public struct VegetationSpritePixel {
        public Map.Tile.Vegetation vegetation;
        public Vector2Int uv00;
        public Vector2Int uv11;
    }

    private struct UVCoords {
        public Vector2 uv00;
        public Vector2 uv11;
    }
    
    [SerializeField] private TerrainSpritePixel[] terrainSpritePixels;
    [SerializeField] private VegetationSpritePixel[] vegetationSpritePixels;

    private Dictionary<Map.Tile.Terrain, UVCoords> terrainUVs;
    private Dictionary<Map.Tile.Vegetation, UVCoords> vegetationUVs;
    private Mesh mesh;
    private Map map;
    private bool updateMesh = false;

    private void Awake() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
        int textureWidth = texture.width;
        int textureHeight = texture.height;

        terrainUVs = new();
        foreach (TerrainSpritePixel terrainSpritePixel in terrainSpritePixels) {
            terrainUVs[terrainSpritePixel.terrain] = new UVCoords {
                uv00 = new Vector2((float) terrainSpritePixel.uv00.x / textureWidth, (float) terrainSpritePixel.uv00.y / textureHeight),
                uv11 = new Vector2((float) terrainSpritePixel.uv11.x / textureWidth, (float) terrainSpritePixel.uv11.y / textureHeight)
            };
        }

        vegetationUVs = new();
        foreach (VegetationSpritePixel vegetationSpritePixel in vegetationSpritePixels) {
            vegetationUVs[vegetationSpritePixel.vegetation] = new UVCoords {
                uv00 = new Vector2((float) vegetationSpritePixel.uv00.x / textureWidth, (float) vegetationSpritePixel.uv00.y / textureHeight),
                uv11 = new Vector2((float) vegetationSpritePixel.uv11.x / textureWidth, (float) vegetationSpritePixel.uv11.y / textureHeight)
            };
        }
    }

    private void LateUpdate() {
        if (updateMesh) {
            updateMesh = false;
            UpdateVisual();
        }
    }

    public void MapUpdate(object sender, Grid<Map.Tile>.CellUpdateEventArgs e) {
        updateMesh = true;
    }

    public void SetMap(Map map) {
        this.map = map;
        updateMesh = true;

        map.GetGrid().CellUpdate += MapUpdate;
    }

    private void UpdateVisual() {
        Grid<Map.Tile> grid = map.GetGrid();

        Vector3[] vertices = new Vector3[grid.GetWidth() * grid.GetHeight() * 6];
        int[] triangles = new int[grid.GetWidth() * grid.GetHeight() * 12];
        Vector2[] uv = new Vector2[grid.GetWidth() * grid.GetHeight() * 6];

        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                Map.Tile tile = grid.GetObject(x, y);
                int cellIndex = x * grid.GetHeight() + y;
                int vertexIndexOffset = cellIndex * 6;
                int triangleIndexOffset = cellIndex * 12;

                Vector3[] cellVertices = grid.GetVertexPositions(x, y);
                cellVertices.CopyTo(vertices, vertexIndexOffset);

                int[] cellTriangles = {
                    vertexIndexOffset + 0, vertexIndexOffset + 1, vertexIndexOffset + 2,
                    vertexIndexOffset + 2, vertexIndexOffset + 3, vertexIndexOffset + 4,
                    vertexIndexOffset + 4, vertexIndexOffset + 5, vertexIndexOffset + 0,
                    vertexIndexOffset + 0, vertexIndexOffset + 2, vertexIndexOffset + 4
                };
                cellTriangles.CopyTo(triangles, triangleIndexOffset);
                
                UVCoords uvCoords;

                if (tile.GetVegetation() != Map.Tile.Vegetation.None) {
                    uvCoords = vegetationUVs[tile.GetVegetation()];
                } else {
                    uvCoords = terrainUVs[tile.GetTerrain()];
                }
                
                uv[vertexIndexOffset] = uvCoords.uv00;
                uv[vertexIndexOffset + 1] = uvCoords.uv00;
                uv[vertexIndexOffset + 2] = uvCoords.uv00;
                uv[vertexIndexOffset + 3] = uvCoords.uv11;
                uv[vertexIndexOffset + 4] = uvCoords.uv11;
                uv[vertexIndexOffset + 5] = uvCoords.uv11;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
    }
}
