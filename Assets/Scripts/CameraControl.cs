using TMPro;
using UnityEngine;

public class CameraControl : MonoBehaviour 
{
    [SerializeField] private GameObject selectedStarText;
    [SerializeField] private GameObject selectedStartText;
    [SerializeField] private GameObject selectedEndText;
    [SerializeField] private GameObject starSpawner;
    [SerializeField] private GameObject UI;
    [SerializeField] private float sensitivity;
    [SerializeField] private float speed;
    [SerializeField] LayerMask raycastMask;
    private Vector3 rotationXY;
    private Vector3 move;
    private bool moveCam;
    private bool UIOpen;
    private Camera cam;
    private RaycastHit hit;
    private Star selectedStart;
    private Star selectedEnd;

    //Set camera start point based on bounds.
    private void Start() {
        cam = Camera.main;
        transform.position = new Vector3(0, 0, starSpawner.GetComponent<StarSpawner>().bounds[2] - 5f);
    }

    private void Update() {
        //Check if quit.
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
        //Allow inputs when finished generating stars.
        if (starSpawner.GetComponent<StarSpawner>().allowInput) {
            //Toggle UI.
            if (Input.GetKeyDown(KeyCode.E)) {
                UIOpen = !UIOpen;
                UI.SetActive(UIOpen);
            }
            //Toggle movement.
            if (Input.GetMouseButton(1)) {
                moveCam = true;
            } else {
                moveCam = false;
            }
            //If moving, allow movement and lock cursor.
            if (moveCam) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                LookInput();
                MoveInput();
            //Else unlock cursor and allow player to click on stars.
            } else {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                //Allow player to click on stars by casting raycasts as long as UI is not open.
                if (Input.GetMouseButtonDown(0) && !UIOpen) {
                    if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit newHit)) {
                        if (newHit.transform.CompareTag("Star")) {
                            if (hit.transform != null) {
                                hit.transform.GetComponent<Star>().selected = false;
                            }
                            hit = newHit;
                            hit.transform.GetComponent<StarLerpScript>().StartSizeLerp();
                            selectedStarText.GetComponent<TextMeshProUGUI>().text = hit.transform.name;
                        }
                    }
                }
            }
        }
    }

    //Uses move movement to change camera rotation.
    private void LookInput() {
        rotationXY.x -= Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;
        rotationXY.y += Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        transform.eulerAngles = rotationXY;
    }

    //Uses directional inputs to translate player.
    //Translation moves player based on rotation rather than along the axis.
    private void MoveInput() {
        move.x = Input.GetAxisRaw("Horizontal");
        move.z = Input.GetAxisRaw("Vertical");
        transform.Translate(move * speed * Time.deltaTime);
    }

    //Star selectors.
    public void SelectStart() {
        selectedStart = hit.transform.GetComponent<Star>();
        selectedStartText.GetComponent<TextMeshProUGUI>().text = selectedStart.transform.name;
    }

    public void SelectEnd() {
        selectedEnd = hit.transform.GetComponent<Star>();
        selectedEndText.GetComponent<TextMeshProUGUI>().text = selectedEnd.transform.name;
    }

    public void PathFind() { 
        starSpawner.GetComponent<PathFind>().StartGetPath(selectedStart, selectedEnd);
    }
}
