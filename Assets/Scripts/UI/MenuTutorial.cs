using UnityEngine;
using UnityEngine.Events;

public class MenuTutorial : MainMenu {

    void Start()
    {
        menu_open_listener = new UnityAction(OpenMenu);
        menu_close_listener = new UnityAction(CloseMenu);

        //This event is triggered from the PlayerController
        EventManager.StartListening("MenuTutorial", menu_open_listener);

        EventManager.StartListening("Messages", menu_close_listener);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

}
