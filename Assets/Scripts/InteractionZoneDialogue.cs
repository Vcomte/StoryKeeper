using UnityEngine;

/** Base class for all dialogues in the game, inherits from Interaction Zone
 * The dialogue is triggered with TriggerInteraction, just like with a regular
 * interaction zone
 * A specific action can take place at the end of the dialogue with the EndDialogueAction.
 */
public class InteractionZoneDialogue : InteractionZone {

    [SerializeField] protected Utils.dialogue_line[] dialogue;
    
    protected override void Start()
    {
        base.Start();
    }

    public override void TriggerInteraction(GameObject character)
    {
        if (!only_once || !triggered)
        {
            character.GetComponent<PlayerController>().dialogue = dialogue;
            EventManager.TriggerEvent("Dialogue");
            triggered = true;
            HideText();
            EventManager.StartListening("DialogueEnded", EndDialogueAction);
        }
    }

    protected virtual void EndDialogueAction()
    {
        EventManager.StopListening("DialogueEnded", EndDialogueAction);
    }
}
