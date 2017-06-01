using UnityEngine;

// This class will manage the cinematic ordering for the cavern entrance 
public class CinematicCavernEntry : InteractionZoneDialogue {

    private PlayerController player;

    // Three more groupds of dialogue to add to the base one
    [SerializeField] Utils.dialogue_line[] dialogue_two;
    [SerializeField] Utils.dialogue_line[] dialogue_three;
    [SerializeField] Utils.dialogue_line[] dialogue_four;

    // Elements that will be affected by the cinematic
    [SerializeField] Animator[] sailors;
    [SerializeField] Animator cyclops;
    [SerializeField] Animator door;
    [SerializeField] Animator wood;

    protected new void Start()
    {
        //to ignore the base start
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !triggered)
        {
            triggered = true;
            player = other.gameObject.GetComponent<PlayerController>();
            player.movement_input = false;
            other.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            
            PlayCinematicPartOne();
        }
    }

    // Each part is played when the dialogue or the specific animation is finished
    // This way, the cinematic is sure to remain synchronised with all its parts
    private void PlayCinematicPartOne()
    {
        EventManager.TriggerEvent("CinematicFrame");        // Moving the black frames for cinematic effect

        Camera.main.transform.parent.GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>().intensity = 0.2f;

        foreach (Animator sailor in sailors)
        {
            sailor.gameObject.SetActive(true);
            sailor.SetBool("Cine0first", true);
        }
        GetComponent<Animator>().SetBool("Cine0first", true);
    }

    private void PlayCinematicPartTwo()
    {
        TriggerInteraction(player.gameObject);      //Starting the first dialogue
        EventManager.StartListening("DialogueEnded", PlayCinematicPartThree);
    }

    private void PlayCinematicPartThree()
    {
        player.movement_input = false;          //After each dialogue, preventing player movement
        EventManager.StopListening("DialogueEnded", PlayCinematicPartThree);

        cyclops.gameObject.SetActive(true);
        cyclops.SetBool("Cine0third", true);
        door.SetBool("Cine0third", true);

        foreach (Animator sailor in sailors)
        {
            sailor.SetBool("Cine0third", true);
        }
        GetComponent<Animator>().SetBool("Cine0third", true);
    }

    private void PlayCinematicPartFour()
    {
        dialogue = dialogue_two;
        TriggerInteraction(player.gameObject);
        EventManager.StartListening("DialogueEnded", PlayCinematicPartFive);
    }

    private void PlayCinematicPartFive()
    {
        player.movement_input = false;
        EventManager.StopListening("DialogueEnded", PlayCinematicPartFive);

        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayMusic(SoundManager.Music_type.attack);

        cyclops.SetBool("Cine0third", false);
        cyclops.SetBool("Cine0fifth", true);

        dialogue = dialogue_three;
        TriggerInteraction(player.gameObject);
        EventManager.StartListening("DialogueEnded", PlayCinematicPartSix);
    }

    private void PlayCinematicPartSix()
    {
        player.movement_input = false;
        EventManager.StopListening("DialogueEnded", PlayCinematicPartSix);

        cyclops.SetBool("Cine0sixth", true);
        foreach(Animator sailor in sailors)
        {
            sailor.SetBool("Cine0sixth", true);
        }

        dialogue = dialogue_four;
        TriggerInteraction(player.gameObject);
        EventManager.StartListening("DialogueEnded", PlayCinematicPartSeven);

    }

    // Deactivating unused objects and setting up the CyclopeDodgeFight
    private void PlayCinematicPartSeven()
    {
        player.movement_input = false;
        EventManager.StopListening("DialogueEnded", PlayCinematicPartSeven);

        sailors[0].transform.SetParent(null);
        sailors[0].gameObject.SetActive(false);
        sailors[1].gameObject.SetActive(false);
        sailors[2].gameObject.SetActive(false);

        cyclops.SetBool("Cine0seventh", true);
        wood.SetTrigger("tas_bois");

        EventManager.TriggerEvent("CinematicFrame");

        cyclops.GetComponent<CyclopsDodgeFight>().saved_player_position = player.transform.position;
        player.movement_input = true;
        player.gameObject.GetComponent<BoxCollider>().isTrigger = false;
        player.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void RotateWilliam()
    {
        player.RotateWilliam(180f, 180f);
    }
}