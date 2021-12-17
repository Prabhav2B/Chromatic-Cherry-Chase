using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManagement : SingleInstance<SceneTransitionManagement>
{
    
    public void LoadNextLevel()
    {
        var nextSceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Assert(nextSceneBuildIndex <= SceneManager.sceneCountInBuildSettings, this.gameObject);
        if (nextSceneBuildIndex > SceneManager.sceneCountInBuildSettings)
        {
            return;
        }
        SceneManager.LoadScene(nextSceneBuildIndex);
    }
    
    public void LoadLastLevel()
    {
        var nextSceneBuildIndex = SceneManager.GetActiveScene().buildIndex - 1;
        //Debug.Assert(nextSceneBuildIndex >= 0, this.gameObject);
        if (nextSceneBuildIndex < 0)
        {
            return;
        }
        SceneManager.LoadScene(nextSceneBuildIndex);
    }

    public bool IsInvalidSceneTransition(int offset)
    {
        return SceneManager.GetActiveScene().buildIndex + offset < 0 || SceneManager.GetActiveScene().buildIndex + offset > SceneManager.sceneCountInBuildSettings;
    }

    public void LoadLevel(int buildIndex)
    {
        Debug.Assert(buildIndex >= SceneManager.sceneCount, this.gameObject);
        if (buildIndex >= SceneManager.sceneCount)
        {
            return;
        }
        SceneManager.LoadScene(buildIndex);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
