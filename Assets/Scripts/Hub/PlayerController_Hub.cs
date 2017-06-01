using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Hub : PlayerController {

    private Transform camera_transform;
    [SerializeField] private float max_distance = 2f;

    protected new void Start()
    {
        base.Start();
        camera_transform = Camera.main.transform;
        transform.forward = camera_transform.right;
    }

    protected override void DetermineVelocity(float sign, out float _x_velocity)
    {
        _x_velocity = sign * camera_transform.transform.right.normalized.x * horizontal_velocity;
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();
        
        // Preventing character from moving away from camera
        Vector3 distance = camera_transform.position - transform.position;
        if (distance.magnitude > max_distance)
        {
            player_rigidbody.velocity = player_rigidbody.velocity + new Vector3(distance.normalized.x, 0, distance.normalized.z);
        }else if (distance.magnitude < max_distance - 0.2f)
        {
            player_rigidbody.velocity = player_rigidbody.velocity - new Vector3(distance.normalized.x, 0, distance.normalized.z);
        }
    }
}
