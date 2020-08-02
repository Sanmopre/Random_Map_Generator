using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class Map_Generator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap, Mesh};
    public DrawMode drawmode;

    public Noise_Generator.NormalizeMode normalizeMode;

    public float noiseScale;

    public int octaves;

    public const int mapChunkSize = 241;

    [Range(0,6)]
    public int editorPreviewLOD;

    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public TerrainType[] regions;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public bool autoUpdate;


    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public void DrawMapInEditor() {

        MapData mapData = GenerateMapData(Vector2.zero);

        Map_Display display = FindObjectOfType<Map_Display>();
        if (drawmode == DrawMode.NoiseMap)
        {
            display.DrawTexture(Texture_Generator.TextureFromHeigthMap(mapData.heightMap));
        }
        else if (drawmode == DrawMode.ColourMap)
        {
            display.DrawTexture(Texture_Generator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawmode == DrawMode.Mesh)
        {
            display.DrawMesh(Mesh_Generator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD), Texture_Generator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
    }


    public void RequestMapData(Vector2 centre, Action<MapData> callback) {
        ThreadStart threadStart = delegate
        {
            MapaDataThread(centre, callback);
        };

        new Thread(threadStart).Start();

    }

    void MapaDataThread(Vector2 centre, Action<MapData> callback) {
        MapData mapData = GenerateMapData(centre);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
        }


    public void RequestMeshData(MapData mapData,int lod, Action<MeshData> callback) {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData,lod, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback) {
        MeshData meshData = Mesh_Generator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }


    void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0) {

            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }

        }


        if (meshDataThreadInfoQueue.Count > 0)
        {

            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }

        }

    }

    MapData GenerateMapData(Vector2 centre) {
        float[,] noiseMap = Noise_Generator.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed , noiseScale, octaves, persistance, lacunarity,centre + offset, normalizeMode);


        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++) {
            for (int x = 0; x < mapChunkSize; x++) {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++) {
                    if (currentHeight >= regions[i].height) {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                    }
                    else {
                        break;
                    }
                }

            }
        }

        return new MapData(noiseMap, colourMap);
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

    struct MapThreadInfo<T> {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter) {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color colour;
}


public struct MapData {
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap) 
    {
    this.heightMap = heightMap;
    this.colourMap = colourMap;
    }

}