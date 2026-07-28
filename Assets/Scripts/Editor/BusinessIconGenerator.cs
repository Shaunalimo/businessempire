#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using BusinessEmpire.Data;
using UnityEditor;
using UnityEngine;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Generates distinct business icon sprites and assigns them to BusinessDefinition assets.
    /// Menu: Business Empire → Generate Business Icons
    /// </summary>
    public static class BusinessIconGenerator
    {
        private const string IconFolder = "Assets/Resources/BusinessIcons";
        private const string DefinitionFolder = "Assets/Resources/BusinessDefinitions";
        private const int Size = 256;

        private static readonly Dictionary<BusinessType, IconStyle> Styles = new Dictionary<BusinessType, IconStyle>
        {
            { BusinessType.GroceryShop,       new IconStyle(new Color(0.18f, 0.72f, 0.45f), "GS", "Grocery") },
            { BusinessType.CoffeeShop,        new IconStyle(new Color(0.55f, 0.35f, 0.20f), "CF", "Coffee") },
            { BusinessType.Restaurant,        new IconStyle(new Color(0.90f, 0.35f, 0.25f), "RT", "Dining") },
            { BusinessType.ClothingStore,     new IconStyle(new Color(0.75f, 0.30f, 0.65f), "CL", "Fashion") },
            { BusinessType.SoftwareCompany,   new IconStyle(new Color(0.25f, 0.55f, 0.95f), "SW", "Software") },
            { BusinessType.LogisticsCompany,  new IconStyle(new Color(0.95f, 0.70f, 0.15f), "LG", "Logistics") },
            { BusinessType.Factory,           new IconStyle(new Color(0.50f, 0.52f, 0.55f), "FC", "Factory") },
            { BusinessType.RealEstateAgency,  new IconStyle(new Color(0.20f, 0.75f, 0.80f), "RE", "Realty") }
        };

        private struct IconStyle
        {
            public Color color;
            public string abbr;
            public string label;
            public IconStyle(Color c, string a, string l) { color = c; abbr = a; label = l; }
        }

        [MenuItem("Business Empire/Generate Business Icons")]
        public static void GenerateAll()
        {
            EnsureFolder("Assets/UI");
            EnsureFolder("Assets/UI/Icons");
            EnsureFolder(IconFolder);

            foreach (var kvp in Styles)
            {
                string path = $"{IconFolder}/{kvp.Key}.png";
                GenerateIconTexture(path, kvp.Value);
                AssignIconToDefinition(kvp.Key, path);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Business Empire] Business icons generated in " + IconFolder);
        }

        private static void GenerateIconTexture(string assetPath, IconStyle style)
        {
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);

            // Rounded square background
            Color bgDark = style.color * 0.55f;
            bgDark.a = 1f;
            Color bgLight = style.color;
            bgLight.a = 1f;

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    float nx = (x / (float)Size) * 2f - 1f;
                    float ny = (y / (float)Size) * 2f - 1f;
                    float dist = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny));
                    float corner = 0.82f;
                    bool inside = dist < corner;

                    if (!inside)
                    {
                        tex.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    float gradient = (nx + ny) * 0.15f + 0.5f;
                    tex.SetPixel(x, y, Color.Lerp(bgDark, bgLight, gradient));
                }
            }

            // Inner highlight circle
            DrawCircle(tex, Size / 2, Size / 2, 72, new Color(1f, 1f, 1f, 0.15f));
            DrawCircle(tex, Size / 2, Size / 2, 68, new Color(1f, 1f, 1f, 0.08f));

            // Abbreviation text rendered via GUI in a separate step — draw pixel blocks
            DrawBlockText(tex, style.abbr, Size / 2, Size / 2 + 10, Color.white);

            byte[] png = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);
            File.WriteAllBytes(assetPath, png);

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 100;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
        }

        private static void DrawCircle(Texture2D tex, int cx, int cy, int radius, Color color)
        {
            for (int y = cy - radius; y <= cy + radius; y++)
            {
                for (int x = cx - radius; x <= cx + radius; x++)
                {
                    if (x < 0 || x >= Size || y < 0 || y >= Size) continue;
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (d <= radius)
                    {
                        Color existing = tex.GetPixel(x, y);
                        tex.SetPixel(x, y, Color.Lerp(existing, color, color.a));
                    }
                }
            }
        }

        // Simple 5x7 block font for 2-char abbreviations
        private static void DrawBlockText(Texture2D tex, string text, int cx, int cy, Color color)
        {
            int charWidth = 6;
            int totalWidth = text.Length * charWidth * 5 + (text.Length - 1) * 4;
            int startX = cx - totalWidth / 2;

            for (int i = 0; i < text.Length; i++)
            {
                DrawChar(tex, text[i], startX + i * (charWidth * 5 + 4), cy - 14, color, 4);
            }
        }

        private static void DrawChar(Texture2D tex, char c, int ox, int oy, Color color, int scale)
        {
            bool[,] pattern = GetCharPattern(c);
            if (pattern == null) return;

            for (int row = 0; row < pattern.GetLength(0); row++)
            {
                for (int col = 0; col < pattern.GetLength(1); col++)
                {
                    if (!pattern[row, col]) continue;
                    for (int sy = 0; sy < scale; sy++)
                    {
                        for (int sx = 0; sx < scale; sx++)
                        {
                            int px = ox + col * scale + sx;
                            int py = oy + (pattern.GetLength(0) - 1 - row) * scale + sy;
                            if (px >= 0 && px < Size && py >= 0 && py < Size)
                                tex.SetPixel(px, py, color);
                        }
                    }
                }
            }
        }

        private static bool[,] GetCharPattern(char c)
        {
            // Minimal patterns for business abbreviations
            switch (c)
            {
                case 'G': return Pattern("11110","10000","10000","10110","10010","10010","11110");
                case 'S': return Pattern("01110","10000","10000","01110","00001","00001","11110");
                case 'C': return Pattern("01110","10001","10000","10000","10000","10001","01110");
                case 'F': return Pattern("11111","10000","10000","11110","10000","10000","10000");
                case 'R': return Pattern("11110","10001","10001","11110","10100","10010","10001");
                case 'T': return Pattern("11111","00100","00100","00100","00100","00100","00100");
                case 'L': return Pattern("10000","10000","10000","10000","10000","10000","11111");
                case 'W': return Pattern("10001","10001","10001","10001","10101","10101","01010");
                case 'E': return Pattern("11111","10000","10000","11110","10000","10000","11111");
                case 'A': return Pattern("01110","10001","10001","11111","10001","10001","10001");
                case 'O': return Pattern("01110","10001","10001","10001","10001","10001","01110");
                case 'I': return Pattern("11111","00100","00100","00100","00100","00100","11111");
                case 'N': return Pattern("10001","11001","10101","10011","10001","10001","10001");
                case 'D': return Pattern("11110","10001","10001","10001","10001","10001","11110");
                case 'M': return Pattern("10001","11011","10101","10001","10001","10001","10001");
                case 'P': return Pattern("11110","10001","10001","11110","10000","10000","10000");
                case 'Y': return Pattern("10001","10001","10001","01110","00100","00100","00100");
                case 'H': return Pattern("10001","10001","10001","11111","10001","10001","10001");
                default: return Pattern("01110","10001","10001","01110","10001","10001","01110");
            }
        }

        private static bool[,] Pattern(params string[] rows)
        {
            var p = new bool[rows.Length, rows[0].Length];
            for (int r = 0; r < rows.Length; r++)
                for (int c = 0; c < rows[r].Length; c++)
                    p[r, c] = rows[r][c] == '1';
            return p;
        }

        private static void AssignIconToDefinition(BusinessType type, string iconPath)
        {
            string defPath = $"{DefinitionFolder}/{type}.asset";
            var def = AssetDatabase.LoadAssetAtPath<BusinessDefinition>(defPath);
            if (def == null) return;

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
            def.icon = sprite;
            EditorUtility.SetDirty(def);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = Path.GetDirectoryName(path).Replace("\\", "/");
            var name = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
