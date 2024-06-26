using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapVisual : MonoBehaviour {

    [System.Serializable] public struct SpritePosition<T> {
        public T sprite;
        public Vector2Int position;
    }
    
    [SerializeField] private SpritePosition<Map.Tile.Terrain.Biome>[] biomeSpritePositions;
    [SerializeField] private SpritePosition<Map.Tile.Terrain.Vegetation>[] vegetationSpritePositions;

    private Dictionary<Map.Tile.Terrain.Biome, Vector2> biomeSpriteCentrePosition;
    private Dictionary<Map.Tile.Terrain.Vegetation, Vector2> vegetationSpriteCentrePosition;
    private Mesh mesh;
    private Map map;
    private bool updateMesh = false;

    private void Awake() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
        float textureWidth = texture.width;
        float textureHeight = texture.height;
        float textureCellWidth = 16;
        float textureCellHeight = 16;

        biomeSpriteCentrePosition = new();
        foreach (SpritePosition<Map.Tile.Terrain.Biome> biomeSpritePosition in biomeSpritePositions) {
            biomeSpriteCentrePosition[biomeSpritePosition.sprite] = new Vector2(
                (biomeSpritePosition.position.x * textureCellWidth + 1) / textureWidth,
                (textureHeight / textureCellHeight - 1 - biomeSpritePosition.position.y) * textureCellHeight / textureHeight
            );
        }

        vegetationSpriteCentrePosition = new();
        foreach (SpritePosition<Map.Tile.Terrain.Vegetation> vegetationSpritePosition in vegetationSpritePositions) {
            vegetationSpriteCentrePosition[vegetationSpritePosition.sprite] = new Vector2(
                (vegetationSpritePosition.position.x * textureCellWidth + 1) / textureWidth,
                (textureHeight / textureCellHeight - 1 - vegetationSpritePosition.position.y) * textureCellHeight / textureHeight
            );
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

                Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
                int textureWidth = texture.width;
                int textureHeight = texture.height;
                int textureCellWidth = 16;
                int textureCellHeight = 16;
                float circumradius = textureCellHeight / 2;
                float inradius = Mathf.Sqrt(3) / 2 * circumradius;
                
                if (tile.GetTerrain().GetVegetation() != Map.Tile.Terrain.Vegetation.None) {
                    uv[vertexIndexOffset] = vegetationSpriteCentrePosition[tile.GetTerrain().GetVegetation()] + (new Vector2(0, circumradius / 2f) / textureWidth);
                    uv[vertexIndexOffset + 1] = vegetationSpriteCentrePosition[tile.GetTerrain().GetVegetation()] + (new Vector2(0, circumradius * 1.5f) / textureWidth);
                    uv[vertexIndexOffset + 2] = vegetationSpriteCentrePosition[tile.GetTerrain().GetVegetation()] + (new Vector2(inradius, circumradius * 2f) / textureWidth);
                    uv[vertexIndexOffset + 3] = vegetationSpriteCentrePosition[tile.GetTerrain().GetVegetation()] + (new Vector2(inradius * 2f, circumradius * 1.5f) / textureWidth);
                    uv[vertexIndexOffset + 4] = vegetationSpriteCentrePosition[tile.GetTerrain().GetVegetation()] + (new Vector2(inradius * 2f, circumradius / 2f) / textureWidth);
                    uv[vertexIndexOffset + 5] = vegetationSpriteCentrePosition[tile.GetTerrain().GetVegetation()] + (new Vector2(inradius, 0) / textureWidth);
                } else {
                    uv[vertexIndexOffset] = biomeSpriteCentrePosition[tile.GetTerrain().GetBiome()] + (new Vector2(0, circumradius / 2f) / textureWidth);
                    uv[vertexIndexOffset + 1] = biomeSpriteCentrePosition[tile.GetTerrain().GetBiome()] + (new Vector2(0, circumradius * 1.5f) / textureWidth);
                    uv[vertexIndexOffset + 2] = biomeSpriteCentrePosition[tile.GetTerrain().GetBiome()] + (new Vector2(inradius, circumradius * 2f) / textureWidth);
                    uv[vertexIndexOffset + 3] = biomeSpriteCentrePosition[tile.GetTerrain().GetBiome()] + (new Vector2(inradius * 2f, circumradius * 1.5f) / textureWidth);
                    uv[vertexIndexOffset + 4] = biomeSpriteCentrePosition[tile.GetTerrain().GetBiome()] + (new Vector2(inradius * 2f, circumradius / 2f) / textureWidth);
                    uv[vertexIndexOffset + 5] = biomeSpriteCentrePosition[tile.GetTerrain().GetBiome()] + (new Vector2(inradius, 0) / textureWidth);
                }
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
    }
}
