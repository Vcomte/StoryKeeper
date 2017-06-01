using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


/** This class manages the whole fight with the cyclope,
 *  as well as the ending of the demo
 */ 
public class CyclopsDodgeFight : InteractionZoneDialogue {

    private Transform player;
    private bool _fight_start = false;
    private bool dodge_coroutine_launched = false;

    private bool _strike_finished = false;      //To know if the cyclops can move
    private int dodge_count = 0;        //To know the advancement of the fight
    private bool hard_strike = false;       //To know if the attack pattern changed
    private bool strike_landed = false;     //To know if the player was hit

    private Animator cyclops_animator;

    private UnityAction strike_touched;

    #region External elements
    [SerializeField] private Image black_panel;
    [SerializeField] private AnimationClip idle_clip;
    [SerializeField] private Image end_screen;
    #endregion

    private Vector3 saved_cyclops_position;
    private Vector3 _saved_player_position;

    #region Accessors
    public Vector3 saved_player_position { set { _saved_player_position = value; } }
    public bool strike_finished { get { return _strike_finished; } set { _strike_finished = value; } }
    public bool fight_start { get { return _fight_start; } set { _fight_start = value; } }
    #endregion

    protected new void Start()
    {
        // Base Start is ignored
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cyclops_animator = transform.GetChild(1).gameObject.GetComponent<Animator>();
        strike_touched = new UnityAction(StrikeTouched);
        EventManager.StartListening("StrikeTouched", strike_touched);
    }

    /** The script stands ready to start the fight when needed
     *  This should be changed ASAP to an Event triggered at the end
     *  of the cinematic
     */
    void FixedUpdate () {
        if (_fight_start && !dodge_coroutine_launched && !strike_landed)
        {
            dodge_coroutine_launched = true;
            StartCoroutine(DodgeFight());
            saved_cyclops_position = transform.position;
            player.gameObject.GetComponent<PlayerController>().fighting = true;
        }
	}

