// by Ceren Birsu YILMAZ
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private GameSceneManager sceneManager;
    private GameSoundManager soundManager;
    [SerializeField] private Sprite mutedSprite;
    [SerializeField] private Sprite unmutedSprite;
    int count = 0;

    void Start(){
        sceneManager = FindAnyObjectByType<GameSceneManager>();
        soundManager = FindAnyObjectByType<GameSoundManager>();
    }

    // Called when the button is clicked
    private void OnMouseDown(){
        if(this.tag == "Quit"){
            sceneManager.QuitGame();
            Debug.Log("q");
        }
        else if(this.tag == "Restart"){
            sceneManager.RestartGame();
            Debug.Log("r");
        }
        else if(this.tag == "Music"){
            soundManager.ToggleMute();
            UpdateButtonAppearance();
            count++;
            Debug.Log("m");
        }
        Debug.Log("a");
    }

    // Updates the button's sprite based on the toggle counter.
    // Even count uses the mutedSprite; odd count uses the unmutedSprite.
    private void UpdateButtonAppearance()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if(count % 2 == 0){
            sr.sprite = mutedSprite;
        }
        
        else{
            sr.sprite = unmutedSprite;
        }
    }

}
