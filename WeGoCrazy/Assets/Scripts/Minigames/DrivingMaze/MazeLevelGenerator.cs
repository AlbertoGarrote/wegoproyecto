using UnityEngine;

public class MazeLevelGenerator : MonoBehaviour
{
    [Header("Prefabs & Settings")]
    [Tooltip("Prefab que har de pared (ej: lpiz, regla, tapn, libro)")]
    public GameObject wallPrefab; 
    [Tooltip("Prefab que har de meta (ej: tarjeta de crdito o el msculo)")]
    public GameObject goalPrefab;
    public float tileSize = 1.8f;
    
    [Header("Configuracion del Laberinto")]
    [TextArea(15, 30)]
    public string mazeTrackLayout = 
@"
#####################
# S                 #
###########   #######
#                   #
#######   ###########
#                   #
###########   #######
#      G #          #
#   ############    #
#                   #
#####################
";

    void Awake()
    {
        // Calculamos ancho y alto para centrarlo
        string[] rows = mazeTrackLayout.Trim().Replace("\r", "").Split('\n');
        int height = rows.Length;
        int width = 0;
        foreach (var r in rows) width = Mathf.Max(width, r.Length);

        // Centramos el laberinto
        Vector2 centerOffset = new Vector2(-(width * tileSize) / 2f, (height * tileSize) / 2f);

        // 1. Generamos el laberinto
        GenerateTrack(mazeTrackLayout, centerOffset);

        // 2. Ajustamos la camara para verlo todo
        AdjustCamera(mazeTrackLayout);

        // 3. Limpiamos colisiones de inicio
        MazeController mc = FindObjectOfType<MazeController>();
        if (mc != null) mc.ClearTouches();
    }

    private void AdjustCamera(string mapData)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        string[] rows = mapData.Trim().Replace("\r", "").Split('\n');
        int height = rows.Length;
        int width = 0;
        foreach (var r in rows) width = Mathf.Max(width, r.Length);

        float mazeHeight = height * tileSize;
        float mazeWidth = width * tileSize;

        // Calculamos el tamaño necesario para que quepa todo
        // (Añadimos un 20% de margen para que se vea bien)
        float margin = 1.2f;
        float sizeByHeight = (mazeHeight / 2f) * margin;
        float sizeByWidth = ((mazeWidth / 2f) / cam.aspect) * margin;

        cam.orthographic = true;
        cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
        
        // Centramos la camara (ajusta Z si tu juego es 3D o 2D)
        cam.transform.position = new Vector3(0, 0, -10f);
    }

    private void GenerateTrack(string mapData, Vector2 offset)
    {
        if (string.IsNullOrEmpty(mapData)) return;

        string[] rows = mapData.Trim().Replace("\r", "").Split('\n');
        for (int y = 0; y < rows.Length; y++)
        {
            string row = rows[y];
            for (int x = 0; x < row.Length; x++)
            {
                char cell = row[x];
                
                Vector2 pos = offset + new Vector2(x * tileSize, -y * tileSize);
                
                if (cell == '#')
                {
                    InstantiateItem(wallPrefab, pos, "MazeWall");
                }
                else if (cell == 'S')
                {
                    ScoopMovement[] players = FindObjectsOfType<ScoopMovement>();
                    foreach(var p in players)
                    {
                        p.transform.position = pos + (p.isLeftScoop ? new Vector2(-0.2f, 0) : new Vector2(0.2f, 0));
                    }
                }
                else if (cell == 'G')
                {
                    GameObject goal = InstantiateItem(goalPrefab, pos, "MazeGoal");
                    if (goal != null)
                    {
                        SpriteRenderer sr = goal.GetComponentInChildren<SpriteRenderer>();
                        if (sr != null) sr.color = Color.green;
                    }
                }
            }
        }
    }
    
    private GameObject InstantiateItem(GameObject prefab, Vector2 pos, string newTag)
    {
        if (prefab != null)
        {
            GameObject newObj = Instantiate(prefab, pos, Quaternion.identity, transform);
            newObj.name = newTag; 
            return newObj;
        }
        return null;
    }
}