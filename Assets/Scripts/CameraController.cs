using System.Collections;
using UnityEngine;

// Different camera types tested during the project. This version now only supports box_follow
public enum Camera_type { hard_centered, smooth_centered, forward_centered, box_follow, smart_follow}


/** This class controls the movement of the camera around the player
 * The camera will look for a 'Player' tagged gameobject and will follow
 * it according to the given parameters
 */ 
public class CameraController : MonoBehaviour {

    #region General elements
    protected int _screen_width = 0;
    protected int _screen_height = 0;

    protected Transform _player_transform;
    protected Rigidbody _rigidbody;

    protected bool coroutine_running = false;
    #endregion

    #region Camera follow parameters
    [SerializeField] protected float detection_box_height = 1f;
    [SerializeField] protected float detection_box_width = 1f;

    [SerializeField] protected float _horizontal_offset = 0f;
    [SerializeField] protected float _vertical_offset = 2f;
    [SerializeField] protected float _depth_offset = -7;

    protected float forward_offset = 2f;

    [SerializeField] protected float box_camera_speed = 3f;
    [SerializeField] protected float camera_rotation_speed = 20f;
    [SerializeField] protected float smooth_offset = 3f;

    [SerializeField] protected float _rotation = 0f;
    #endregion

    #region Calcul variables
    protected Vector3 new_pos;
    protected float threshold = 1f;
    protected float _saved_rotation = 0f;
    #endregion

    #region Accessors
    public float rotation { get { return _rotation; } set { _rotation = value; } }
    public float horizontal_offset { get { return _horizontal_offset; } set { _horizontal_offset = value; } }
    public float vertical_offset { get { return _vertical_offset; } set { _vertical_offset = value; } }
    public float depth_offset { get { return _depth_offset; } set { _depth_offset = value; } }
    #endregion

    public Camera_type camera_type;

    // Limits load on mobile
    private void Awake()
    {
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        Screen.SetResolution((int)Screen.width, (int)Screen.height, true);
    }

    // Setting screen orientation and basic variables
    protected void Start () {
        Screen.orientation = ScreenOrientation.LandscapeRight;
        _screen_width = Screen.width;
        _screen_height = Screen.height;
        _player_transform = GameObject.FindGameObjectWithTag("Player").transform;
        _rigidbody = GetComponent<Rigidbody>();
        new_pos = transform.position;
    }

