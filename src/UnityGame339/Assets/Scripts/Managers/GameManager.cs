using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Title Screen")]
    [SerializeField] private GameObject titleScreenPrefab;

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 cameraPosition = new Vector3(0f, 0f, -1f);
    [SerializeField] private Vector3 cameraRotation = new Vector3(0f, 0f, 0f);

    [Header("Light Settings")]
    [SerializeField] private Light mainLight;
    [SerializeField] private Vector3 lightPosition = new Vector3(0f, 2f, 0f);
    [SerializeField] private Vector3 lightRotation = new Vector3(90f, 0f, 0f);

    void Start()
    {
        if (titleScreenPrefab != null)
        {
            GameObject titleScreenInstance = Instantiate(titleScreenPrefab, new Vector3(0f, 0f, 2f), Quaternion.identity);
            TitleScreen titleScreen = titleScreenInstance.GetComponent<TitleScreen>();
            if (titleScreen != null)
                titleScreen.Initialize();
        }

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            mainCamera.transform.position = cameraPosition;
            mainCamera.transform.eulerAngles = cameraRotation;
        }

        if (mainLight != null)
        {
            mainLight.transform.position = lightPosition;
            mainLight.transform.eulerAngles = lightRotation;
        }
    }


}
