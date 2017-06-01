using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_Hub : CameraController {


	
	// Update is called once per frame
	protected void LateUpdate () {
        if (_player_transform == null)
        {
            _player_transform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        transform.LookAt( _player_transform.position );
    }
}
