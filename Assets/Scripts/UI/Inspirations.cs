using UnityEngine;
using UnityEngine.Events;

public class Inspirations : MainMenu
{

    void Start()
    {
        menu_open_listener = new UnityAction(OpenMenu);
        menu_close_listener = new UnityAction(CloseMenu);
        EventManager.StartListening("Inspirations", menu_open_listener);
        EventManager.StartListening("CloseInspirations", menu_close_listener);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
}
