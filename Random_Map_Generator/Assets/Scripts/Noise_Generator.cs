using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise_Generator
{

    public enum NormalizeMode { 
    Local, Global
    };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight,int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode) { 
    float[,] noiseMap = new float[mapWidth, mapHeight];


        System.Random prng = new System.Random(seed);
        Vector2[] octavesOffsets = new Vector2[octaves];

        float maxPossilbeHeight = 0;

        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossilbeHeight += amplitude;
            amplitude *= persistance;
        }


        if (scale <= 0) {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;


        float halfWidht = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x-halfWidht + octavesOffsets[i].x) / scale * frequency ;
                    float sampleY = (y-halfHeight + octavesOffsets[i].y) / scale * frequency ;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }


                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                if (normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else{
                    float normalizeHeight = (noiseMap[x, y] + 1) / (maxPossilbeHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizeHeight,0,int.MaxValue);
                }
            }
        }

                return noiseMap;
    }
}
