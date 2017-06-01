using UnityEngine;

/** Old script for testing, not used anymore, except on an old test scene
 */
public class InteractionZoneMechanism : InteractionZone {

    [SerializeField] private GameObject _missing_part;
    [SerializeField] private GameObject _bridge;
    private bool _mechanism_triggered = false;

    public override void TriggerInteraction(GameObject character)
    {
        PlayerController player_controller = character.GetComponent<PlayerController>();
        if (!_mechanism_triggered && player_controller.held_item != null)
        {
            _missing_part.GetComponent<MeshRenderer>().enabled = true;
            //player_controller.held_item.GetComponent<InteractionZoneAmphore>().has_activated = true;
            player_controller.held_item = null;

            _mechanism_triggered = true;
            GetComponentInChildren<Animator>().SetBool("down", true);
        }
    }
}
