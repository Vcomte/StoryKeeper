using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoZoneMenu : TutoZone {

    public override void StartTuto()
    {
        base.StartTuto();
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.menu_tutorial_running = true;
        player.Animate("Touch_bracelet_start");
        player.StartBlinking();
    }

    public override void EndTuto()
    {
        base.EndTuto();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().menu_tutorial_running = false;
    }
}