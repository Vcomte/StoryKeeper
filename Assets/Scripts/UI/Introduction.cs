using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/** This script manages the whole introduction of the demo
 */
public class Introduction : MonoBehaviour {

    [SerializeField] string advice_video_path;          // First video leading to the welcome screen
    [SerializeField] string introduction_video_path;    // Second video leading to the game and william awaking

    private PlayerController player;
    private bool cinematic = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.movement_input = false;

        StartCoroutine(Sequencer());        // Starts the introduction sequence as soon as possible
    }

    // Called when the welcome screen is being touched
    public void GoToCinematic()
    {
        cinematic = true;
    }

    // Main coroutine that will sequence the whole introduction
    IEnumerator Sequencer()
    {
        yield return Utils.Utils.PlayingVideo(advice_video_path);

        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayMusic(SoundManager.Music_type.intro);

        transform.GetChild(1).gameObject.SetActive(true);       // Displays the welcome screen at the end of the video

        while (!cinematic)
        {
            yield return new WaitForSeconds(0.1f);
        }

        GameObject.FindGameObjectWithTag("SoundSystem").GetComponent<SoundManager>().PlayMusic(SoundManager.Music_type.empty);

        yield return FadeOut(transform.GetChild(1).gameObject);

        transform.GetChild(1).gameObject.SetActive(false);

        yield return Utils.Utils.PlayingVideo(introduction_video_path);

        yield return FadeOut(transform.GetChild(0).gameObject);     // Fading out the white screen to display the game scene

        transform.GetChild(0).gameObject.SetActive(false);

        EventManager.TriggerEvent("CloseIntroduction");
    }

    IEnumerator FadeOut(GameObject item)
    {
        float time = 1f;
        Image image = item.GetComponent<Image>();
        while(time > 0f)
        {
            time -= Time.deltaTime/3f;
            image.color = new Color(1f, 1f, 1f, time);
            yield return null;
        }
    }
}
