using System;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace WindRose
{
    namespace MenuActions
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
        }
    }
}
