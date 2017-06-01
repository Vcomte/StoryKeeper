using UnityEngine;

/** Script for the fourth sailor of the demo
 *  If william has taken the amphore, the dialogue changes
 *  and the sailor moves away from the path
 */
public class InteractionZoneDSailor : InteractionZoneDialogue {

    [SerializeField] private Utils.dialogue_line[] dialogue_alternate;  // When the required item has been taken, the dialogue changes
    [SerializeField] private GameObject required_item;                  // Required item for william to have on him to continue

    public override void TriggerInteraction(GameObject character)
    {
        if (required_item == GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().held_item && !only_once)
        {
            dialogue = dialogue_alternate;
            only_once = true;
            triggered = false;
        }
        base.TriggerInteraction(character);
    }

    protected override void EndDialogueAction()
    {
        if (required_item == GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().held_item)
        {
            GetComponentInChildren<Animator>().SetBool("move_away", true);
            transform.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("Action_idle", true);
            transform.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("Action_left_turn_inPlace", true);
            EventManager.StopListening("DialogueEnded", EndDialogueAction);
        }
    }

}
