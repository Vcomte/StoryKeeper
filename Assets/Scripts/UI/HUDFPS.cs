using UnityEngine;
using System.Collections;

public class HUDFPS : MonoBehaviour
{
    // FPS indicator over each UpdateInterval
    // It calculates frames/second over each updateInterval

    public Rect startRect = new Rect(10, 10, 75, 50);
    public bool updateColor = true;
    public bool allowDrag = true;
    public float frequency = 0.5F;
    public int nbDecimal = 1;

    private float accum = 0f;
    private int frames = 0;
    private Color color = Color.white;
    private string sFPS = "";
    private GUIStyle style;

    void Start()
    {
        StartCoroutine(FPS());
    }

    void Update()
    {
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
    }

    IEnumerator FPS()
    {
        // Infinite loop executed every "frenquency" secondes.
        while (true)
        {
            // Update the FPS
            float fps = accum / frames;
            sFPS = fps.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));

            //Update the color
            color = (fps >= 24) ? Color.green : ((fps > 15) ? Color.red : Color.yellow);

            accum = 0.0F;
            frames = 0;

            yield return new WaitForSeconds(frequency);
        }
    }

    void OnGUI()
    {
        // Copy the default label skin, change the color and the alignement
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
        }

        GUI.color = updateColor ? color : Color.white;
        startRect = GUI.Window(0, startRect, DoMyWindow, "");
    }

    void DoMyWindow(int windowID)
    {
        GUI.Label(new Rect(0, 0, startRect.width, startRect.height), sFPS + " FPS", style);
        if (allowDrag) GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }
}