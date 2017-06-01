using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/** This class manages an area that will trigger a Tutorial when triggered
 *  The tutorial expires after a time_playing amount of time or (and if specified)
 *  when event_awaited event is triggered. The tutorials was made a list initially
 *  to play different parts simultaneously, to represent the multitouch.
 */
public class TutoZone : MonoBehaviour {

    [SerializeField] protected float time_playing;
    [SerializeField] protected Tuto_type[] tutorials;
    [SerializeField] protected string event_awaited;
    [SerializeField] protected bool is_first_tutorial;

    protected bool running = false;
    protected bool _has_expired = false;
    protected float count = 0;

    protected UnityAction action_triggered;

    public bool has_expired { get { return _has_expired; } set { _has_expired = value; } }

    protected virtual void Start()
    {
        if(event_awaited != "")
        {
            action_triggered = new UnityAction(EventTrigger);
            EventManager.StartListening(event_awaited, action_triggered);
        }
    }

    //These functions can be called outside, to make more evolved tutorial interactions
    public virtual void StartTuto()
    {
        GameObject.FindGameObjectWithTag("TutoUI").GetComponent<Tuto>().tutorials = tutorials;
        EventManager.TriggerEvent("Tuto");
        StartCoroutine(TutoRunning());
    }

    public virtual void EndTuto()
    {
        EventManager.TriggerEvent("CloseTuto");
        if (running)
        {
            StopCoroutine(TutoRunning());
            running = false;
            count = 0f;
        }
    }

    protected void EventTrigger()
    {
        _has_expired = true;

    }

    protected virtual IEnumerator TutoRunning()
    {
        running = true;
        while(count < time_playing && !has_expired)
        {
            count += Time.deltaTime;
            yield return null;
        }
        _has_expired = true;
        running = false;
        EventManager.StopListening(event_awaited, action_triggered);
        EndTuto();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !_has_expired)
        {
            //First tutorial will be triggered after the introduction rather than by collision
            if (is_first_tutorial)
            {
                is_first_tutorial = false;
            }
            else
            {
                StartTuto();
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            EndTuto();
        }
    }
}
