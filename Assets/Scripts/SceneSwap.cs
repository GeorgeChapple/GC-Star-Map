using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwap : MonoBehaviour
{
    //Used to reload the scene and generate new stars.
    void Start() {
        SceneManager.LoadScene("Main");
    }
}
