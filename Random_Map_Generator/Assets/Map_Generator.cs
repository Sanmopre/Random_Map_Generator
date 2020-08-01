using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Generator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap, Mesh};
    public DrawMode drawmode;


    public float noiseScale;

    public int octaves;

    const int mapChunkSize = 241;

    [Range(0,6)]
    public int levelOfDetail;

    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public TerrainType[] regions;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public bool autoUpdate;

    public void GenerateMap() {
        float[,] noiseMap = Noise_Generator.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed , noiseScale, octaves, persistance, lacunarity, offset);


        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++) {
            for (int x = 0; x < mapChunkSize; x++) {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++) {
                    if (currentHeight <= regions[i].height) {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                        break;
                    }

                }

            }
        }

        Map_Display display = FindObjectOfType<Map_Display>();

        if (drawmode == DrawMode.NoiseMap)
        {
            display.DrawTexture(Texture_Generator.TextureFromHeigthMap(noiseMap));
        }
        else if (drawmode == DrawMode.ColourMap) {
            display.DrawTexture(Texture_Generator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        } else if (drawmode == DrawMode.Mesh) {
            display.DrawMesh(Mesh_Generator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), Texture_Generator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
        
    }


    private void OnValidate()
    {

        if (lacunarity < 1) {
            lacunarity = 1;
        }

        if (octaves < 0) {
            octaves = 0;
        }

        

    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color colour;
}