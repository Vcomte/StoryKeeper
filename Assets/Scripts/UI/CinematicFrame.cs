using UnityEngine;

public class CinematicFrame : MonoBehaviour {

    private bool display = false;

	void Start () {
        EventManager.StartListening("CinematicFrame", ChangeDisplay);
    }
	
    void ChangeDisplay()
    {
        display = !display;
        GetComponent<Animator>().SetBool("display", display);
    }

}
