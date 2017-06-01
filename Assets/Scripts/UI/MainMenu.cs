using System.Collections;
using UnityEngine;
using UnityEngine.Events;


/**This class is the base class for all Menu related elements
 * Every menu will have an Open and a Close action. Each bound to Events.
 * Each child class should override the Start method to use the right Events
 */
public class MainMenu : MonoBehaviour {

    protected UnityAction menu_open_listener;
    protected UnityAction menu_close_listener;
    protected PlayerController player;

	void Start ()
    {
        menu_open_listener = new UnityAction(OpenMenu);
        menu_close_listener = new UnityAction(CloseMenu);
        EventManager.StartListening("AlexandrI", menu_close_listener);
        EventManager.StartListening("Messages", menu_close_listener);
        EventManager.StartListening("Options", menu_close_listener);
        EventManager.StartListening("MainMenu", menu_open_listener);
        EventManager.StartListening("CloseMainMenu", menu_close_listener);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
	
    protected virtual void OpenMenu ()
    {
        DisablePlayerMovement();
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    protected virtual void CloseMenu()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }


    /** These functions can be called from the UI elements, like buttons
     *  They can trigger events, disable or enable player movement
     *  and play UI sounds
     */
    public void DisablePlayerMovement()
    {
        player.movement_input = false;
    }

    public void EnablePlayerMovement()
    {
        player.movement_input = true;
    }

    public void InstanceTriggerEvent(string eventName)
    {
        StartCoroutine(TriggerDelayedEvent(eventName));
    }

    IEnumerator TriggerDelayedEvent(string eventName)
    {
        yield return new WaitForSeconds(0.3f);
        EventManager.TriggerEvent(eventName);
    }

    public void PlayGoUI()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayUI(SoundManager.UI_type.go);
    }

    public void PlayBackUI()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayUI(SoundManager.UI_type.back);
    }
}
