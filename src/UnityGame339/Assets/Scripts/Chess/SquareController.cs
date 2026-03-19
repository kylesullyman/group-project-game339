using TMPro;
using UnityEngine;

public class SquareController : MonoBehaviour
{
    public int x;
    public int y;

    private SpriteRenderer _sr;
    private Color _originalColor;
    private TextMeshPro _label;
    private ChessGameManager _gameManager;

    public void Initialize(int boardX, int boardY, ChessGameManager manager, float squareSize)
    {
        x = boardX;
        y = boardY;
        _gameManager = manager;

        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;

        // Ensure there's a collider so OnMouseDown fires
        if (GetComponent<Collider>() == null && GetComponent<Collider2D>() == null)
        {
            var col = gameObject.AddComponent<BoxCollider>();
            col.size = Vector3.one;
        }

        // Piece label rendered in world space on top of the square
        GameObject labelObj = new GameObject("PieceLabel");
        labelObj.transform.SetParent(transform);
        labelObj.transform.localPosition = new Vector3(0f, 0f, -0.1f);
        // Cancel parent scale so font-size maps directly to world units
        float inv = 1f / squareSize;
        labelObj.transform.localScale = new Vector3(inv, inv, 1f);

        _label = labelObj.AddComponent<TextMeshPro>();
        _label.rectTransform.sizeDelta = new Vector2(squareSize, squareSize);
        _label.fontSize = squareSize * 0.75f;
        _label.alignment = TextAlignmentOptions.Center;
        _label.enableWordWrapping = false;
        _label.overflowMode = TextOverflowModes.Overflow;
        _label.sortingOrder = 2;
    }

    public void SetPieceDisplay(ChessPiece piece)
    {
        if (_label == null) return;
        if (piece == null)
        {
            _label.text = "";
            return;
        }
        _label.text = piece.GetSymbol();
        // Dark outline color so pieces are visible on both square colors
        _label.color = piece.color == PieceColor.White
            ? new Color(1f, 1f, 1f)
            : new Color(0.1f, 0.1f, 0.1f);
    }

    public void Highlight(Color color)
    {
        _sr.color = color;
    }

    public void ResetColor()
    {
        _sr.color = _originalColor;
    }

    private void OnMouseDown()
    {
        _gameManager?.OnSquareClicked(x, y);
    }
}
