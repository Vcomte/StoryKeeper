using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoZoneAlexandri : TutoZone
{
    public override void StartTuto()
    {
        base.StartTuto();
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.alexandri_tutorial_running = true;
        player.Animate("Touch_bracelet_start");
        player.StartBlinking();
    }

    public override void EndTuto()
    {
        base.EndTuto();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().alexandri_tutorial_running = false;
    }
}