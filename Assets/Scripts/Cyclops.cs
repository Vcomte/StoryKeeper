using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This script is attached to the cyclops and gives the necessary elements
 *  to control it during cinematic and animations
 */
public class Cyclops : MonoBehaviour {

    [SerializeField] private GameObject hand;                               // The gameobject on witch sailor and william can be attached to
    public GameObject sailor_to_eat;                                        // The sailor that will get eaten during the cinematic
    private bool eaten = false;

    private Animator model_animator;

    private bool attack = false;
    private bool walking = false;
    private bool turning = false;

    private bool final_attack_played = false;

	void Start () {
        model_animator = transform.GetChild(1).GetComponent<Animator>();
	}

    #region Generic animation function (mainly called from animator)
    public void ToggleAttack()
    {
        attack = !attack;
        model_animator.SetBool("Attaque_idle_base", true);
    }
	
    public void ToggleWalk()
    {
        walking = !walking;
        if (attack)
        {
            model_animator.SetBool("Attaque_walkWalk", walking);
        }
        else
        {
            model_animator.SetBool("Walk", walking);
        }
    }

    public void ToggleTurning()
    {
        turning = !turning;
        if (attack)
        {
            model_animator.SetBool("Attaque_left_turn_inPlace", turning);
        }
        else
        {
            model_animator.SetBool("left_turn_inPlace", turning);
        }
    }

    public void ToggleCloseDoor()
    {
        if (!attack)
        {
            model_animator.SetTrigger("Closed_door");
        }
    }

    public void ToggleRoar()
    {
        if (!attack)
        {
            ToggleAttack();
        }
        model_animator.SetTrigger("Attaque_idle_roaring");
    }

    public void StrikeEnded()
    {
        GetComponentInParent<CyclopsDodgeFight>().strike_finished = true;
    }

    public void ShakeCamera()
    {
        Camera.main.GetComponentInParent<CameraController>().CameraShake(0.2f, 0.5f);
    }

    #endregion

    #region Sailor/William eating functions (mainly called from animator and animations)
    void CatchSailor()
    {
        eaten = false;
        sailor_to_eat.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        if (sailor_to_eat.tag != "Player")
        {
            sailor_to_eat.GetComponent<Animator>().enabled = false;
        }
        sailor_to_eat.transform.SetParent(hand.transform);
        sailor_to_eat.transform.localPosition = new Vector3(0.1472f, -0.068f, 0.0937f);
        sailor_to_eat.transform.localRotation = Quaternion.Euler(-49.506f, 33.01f, 7.097f);
        StartCoroutine(ReplaceSailor());
        if (sailor_to_eat.gameObject.tag != "Player")
        {
            sailor_to_eat.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("Ecrasement_main", true);
        }
        else
        {
            sailor_to_eat.gameObject.GetComponent<Animator>().SetBool("Ecrasement_main", true);
        }
    }

    // Replaces the character at the center of the hand every update
    IEnumerator ReplaceSailor()
    {
        while (!eaten)
        {
            sailor_to_eat.transform.localPosition = new Vector3(0.1472f, -0.068f, 0.0937f);
            sailor_to_eat.transform.localRotation = Quaternion.Euler(-49.506f, 33.01f, 7.097f);
            yield return null;
        }
    }

    void SquishSailor()
    {
        if (sailor_to_eat.gameObject.tag != "Player") {
            sailor_to_eat.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("Ecrasement_main", false);
        }
        else
        {
            sailor_to_eat.gameObject.GetComponent<Animator>().SetBool("Ecrasement_main", false);
        }
    }

    void EatSailor()
    {
        if (sailor_to_eat.gameObject.tag != "Player")
        {
            sailor_to_eat.SetActive(false);
        }
        eaten = true;
    }
    #endregion

    void StartFight()
    {
        GetComponent<CyclopsDodgeFight>().fight_start = true;
    }

    #region Sound playing function (mainly called from animator and animations)
    public void PlayVoiceAttaqueMain()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayVoice(SoundManager.Voice_type.cyclops_voice_hand_attack);
    }

    public void PlayVoiceAttaqueFinale()
    {
        if (!final_attack_played)
        {
            GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayVoice(SoundManager.Voice_type.cyclops_final_attack);
            final_attack_played = true;
        }
    }

    public void PlayVoiceAttaquePied()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayVoice(SoundManager.Voice_type.cyclops_voice_stomping);
    }

    public void PlayVoiceSailorScream()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayVoice(SoundManager.Voice_type.sailor_screaming);
    }

    public void PlayVoiceSailorScreamLong()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayVoice(SoundManager.Voice_type.sailor_screaming_long);
    }

    public void PlayBonesCracking()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayNoise(SoundManager.Noise_type.bones_cracking);
    }

    public void PlayAttaqueMain()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayNoise(SoundManager.Noise_type.cyclop_hand_strike);
    }

    public void PlayAttaquePied()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayNoise(SoundManager.Noise_type.cyclops_stomping);
    }

    public void PlayRockDoor()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayNoise(SoundManager.Noise_type.rock_door);
    }

    public void PlayCrie()
    {
        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayVoice(SoundManager.Voice_type.cyclops_roaring);
    }
    #endregion

    public void TriggerStrike()
    {
        transform.parent.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("Strike");
    }

    public void TriggerStomp()
    {
        transform.parent.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("Stomp");
    }
}
