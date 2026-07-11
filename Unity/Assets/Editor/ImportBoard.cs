using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Smallworld.Models;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using UnityRegion = UnityModels.Region;

public static class ImportBoard
{
    const string IMPORT_FOLDER = "Assets/AssetFiles/GeneratedBoards";

    [MenuItem("Tools/Import SVG Board (Polygons)")]
    public static void ImportPolygons()
    {
        var path = EditorUtility.OpenFilePanel("Choose SVG", "", "svg");

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var doc = XDocument.Load(path);
        var board = new GameObject("Board");
        var filename = path.Substring(path.LastIndexOf("/") + 1).Split(".")[0] ?? "Board";
        var regions = new List<RegionDef>();
        var idCounter = 0;

        foreach (var polygonSVG in doc.Descendants().Where(el => el.Name.LocalName == "polygon"))
        {
            var pointStr = polygonSVG.Attribute("points")?.Value;
            if (string.IsNullOrEmpty(pointStr))
            {
                continue;
            }

            var points = pointStr.Split(" ");
            List<Vector2> vertices = new();
            for (int i = 0; i < points.Length - 1; i += 2)
            {
                var x = float.Parse(points[i]);
                var y = float.Parse(points[i + 1]);
                vertices.Add(new Vector2(x, -y));
            }

            regions.Add(new()
            {
                id = polygonSVG.Attribute("id")?.Value ?? idCounter++.ToString(),
                vertices = vertices
            });
        }

        foreach (var def in regions)
        {
            var region = new GameObject(def.id);
            region.transform.SetParent(board.transform);

            var collider = region.AddComponent<PolygonCollider2D>();
            collider.SetPath(0, def.vertices);
        }

        PrefabUtility.SaveAsPrefabAsset(board, $"{IMPORT_FOLDER}/{filename}.prefab");

        Debug.Log($"Imported and saved to {IMPORT_FOLDER}/{filename}");

        Object.DestroyImmediate(board);
    }

    [MenuItem("Tools/Import SVG Board (Paths)")]
    public static void ImportPaths()
    {
        var path = EditorUtility.OpenFilePanel("Choose SVG", "", "svg");

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var doc = XDocument.Load(path);
        var board = new GameObject("Board");
        var filename = path.Substring(path.LastIndexOf("/") + 1).Split(".")[0] ?? "Board";

        foreach (var pathSVG in doc.Descendants().Where(el => el.Name.LocalName == "path"))
        {
            var dVal = pathSVG.Attribute("d")?.Value;
            if (string.IsNullOrEmpty(dVal))
            {
                continue;
            }

            var paths = ParseSvgPath(dVal);

            var regionNum = pathSVG.Attribute("region-num")?.Value ?? "0";
            var title = pathSVG.Descendants().First(el => el.Name.LocalName == "title")?.Value ?? "Region";

            var region = new GameObject($"{title}{regionNum}");
            region.transform.SetParent(board.transform, false);

            // var regionScriptableObj = CreateRegion(pathSVG, filename);
            // if (regionScriptableObj != null)
            // {
            //     region.AddComponent<UnityRegion>().region = regionScriptableObj;
            // }

            var collider = region.AddComponent<PolygonCollider2D>();
            collider.pathCount = paths.Count;

            for (int i = 0; i < paths.Count; i++)
            {
                collider.SetPath(i, paths[i].ToArray());
            }
        }

        PrefabUtility.SaveAsPrefabAsset(board, $"{IMPORT_FOLDER}/{filename}.prefab");

        Debug.Log($"Imported and saved to {IMPORT_FOLDER}/{filename}");

        Object.DestroyImmediate(board);
    }

