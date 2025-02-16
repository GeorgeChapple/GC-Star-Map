using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class StarLerpScript : MonoBehaviour
{
    [SerializeField] private Color[] colours;
    private Renderer starRenderer;
    private Color lerpColour;
    private int startIndex;
    private int nextIndex;
    public float minLerpTime;
    public float maxLerpTime;
    public float sizeLerpTime;
    public Vector3 pos;
    private bool sizeLerp;

    //Pick random colours to lerp between and start lerping.
    private void Awake() {
        starRenderer = GetComponent<Renderer>();
        lerpColour = starRenderer.material.color;
        if (colours != null) {
            startIndex = Random.Range(1, colours.Length - 1);
            lerpColour = colours[startIndex];
            StartColourLerp();
        }
    }

    //Set material colour based on lerp.
    private void Update() {
        starRenderer.material.color = lerpColour;
    }

    //Lerps between the given colours.
    private IEnumerator LerpColour(Color start, Color end) {
        float timeToTake =  Random.Range(0.5f, 1.5f);
        float time = 0;
        float timeTaken = 0;
        while (time < 1) {
            lerpColour = LerpC(start, end, time);
            timeTaken += Time.deltaTime;
            time = timeTaken / timeToTake;
            yield return null;
        }
        StartColourLerp();
    }

    //Lerps between the two positions.
    private IEnumerator LerpPosition(Vector3 start, Vector3 end) {
        float timeToTake = Random.Range(minLerpTime, maxLerpTime);
        float time = 0;
        float timeTaken = 0;
        Vector3 lerpVector = start;
        while (time < 1) {
            lerpVector = LerpV3(start, end, time);
            timeTaken += Time.deltaTime;
            time = timeTaken / timeToTake;
            transform.position = lerpVector;
            yield return null;
        }
        transform.position = pos;
    }

    //Lerps between the two given sizes when selected, then waits to be unselected, then lerps back.
    private IEnumerator LerpSize(Vector3 start, Vector3 end) {
        GetComponent<Star>().selected = true;
        float timeToTake = sizeLerpTime;
        float time = 0;
        float timeTaken = 0;
        float perc;
        Vector3 lerpVector = start;
        while (time < 1) {
            perc = time * (2f - time);
            lerpVector = LerpV3(start, end, perc);
            timeTaken += Time.deltaTime;
            time = timeTaken / timeToTake;
            transform.localScale = lerpVector;
            yield return null;
        }
        while (GetComponent<Star>().selected) {
            yield return null;
        }
        time = 0;
        timeTaken = 0;
        while (time < 1) {
            perc = time * (2f - time);
            lerpVector = LerpV3(end, start, perc);
            timeTaken += Time.deltaTime;
            time = timeTaken / timeToTake;
            transform.localScale = lerpVector;
            yield return null;
        }
        sizeLerp = false;
    }

    //Alternates the colour to lerp to.
    private void GetNextColourIndex() {
        if (nextIndex > startIndex) {
            nextIndex = startIndex - 1;
        }
        else {
            nextIndex = startIndex + 1;
        }
    }

    //Starts colour lerp.
    private void StartColourLerp() {
        GetNextColourIndex();
        StartCoroutine(LerpColour(lerpColour, colours[nextIndex]));
    }

    //Colour lerp function.
    private static Color LerpC(Color startValue, Color endValue, float t) {
        return (startValue + (endValue - startValue) * t);
    }

    //Starts position lerp.
    public void StartPositionLerp() {
        StartCoroutine(LerpPosition(transform.position, pos));
    }
    
    //Starts size lerp if size lerp is not already in progress.
    public void StartSizeLerp() {
        if (sizeLerp == false) {
            sizeLerp = true;
            StartCoroutine(LerpSize(transform.localScale, transform.localScale * 2f));
        }
    }

    //Vector3 lerp function.
    private static Vector3 LerpV3(Vector3 startValue, Vector3 endValue, float t) {
        return (startValue + (endValue - startValue) * Mathf.Sin(t * Mathf.PI * 0.5f));
    }
}
