using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Jump_type {climb, jump_long, jump_court, jump_court_cut}            // Multiple jump styles have been tested, with different animations/movements


/** This script will make william jump an obstacle when the player interacts with it
 *  It works around WayPoints: william will move through those waypoints in order to recreate the movement
 *  This system should be revamped to a more autonomous system with Bezier curves, as right now
 *  it requires too much hand tunning
 */
public class InteractionZoneJump : InteractionZone {

    [System.Serializable]
    public class WayPoint
    {
        public float dy, dz, speed;
    }

    [SerializeField] protected Jump_type jump;
    [SerializeField] protected WayPoint[] points;

    public override void TriggerInteraction(GameObject character)
    {
        StartCoroutine(Jump(character.transform));
    }
    
    IEnumerator Jump(Transform character)
    {
        PlayerController player = character.GetComponent<PlayerController>();
        player.movement_input = false;
        player.gameObject.GetComponent<Rigidbody>().useGravity = false;

        yield return player.WaitForIdle();

        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayVoice(SoundManager.Voice_type.ulysse_jump);

        if (jump == Jump_type.jump_court)
        {
            player.Animate("jump_court");
        }
        else if(jump == Jump_type.jump_court_cut)
        {
            player.Animate("jump_court_cut_start");
        }
        else if(jump == Jump_type.jump_long)
        {
            player.Animate("Jump_long");
        }

        for(int i = 0; i < points.Length; i++)
        {
            if (i == points.Length - 1)                     // When the last point approaches, we terminate the animation
            {
                if (jump == Jump_type.jump_court_cut)
                {
                    player.Animate("jump_court_cut_stop");
                }
                else if (jump== Jump_type.jump_long)
                {
                    player.Animate("Jump_end_falling_idle");
                }
            }

            Vector3 pos = character.worldToLocalMatrix * character.position;
            pos = new Vector3(pos.x, pos.y + points[i].dy, pos.z + points[i].dz);
            yield return Utils.Utils.MoveToTarget(character.localToWorldMatrix * pos, points[i].speed, new Vector2(points[i].dy, points[i].dz).magnitude, character);            
        }
        player.gameObject.GetComponent<Rigidbody>().useGravity = true;
        player.movement_input = true;
    }
}