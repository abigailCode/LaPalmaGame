using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pausePanel;

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0 && (Input.GetKeyDown(KeyCode.Escape)|| Input.GetButtonDown("Fire3"))){
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }   
    }

    public void Restart()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1.0f;
        
    }

    public void BackToMenu()
    {
        Time.timeScale = 1.0f;
        SceneController.instance.LoadScene("Menu");

    }
}
