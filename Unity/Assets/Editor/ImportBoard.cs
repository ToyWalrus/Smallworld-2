using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using UnityEditor;
using UnityEngine;

public static class ImportBoard
{
    const string IMPORT_FOLDER = "Assets/AssetFiles/GeneratedBoards";

    [MenuItem("Tools/Import SVG Board")]
    public static void Import()
    {
        var path = EditorUtility.OpenFilePanel("Choose SVG", "", "svg");

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("Cancelled");
            return;
        }

        var doc = XDocument.Load(path);

        var idCounter = 0;
        List<RegionDef> regions = new();
        foreach (var polygon in doc.Descendants().Where(el => el.Name.LocalName == "polygon"))
        {
            var pointStr = polygon.Attribute("points")?.Value;
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
                vertices.Add(new Vector2(x, y));
            }

            regions.Add(new()
            {
                id = idCounter.ToString(),
                vertices = vertices
            });

            idCounter++;
        }


        var board = new GameObject("Board");
        foreach (var def in regions)
        {
            var region = new GameObject(def.id);
            region.transform.SetParent(board.transform);

            var collider = region.AddComponent<PolygonCollider2D>();
            collider.SetPath(0, def.vertices);
        }

        var filename = path.Substring(path.LastIndexOf("/")).Split(".")[0] ?? "Board";
        PrefabUtility.SaveAsPrefabAsset(board, $"{IMPORT_FOLDER}/{filename}.prefab");

        Debug.Log($"Imported and saved to {IMPORT_FOLDER}/{filename}");

        Object.DestroyImmediate(board);
    }

    private struct RegionDef
    {
        public string id;
        public List<Vector2> vertices;
    }
}
