using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class StarSpawner : MonoBehaviour
{
    [SerializeField] private Slider starsSlider;
    [SerializeField] private Slider boundsSlider;
    [SerializeField] private TextMeshProUGUI starsSliderText;
    [SerializeField] private TextMeshProUGUI boundsSliderText;
    [SerializeField] private GameObject templateStar;
    [SerializeField] private GameObject enumDropDown;
    [SerializeField] private int starCollideMagnitude;
    private List<String> firstNames = new List<String>();
    private List<String> lastNames = new List<String>();
    private int starFail;
    private int stars;
    public List<Star> allStars = new List<Star>();
    public float[] bounds = new float[6]; // Lower XYZ, Higher XYZ
    public bool allowInput;

    // Start is called before the first frame update
    private void Start() {
        //Set up playerprefs and get star names from files.
        stars = PlayerPrefs.GetInt("stars");
        starsSlider.value = stars;
        starsSliderText.text = "Star Count: " + Convert.ToString(PlayerPrefs.GetInt("stars"));
        boundsSlider.value = PlayerPrefs.GetInt("bounds");
        boundsSliderText.text = "Bounds: " + Convert.ToString(PlayerPrefs.GetInt("bounds"));
        for (int i = 0; i < bounds.Length; i++) {
            int newValue = PlayerPrefs.GetInt("bounds");
            if (i <= 2) {
                newValue *= -1;
            }
            bounds[i] = newValue;    
        }
        foreach (string line in File.ReadLines("FirstNames.txt")) {
            firstNames.Add(line);
        }
        foreach (string line in File.ReadLines("LastNames.txt")) {
            lastNames.Add(line);
        }
        GenerateStars();
    }

    //On slider change update text and playerprefs.
    public void StarSliderChange() {
        PlayerPrefs.SetInt("stars", Convert.ToInt32(starsSlider.value));
        starsSliderText.text = "Star Count: " + Convert.ToString(PlayerPrefs.GetInt("stars"));
    }

    //On input box change, update bounds value playerprefs based on input box name.
    public void BoundsSliderChange () {
        PlayerPrefs.SetInt("bounds", Convert.ToInt32(boundsSlider.value));
        boundsSliderText.text = "Bounds: " + Convert.ToString(PlayerPrefs.GetInt("bounds"));
    }

    //Swaps the scene to regenerate stars.
    public void SwapScene() {
        SceneManager.LoadScene("Swap");
    }

    //Changes galaxy generator option.
    public void GenerateButtonChange() {
        PlayerPrefs.SetString("option", enumDropDown.GetComponent<TextMeshProUGUI>().text);
    }

    //Iterates through each star generating their lines.
    private IEnumerator LineGenerating() {
        yield return new WaitForSeconds(templateStar.GetComponent<StarLerpScript>().maxLerpTime + 1f);
        for (int i = 0; i < transform.childCount; i++) {
            bool next = true;
            transform.GetChild(i).GetComponent<Star>().StartMakeLineRenderers();
            while (next) {
                next = transform.GetChild(i).GetComponent<Star>().LineGenerated;
                yield return null;
            }
        }
        allowInput = true;
    }

    //Generates star positions.
    public void GenerateStars() {
        Vector3 newPos = Vector3.zero;
        bool invalid;
        for (int i = 0; i < stars; i++) {
            starFail = 0;
            invalid = true;
            GameObject s = Instantiate(templateStar, transform.position, transform.rotation);
            s.name = firstNames[UnityEngine.Random.Range(0, firstNames.Count)] + " " + lastNames[UnityEngine.Random.Range(0, lastNames.Count)];
            s.transform.parent = transform;
            while (invalid) {
                newPos = GetCoordinateGenerator();
                invalid = ProximityCheck(newPos, starCollideMagnitude);
                starFail++;
                if (starFail > 10) {
                    Destroy(s);
                    stars--;
                    invalid = false;
                }
            }
            s.GetComponent<StarLerpScript>().pos = newPos;
        }
        for (int i = 0; i < stars; i++) {
            transform.GetChild(i).GetComponent<StarLerpScript>().StartPositionLerp();
            allStars.Add(transform.GetChild(i).GetComponent<Star>());
        }
        StartCoroutine(LineGenerating());
    }

    //Gets the coordinate bounds for the galaxy.
    private Vector3 GetCoordinateGenerator() {
        string text = PlayerPrefs.GetString("option");
        if (text == "Cone") {
            return CoordinateGenerators.Cone(bounds);
        }
        else if (text == "Box") {
            return CoordinateGenerators.Box(bounds);
        }
        else if (text == "Pyramid") {
            return CoordinateGenerators.SqureBasedPyramid(bounds);
        }
        else if (text == "Ovoid") {
            return CoordinateGenerators.Ovoid(bounds);
        }
        else {
            return CoordinateGenerators.Box(bounds);
        }
    }

    //Checks if star position is valid.
    private bool ProximityCheck(Vector3 center, float collideMagnitude)  {
        float distance;
        bool proxCheck = false;
        for (int i = 0; i < transform.childCount - 1; i++) {
            distance = Vector3.SqrMagnitude(transform.GetChild(i).GetComponent<StarLerpScript>().pos - center);
            if (distance < collideMagnitude) {
                proxCheck = true;
                break;
            }
        }
        return proxCheck;
    }
}
