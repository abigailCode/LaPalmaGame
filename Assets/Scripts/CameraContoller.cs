using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private float cameraHeight;


    void Start()
    {
        //Guardamos el alto de la c�mara
        cameraHeight = GetCameraHeight();
        //AudioManager.instance.PlayMusic("MainTheme");
    }

    void OnBecameInvisible()
    {
        if (mainCamera != null)
        {
            //Comprobamos si el player est� por encima o por debajo de la c�mara 
            //para saber si subimo o bajamos la c�mara

            float objectY = transform.position.y;
            float cameraBottomY = mainCamera.transform.position.y - cameraHeight / 2f;
            float cameraTopY = mainCamera.transform.position.y + cameraHeight / 2f;

            if (objectY < cameraBottomY)
            {
                RepositionCamera(-cameraHeight/2f);
            }
            else if (objectY > cameraTopY)
            {
                RepositionCamera(cameraHeight/2f);
            }
        }
    }

    void RepositionCamera(float offsetY)
    {
        //modificamos la posici�n de la c�mara en "y", sumando la altura de la c�mara 
        //negativo o positivo en funci�n de si el player est� por debajo o por encima
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, transform.position.y + offsetY, mainCamera.transform.position.z);
    }

    public float GetCameraHeight()
    {
        return mainCamera.orthographicSize * 2f;
    }
}
