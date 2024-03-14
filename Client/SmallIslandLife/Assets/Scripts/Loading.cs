using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    Title,
    Lobby,
    Game
}

public class Loading : MonoBehaviour
{
    public static Loading instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

            return;
        }

        Destroy(gameObject);
    }

    public void LoadScene(Scenes scenes)
    {
        StartCoroutine(LoadSceneCoroutine(scenes));
    }

    private IEnumerator LoadSceneCoroutine(Scenes scenes)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)scenes);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}
