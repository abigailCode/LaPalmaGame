using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject camera;
    [SerializeField] float yMin;
    [SerializeField] float yMax;
    // Start is called before the first frame update
    void Start()
    {
        //AudioManager.instance.PlayMusic("MainTheme");
    }

    //void Update()
    //{
    //    Mathf.Clamp(1, yMin , yMax);

    //    transform.position = new Vector3(transform.position.x, Mathf.Clamp(player.transform.position.y, yMin, yMax), transform.position.z);
    //}

    //void Update()
    //{
    //    float clampedY = Mathf.Clamp(player.transform.position.y, yMin, yMax);

    //    if (player.transform.position.y < yMin || player.transform.position.y > yMax)
    //    {
    //        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    //    }
    //}

    void OnBecameInvisible()
    {
        if (camera != null)
        {
            camera.transform.position = new Vector3(camera.transform.position.x, transform.position.y, camera.transform.position.z);
        }
    }
}
