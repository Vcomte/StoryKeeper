using System.Collections;
using UnityEngine;


/** This class manages the rotation areas where the player and the camera change direction
 *  The rotation zones can display or hide groups of elements in order to change the environment
 *  if needed.
 */
public class RotationZone : MonoBehaviour {

    [SerializeField] float rotation;
    [SerializeField] GameObject[] to_hide;
    [SerializeField] GameObject[] to_display;
    private float rotation_sign = 1;

    private void OnTriggerEnter(Collider other)
    {
         if(other.gameObject.tag == "Player")
        {
            //Rotating everything
            other.gameObject.GetComponent<PlayerController>().rotation += rotation * rotation_sign;
            Camera.main.GetComponentInParent<CameraController>().rotation += rotation * rotation_sign;

            //Moving collider to prevent wrong directions while on the zone
            transform.position = Utils.Utils.RotatePointAroundPivot(transform.position, other.transform.position,
                new Vector3(0, rotation * rotation_sign - 180, 0));

            StartCoroutine(FadeObjects(rotation_sign > 0 ? false : true));
            rotation_sign *= -1;
        }
    }

    private IEnumerator FadeObjects(bool fade_in)
    {
        yield return new WaitForSeconds(0.7f);
        foreach(GameObject item in to_hide)
        {
            item.SetActive(fade_in);
        }
        foreach(GameObject item in to_display)
        {
            item.SetActive(!fade_in);
        }
        /** Proper fading to be implemented instead of just deactivating items
        float aValue = fade_in ? 1f : 0f;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime)
        {
            foreach (GameObject item in to_hide) {
                float alpha = item.transform.GetComponent<Renderer>().material.color.a;
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
                item.transform.GetComponent<Renderer>().material.color = newColor;
                yield return null;
            }
        }*/
    }
}
