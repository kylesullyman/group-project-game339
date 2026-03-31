using Game.Runtime;
using UnityEngine;

public class PieceInitializer : MonoBehaviour
{
    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private float _pieceScale = 0.25f;

    [Header("White Pieces")]
    [SerializeField] private Sprite _whitePawn;
    [SerializeField] private Sprite _whiteKnight;
    [SerializeField] private Sprite _whiteBishop;
    [SerializeField] private Sprite _whiteRook;
    [SerializeField] private Sprite _whiteQueen;
    [SerializeField] private Sprite _whiteKing;

    [Header("Black Pieces")]
    [SerializeField] private Sprite _blackPawn;
    [SerializeField] private Sprite _blackKnight;
    [SerializeField] private Sprite _blackBishop;
    [SerializeField] private Sprite _blackRook;
    [SerializeField] private Sprite _blackQueen;
    [SerializeField] private Sprite _blackKing;

    private readonly UnityGameLogger _log = new UnityGameLogger();

    private static readonly PieceType[] BackRank =
    {
        PieceType.Rook, PieceType.Knight, PieceType.Bishop,
        PieceType.Queen, PieceType.King,
        PieceType.Bishop, PieceType.Knight, PieceType.Rook
    };

    public void InitializePieces()
    {
        GameObject pieceHolder = new GameObject("Piece Holder");

        for (int x = 0; x < 8; x++)
        {
            SpawnPiece(BackRank[x], PieceColor.White, x, 0, pieceHolder.transform);
            SpawnPiece(PieceType.Pawn, PieceColor.White, x, 1, pieceHolder.transform);
            SpawnPiece(PieceType.Pawn, PieceColor.Black, x, 6, pieceHolder.transform);
            SpawnPiece(BackRank[x], PieceColor.Black, x, 7, pieceHolder.transform);
        }

        _log.Info("Pieces initialized.");
    }

    private void SpawnPiece(PieceType type, PieceColor color, int x, int y, Transform parent)
    {
        Sprite sprite = GetSprite(type, color);
        if (sprite == null)
        {
            _log.Warn($"Missing sprite for {color} {type} — skipping ({x},{y}).");
            return;
        }

        GameObject square = _boardManager.GetSquare(x, y);
        if (square == null)
        {
            _log.Warn($"Square ({x},{y}) not found — skipping piece.");
            return;
        }

        GameObject piece = new GameObject();
        piece.transform.SetParent(parent);
        piece.transform.position = new Vector3(square.transform.position.x, square.transform.position.y, -0.1f);
        piece.transform.localScale = Vector3.one * _pieceScale;

        SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 1;

        ChessPiece chessPiece = piece.AddComponent<ChessPiece>();
        chessPiece.Initialize(type, color);
    }

    private Sprite GetSprite(PieceType type, PieceColor color) => (color, type) switch
    {
        (PieceColor.White, PieceType.Pawn)   => _whitePawn,
        (PieceColor.White, PieceType.Knight) => _whiteKnight,
        (PieceColor.White, PieceType.Bishop) => _whiteBishop,
        (PieceColor.White, PieceType.Rook)   => _whiteRook,
        (PieceColor.White, PieceType.Queen)  => _whiteQueen,
        (PieceColor.White, PieceType.King)   => _whiteKing,
        (PieceColor.Black, PieceType.Pawn)   => _blackPawn,
        (PieceColor.Black, PieceType.Knight) => _blackKnight,
        (PieceColor.Black, PieceType.Bishop) => _blackBishop,
        (PieceColor.Black, PieceType.Rook)   => _blackRook,
        (PieceColor.Black, PieceType.Queen)  => _blackQueen,
        (PieceColor.Black, PieceType.King)   => _blackKing,
        _ => null
    };
}
