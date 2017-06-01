using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum Control_type { swipe, joypad}
public enum Control_state { empty, right, left, interact, jump, hold}

// This is the Caharacter controller class
public class PlayerController : MonoBehaviour {

    protected float screen_width = 0f;
    protected float screen_height = 0f;

    protected Rigidbody player_rigidbody;

    [SerializeField] protected Control_type player_control;
    protected Control_state[] touch_control_state;

    #region Movement area
    //[SerializeField] protected float jump_velocity = 10f;
    [SerializeField] protected float horizontal_velocity = 5f;
    protected bool is_grounded = true;
    protected float movement_ratio = 1000f;

    protected bool go_right = false;
    protected bool go_left = false;
    protected bool interact = false;
    //protected bool jump = false;
    protected bool _orientation_right = true;

    protected float x_velocity;
    protected float y_velocity;
    public float _rotation = 0f;

    protected bool _movement_input = true;
    #endregion

    #region Interaction
    protected InteractionZone current_interaction_zone = null;
    protected float interaction_cooldown = 0f;

    protected float touch_hold_duration = 1.5f;
    protected float touch_hold_count = 0f;
    protected bool touch_held = false;

    protected Utils.dialogue_line[] _dialogue;
    #endregion

    #region Animation
    protected Animator animator;
    [SerializeField] AnimationClip idle_clip;
    protected UnityAction menu_close;
    protected bool _cinematic = false;
    #endregion

    #region Tutorial
    [SerializeField] TutoZone first_tutorial;
    protected bool _menu_tutorial_running = false;
    protected bool _alexandri_tutorial_running = false;
    protected UnityAction player_awaking;
    protected bool blinking = false;
    #endregion

    #region Specific level interaction
    protected GameObject _held_item;
    [SerializeField] private Transform hand;
    protected Transform _hand_item;
    protected Vector3 _hand_position;
    protected Vector3 _hand_euler;
    protected bool stun_coroutine = false;
    protected bool _fighting = false;
    #endregion

    #region Accessors
    public GameObject held_item { get { return _held_item; } set { _held_item = value; } }
    public float rotation { get { return _rotation; } set { _rotation = value; } }
    public Utils.dialogue_line[] dialogue { get { return _dialogue; } set { _dialogue = value; } }
    public bool movement_input { get { return _movement_input;} set { _movement_input = value; } }
    public bool menu_tutorial_running { get { return _menu_tutorial_running; } set { _menu_tutorial_running = value; } }
    public bool alexandri_tutorial_running { get { return _alexandri_tutorial_running; } set { _alexandri_tutorial_running = value; } }
    public bool orientation_right { get { return _orientation_right; } set { _orientation_right = value; } }
    public bool fighting { get { return _fighting; } set { _fighting = value; } }
    public bool cinematic { get { return _cinematic; } set { _cinematic = value; } }
    public Transform hand_item { get { return _hand_item; } set { _hand_item = value; } }
    public Vector3 hand_position { get { return _hand_position; } set { _hand_position = value; } }
    public Vector3 hand_euler { get { return _hand_euler; } set { _hand_euler = value; } }
    #endregion

    protected void Start()
    {
        screen_width = Screen.width;
        screen_height = Screen.height;
        touch_control_state = new Control_state[10];

        player_awaking = new UnityAction(PlayerAwake);
        EventManager.StartListening("CloseIntroduction", player_awaking);

        menu_close = new UnityAction(OnMenuClose);
        EventManager.StartListening("CloseMainMenu", menu_close);

        _held_item = null;

        animator = GetComponent<Animator>();
        player_rigidbody = GetComponent<Rigidbody>();
    }
    
    protected void JoypadInput(out bool go_right, out bool go_left, out bool interact)
    {
        go_right = false;
        go_left = false;
        interact = false;
        foreach (Touch touch in Input.touches)
        {
            if(touch.position.y / screen_height < 0.2f)
            {   
                if (touch.position.x / screen_width > 0.8f)
                {
                    go_right = true;
                }
                else if (touch.position.x / screen_width < 0.2f)
                {
                    go_left = true;
                }
            }
            if (touch.position.y / screen_height > 0.25f && touch.position.y / screen_height < 0.5f
                   && (touch.position.x / screen_width > 0.8f || touch.position.x / screen_width < 0.2f))
            {
                interact = true;
            }
        }
    }