    // Calls the proper camera function to determine movement
    protected void FixedUpdate () {
        // In case player is lost
		if(_player_transform == null)
        {
           _player_transform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        if(camera_type == Camera_type.box_follow)
        {
            BoxFollow();
        }else if(camera_type == Camera_type.smart_follow)
        {
            SmartFollow();
        }
        else if(camera_type == Camera_type.hard_centered)
        {
            HardCentered();
        }else if(camera_type == Camera_type.smooth_centered)
        {
            SmoothCentered();
        }else if(camera_type == Camera_type.forward_centered)
        {
            ForwardCentered();
        }

        // Setting lookat to always look at the player transform. 
        // Lookat stays on the horizontal plan. Rounding the player transform to avoid shaking
        Vector3 rounded_transform = new Vector3(Mathf.Round(_player_transform.position.x * 100f)/100f, 
            Mathf.Round(_player_transform.position.y * 100f)/100f, Mathf.Round(_player_transform.position.z * 100f)/100f);
        Vector3 lookAt = (rounded_transform - transform.position + transform.TransformDirection(
            new Vector3(_horizontal_offset, _vertical_offset, 0f))).normalized;
        lookAt.y = 0;
        transform.GetComponentsInChildren<Transform>()[1].forward = lookAt;
    }

    /** Overall better camera, beware not to have a box that is too big
     *  Camera speed should be the same as player speed, or a bit faster
     *  This camera uses rigidbody to move around in order to smoothly match the
     *  player movement
     */
    protected void BoxFollow()
    {
       /** Determining if new rotation has been given, if so rotates the camera
        * This should be changed to a Coroutine when possible
        */ 
        if (Mathf.Abs(_saved_rotation - rotation) > threshold)
        {
            float sign = _saved_rotation > rotation ? -1 : 1;
            transform.RotateAround(_player_transform.position, new Vector3(0, 1, 0), sign * Time.fixedDeltaTime * camera_rotation_speed);
            _saved_rotation += sign * Time.fixedDeltaTime * camera_rotation_speed;
        }

        //Determining if new pos required
        Vector3 camera_space_location = transform.InverseTransformPoint(_player_transform.position);
         
        if (Mathf.Abs(camera_space_location.x - _horizontal_offset) > detection_box_width || Mathf.Abs(camera_space_location.y - _vertical_offset) > detection_box_height || Mathf.Abs(camera_space_location.z - _depth_offset) > 0f)
        {
            new_pos = new Vector3(camera_space_location.x + _horizontal_offset, camera_space_location.y + _vertical_offset, camera_space_location.z + _depth_offset);
        }
        
        //Determining if new_pos is close enough of the target
        if (Mathf.Abs(new_pos.x) < threshold && Mathf.Abs(new_pos.y) < threshold && Mathf.Abs(new_pos.z) < threshold)
        {
            new_pos = new Vector3(0f, 0f, 0f);
        }
         
        Vector3 velocity = Vector3.zero;
        velocity.x = Mathf.Abs(new_pos.x / detection_box_width) > 1f ? Mathf.Sign(new_pos.x) * box_camera_speed : new_pos.x / detection_box_width * box_camera_speed;
        velocity.y = Mathf.Abs(new_pos.y / detection_box_height) > 1f ? Mathf.Sign(new_pos.y) * box_camera_speed : new_pos.y / detection_box_height * box_camera_speed;
        velocity.z = Mathf.Abs(new_pos.z / box_camera_speed) > 1f ? Mathf.Sign(new_pos.z) * box_camera_speed : new_pos.z;

        //Switching back to world space
        velocity = transform.TransformVector(velocity);

        _rigidbody.velocity = velocity;
    }

    public void CameraShake(float magnitude, float duration)
    {
        if (!coroutine_running)
        {
            StartCoroutine(CameraShaking(magnitude, duration));
        }
    }

    /** Camera shaking using position too directly move the camera
     *  This way the physic based Box Follow can still control de camera
     *  withour the CameraShake interfering
     */ 
    IEnumerator CameraShaking(float magnitude, float duration)
    {
        coroutine_running = true;

        float time = 0f;

        while(time < duration)
        {
            float completion = time / duration;
            float damper = 1f - Mathf.Clamp(4f * completion - 3f, 0f, 1f);

            float x = (Random.value * 2f - 1f) * magnitude * damper;
            float y = (Random.value * 2f - 1f) * magnitude * damper;

            transform.position += transform.TransformDirection(new Vector3(x, y, 0));

            time += Time.deltaTime;

            yield return null;
        }
        coroutine_running = false;
    }

    #region Not supported cameras
    // All Cameras beneath have not been programmed for the final versions and will not be working if you chose them in the EDITOR.
    // Stick to the Box Follow for now

    // Mixed BoxFollow with a bit of forward and other cool stuff
    protected void SmartFollow()
    {

    }

    // Overall worst camera out there
    protected void HardCentered()
    {
       transform.position = Utils.Utils.RotatePointAroundPivot(
            new Vector3(_player_transform.position.x + _horizontal_offset, _player_transform.position.y + _vertical_offset, _player_transform.position.z + _depth_offset),
            _player_transform.position,
            new Vector3(0, _rotation, 0));
    }

    // Some games like limbo use this camera, with some offsetting done at some moments
    protected void SmoothCentered()
    {
        transform.position = Utils.Utils.RotatePointAroundPivot( 
            Vector3.Lerp(transform.position, Quaternion.Euler(0, rotation, 0) * new Vector3(_player_transform.position.x +
                 _horizontal_offset, _player_transform.position.y + _vertical_offset, _player_transform.position.z + _depth_offset),
                Time.deltaTime * smooth_offset),
            _player_transform.position,
            new Vector3(0, _rotation, 0));
    }

    // Might be a decent choice in case of a runner phase, based on SmoothCentered
    protected void ForwardCentered()
    {
        transform.position = Utils.Utils.RotatePointAroundPivot(
            Vector3.Lerp(transform.position, Quaternion.Euler(0, rotation, 0) * (new Vector3(_player_transform.position.x + 
                _horizontal_offset, _player_transform.position.y + _vertical_offset, _player_transform.position.z + _depth_offset) +
                _player_transform.forward.normalized * forward_offset),
                Time.deltaTime * smooth_offset),
            _player_transform.position,
            new Vector3(0, _rotation, 0));
    }
    #endregion
}
