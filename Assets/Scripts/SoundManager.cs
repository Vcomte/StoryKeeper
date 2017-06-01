using System.Collections;
using UnityEngine;

/** This script manages most of the sounds in the game
 *  Any script can find the "SoundSystem" GameObject during runtime and ask the SoundManager 
 *  to play a specific sound.
 *  Sounds are categorized between: Music / Voice / Background / Noise / Ui
 *  Every sound type is played by a different source, all controlled by the SoundManager
 *  All sounds must be specified to the SoundManager by hand before runtime: this should be automated
 */
public class SoundManager : MonoBehaviour {

    public enum AudioSourceType { Music, Background, Dialogue, Noises, Uis, NumberOfTypes }

    public enum Music_type { forest, cavern, attack, intro, empty}
    public enum Voice_type
    {   sailor_A_interrogate, sailor_A_normal, sailor_A_scared, sailor_B_interrogate, sailor_B_normal, sailor_B_scared,
        sailor_C_interrogate, sailor_C_normal, sailor_C_scared, sailor_screaming, sailor_screaming_long, cyclops_voice_hand_attack,
        cyclops_voice_stomping, cyclops_roaring, cyclops_entrance, cyclops_final_attack, cyclops_convers_1, cyclops_convers_2,
        ulysse_jump, empty
    }
    public enum Background_type { forest_background, empty}
    public enum Noise_type { cyclop_hand_strike, cyclops_stomping, cyclops_walking, bones_cracking, ulysse_interact, rock_door, empty }
    public enum UI_type { open, go, back, empty}

    private AudioSource[] sources = new AudioSource[(int)AudioSourceType.NumberOfTypes];

    #region Musics
    [SerializeField] AudioClip music_forest;
    [SerializeField] AudioClip music_cavern;
    [SerializeField] AudioClip music_attack;
    [SerializeField] AudioClip music_intro;
    #endregion

    private Music_type _current_music;

    private AudioClip[] musics;

    #region Voices
    [SerializeField] AudioClip sailor_A_interrogate;
    [SerializeField] AudioClip sailor_A_normal;
    [SerializeField] AudioClip sailor_A_scared;
    [SerializeField] AudioClip sailor_B_interrogate;
    [SerializeField] AudioClip sailor_B_normal;
    [SerializeField] AudioClip sailor_B_scared;
    [SerializeField] AudioClip sailor_C_interrogate;
    [SerializeField] AudioClip sailor_C_normal;
    [SerializeField] AudioClip sailor_C_scared;
    [SerializeField] AudioClip sailor_screaming;
    [SerializeField] AudioClip sailor_screaming_long;
    [SerializeField] AudioClip cyclops_voice_hand_attack;
    [SerializeField] AudioClip cyclops_voice_stomping;
    [SerializeField] AudioClip cyclops_roaring;
    [SerializeField] AudioClip cyclops_entrance;
    [SerializeField] AudioClip cyclops_final_attack;
    [SerializeField] AudioClip cyclops_convers_1;
    [SerializeField] AudioClip cyclops_convers_2;
    [SerializeField] AudioClip ulysse_jump;
    #endregion

    private AudioClip[] voices;

    #region Backgrounds
    [SerializeField] AudioClip forest_background;
    #endregion

    private Background_type _current_background;

    private AudioClip[] backgrounds;

    #region Noises
    [SerializeField] AudioClip cyclops_hand_strike;
    [SerializeField] AudioClip cyclops_stomping;
    [SerializeField] AudioClip cyclops_walking;
    [SerializeField] AudioClip bones_cracking;
    [SerializeField] AudioClip ulysse_interact;
    [SerializeField] AudioClip rock_door;
    #endregion

    private AudioClip[] noises;

    #region Uis
    [SerializeField] AudioClip open;
    [SerializeField] AudioClip go;
    [SerializeField] AudioClip back;
    #endregion

    private AudioClip[] uis;

    private bool music_coroutine_running = false;
    private Coroutine music_coroutine;
    private bool background_coroutine_running = false;
    private Coroutine background_coroutine;

