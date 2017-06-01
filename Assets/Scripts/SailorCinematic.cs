using UnityEngine;

/** This script gives external scripts the tools needed to animate the sailors
 *  especially for the cavern entrance cinematic
 */
public class SailorCinematic : MonoBehaviour {

    private Animator model_animator;

    private bool running = false;
    private bool action_idle = false;
    private bool turning = false;
    private bool fear = false;

	// Use this for initialization
	void Start () {
        model_animator = transform.GetChild(0).GetComponent<Animator>();
	}
	
    public void ToggleActionIdle()
    {
        action_idle = !action_idle;
        model_animator.SetBool("Action_idle", action_idle);
    }

    public void ToggleRun()
    {
        if (!action_idle)
        {
            ToggleActionIdle();
        }
        running = !running;
        model_animator.SetBool("Jog_forward", running);
    }

    public void ToggleTurning()
    {
        if (action_idle)
        {
            ToggleActionIdle();
        }

        turning = !turning;
        model_animator.SetBool("Dialogue_right_turn_inPlace", turning);
    }

    public void ToggleFear()
    {
        if (action_idle)
        {
            ToggleActionIdle();
        }
        if (fear)
        {
            model_animator.SetTrigger("Stop_walkbackward");
            fear = false;
        }
        else
        {
            model_animator.SetTrigger("Reacting_fear");
            fear = true;
        }
    }

    public void ToggleRunnaway() {
        if (action_idle)
        {
            ToggleActionIdle();
        }
            model_animator.SetTrigger("Running_fear_start");
    }
}
