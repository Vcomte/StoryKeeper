using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This is the base class for all interaction zones
 *  An interaction zone can have an icon to display when the player is nearby
 *  Every interaction zone has a public method that other scripts can use to trigger
 *  their interaction
 */
public class InteractionZone : MonoBehaviour {
    [SerializeField] protected bool has_icon;
    [SerializeField] protected bool only_once = false;
    protected bool triggered = false;

    protected virtual void Start()
    {
        HideText();
    }

    public virtual void TriggerInteraction(GameObject character)
    {
        return;
    }

    public void DisplayText()
    {
        if ((!only_once || !triggered) && has_icon)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void HideText()
    {
        if (has_icon)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
