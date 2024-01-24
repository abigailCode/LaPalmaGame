using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Camera mainCamera;

    void Start()
    {
        FindMainCamera();
    }

    void LateUpdate()
    {
        if (player != null)
        {
            float cameraJumpDistance = mainCamera.orthographicSize * 2f;

            float objectY = player.position.y;

            // Calculamos los límites superior e inferior de la cámara
            float cameraBottomY = mainCamera.transform.position.y - mainCamera.orthographicSize;
            float cameraTopY = mainCamera.transform.position.y + mainCamera.orthographicSize;

            // Verificamos si el jugador está por encima o por debajo de la cámara
            if (objectY > cameraTopY)
            {
                JumpCamera(cameraJumpDistance);
            }
            else if (objectY < cameraBottomY)
            {
                JumpCamera(-cameraJumpDistance);
            }
        }
    }

    private void FindMainCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("No se encontró la cámara principal en la escena.");
            }
        }
    }

    private void JumpCamera(float offsetY)
    {
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + offsetY, mainCamera.transform.position.z);
    }
}
