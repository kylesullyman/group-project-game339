using Game.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private float squareSize = 0.75f;

    private Renderer squareRenderer;
    private readonly UnityGameLogger _log = new UnityGameLogger();

    private GameObject[,] squares = new GameObject[8, 8];

    private void Start()
    {
        squareRenderer = squarePrefab.GetComponent<Renderer>();
        // GenerateBoard();
    }

    public void GenerateBoard()
    {
        GameObject boardHolder = new GameObject("Board Holder");

        float offset = (8 * squareSize) / 2f - squareSize / 2f;
        Vector3 origin = squarePrefab.transform.position;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector3 pos = origin + new Vector3(x * squareSize - offset, y * squareSize - offset, 0);
                GameObject square = Instantiate(squarePrefab, pos, Quaternion.identity, boardHolder.transform);
                square.transform.localScale = Vector3.one * squareSize;
                square.name = $"Square_{x}_{y}";

                SpriteRenderer sr = square.GetComponent<SpriteRenderer>();
                bool isLight = (x + y) % 2 == 0;
                sr.color = isLight ? new Color(0.93f, 0.85f, 0.7f) : new Color(0.36f, 0.22f, 0.13f);

                if (square.GetComponent<BoxCollider2D>() == null)
                {
                    square.AddComponent<BoxCollider2D>();
                }

                BoardSquare boardSquare = square.GetComponent<BoardSquare>();
                if (boardSquare == null)
                {
                    boardSquare = square.AddComponent<BoardSquare>();
                }

                boardSquare.SetCoordinates(x, y);

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
    
    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            Collider2D hit = Physics2D.OverlapPoint(mousePos2D);

            if (hit != null)
            {
                BoardSquare square = hit.GetComponent<BoardSquare>();
                if (square != null)
                {
                    square.PrintCoordinates();
                }
            }
        }
    }
}