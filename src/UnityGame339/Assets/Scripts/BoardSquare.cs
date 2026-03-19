using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    private int x;
    private int y;

    public void SetCoordinates(int boardX, int boardY)
    {
        x = boardX;
        y = boardY;
    }

    public void PrintCoordinates()
    {
        Debug.Log($"Clicked square: ({x}, {y})");
    }
}