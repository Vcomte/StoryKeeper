using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


//The DialogueController will control the dialogue UI parts after receiving instructions from the PlayerController
public class DialogueController : MonoBehaviour {

    private UnityAction dialogue_listener;
    private bool coroutine_running = false;
    private bool coroutine_checker = false;
    private PlayerController player;
    private bool touch_caught = false;
    private SoundManager sound_manager;

    [SerializeField] private Text text_dialogue;
    [SerializeField] private float initial_time = 2.0f;
    [SerializeField] private float character_time_factor = 0.1f;

    // Use this for initialization
    void Start () {
        dialogue_listener = new UnityAction(StartDialogue);
        EventManager.StartListening("Dialogue", dialogue_listener);
        GameObject player_object = GameObject.FindGameObjectWithTag("Player");
        player = player_object.GetComponent<PlayerController>();
        sound_manager = GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>();
	}

    void Update()
    {
        //Stops the coroutine if the work is finished and triggers the end of the dialogue
        if(coroutine_checker != coroutine_running)
        {
            if(!coroutine_running)
            {
                StopCoroutine("Dialogue");
                EventManager.TriggerEvent("DialogueEnded");
            }
            coroutine_checker = coroutine_running;
        }
        //Catch touch input to pass dialogue if needed
        if(coroutine_running)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.phase == TouchPhase.Ended)
                {
                    touch_caught = true;
                }
            }
            
        }
    }
	
    //Function called when the StartDialogue event is triggered
	void StartDialogue()
    {
        if(!coroutine_running)
        {
            coroutine_running = true;
            StartCoroutine("Dialogue");
        }
    }

    IEnumerator Dialogue()
    {
        player.movement_input = false;
        transform.GetChild(0).gameObject.SetActive(true);
   
        player.gameObject.GetComponent<Animator>().SetBool("Dialogue_idle", true);

        for(int i = 0; i < player.dialogue.Length; i++)
        {
            text_dialogue.text = player.dialogue[i].speaker + "  :  " + player.dialogue[i].line;

            //If there is a voiceline, plays it
            if (!player.dialogue[i].to_play.Equals(SoundManager.Voice_type.empty))
            {
                sound_manager.PlayVoice(player.dialogue[i].to_play);
            }

            //If an animation parameter as been selected, sets it to the given gameObject
            if(player.dialogue[i].parameter_type != Utils.ParameterType.empty)
            {
                if (player.dialogue[i].parameter_type == Utils.ParameterType.boolean)
                {
                    player.dialogue[i].to_animate.GetComponent<Animator>().SetBool(player.dialogue[i].parameter, player.dialogue[i].value);
                }
                else
                {
                    player.dialogue[i].to_animate.GetComponent<Animator>().SetTrigger(player.dialogue[i].parameter);
                }
            }

            //Time buffer before being able to change text
            yield return new WaitForSeconds(1f);
            touch_caught = false;

            //Waiting before going to next text or catching a touch to pass
            for (float wait_time = 0f; wait_time < initial_time + player.dialogue[i].line.Length * character_time_factor; wait_time += Time.deltaTime)
            {
                yield return new WaitForSeconds(0.1f);

                if (touch_caught)
                {
                    touch_caught = false;
                    wait_time = 1000f;
                }
            }
        }

        //All dialogue lines are finished, going out of dialogue
        text_dialogue.text = "";

        transform.GetChild(0).gameObject.SetActive(false);
        player.gameObject.GetComponent<Animator>().SetBool("Dialogue_idle", false);
        player.movement_input = true;
        coroutine_running = false;
    }
}
