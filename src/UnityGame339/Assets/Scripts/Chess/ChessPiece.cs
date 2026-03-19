public enum PieceType { None, Pawn, Rook, Knight, Bishop, Queen, King }
public enum PieceColor { White, Black }

public class ChessPiece
{
    public PieceType type;
    public PieceColor color;
    public int x;
    public int y;
    public bool hasMoved;

    public ChessPiece(PieceType type, PieceColor color, int x, int y)
    {
        this.type = type;
        this.color = color;
        this.x = x;
        this.y = y;
    }

    public string GetSymbol()
    {
        if (color == PieceColor.White)
        {
            return type switch
            {
                PieceType.King   => "\u2654",
                PieceType.Queen  => "\u2655",
                PieceType.Rook   => "\u2656",
                PieceType.Bishop => "\u2657",
                PieceType.Knight => "\u2658",
                PieceType.Pawn   => "\u2659",
                _ => ""
            };
        }
        else
        {
            return type switch
            {
                PieceType.King   => "\u265A",
                PieceType.Queen  => "\u265B",
                PieceType.Rook   => "\u265C",
                PieceType.Bishop => "\u265D",
                PieceType.Knight => "\u265E",
                PieceType.Pawn   => "\u265F",
                _ => ""
            };
        }
    }
}
