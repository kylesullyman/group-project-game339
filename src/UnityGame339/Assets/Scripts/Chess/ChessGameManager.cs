using System.Collections.Generic;
using UnityEngine;

public class ChessGameManager : MonoBehaviour
{
    private ChessPiece[,] _board = new ChessPiece[8, 8];
    private SquareController[,] _squares;

    private PieceColor _currentTurn = PieceColor.White;
    private int _selectedX = -1;
    private int _selectedY = -1;
    private List<Vector2Int> _validMoves = new List<Vector2Int>();
    private bool _gameOver;

    private static readonly Color SelectedColor  = new Color(1.00f, 1.00f, 0.00f, 1f);
    private static readonly Color ValidMoveColor = new Color(0.20f, 0.90f, 0.20f, 1f);
    private static readonly Color CaptureColor   = new Color(0.90f, 0.20f, 0.20f, 1f);

    public void Initialize(SquareController[,] squares)
    {
        _squares = squares;
        PlacePieces();
        RefreshDisplay();
        Debug.Log("Chess game started! White moves first.");
    }

    // ------------------------------------------------------------------ setup

    private void PlacePieces()
    {
        PlaceBackRank(PieceColor.White, 0);
        for (int x = 0; x < 8; x++)
            _board[x, 1] = new ChessPiece(PieceType.Pawn, PieceColor.White, x, 1);

        PlaceBackRank(PieceColor.Black, 7);
        for (int x = 0; x < 8; x++)
            _board[x, 6] = new ChessPiece(PieceType.Pawn, PieceColor.Black, x, 6);
    }

    private void PlaceBackRank(PieceColor color, int row)
    {
        _board[0, row] = new ChessPiece(PieceType.Rook,   color, 0, row);
        _board[1, row] = new ChessPiece(PieceType.Knight, color, 1, row);
        _board[2, row] = new ChessPiece(PieceType.Bishop, color, 2, row);
        _board[3, row] = new ChessPiece(PieceType.Queen,  color, 3, row);
        _board[4, row] = new ChessPiece(PieceType.King,   color, 4, row);
        _board[5, row] = new ChessPiece(PieceType.Bishop, color, 5, row);
        _board[6, row] = new ChessPiece(PieceType.Knight, color, 6, row);
        _board[7, row] = new ChessPiece(PieceType.Rook,   color, 7, row);
    }

    // -------------------------------------------------------------- input

    public void OnSquareClicked(int x, int y)
    {
        if (_gameOver) return;

        ChessPiece clicked = _board[x, y];

        if (_selectedX >= 0)
        {
            // Attempt to move to this square
            if (_validMoves.Contains(new Vector2Int(x, y)))
            {
                ExecuteMove(_selectedX, _selectedY, x, y);
                Deselect();
                return;
            }
            Deselect();
        }

        // Select piece belonging to current player
        if (clicked != null && clicked.color == _currentTurn)
        {
            _selectedX = x;
            _selectedY = y;
            _validMoves = GetLegalMoves(x, y);
            HighlightMoves();
        }
    }

    // -------------------------------------------------------------- move execution

    private void ExecuteMove(int fromX, int fromY, int toX, int toY)
    {
        ChessPiece piece = _board[fromX, fromY];
        _board[toX, toY] = piece;
        _board[fromX, fromY] = null;
        piece.x = toX;
        piece.y = toY;
        piece.hasMoved = true;

        // Pawn promotion to queen
        if (piece.type == PieceType.Pawn)
        {
            if (piece.color == PieceColor.White && toY == 7)
                piece.type = PieceType.Queen;
            else if (piece.color == PieceColor.Black && toY == 0)
                piece.type = PieceType.Queen;
        }

        RefreshDisplay();

        PieceColor nextTurn = _currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
        _currentTurn = nextTurn;

        if (IsCheckmate(nextTurn))
        {
            string winner = nextTurn == PieceColor.White ? "Black" : "White";
            Debug.Log($"Checkmate! {winner} wins!");
            _gameOver = true;
        }
        else if (IsStalemate(nextTurn))
        {
            Debug.Log("Stalemate! It's a draw.");
            _gameOver = true;
        }
        else if (IsInCheck(nextTurn))
        {
            Debug.Log($"{nextTurn} is in check!");
        }
        else
        {
            Debug.Log($"{nextTurn}'s turn.");
        }
    }

    // -------------------------------------------------------------- legal moves

    private List<Vector2Int> GetLegalMoves(int x, int y)
    {
        ChessPiece piece = _board[x, y];
        if (piece == null) return new List<Vector2Int>();

        var candidates = GetCandidateMoves(piece);
        var legal = new List<Vector2Int>();
        foreach (var m in candidates)
            if (!SimulateMoveResultsInCheck(x, y, m.x, m.y, piece.color))
                legal.Add(m);

        return legal;
    }

    private bool SimulateMoveResultsInCheck(int fromX, int fromY, int toX, int toY, PieceColor color)
    {
        ChessPiece captured = _board[toX, toY];
        ChessPiece moving   = _board[fromX, fromY];
        _board[toX, toY]   = moving;
        _board[fromX, fromY] = null;

        bool inCheck = IsInCheck(color);

        _board[fromX, fromY] = moving;
        _board[toX, toY]     = captured;
        return inCheck;
    }

    // -------------------------------------------------------------- check / checkmate

