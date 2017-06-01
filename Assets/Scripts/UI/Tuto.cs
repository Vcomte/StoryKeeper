using UnityEngine;
using UnityEngine.Events;

public enum Tuto_type { move, jump, interact, open_menu }


/** This script manages the UI tutorials opening and closing
 *  As the tutorial UI has multiple tutorials, this script will only open
 *  the ones specified in the _tutorials list.
 */
public class Tuto : MainMenu
{

    [SerializeField] Tuto_type[] _tutorials;

    public Tuto_type[] tutorials { get { return _tutorials;} set { _tutorials = value; } }

    void Start()
    {
        menu_open_listener = new UnityAction(OpenMenu);
        menu_close_listener = new UnityAction(CloseMenu);
        EventManager.StartListening("Tuto", menu_open_listener);
        EventManager.StartListening("CloseTuto", menu_close_listener);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    protected override void OpenMenu()
    {
        foreach(Tuto_type tuto in _tutorials)
        {
            transform.GetChild((int)tuto).gameObject.SetActive(true);
        }
    }

    protected override void CloseMenu()
    {
        foreach(Tuto_type tuto in _tutorials)
        {
            transform.GetChild((int)tuto).gameObject.SetActive(false);
        }
    }
}