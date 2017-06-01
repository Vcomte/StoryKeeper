using UnityEngine;
using UnityEngine.Events;

/** This script manages the old introduction to the game
 *  Now see Introduction script to see how the introduction to the game is managed
 */
public class WelcomePopUp : MainMenu {

	void Start () {
        menu_open_listener = new UnityAction(OpenMenu);
        menu_close_listener = new UnityAction(CloseMenu);
        EventManager.StartListening("WelcomePopUp", menu_open_listener);
        EventManager.StartListening("CloseWelcomePopUp", menu_close_listener);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        DisablePlayerMovement();
    }

    //Functions callable from UI elements, like buttons
    public void TriggerSplashScreen()
    {
        GetComponent<Animator>().SetBool("SplashScreen", true);
    }
   
    public void StartAudio()
    {
        Transform audio_system = GameObject.FindGameObjectWithTag("SoundSystem").transform;
        audio_system.GetChild(0).gameObject.GetComponent<AudioSource>().Play();
        audio_system.GetChild(1).gameObject.GetComponent<AudioSource>().Play();
    }
}