    private bool IsInCheck(PieceColor color)
    {
        int kx = -1, ky = -1;
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if (_board[x, y] != null && _board[x, y].type == PieceType.King && _board[x, y].color == color)
                { kx = x; ky = y; }

        if (kx < 0) return false;

        PieceColor opp = Opponent(color);
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                ChessPiece p = _board[x, y];
                if (p != null && p.color == opp)
                    if (GetCandidateMoves(p).Contains(new Vector2Int(kx, ky)))
                        return true;
            }
        return false;
    }

    private bool IsCheckmate(PieceColor color)
    {
        if (!IsInCheck(color)) return false;
        return HasNoLegalMoves(color);
    }

    private bool IsStalemate(PieceColor color)
    {
        if (IsInCheck(color)) return false;
        return HasNoLegalMoves(color);
    }

    private bool HasNoLegalMoves(PieceColor color)
    {
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if (_board[x, y] != null && _board[x, y].color == color)
                    if (GetLegalMoves(x, y).Count > 0)
                        return false;
        return true;
    }

    // -------------------------------------------------------------- candidate moves (pseudo-legal)

    private List<Vector2Int> GetCandidateMoves(ChessPiece piece)
    {
        var moves = new List<Vector2Int>();
        int x = piece.x, y = piece.y;
        PieceColor c = piece.color;

        switch (piece.type)
        {
            case PieceType.Pawn:
                AddPawnMoves(moves, x, y, c);
                break;
            case PieceType.Rook:
                AddSlidingMoves(moves, x, y, c,
                    new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right });
                break;
            case PieceType.Bishop:
                AddSlidingMoves(moves, x, y, c,
                    new[] { new Vector2Int(1,1), new Vector2Int(-1,1), new Vector2Int(1,-1), new Vector2Int(-1,-1) });
                break;
            case PieceType.Queen:
                AddSlidingMoves(moves, x, y, c,
                    new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                            new Vector2Int(1,1), new Vector2Int(-1,1), new Vector2Int(1,-1), new Vector2Int(-1,-1) });
                break;
            case PieceType.Knight:
                AddKnightMoves(moves, x, y, c);
                break;
            case PieceType.King:
                AddKingMoves(moves, x, y, c);
                break;
        }

        return moves;
    }

    private void AddPawnMoves(List<Vector2Int> moves, int x, int y, PieceColor color)
    {
        int dir      = color == PieceColor.White ? 1 : -1;
        int startRow = color == PieceColor.White ? 1 : 6;

        // One step forward
        if (InBounds(x, y + dir) && _board[x, y + dir] == null)
        {
            moves.Add(new Vector2Int(x, y + dir));
            // Two steps from starting row
            if (y == startRow && _board[x, y + 2 * dir] == null)
                moves.Add(new Vector2Int(x, y + 2 * dir));
        }

        // Diagonal captures
        foreach (int dx in new[] { -1, 1 })
        {
            int nx = x + dx, ny = y + dir;
            if (InBounds(nx, ny) && _board[nx, ny] != null && _board[nx, ny].color != color)
                moves.Add(new Vector2Int(nx, ny));
        }
    }

    private void AddSlidingMoves(List<Vector2Int> moves, int x, int y, PieceColor color, Vector2Int[] dirs)
    {
        foreach (var dir in dirs)
        {
            int nx = x + dir.x, ny = y + dir.y;
            while (InBounds(nx, ny))
            {
                if (_board[nx, ny] == null)
                    moves.Add(new Vector2Int(nx, ny));
                else
                {
                    if (_board[nx, ny].color != color)
                        moves.Add(new Vector2Int(nx, ny));
                    break;
                }
                nx += dir.x; ny += dir.y;
            }
        }
    }

    private void AddKnightMoves(List<Vector2Int> moves, int x, int y, PieceColor color)
    {
        int[,] offsets = { {2,1},{2,-1},{-2,1},{-2,-1},{1,2},{1,-2},{-1,2},{-1,-2} };
        for (int i = 0; i < 8; i++)
        {
            int nx = x + offsets[i, 0], ny = y + offsets[i, 1];
            if (InBounds(nx, ny) && (_board[nx, ny] == null || _board[nx, ny].color != color))
                moves.Add(new Vector2Int(nx, ny));
        }
    }

    private void AddKingMoves(List<Vector2Int> moves, int x, int y, PieceColor color)
    {
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx, ny = y + dy;
                if (InBounds(nx, ny) && (_board[nx, ny] == null || _board[nx, ny].color != color))
                    moves.Add(new Vector2Int(nx, ny));
            }
    }

    // -------------------------------------------------------------- helpers

    private bool InBounds(int x, int y) => x >= 0 && x < 8 && y >= 0 && y < 8;

    private PieceColor Opponent(PieceColor color) =>
        color == PieceColor.White ? PieceColor.Black : PieceColor.White;

    // -------------------------------------------------------------- display

    private void Deselect()
    {
        _selectedX = -1;
        _selectedY = -1;
        _validMoves.Clear();
        ResetHighlights();
    }

    private void HighlightMoves()
    {
        ResetHighlights();
        if (_squares == null) return;

        _squares[_selectedX, _selectedY].Highlight(SelectedColor);
        foreach (var m in _validMoves)
        {
            bool capture = _board[m.x, m.y] != null;
            _squares[m.x, m.y].Highlight(capture ? CaptureColor : ValidMoveColor);
        }
    }

    private void ResetHighlights()
    {
        if (_squares == null) return;
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                _squares[x, y].ResetColor();
    }

    private void RefreshDisplay()
    {
        if (_squares == null) return;
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                _squares[x, y].SetPieceDisplay(_board[x, y]);
    }
}
