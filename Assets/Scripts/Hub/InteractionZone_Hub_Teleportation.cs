using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionZone_Hub_Teleportation : InteractionZone {

    public override void TriggerInteraction(GameObject character)
    {
        Application.LoadLevel("TestScene");
    }
}
