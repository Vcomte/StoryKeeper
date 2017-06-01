using UnityEngine;
using UnityEngine.Events;

public class AlexandriSecondTutorial : MainMenu
{
    void Start()
    {
        menu_open_listener = new UnityAction(OpenMenu);
        menu_close_listener = new UnityAction(CloseMenu);
        EventManager.StartListening("AlexandriSecondTutorial", menu_open_listener);
        EventManager.StartListening("Extract", menu_close_listener);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }                       
}
