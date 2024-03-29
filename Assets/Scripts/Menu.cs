using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
            SceneController.instance.LoadScene("StarsFinal");
            AudioManager.instance.PlayMusic("MenuTheme");
        }

        if ((CreditsPanel.activeSelf) && (Input.GetButtonDown("Fire3")) || Input.GetButtonDown("Fire2")) HideCredits(); 
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

    //MENU
    public void GoToMenu()
    {
        AudioManager.instance.PlaySFX("Boton");
        SceneController.instance.LoadScene("Menu");
    }

    //QUIT
    public void QuitGame()
    {
        AudioManager.instance.PlaySFX("Boton");
        Application.Quit();
    }
}
