using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Generator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public bool autoUpdate;

    public void GenerateMap() {
        float[,] noiseMap = Noise_Generator.GenerateNoiseMap(mapWidth, mapHeight, noiseScale);


        Map_Display display = FindObjectOfType<Map_Display>();
        display.DrawNoiseMap(noiseMap);
    }

}
