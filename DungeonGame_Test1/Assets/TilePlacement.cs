using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;

public class TilePlacement : MonoBehaviour
{
    [Header("Main data/components")]
    public string filename = "Map1_data.json";
    public Tilemap FloorTilemap;
    public Tilemap WallTilemap;


    public TileBase Wall_bottom;

    [Header("Brick tiles")]
    public TileBase Wall_W1;
    public TileBase Wall_W2;
    public TileBase Wall_W3;
    public TileBase Wall_W4;
    public TileBase Wall_W5;
    [Header("full/partial moss wall tiles")]
    public TileBase Wall_M1;
    public TileBase Wall_M2;
    public TileBase Wall_M3;
    public TileBase Wall_M4;
    public TileBase Wall_M5;
    public TileBase Wall_M6;
    public TileBase Wall_M7;
    public TileBase Wall_M8;
    [Header("Broken moss tiles")]
    public TileBase Wall_BM1;
    public TileBase Wall_BM2;
    public TileBase Wall_BM3;
    public TileBase Wall_BM4;
    public TileBase Wall_BM5;
    public TileBase Wall_BM6;
    public TileBase Wall_BM7;
    public TileBase Wall_BM8;
    public TileBase Wall_BM9;
    [Header("Debug")]
    public TileBase TestTile;
    public TileBase Floor;
    public class MapData
    {
        public float[][] map_data;
    }

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, filename);
        Debug.Log("Loading map file at: " + path);

        if (!File.Exists(path))
        {
            Debug.LogError($"Map file not found at: {path}");
            return;
        }

        string jsonText = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(jsonText))
        {
            Debug.LogError("Map file is empty or null: " + path);
            return;
        }

        try
        {
            MapData mapData = JsonConvert.DeserializeObject<MapData>(jsonText);
            if (mapData == null || mapData.map_data == null || mapData.map_data.Length == 0)
            {
                Debug.LogError("Parsed map data is empty or invalid.");
                return;
            }

            GenerateTiles(mapData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("JSON parsing failed: " + ex.Message);
        }
        PlaceWalls();
    }

    void GenerateTiles(MapData mapData)
    {
        for (int y = 0; y < mapData.map_data.Length; y++)
        {
            float[] row = mapData.map_data[y];
            for (int x = 0; x < row.Length; x++)
            {
                print(row);
                if (Mathf.Approximately(row[x], 1f))
                    FloorTilemap.SetTile(new Vector3Int(x, -y, 0), Floor);
                else
                    FloorTilemap.SetTile(new Vector3Int(x, -y, 0), null);
            }
        }
        Debug.Log("Tiles generated!");
    }

    void PlaceWalls()
    {
        int i;
        //horizontal walls
        for (i = -1; i < 52; i++)
        {
            WallTilemap.SetTile(new Vector3Int(i, 1, 0), Wall_W1);
            WallTilemap.SetTile(new Vector3Int(i, -51, 0), Wall_M8);
        }
        for (i = -2; i < 53; i++)
        {
            WallTilemap.SetTile(new Vector3Int(i, 2, 0), Wall_M1);
            WallTilemap.SetTile(new Vector3Int(i, -52, 0), Wall_W1);

        }
        //vertical walls
        for (i = 1; i > -26; i--)
        {            
            WallTilemap.SetTile(new Vector3Int(51, i, 0), Wall_M1);
            WallTilemap.SetTile(new Vector3Int(-1, i, 0), Wall_M1);
            WallTilemap.SetTile(new Vector3Int(52, i, 0), Wall_M1);
            WallTilemap.SetTile(new Vector3Int(-2, i, 0), Wall_M1);

        }

        for (i = -29; i > -52; i--)
        {
            WallTilemap.SetTile(new Vector3Int(-1, i, 0), Wall_M2);
            WallTilemap.SetTile(new Vector3Int(51, i, 0), Wall_M3);
            WallTilemap.SetTile(new Vector3Int(-2, i, 0), Wall_W1);
            WallTilemap.SetTile(new Vector3Int(52, i, 0), Wall_W1);
            if (i == -29)
            {
                WallTilemap.SetTile(new Vector3Int(-1, i, 0), Wall_M5);
                WallTilemap.SetTile(new Vector3Int(51, i, 0), Wall_M4);
                WallTilemap.SetTile(new Vector3Int(-2, i, 0), Wall_M8);
                WallTilemap.SetTile(new Vector3Int(52, i, 0), Wall_M8);
            }
            if(i == -51)
            {
                WallTilemap.SetTile(new Vector3Int(-1, i, 0), Wall_M7);
                WallTilemap.SetTile(new Vector3Int(51, i, 0), Wall_M6);
            }

            
        }
    }
}
