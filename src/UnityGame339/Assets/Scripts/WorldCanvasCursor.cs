using UnityEngine;

public class WorldCanvasCursor : MonoBehaviour
{
    [Header("References")]
    public RectTransform canvasRect;
    public RectTransform fakeCursor;
    public Camera eventCamera;
    public ParticleSystem cursorParticles;

    [Header("Movement")]
    public float moveThreshold = 0.1f;

    private Vector2 lastCursorPosition;
    private bool hasCursorPosition;

    private void Start()
    {
        Cursor.visible = false;

        if (cursorParticles != null)
            cursorParticles.Stop();
    }

    private void Update()
    {
        Vector2 localPoint;

        bool gotPoint = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            eventCamera,
            out localPoint
        );

        if (!gotPoint || !canvasRect.rect.Contains(localPoint))
        {
            StopParticles();
            return;
        }

        fakeCursor.anchoredPosition = localPoint;

        if (!hasCursorPosition)
        {
            hasCursorPosition = true;
            lastCursorPosition = localPoint;
            StopParticles();
            return;
        }

        float moveAmount = Vector2.Distance(localPoint, lastCursorPosition);

        if (moveAmount > moveThreshold)
            PlayParticles();
        else
            StopParticles();

        lastCursorPosition = localPoint;
    }

    private void PlayParticles()
    {
        if (cursorParticles != null && !cursorParticles.isPlaying)
            cursorParticles.Play();
    }

    private void StopParticles()
    {
        if (cursorParticles != null && cursorParticles.isPlaying)
            cursorParticles.Stop();
    }
}