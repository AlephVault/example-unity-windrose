using System;
using UnityEngine;

namespace Support
{
    namespace Utils
    {
        public class Textures
        {
            public static void Flip(Color32[] pixels, int width, int height)
            {
                int iterations = height / 2;
                Color32[] buffer = new Color32[width];
                for (int row = 0; row < iterations; row++)
                {
                    int index = row * width;
                    int reflectedIndex = (height - row - 1) * width;
                    Array.Copy(pixels, index, buffer, 0, width);
                    Array.Copy(pixels, reflectedIndex, pixels, index, width);
                    Array.Copy(buffer, 0, pixels, reflectedIndex, width);
                }
            }
        }
    }
}
