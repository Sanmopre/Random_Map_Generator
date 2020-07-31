using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Generator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap};
    public DrawMode drawmode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;

    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public TerrainType[] regions;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public void GenerateMap() {
        float[,] noiseMap = Noise_Generator.GenerateNoiseMap(mapWidth, mapHeight, seed , noiseScale, octaves, persistance, lacunarity, offset);


        Color[] colourMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++) {
                    if (currentHeight <= regions[i].height) {
                        colourMap[y * mapWidth + x] = regions[i].colour;
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
            display.DrawTexture(Texture_Generator.TextureFromColourMap(colourMap,mapWidth,mapHeight));
        }
        
    }


    private void OnValidate()
    {
        if (mapWidth < 1) {
            mapWidth = 1;
        }

        if (mapHeight < 1) {
            mapHeight = 1;
        }

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