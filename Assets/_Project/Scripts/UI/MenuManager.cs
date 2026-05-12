using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    [SerializeField] string gameSceneName = "Kitchen";

    public void PlayGame () {
        SceneManager.LoadScene (gameSceneName);
    }
}