    void ActivatePlayer()
    {
        player.gameObject.GetComponent<PlayerController>().movement_input = true;
        player.gameObject.GetComponent<BoxCollider>().isTrigger = false;
        player.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    /** This is the main coroutine that controls the cyclops for the duration of the fight
     *  The fight is set to ONE SHOT the player as it is. hence the fight ending when strike_landed is true
     */
    IEnumerator DodgeFight()
    {
        float speed = 5f;

        while(dodge_count < 6 && !strike_landed)
        {
            if(dodge_count == 3)
            {
                speed = 5f;
                hard_strike = true;
            }

            yield return WaitForIdle();

            //If the player is not at range, the cyclops moves to reach the player current position
            Vector3 player_target = new Vector3(player.position.x, transform.position.y, player.position.z + 3f);
            Vector3 distance_vector = transform.InverseTransformPoint(player_target);
            if (Mathf.Abs(distance_vector.x) > 2f)
            {
                cyclops_animator.SetBool("Attaque_walk", true);
                transform.forward = transform.TransformDirection(new Vector3(distance_vector.x, 0f, 0f));
                yield return Utils.Utils.MoveToTarget(player_target, speed, distance_vector.magnitude, transform);
                cyclops_animator.SetBool("Attaque_walk", false);
                transform.forward = transform.right * (distance_vector.x >= 0 ? -1f : 1f);
            }
            
            // Prepares to strike
            _strike_finished = false;
            strike_landed = false;
            if(hard_strike)
            {
                cyclops_animator.SetTrigger("Attaque_main");
            }
            else
            {
                cyclops_animator.SetTrigger("Attaque_pied_v1");
            }
            //Wait for strike to end
            while (!_strike_finished && !strike_landed)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // The strike touched the player, and so it ends the fight
            if (strike_landed)
            {
                //player.gameObject.GetComponent<PlayerController>().Stun();      //Player receives the hit
                cyclops_animator.SetTrigger("Attaque_idle_flexing_muscles");    //Small taunt from the cyclops
                yield return new WaitForSeconds(4f);

                // The cyclops moves in position to eat the player
                player_target = new Vector3(player.position.x - 0.05f, transform.position.y, player.position.z + 0.856f);
                distance_vector = transform.InverseTransformPoint(player_target);

                cyclops_animator.SetBool("Attaque_walk", true);
                yield return Utils.Utils.MoveToTarget(player_target, speed, distance_vector.magnitude, transform);
                cyclops_animator.SetBool("Attaque_walk", false);

                // Eat the player
                transform.GetChild(1).GetComponent<Cyclops>().sailor_to_eat = player.gameObject;
                cyclops_animator.SetBool("Attaque_idle_base", false);

                cyclops_animator.SetTrigger("Attraper_marin");

                StartCoroutine(RestartFight());     // Restarts the fight
            }
            else
            {
                ++dodge_count;      //The player has dodged the attack, the fight goes on
                yield return new WaitForSeconds(1f);
            }
        }

        // If the player has dodged enough strikes, the player wins the fight and a new cinematic is triggered
        if (!strike_landed)
        {
            EventManager.TriggerEvent("CinematicFrame");
            PlayerController player_controller = player.GetComponent<PlayerController>();
            player_controller.movement_input = false;

            //The strike dodging is over. Now for the charge
            Vector3 charge_position = new Vector3(100.66f, 1.3f, -23.27f);
            Vector3 player_position = new Vector3(91.85f, 1.3f, -25.57f);
            yield return WaitForIdle();
            yield return player_controller.WaitForIdle();
            player_controller.cinematic = true;

            // The cyclops moves into position
            transform.LookAt(charge_position);
            cyclops_animator.SetBool("Attaque_walk", true);
            yield return Utils.Utils.MoveToTarget(charge_position, speed, Vector3.Distance(charge_position, transform.position), transform);
            cyclops_animator.SetBool("Attaque_walk", false);
            transform.LookAt(player);

            // The player moves into position
            player.LookAt(player_position);
            player_controller.Animate("Jog_forward", true);
            yield return Utils.Utils.MoveToTarget(player_position, speed, Vector3.Distance(player_position, player.position), player);
            player_controller.Animate("Jog_forward", false);
            player.LookAt(charge_position);

            transform.LookAt(player);
            cyclops_animator.SetTrigger("Attaque_idle_roaring");
            yield return new WaitForSeconds(6f);

            //Charging
            cyclops_animator.SetBool("Attaque_foncer", true);
            StartCoroutine(Utils.Utils.MoveToTarget(player_position, 3f, Vector3.Distance(player_position, transform.position), transform));
            player_controller.Animate("Jog_forward", true);
            StartCoroutine(Utils.Utils.MoveToTarget(charge_position, 3f, Vector3.Distance(charge_position, player.position), player));
            player_controller.Animate("Jog_forward", true);
            StartCoroutine(EndGame(player_controller));     // Ends the demo
        }
        dodge_coroutine_launched = false;
    }

    public void StrikeTouched()
    {
        strike_landed = true;
        player.gameObject.GetComponent<PlayerController>().Stun();
    }

    /** This coroutine is called when the player loses the fight
     *  It will reset all positions and variables to restart the fight
     */
    IEnumerator RestartFight()
    {
        yield return new WaitForSeconds(4f);

        float time = 0f;
        
        while(time < 1f)
        {
            time += Time.deltaTime / 2f;
            black_panel.color = new Color(black_panel.color.r, black_panel.color.g, black_panel.color.b, time);
            yield return null;
        }

        player.gameObject.GetComponent<Animator>().ResetTrigger("Ecrasement_pied_poing");
        yield return new WaitForSeconds(2f);

        hard_strike = false;
        _strike_finished = false;
        dodge_count = 0;
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayMusic(SoundManager.Music_type.attack);
        player.gameObject.SetActive(true);
        player.parent = null;
        player.position = _saved_player_position;
        player.rotation = Quaternion.Euler(0f, 71f, 0f);
        transform.position = saved_cyclops_position;
        cyclops_animator.SetBool("Attaque_idle_base", true);

        yield return player.GetComponent<PlayerController>().WaitForIdle();

        player.GetComponent<PlayerController>().movement_input = true;

        while (time > 0f)
        {
            time -= Time.deltaTime / 2f;
            black_panel.color = new Color(black_panel.color.r, black_panel.color.g, black_panel.color.b, time);
            yield return null;
        }

        //Restarts the fight
        strike_landed = false;
    }

    /** This coroutine manages the end of the demo, during the cyclops/william charge
     *  At the end of the slow motion, the end screen is displayed, and after 10 seconds, the demo restarts
     */
    private IEnumerator EndGame(PlayerController player_controller)
    {
        float time = 1f;
        while(time > 0.1f)
        {
            player_controller.Animate("Jog_forward", true);      //Some chenanigans forces us to reset the jog_forward to true

            Time.timeScale = time;
            time -= Time.unscaledDeltaTime / 2f;
            yield return null;
        }
        time = Time.timeScale = 0.1f;

        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayMusic(SoundManager.Music_type.empty);
        while(time < 1f)
        {
            player_controller.Animate("Jog_forward", true);     //Some shenanigans forces us to reset the jog_forward to true

            time += Time.unscaledDeltaTime / 3f;
            end_screen.color = new Color(1f, 1f, 1f, time);
            yield return null;
        }
        player_controller.Animate("Jog_forward", true);      //Some shenanigans forces us to reset the jog_forward to true
        yield return new WaitForSecondsRealtime(10f);

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
    }

    public IEnumerator WaitForIdle()
    {
        while (cyclops_animator.GetCurrentAnimatorClipInfo(0)[0].clip != idle_clip)
        {
            yield return new WaitForSeconds(0.2f);
        }
    }
}
