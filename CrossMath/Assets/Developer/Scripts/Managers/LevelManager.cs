using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager instance;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void ReloadLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevel(int _sceneBuildIndex) {
        if(Application.CanStreamedLevelBeLoaded(_sceneBuildIndex)) {
            SceneManager.LoadScene(_sceneBuildIndex);
        } else {
            Debug.LogError($"Level Manager: Couldn't load scene with index {_sceneBuildIndex}");
        }
    }
}
