using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
  [SerializeField] Slider loadingSlider;

  private void Start()
  {
    LoadLevel(KeyString.NAME_SCENE_GAMEPLAY);
  }

  public void LoadLevel(string levelToLoad)
  {
    StartCoroutine(LoadSceneFake(levelToLoad));
  }

  IEnumerator LoadLevelAsync(string levelToLoad)
  {
    AsyncOperation loadAsync = SceneManager.LoadSceneAsync(levelToLoad);
    while (!loadAsync.isDone)
    {
      float progressValue = Mathf.Clamp01(loadAsync.progress / .9f);
      loadingSlider.value = progressValue;
      yield return null;
    }
  }

  float[] waitLengths = new float[4] { .5f, .7f, .9f, .9f };
  IEnumerator LoadSceneFake(string levelToLoad)
  {
    var LENGTH = waitLengths[UnityEngine.Random.Range(0, waitLengths.Length)];
    var timer = LENGTH;
    while (timer > 0)
    {
      float progressValue = Mathf.Clamp01((LENGTH - timer) / LENGTH);
      loadingSlider.value = progressValue;
      timer -= Time.deltaTime;
      yield return null;
    }

    AsyncOperation loadAsync = SceneManager.LoadSceneAsync(levelToLoad);
    while (!loadAsync.isDone)
    {
      yield return null;
    }
  }
}