    private static List<List<Vector2>> ParseSvgPath(string pathStr)
    {
        List<List<Vector2>> paths = new();

        var tokens = Regex.Matches(pathStr, @"[MmLlHhVvZz]|-?\d*\.?\d+(?:[eE][-+]?\d+)?");

        int idx = 0;
        char command = '\0';

        var current = Vector2.zero;
        var subpathStart = Vector2.zero;
        List<Vector2> currentPath = null;

        float ReadNextFloat()
        {
            var txt = tokens[idx++].Value;
            return float.Parse(txt);
        }

        Vector2 GetNextPoint()
        {
            var x = ReadNextFloat();
            var y = ReadNextFloat();

            var p = Regex.IsMatch(command.ToString(), @"[mlhvz]") ? current + new Vector2(x, -y) : new Vector2(x, -y);
            current = p;

            return p;
        }

        void StartNewContour(Vector2 pt)
        {
            currentPath = new();
            paths.Add(currentPath);

            currentPath.Add(pt);
            subpathStart = pt;
        }


        bool IsCommand(string token)
        {
            return token.Length == 1 && char.IsLetter(token[0]);
        }

        while (idx < tokens.Count)
        {
            var token = tokens[idx].Value;

            if (IsCommand(token))
            {
                command = token[0];
                idx++;

                if (command == 'Z' || command == 'z')
                {
                    if (currentPath != null && currentPath.Count > 0)
                    {
                        // Ensure path is closed
                        if (currentPath[0] != currentPath[^1])
                        {
                            currentPath.Add(currentPath[0]);
                        }

                        current = subpathStart;
                    }

                    continue;
                }
            }

            switch (command)
            {
                case 'M':
                case 'm':
                    {
                        StartNewContour(GetNextPoint());

                        // SVG allows implicit LineTo commands after the first MoveTo pair
                        while (idx < tokens.Count && !IsCommand(tokens[idx].Value))
                        {
                            currentPath.Add(GetNextPoint());
                        }

                        break;
                    }

                case 'L':
                case 'l':
                    {
                        var p = GetNextPoint();
                        currentPath ??= new();

                        if (!paths.Contains(currentPath))
                        {
                            paths.Add(currentPath);
                        }

                        currentPath.Add(p);
                        break;
                    }

                case 'H':
                case 'h':
                    {
                        var x = ReadNextFloat();
                        var p = command == 'h' ? new Vector2(current.x + x, current.y) : new Vector2(x, current.y);

                        current = p;
                        currentPath ??= new();

                        if (!paths.Contains(currentPath))
                        {
                            paths.Add(currentPath);
                        }

                        currentPath.Add(p);
                        break;
                    }

                case 'V':
                case 'v':
                    {
                        var y = ReadNextFloat();
                        var p = command == 'v' ? new Vector2(current.x, current.y - y) : new Vector2(current.x, -y);

                        current = p;
                        currentPath ??= new();

                        if (!paths.Contains(currentPath))
                        {
                            paths.Add(currentPath);
                        }

                        currentPath.Add(p);
                        break;
                    }

                default:
                    {
                        Debug.LogWarning($"Unsupported SVG path command '{command}' in path: {pathStr}");
                        return paths;
                    }
            }
        }

        return paths;
    }

    private static RegionScriptableObject CreateRegion(XElement el, string mapName)
    {

        RegionAttribute GetAttribute(string str)
        {
            return str?.ToLower() switch
            {
                "mine" => RegionAttribute.Mine,
                "underworld" => RegionAttribute.Underworld,
                "magic" => RegionAttribute.Magic,
                _ => RegionAttribute.None,
            };
        }

        RegionType GetRegionType(string str)
        {
            return str?.ToLower() switch
            {
                "hill" => RegionType.Hill,
                "sea" => RegionType.Sea,
                "lake" => RegionType.Lake,
                "forest" => RegionType.Forest,
                "mountain" => RegionType.Mountain,
                "farmland" => RegionType.Farmland,
                "swamp" => RegionType.Swamp,
                _ => RegionType.Hill,
            };
        }

        if (string.IsNullOrEmpty(el.Attribute("region-type")?.Value))
        {
            return null;
        }

        var regionType = GetRegionType(el.Attribute("region-type")?.Value);
        var regionAttr1 = GetAttribute(el.Attribute("attr-1")?.Value);
        var regionAttr2 = GetAttribute(el.Attribute("attr-2")?.Value);
        var isBorder = el.Attribute("is-border")?.Value == "true";
        var isLostTribe = el.Attribute("is-lost-tribe")?.Value == "true";

        var region = ScriptableObject.CreateInstance<RegionScriptableObject>();
        var regionName = $"{regionType}";
        region.SetFields(regionName, regionType, regionAttr1, regionAttr2, isBorder, isLostTribe);

        var assetPath = "Assets/RegionObjects/Map/" + regionName + ".asset";
        AssetDatabase.GenerateUniqueAssetPath(assetPath);
        AssetDatabase.CreateAsset(region, assetPath);
        AssetDatabase.SaveAssets();

        return region;
    }

    private struct RegionDef
    {
        public string id;
        public List<Vector2> vertices;
    }
}
