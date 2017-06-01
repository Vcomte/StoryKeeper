using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour {
    private Vector3 _initial_pos;
    [SerializeField] float _max_distance = 3f;
    [SerializeField] int seed = 1;

    private Vector3 destination;

	// Use this for initialization
	void Start () {
        _initial_pos = transform.position;
        destination = transform.position;
        Random.InitState(seed);
	}
	
	// Update is called once per frame
	void Update () {
        if((transform.position - destination).magnitude < 0.1f)
        {
            Vector3 new_destination = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            if ((new_destination - _initial_pos).magnitude < _max_distance)
            {
                destination = new_destination; 
            }
        }
        transform.position = Vector3.Lerp(transform.position, destination, 0.01f);
    }
}
