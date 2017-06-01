using UnityEngine;
using UnityEngine.Events;

public class Author : MainMenu {

    void Start()
    {
        menu_open_listener = new UnityAction(OpenMenu);
        menu_close_listener = new UnityAction(CloseMenu);
        EventManager.StartListening("Author", menu_open_listener);
        EventManager.StartListening("CloseAuthor", menu_close_listener);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
}
