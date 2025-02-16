using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private LineRenderer linePrefab;
    public Dictionary<Star, float> starRoutes = new Dictionary<Star, float>(); //Stores connections to other stars.
    public bool LineGenerated = false;
    public bool selected;

    //Starts function to get star connections.
    private void Awake() {
        StartGetStarRoutes();
    }

    //Get stars within sphere radius 4, calc distance.
    private IEnumerator GetStarRoutes()
    {
        yield return new WaitForSeconds(GetComponent<StarLerpScript>().maxLerpTime + 0.5f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, 12);
        foreach (Collider collider in colliders)
        {
            starRoutes.Add(collider.transform.GetComponent<Star>(), Vector3.SqrMagnitude(transform.position - collider.transform.position));
            yield return null;
        }
    }

    public void StartGetStarRoutes() {
        StartCoroutine(GetStarRoutes());
    }

    //Generates line renderers to the connected stars.
    private IEnumerator MakeLineRenderers()
    {
        foreach (KeyValuePair<Star, float> entry in starRoutes)
        {
            LineRenderer newLine = linePrefab;
            Vector3[] positions = { transform.position, entry.Key.transform.position };
            newLine.SetPositions(positions);
            Instantiate(newLine);
            yield return null;
        }
        LineGenerated = true;
    }

    public void StartMakeLineRenderers() {
        StartCoroutine(MakeLineRenderers());
    }
}
