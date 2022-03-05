using System;
using UnityEngine;
using UnityEngine.Rendering;


namespace AlephVault.Unity.TextureUtils
{
    namespace Samples
    {
        [RequireComponent(typeof(SpriteRenderer))]
        public class SampleBlitSprite : MonoBehaviour
        {
            private SpriteRenderer spriteRenderer;
            
            [SerializeField]
            private Texture2D[] textures;

            private void Awake()
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            private void Start()
            {
                // I'm not sure about the performance of doing this.
                // The algorithm works, but I need a more performant
                // way to do this, if any. I did not stress-test this
                // solution, even when the pooling system may mitigate
                // a good part of the impact. Perhaps there is a more
                // performant way of doing this.
                //
                // For this script to work, all the textures in the
                // array must be either created dynamically or marked
                // as "Read/Write enabled" while importing the texture
                // in the TextureImporter Editor settings.
                Texture2D tex2d = new Texture2D(144, 192, TextureFormat.ARGB32, false);
                Color[] pixels = tex2d.GetPixels(0, 0, 144, 192);
                for (int index = 0; index < pixels.Length; index++)
                {
                    pixels[index] = new Color(0, 0, 0, 0);
                }
                foreach (var texture in textures)
                {
                    Color[] sourcePixels = texture.GetPixels(0, 0, 144, 192);
                    for (int index = 0; index < pixels.Length; index++)
                    {
                        if (sourcePixels[index].a > 0)
                        {
                            float a = sourcePixels[index].a + (1 - sourcePixels[index].a) * pixels[index].a;
                            Color c = (sourcePixels[index] * sourcePixels[index].a +
                                       pixels[index] * pixels[index].a * (1 - sourcePixels[index].a)) / a;
                            c.a = a;
                            pixels[index] = c;
                        }
                    }
                }
                tex2d.SetPixels(0, 0, 144, 192, pixels);
                tex2d.Apply();
                spriteRenderer.sprite = Sprite.Create(tex2d, new Rect(48, 144, 48, 48), Vector2.zero, 48);
            }
        }
    }
}