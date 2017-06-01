using UnityEngine;
using UnityEngine.Events;

public class Extract : MainMenu
{
    void Start()
    {
        menu_open_listener = new UnityAction(OpenMenu);
        menu_close_listener = new UnityAction(CloseMenu);
        EventManager.StartListening("Extract", menu_open_listener);
        EventManager.StartListening("CloseExtract", menu_close_listener);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
}
