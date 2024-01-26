using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public GameObject CreditsPanel;
    //START

    private void Start() {
        AudioManager.instance.PlayMusic("MenuTheme");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SceneController.instance.LoadScene("Menu");
            AudioManager.instance.PlayMusic("MenuTheme");
        } 
        if (Input.GetKeyDown(KeyCode.V))
        {
            SceneController.instance.LoadScene("Beach");
            AudioManager.instance.PlayMusic("Theme1-2");
        } 
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneController.instance.LoadScene("Houses");
            AudioManager.instance.PlayMusic("Theme1-2");
        }  
        if (Input.GetKeyDown(KeyCode.N))
        {
            SceneController.instance.LoadScene("Stars");
            AudioManager.instance.PlayMusic("MenuTheme");
        }    
    }
    public void StartGame()
    {
        AudioManager.instance.PlaySFX("Boton");
        AudioManager.instance.PlayMusic("Theme1-2");
        SceneController.instance.LoadScene("Beach");
    }
    //CREDITS
    public void ShowCredits()
    {
        AudioManager.instance.PlaySFX("Boton");
        CreditsPanel.SetActive(true);
    }
    
    public void HideCredits()
    {
        AudioManager.instance.PlaySFX("Boton");
        CreditsPanel.SetActive(false);
    }
    //QUIT

    public void QuitGame()
    {
        AudioManager.instance.PlaySFX("Boton");
        Application.Quit();
    }
}
