using UnityEngine;

public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }
public enum PieceColor { White, Black }

public class ChessPiece : MonoBehaviour
{
    public PieceType Type { get; private set; }
    public PieceColor Color { get; private set; }

    public void Initialize(PieceType type, PieceColor color)
    {
        Type = type;
        Color = color;
        gameObject.name = $"{color}_{type}";
    }
}
