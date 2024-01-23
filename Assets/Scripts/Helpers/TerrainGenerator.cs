using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Helpers
{
    public class HeightMapGenerator
    {
        private readonly string filePath = "Assets/Heightmap.png";
        private readonly int height = 1025;
        private readonly float mountainHeight = 0.1f;

        private readonly int perlinSeed = Random.Range(0, 10000);
        private readonly int width = 1025;

        private float[,] heightMap = null!;

        public float[,] Generate()
        {
            GenerateHeightMap();
            SaveHeightMapToPNG();
            return heightMap;
        }

        private void GenerateHeightMap()
        {
            heightMap = new float[width, height];

            var largeScaleFactor = 0.007f;
            var largeScaleActivationHeight = 0.6f;
            var largeScaleActivationSpeed = 64f;
            var originalNoiseInfluenceFactor = 0.3f;

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var originalNoise = Mathf.PerlinNoise(x * 0.03f + perlinSeed, y * 0.03f + perlinSeed);
                var largeScaleNoise =
                    Mathf.PerlinNoise(x * largeScaleFactor + perlinSeed, y * largeScaleFactor + perlinSeed);

                largeScaleNoise = Mathf.Sqrt(largeScaleActivationSpeed) *
                                  (largeScaleNoise - largeScaleActivationHeight) /
                                  Mathf.Sqrt(1 + largeScaleActivationSpeed *
                                      Mathf.Pow(largeScaleNoise - largeScaleActivationHeight, 2))
                                  + 0.5f;

                var formationSteps = 4;
                for (var i = 1; i <= formationSteps; i++)
                    if (largeScaleNoise <= (float)i / formationSteps)
                        largeScaleNoise /= 1.2f;

                var combinedNoise =
                    Mathf.Max(
                        largeScaleNoise * (1f - originalNoiseInfluenceFactor) +
                        originalNoise * originalNoiseInfluenceFactor - originalNoiseInfluenceFactor / 3, 0);

                combinedNoise *= (Math.Abs(x - (float)width / 2) + Math.Abs(y - (float)height / 2)) /
                                 (0.2f * (height + width));

                heightMap[x, y] = combinedNoise * mountainHeight;
            }

            heightMap = SmoothArray(heightMap);
        }

        private void SaveHeightMapToPNG()
        {
            var texture = new Texture2D(width, height);
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var value = heightMap[x, y] / mountainHeight;
                var color = new Color(value, value, value, 1);
                texture.SetPixel(x, y, color);
            }

            texture.Apply();

            var bytes = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
        }

        private float[,] SmoothArray(float[,] inputArray)
        {
            var arrayWidth = inputArray.GetLength(0);
            var arrayHeight = inputArray.GetLength(1);
            var outputArray = new float[arrayWidth, arrayHeight];

            for (var x = 0; x < arrayWidth; x++)
            for (var y = 0; y < arrayHeight; y++)
            {
                float sum = 0;
                var count = 0;

                // Iterate through neighboring cells including the current cell
                for (var nx = -1; nx <= 1; nx++)
                for (var ny = -1; ny <= 1; ny++)
                {
                    var ix = x + nx;
                    var iy = y + ny;

                    // Check if neighbor is within the bounds of the array
                    if (ix >= 0 && ix < arrayWidth && iy >= 0 && iy < arrayHeight)
                    {
                        sum += inputArray[ix, iy];
                        count++;
                    }
                }

                // Calculate the average value
                outputArray[x, y] = sum / count;
            }

            return outputArray;
        }
    }
}