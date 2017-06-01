using UnityEngine;

// This script is attached to the strike colliders of the cyclops
public class CyclopsArmStrike : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            EventManager.TriggerEvent("StrikeTouched");
        }
    }
}
