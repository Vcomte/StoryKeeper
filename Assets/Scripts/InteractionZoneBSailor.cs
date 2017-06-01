using UnityEngine;

/** Script for the second sailor in the demo
 *  it will move away from the path after the dialogue is completed
 */
public class InteractionZoneBSailor : InteractionZoneDialogue {

    protected override void Start()
    {
        base.Start();
    }

    protected override void EndDialogueAction()
    {
        GetComponentInChildren<Animator>().SetBool("move_away", true);
        transform.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("Action_idle", true);
        transform.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("Action_left_turn_inPlace", true);
        EventManager.StopListening("DialogueEnded", EndDialogueAction);
    }
}
