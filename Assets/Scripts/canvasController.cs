using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class canvasController : MonoBehaviour
{
    public playerController playerController;
    public TextMeshProUGUI textHealth;
    public TextMeshProUGUI textGameOver;
    public Image gameOverScreen;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //if the player has died
        textHealth.text = playerController.playerHealth.ToString();
        if(playerController.playerHealth <= 0 )
        {
            textGameOver.text = "ur ded";
            gameOverScreen.enabled = true;
            textGameOver.enabled = true;
        }
        //if the player has reached the exit
        else if(playerController.GameWin)
        {
            textGameOver.text = "u win :)";
            gameOverScreen.enabled = true;
            textGameOver.enabled = true;
        }
        //otherwise keep from drawing screen
        else
        {
            gameOverScreen.enabled = false;
            textGameOver.enabled = false;
        }
    }
}
