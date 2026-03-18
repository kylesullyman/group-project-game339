using Game.Runtime;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject squarePrefab; // drag your square prefab here
    [SerializeField] private float squareSize = 0.75f;

    private Renderer squareRenderer;
    private readonly UnityGameLogger _log = new UnityGameLogger();

    private GameObject[,] squares = new GameObject[8, 8];

    private void Start()
    {
        squareRenderer = squarePrefab.GetComponent<Renderer>();
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        float offset = (8 * squareSize) / 2f - squareSize / 2f;
        Vector3 origin = squarePrefab.transform.position;

        // float width = getBoardWidth();
        // origin.x += width / 2;
        // origin.y += width / 2;
        

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector3 pos = origin + new Vector3(x * squareSize - offset, y * squareSize - offset, 0);
                GameObject square = Instantiate(squarePrefab, pos, Quaternion.identity, transform);
                square.transform.localScale = Vector3.one * squareSize;
                square.name = $"Square_{x}_{y}";

                SpriteRenderer sr = square.GetComponent<SpriteRenderer>();
                bool isLight = (x + y) % 2 == 0;
                sr.color = isLight ? new Color(0.93f, 0.85f, 0.7f) : new Color(0.36f, 0.22f, 0.13f);

                squares[x, y] = square;
            }
        }
    }

    public float getBoardWidth()
    {
        float width;
        if (squareRenderer != null)
        {
            width = squareRenderer.bounds.size.x;
            return width;
        }
        
        _log.Info("No renderer found.");
        
        return 0;
    }
}