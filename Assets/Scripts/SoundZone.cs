using UnityEngine;

/** This class manages areas where, when triggered, play sounds.
 *  This script would require a revamp and an Editor Script to be used properly
 *  The music and the background are saved when changed, in order to be reset
 *  when the zone is triggered once more
 */
public class SoundZone : MonoBehaviour {
    
    [SerializeField] private SoundManager.AudioSourceType[] audio_types;

    //This would require an editor script to appear nicely in the editor
    [SerializeField] private SoundManager.Music_type music;
    [SerializeField] private SoundManager.Voice_type voice;   
    [SerializeField] private SoundManager.Noise_type noise;   
    [SerializeField] private SoundManager.Background_type background;

    [SerializeField] private bool only_once = false;
    private SoundManager.Music_type saved_music;
    private SoundManager.Background_type saved_background;
    private bool revert_sound = false;
    private bool triggered = false;

    // If the source is a background or a music, the script will save the old sound, and when the player passes through the zone once more
    // will set back the old sound. If the source is a dialogue or noise, the sound will simply change to what's required without saving
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && (!only_once || !triggered))
        {
            SoundManager sound_manager = GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>();
            foreach (SoundManager.AudioSourceType type in audio_types) {
                if (type.Equals(SoundManager.AudioSourceType.Music)) {
                    if (revert_sound)
                    {
                        sound_manager.PlayMusic(saved_music);
                    }
                    else
                    {
                        saved_music = sound_manager.current_music;
                        sound_manager.PlayMusic(music);
                    }
                }
                else if (type.Equals(SoundManager.AudioSourceType.Background))
                {
                    if (revert_sound)
                    {
                        sound_manager.PlayBackground(saved_background);
                    }
                    else
                    {
                        saved_background = sound_manager.current_background;
                        sound_manager.PlayBackground(background);
                    }
                }
                else if (type.Equals(SoundManager.AudioSourceType.Noises))
                {
                    sound_manager.PlayNoise(noise);
                }
                else
                {
                    sound_manager.PlayVoice(voice);
                }
            }
            revert_sound = !revert_sound;
            triggered = true;
            transform.position = other.transform.position + other.transform.forward * 2 * (revert_sound ? -1f : 1f);
        }        
    }
}
