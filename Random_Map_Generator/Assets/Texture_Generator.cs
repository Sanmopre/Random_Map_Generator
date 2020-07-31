using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Texture_Generator
{
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height) {
        Texture2D texture = new Texture2D(width, height);

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }



    public static Texture2D TextureFromHeigthMap(float[,] heigthMap) {
        int width = heigthMap.GetLength(0);
        int height = heigthMap.GetLength(1);


        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heigthMap[x, y]);
            }
        }
        return TextureFromColourMap(colorMap, width, height);
    }


}
