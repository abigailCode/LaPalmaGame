using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float yMin;
    [SerializeField] float yMax;
    // Start is called before the first frame update
    void Start()
    {
        //AudioManager.instance.PlayMusic("MainTheme");
    }

    // Update is called once per frame
    void Update()
    {
        Mathf.Clamp(1, yMin , yMax);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(player.transform.position.y, yMin, yMax), transform.position.z);
    }
}
