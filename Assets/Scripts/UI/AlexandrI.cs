using UnityEngine;
using UnityEngine.Events;

public class AlexandrI : MainMenu {

    void Start () {
        menu_open_listener = new UnityAction(OpenMenu);
        menu_close_listener = new UnityAction(CloseMenu);
        EventManager.StartListening("AlexandrI", menu_open_listener);
        EventManager.StartListening("Author", menu_close_listener);
        EventManager.StartListening("Extract", menu_close_listener);
        EventManager.StartListening("Inspirations", menu_close_listener);
        EventManager.StartListening("CloseAlexandrI", menu_close_listener);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
}
