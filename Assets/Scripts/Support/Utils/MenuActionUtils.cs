using System;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace Support
{
    namespace Utils
    {
        public static class MenuActionUtils
        {
            public static GUIStyle GetWordWrappingStyle()
            {
                GUIStyle style = new GUIStyle();
                style.wordWrap = true;
                return style;
            }

            public static GUIStyle GetSingleLabelStyle()
            {
                GUIStyle style = GetWordWrappingStyle();
                style.wordWrap = true;
                style.margin.left = 6;
                style.margin.right = 6;
                style.margin.bottom = 3;
                style.margin.top = 3;
                return style;
            }

            public static GUIStyle GetCaptionLabelStyle()
            {
                GUIStyle style = GetSingleLabelStyle();
                style.fontStyle = FontStyle.Bold;
                return style;
            }

            public static GUIStyle GetIndentedStyle()
            {
                GUIStyle style = GetSingleLabelStyle();
                style.margin.left = 10;
                return style;
            }

            public static string SimplifySpaces(string source)
            {
                return Regex.Replace(source, @"\s{2,}", " ").Trim();
            }

            public static string EnsureNonEmpty(string source, string defaultValue)
            {
                string simplified = SimplifySpaces(source);
                return simplified != "" ? simplified : defaultValue;
            }

            /// <summary>
            ///   Returns the default colors to be used in any color transition.
            ///   These colors are just a suggestion and can be changed.
            /// </summary>
            /// <returns>A <see cref="ColorBlock"/> with default colors.</returns>
            public static ColorBlock DefaultColors()
            {
                ColorBlock colors = new ColorBlock();
                colors.normalColor = new Color32(255, 255, 255, 255);
                colors.highlightedColor = new Color32(245, 245, 245, 255);
                colors.pressedColor = new Color32(200, 200, 200, 255);
                colors.disabledColor = new Color32(200, 200, 200, 255);
                colors.fadeDuration = 0.1f;
                colors.colorMultiplier = 1f;
                return colors;
            }

        }
    }
}
