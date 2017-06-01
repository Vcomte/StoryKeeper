using UnityEngine;
using UnityEngine.Events;

public class Messages : MainMenu
{
    void Start()
    {
        menu_open_listener = new UnityAction(OpenMenu);
        menu_close_listener = new UnityAction(CloseMenu);
        EventManager.StartListening("Messages", menu_open_listener);
        EventManager.StartListening("CloseMessages", menu_close_listener);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
}
