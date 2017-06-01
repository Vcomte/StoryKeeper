namespace Utils{

    using System.Collections;
    using UnityEngine;

    // The ParameterType are used to specify in the editor what type of parameter is set in the animator
    public enum ParameterType { boolean, trigger, empty}

    /** The dialogue line class can be used anywhere and then fed to the dialogue controller
     *  A gameobject with an animator can be given. If so, it will take the parameter_type, the parameter
     *  and the value in order to trigger a parameter or to set a boolean.
     *  A voice line can be played too.
     */
    [System.Serializable]
    public class dialogue_line
    {
        public string speaker, line;
        public GameObject to_animate;
        public ParameterType parameter_type;
        public string parameter;
        public bool value;
        public SoundManager.Voice_type to_play;
    }

    public class Utils : MonoBehaviour {

        public static Vector3 RotatePointAroundPivot (Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 direction = point - pivot;
            direction = Quaternion.Euler(angles) * direction;
            Vector3 new_point = direction + pivot;
            return new_point;
        }

        public static IEnumerator RotateAroundSelf(Quaternion from, float angle, float speed, Transform transform)
        {
            Quaternion target = from * Quaternion.Euler(0f, angle, 0f);

            float rate = 1f / (Mathf.Abs(angle) / speed);
            float time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime * rate;
                transform.rotation = Quaternion.Lerp(from, target, time);
                yield return null;
            }
        }

        public static IEnumerator MoveToTarget(Vector3 target, float speed, float distance, Transform transform)
        {
            Vector3 start_position = transform.position;
            float rate = 1f / (distance / speed);
            float time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime * rate;
                transform.position = Vector3.Lerp(start_position, target, time);
                
                yield return null;
            }
        }

        public static IEnumerator PlayingVideo(string path)
        {
            Handheld.PlayFullScreenMovie(path, Color.black, FullScreenMovieControlMode.Hidden);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }
    }
}