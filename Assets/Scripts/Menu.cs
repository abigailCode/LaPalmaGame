using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    public GameObject CreditsPanel;
    //START
    public void StartGame()
    {
        SceneController.instance.LoadScene("Beach");
    }
    //CREDITS
    public void ShowCredits()
    {
        CreditsPanel.SetActive(true);
    }
    
    public void HideCredits()
    {
        CreditsPanel.SetActive(false);
    }
    //QUIT

    public void QuitGame()
    {
        Application.Quit();
    }
}
