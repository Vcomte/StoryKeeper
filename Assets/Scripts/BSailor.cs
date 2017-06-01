using UnityEngine;

public class BSailor : MonoBehaviour{

    //Function called at the end of the movement animation
    public void OnMovementEnding()
    {
        transform.GetChild(0).GetComponent<Animator>().SetBool("Action_idle", false);
        transform.GetChild(0).GetComponent<Animator>().SetBool("Action_left_turn_inPlace", false);
    }
}


/** OLD version of the script where the sailor would wave to the player
    string[,] actual_dialogue = {
        { "Marin", "Par ici capitaine !", "" }
    };

    [SerializeField] private string tuto_text;

    private void Start()
    {
        //dialogue = actual_dialogue;
        EventManager.StartListening("CloseTuto", TutoMovement);
        //EventManager.TriggerEvent("Tuto");
    }

    private void TutoMovement()
    {
        EventManager.StopListening("CloseTuto", TutoMovement);
        EventManager.StartListening("CloseTuto", Wave);
        StartCoroutine("CoroutineMouvement");   
    }

    IEnumerator CoroutineMouvement()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject.FindGameObjectWithTag("TutoUI").transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2(323f, 515f);
        GameObject.FindGameObjectWithTag("TutoUI").transform.GetChild(1).GetChild(0).GetComponent<Text>().text = tuto_text;
        EventManager.TriggerEvent("Tuto");
    }

    void Wave()
    {
        StartCoroutine("StartCoroutine");
        EventManager.StopListening("CloseTuto", Wave);
    }

    IEnumerator StartCoroutine()
    {
        yield return new WaitForSeconds(1f);
        TriggerInteraction(GameObject.FindGameObjectWithTag("Player"));
        GetComponent<Animator>().SetBool("wave", true);
    }
 */