using System.Collections;
using UnityEngine;

/** Interaction zone where the player can catch an Amphore from the ground
 *  Synchronisation with the animation requires a precise position for the animation
 *  Hence animation_position should be set with precision
 */
public class InteractionZoneAmphore : InteractionZone {

    private bool _is_held = false;

    [SerializeField] private Vector3 animation_position;
    
    protected override void Start()
    {
        base.Start();
    }


    public override void TriggerInteraction(GameObject character)
    {
        PlayerController player_controller = character.GetComponent<PlayerController>();
        if (!_is_held && player_controller.held_item == null)
        {
            _is_held = true;
            player_controller.held_item = character.transform.GetChild(0).gameObject;
            StartCoroutine(AnimateInteraction(player_controller));

            HideText();
        }
        /** Code prepared in case the player had to drop the item but unused at the moment
        else if( _is_held && player_controller.held_item != null)
        {
            _is_held = false;
            player_controller.held_item = null;
            transform.position = transform.position - new Vector3(0, 0, 2.5f);
            transform.SetParent(null);
        }
        */
    }

    private IEnumerator AnimateInteraction(PlayerController player)
    {
        player.movement_input = false;

        float sign = player.orientation_right ? 1f : -1f;
        player.RotateWilliam(sign * 90f, 60f);
        yield return Utils.Utils.MoveToTarget(animation_position, 1f, Vector3.Distance(animation_position, player.transform.position), player.transform);

        yield return new WaitForSeconds(1f);
        yield return player.WaitForIdle();

        player.hand_item = transform.GetChild(1);
        player.hand_position = new Vector3(0.066f, 0.035f, 0.018f);         // The values were determined by hand testing
        player.hand_euler = new Vector3(-33.099f, 123.11f, -161.2f);        // The values were determined by hand testing
        player.Animate("Lift_outre");

        yield return new WaitForSeconds(1f);
        yield return player.WaitForIdle();

        player.RotateWilliam(sign * -90f, 60f);

        yield return new WaitForSeconds(1f);
        yield return player.WaitForIdle();

        player.movement_input = true;
        gameObject.SetActive(false);
    }
}
