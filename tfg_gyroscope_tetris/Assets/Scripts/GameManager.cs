using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    [SerializeField] RotationManager _rotManager;

    private void Awake()
    {
        if (_instance != null)
            Destroy(this.gameObject);

        _instance = this;
        DontDestroyOnLoad(this);
        Init();
    }

    private void Init()
    {
        Screen.sleepTimeout = 0; //Avoid the device screens from ever turning off alone

        Input.gyro.enabled = true;
        _rotManager.Init();
    }

    public static void LoadScene(GAME_SCENES scene)
    {
        SceneManager.LoadScene((int)scene);
    }

    public static void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//Reset the current scene
    }
}

public enum GAME_SCENES
{
    StartScene = 0, CoreLoopScene = 1
}
