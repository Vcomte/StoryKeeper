using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionZoneOpenDoor : InteractionZone {

    private Animator _animator;

    // Use this for initialization
    protected override void Start () {
        _animator = GetComponentInChildren<Animator>();
	}

    public override void TriggerInteraction(GameObject character)
    {
        _animator.SetBool("is_open",!_animator.GetBool("is_open"));
    }
}