    public Music_type current_music { get { return _current_music; } }
    public Background_type current_background { get { return _current_background; } }

    // Getting the sources and creating the arrays containing all the sounds
    void Start () {
		for(int i = 0; i < (int)AudioSourceType.NumberOfTypes; i++)
        {
            sources[i] = transform.GetChild(i).gameObject.GetComponent<AudioSource>();
        }

        musics = new AudioClip[] { music_forest, music_cavern, music_attack, music_intro};
        voices = new AudioClip[] 
        {
            sailor_A_interrogate, sailor_A_normal, sailor_A_scared, sailor_B_interrogate, sailor_B_normal, sailor_B_scared,
            sailor_C_interrogate, sailor_C_normal, sailor_C_scared, sailor_screaming, sailor_screaming_long, cyclops_voice_hand_attack,
            cyclops_voice_stomping, cyclops_roaring, cyclops_entrance, cyclops_final_attack, cyclops_convers_1, cyclops_convers_2, ulysse_jump
        };
        backgrounds = new AudioClip[] { forest_background};
        noises = new AudioClip[] { cyclops_hand_strike, cyclops_stomping, cyclops_walking, bones_cracking, ulysse_interact, rock_door};
        uis = new AudioClip[] { open, go, back};

        _current_music = Music_type.forest;
        _current_background = Background_type.forest_background;
	} 

    public void PlayMusic(Music_type type)
    {
        if (!type.Equals(Music_type.empty))
        {
            if (music_coroutine_running)
            {
                Debug.Log("stop coroutine");
                StopCoroutine(music_coroutine);
            }
            music_coroutine = StartCoroutine(FadeChange((int)AudioSourceType.Music, musics[(int)type], 1f));
            _current_music = type;
        }
        else
        {
            StartCoroutine(FadeChange((int) AudioSourceType.Music, null, 1f));
        }
    }

    public void PlayBackground(Background_type type) {
        if (!type.Equals(Background_type.empty))
        {
            if (background_coroutine_running)
            {
                Debug.Log("stop coroutine");
                StopCoroutine(background_coroutine);
            }
            background_coroutine = StartCoroutine(FadeChange((int)AudioSourceType.Background, backgrounds[(int)type], 1f));
            _current_background = type;
        }
        else
        {
            sources[(int)AudioSourceType.Background].Stop();
        }
    }

    public void PlayVoice(Voice_type type)
    {
        AudioSource source = sources[(int)AudioSourceType.Dialogue];
        if (!type.Equals(Voice_type.empty))
        {
            Debug.Log(type);
            source.Stop();
            source.clip = voices[(int)type];
            source.Play();
        }
        else
        {
            source.Stop();
        }
    }

    public void PlayNoise(Noise_type type)
    {
        AudioSource source = sources[(int)AudioSourceType.Noises];
        if (!type.Equals(Noise_type.empty))
        {
            source.Stop();
            source.clip = noises[(int)type];
            source.Play();
        }
        else
        {
            source.Stop();
        }
    }

    public void PlayUI(UI_type type)
    {
        AudioSource source = sources[(int)AudioSourceType.Uis];
        if (!type.Equals(UI_type.empty))
        {
            source.Stop();
            source.clip = uis[(int)type];
            source.Play();
        }
    }

    private IEnumerator FadeChange(int source_type, AudioClip clip, float fade_time)
    {
        if(source_type == 0)
        {
            music_coroutine_running = true;
        }
        else if (source_type == 1)
        {
            background_coroutine_running = true;
        }

        AudioSource source = sources[source_type];
        float volume = source.volume;
        for (float i = 0; i < fade_time * 10f; i++)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            source.volume -= volume / (fade_time * 10f);
        }
        source.Stop();

        source.clip = clip;

        if (clip != null)
        {
            source.Play();
            for (float i = 0; i < fade_time * 10f; i++)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                source.volume += volume / (fade_time * 10f);
            }
        }

        source.volume = volume;
        if (source_type == 0)
        {
            music_coroutine_running = false;
        }
        else if (source_type == 1)
        {
            background_coroutine_running = false;
        }
    }
}
