using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : Manager<MenuManager>
{
    [SerializeField]
    private CursorLockMode lockMode = CursorLockMode.Locked;

    public static bool IsGame => Instance._Mode == Mode.Game;
    public static bool IsNewGame { get; set; }

    public delegate void Event();
    public event Event OnChange;

    private KeyAction pause;

    public enum Mode { Init, Menu, Game, View, Shot, All }
    [SerializeField]
    private Mode m_mode;

    public Mode _Mode
    {
        get => m_mode;
        set {
            switch (value)
            {
                case Mode.Init:
                    IsNewGame = true;
                    Time.timeScale = 1;
                    Cursor.visible = false;
                    break;
                case Mode.Menu:
                    Time.timeScale = 0;
                    Cursor.visible = true;
                    break;
                default:
                    Time.timeScale = 1f;
                    Cursor.visible = false;
                    break;
            }
            Cursor.lockState = Cursor.visible
                ? CursorLockMode.None
                : lockMode;

            m_mode = value;
            OnChange.Invoke();
        }
    }

    [SerializeField]
    private Image panel = null;
    [SerializeField]
    private GameObject lights = null;
    [SerializeField]
    private GameObject options = null;

    private void SetInput()
    {
        pause = new KeyAction(KeyInputMode.KeyDown, InputManager.PAUSE_KEY, Pause, Mode.Game);
        InputManager.Instance.AddKeyAction(pause);
    }

    private void Awake()
    {
        #if UNITY_EDITOR
            if (SceneManager.sceneCount > 1)
                SceneManager.UnloadSceneAsync(1);
        #else
            // SceneManager.LoadScene(1, LoadSceneMode.Additive);
        #endif

        SetInput();

        OnChange += () => {
            options.SetActive(_Mode == Mode.Menu);
        };

        
    }

    private void OnDestroy()
    {
        Prefs.FirstPass = true;
    }

    private IEnumerator AttachLevel()
    {
        while (panel.color.a < 1f) {
            panel.color = new Color(0, 0, 0, panel.color.a + Time.deltaTime * 2);
            yield return null;
        }

        foreach (Light light in lights.GetComponentsInChildren<Light>()) {
            light.enabled = true;
        }

        if (Prefs.FirstPass) {
            Gimbal.Instance.GetCinemachine(FindObjectOfType<UnitController>());
            _Mode = Mode.View;
            Prefs.FirstPass = false;
        }
        else {
            _Mode = Mode.Game;
            Scenario.Instance.Clear();
        }

        while (panel.color.a > 0) {
            panel.color = new Color(0, 0, 0, panel.color.a - Time.deltaTime * 2);
            yield return null;
        }
    }

    public void StartGame()
    {
        _Mode = Mode.Init;
        StopAllCoroutines();
        StartCoroutine(AttachLevel());
    }

    public void Pause()
    {
        _Mode = Mode.Menu;
    }

    public void Play()
    {
        if (_Mode == Mode.Menu)
        {
            _Mode = Mode.Game;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    /*private IEnumerator AttachLevel()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        operation.allowSceneActivation = true;

        while (operation.progress < 0.9f) {
            yield return null;
            if (SceneManager.sceneCount > 1) {
                AsyncOperation unload = SceneManager.UnloadSceneAsync(1);
                while (!unload.isDone)
                    yield return null;
            }
        }

        while (!operation.isDone) {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        Scenario.Instance.Init();
        m_nextMode = Mode.Game;
        IsNewGame = true;
        Play();
    }*/
}