    protected void SwipeInput()
    {
        go_right = false;
        go_left = false;
        
        foreach(Touch touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Began)
            {
                touch_control_state[touch.fingerId] = Control_state.empty;
            }
            else if(touch.phase == TouchPhase.Moved && touch_control_state[touch.fingerId] != Control_state.interact && touch_control_state[touch.fingerId] != Control_state.jump)
            {
                if (touch.deltaPosition.x/touch.deltaTime > movement_ratio)
                {
                    touch_control_state[touch.fingerId] = Control_state.right;
                }
                else if (touch.deltaPosition.x/touch.deltaTime < -movement_ratio)
                {
                    touch_control_state[touch.fingerId] = Control_state.left;
                }
                /**else if (touch.deltaPosition.y/touch.deltaTime > movement_ratio && touch_control_state[touch.fingerId] != Control_state.right && touch_control_state[touch.fingerId] != Control_state.left
                    && touch_control_state[touch.fingerId] != Control_state.interact)
                {
                    touch_control_state[touch.fingerId] = Control_state.jump;
                }*/
            }
            /** The hold touch detection should be revamped, as multiple touches can increase the count simultaneously
             *  Also, every 'empty' touch will increase the count, hence forcing us to reset the count everytime an interact
             *  is registered.
             */
            else if (touch.phase == TouchPhase.Stationary && touch_control_state[touch.fingerId] == Control_state.empty)
            {
                if(touch_hold_count >= touch_hold_duration)
                {
                    touch_control_state[touch.fingerId] = Control_state.hold;
                    touch_hold_count = 0f;
                }
                else
                {
                    touch_hold_count += Time.deltaTime; 
                }
            }
            else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (touch_control_state[touch.fingerId] != Control_state.empty)
                {
                    touch_control_state[touch.fingerId] = Control_state.empty;
                }
                else
                {
                    touch_control_state[touch.fingerId] = Control_state.interact;
                    touch_hold_count = 0f;
                }
            }
        }
        for(int i = 0; i < 10; i++)
        {
            if(touch_control_state[i] == Control_state.right)
            {
                go_right = true;
            }
            else if(touch_control_state[i] == Control_state.left)
            {
                go_left = true;
            }
            else if(touch_control_state[i] == Control_state.interact)
            {
                interact = true;
                touch_control_state[i] = Control_state.empty;
            }
            /**else if(touch_control_state[i] == Control_state.jump && is_grounded)
            {
                jump = true;
                touch_control_state[i] = Control_state.empty;
            }*/
            else if(touch_control_state[i] == Control_state.hold){
                touch_held = true;
                touch_control_state[i] = Control_state.empty;
            }
        }
    }

    protected void FlushControls()
    {
        for(int i = 0; i < touch_control_state.Length; i++)
        {
            touch_control_state[i] = Control_state.empty;
        }
    }

    // Getting player input
    protected void Update()
    {
        if (_movement_input)
        {
            if (player_control == Control_type.joypad)
            {
                JoypadInput(out go_right, out go_left, out interact);
            }
            else if (player_control == Control_type.swipe)
            {
                SwipeInput();
            }
        }
        else
        {
            go_right = false;
            go_left = false;
            interact = false;
            //jump = false;
            touch_held = false;
            FlushControls();
        }
    }

    // Using player input to move/interact/open menu
    protected void FixedUpdate() {

        // Movement
        x_velocity = 0;

        if (!_menu_tutorial_running && !_alexandri_tutorial_running) {
            if (go_right)
            {
                x_velocity = horizontal_velocity;
                orientation_right = true;
            }
            else if (go_left)
            {
                x_velocity = -1f * horizontal_velocity;
                orientation_right = false;
            }

            y_velocity = player_rigidbody.velocity.y;

            //Triggering interaction
            if (interact && current_interaction_zone != null && interaction_cooldown <= 0f)
            {
                //The interaction might prevent movement input for the duration of the interaction
                current_interaction_zone.TriggerInteraction(this.gameObject);
                interaction_cooldown = 2f;
                interact = false;
            }
            interact = false;

            // Managing interaction cooldown
            if (interaction_cooldown > 0f)
            {
                interaction_cooldown -= Time.fixedDeltaTime;
            }
        }

        //Preventing menu opening when doing an other action
        if (touch_held && !go_left && !go_right && !_fighting)
        {
            GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayUI(SoundManager.UI_type.open);
            //When the menu tutorial is running, displays the tutorial windows instead
            if (menu_tutorial_running)
            {
                EventManager.TriggerEvent("MenuTutorial");
                blinking = false;
            }
            else if (alexandri_tutorial_running)
            {
                EventManager.TriggerEvent("AlexandriTutorial");
                blinking = false;
            }
            else
            {
                EventManager.TriggerEvent("MainMenu");
                Animate("Touch_bracelet_start");
            }
            touch_held = false;
        }

        /** Simple jump  -- Deprecated since Jump with interaction zone
         *
         * if (jump && _is_grounded)
         * {
         *    _y_velocity += _jump_velocity;
         *    jump = false;
         *    _is_grounded = false;
         * } 
         * jump = false;
         */

        // Adding velocity
        player_rigidbody.velocity = Quaternion.Euler(0, _rotation, 0) * new Vector3(x_velocity, y_velocity);

        //If in movement: setting lookat and run animation
        if(x_velocity != 0)
        {
            transform.LookAt(Utils.Utils.RotatePointAroundPivot(transform.position + new Vector3(x_velocity * 10f, transform.position.y), transform.position,
                new Vector3(0, _rotation, 0)));
            animator.SetBool("Jog_forward", true);
        }
        else if(!_cinematic)                                        // In case of cinematic, we leave the animator controls to other scripts
        {
            animator.SetBool("Jog_forward", false);
        }
    }

    #region Animation function + rotation
    // William waking up at the begining of the level
    protected void PlayerAwake()
    {
        SoundManager audio = GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>();
        audio.PlayMusic(SoundManager.Music_type.forest);
        audio.PlayBackground(SoundManager.Background_type.forest_background);

        StartCoroutine(PlayerAwaking());
        EventManager.StopListening("CloseWelcomePopUp", player_awaking);
    }

    protected IEnumerator PlayerAwaking()
    {
        animator.SetTrigger("Start_standing_up");
        yield return WaitForIdle();
        movement_input = true;
        first_tutorial.StartTuto();
    }
    
    // William is thrown on the ground after being hit
    public void Stun()
    {
        if (!stun_coroutine)
        {
            StartCoroutine(Stunned());
        }
    }

    protected IEnumerator Stunned()
    {
        stun_coroutine = true;
        movement_input = false;
        animator.SetTrigger("Ecrasement_pied_poing");
        yield return new WaitForSeconds(1.2f);
        animator.SetTrigger("Ecrasement_standing");
        yield return WaitForIdle();
        stun_coroutine = false;
        /** Now we make the cyclop eat William if hit by a strike
        movement_input = true;
        */
    }

    public void Animate(string parameter)
    {
        animator.SetTrigger(parameter);
    }

    public void Animate(string parameter, bool boolean)
    {
        animator.SetBool(parameter, boolean);
    }

    public IEnumerator WaitForIdle()
    {
        while(animator.GetCurrentAnimatorClipInfo(0)[0].clip != idle_clip)
        {
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnMenuClose()
    {
        Animate("Touch_bracelet_stop");
    }

    // Shining effect with William's wristlet
    public void StartBlinking()
    {
        blinking = true;
        StartCoroutine(Blinking());
    }

    public IEnumerator Blinking()
    {
        SpriteRenderer one = transform.GetChild(1).GetComponent<SpriteRenderer>();
        SpriteRenderer two = transform.GetChild(2).GetComponent<SpriteRenderer>();
        float time = 0f;
        float sign = 1f;
        while (blinking)
        {
            time += Time.deltaTime * sign;
            if (time > 1f || time < 0f)
            {
                sign = -sign;
                time += Time.deltaTime * sign;
            }
            one.color = two.color = new Color(one.color.r, one.color.g, one.color.b, time);
            yield return null;
        }
        one.color = two.color = new Color(one.color.r, one.color.g, one.color.b, 0f);
    }

    //Rotation used for animations or cinematics
    public void RotateWilliam(float angle, float speed)
    {
        StartCoroutine(RotatingWilliam(angle, speed));
    }

    IEnumerator RotatingWilliam(float angle, float speed)
    {  
        bool reset_dialogue = false;
        if (animator.GetBool("Dialogue_idle"))
        {
            reset_dialogue = true;
            animator.SetBool("Dialogue_idle", false);
        }
        animator.SetBool("Action_left_turn_inPlace", true);

        yield return Utils.Utils.RotateAroundSelf(transform.rotation, angle, speed, transform);

        animator.SetBool("Action_left_turn_inPlace", false);
        if (reset_dialogue)
        {
            animator.SetBool("Dialogue_idle", true);
        }
    }
    #endregion

    #region Object carrying functions
    public void AttachObjectToHand()
    {
        _hand_item.SetParent(hand);
        _hand_item.localPosition = _hand_position;
        _hand_item.localRotation = Quaternion.Euler(_hand_euler);
    }

    public void DestroyFromHand()
    {
        _hand_item.SetParent(null);
        _hand_item.gameObject.SetActive(false);
        _hand_item = null;
    }

    public void ToggleOutre()
    {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
    }
    #endregion

    #region Collision functions
    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            is_grounded = true;
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && Mathf.Abs(y_velocity) > 0.1f )
        {
            is_grounded = false;
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "InteractionZone")
        {
            current_interaction_zone = other.gameObject.GetComponent<InteractionZone>();
            current_interaction_zone.DisplayText();
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "InteractionZone")
        {
            current_interaction_zone.HideText();
            current_interaction_zone = null;
        }
    }
    #endregion

    #region Sound functions for animation
    protected void PlayInteract()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayNoise(SoundManager.Noise_type.ulysse_interact);
    }
    #endregion

    //Not used anymore
    protected virtual void DetermineVelocity(float sign, out float _x_velocity)
    {
        _x_velocity = sign * horizontal_velocity;
    }
}