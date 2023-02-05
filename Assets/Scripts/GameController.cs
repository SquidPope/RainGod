using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameStateEvent : UnityEvent<GameState> {}
public enum GameState {Playing, Paused, Over}
public class GameController : MonoBehaviour
{
    // We all know what this is

    [SerializeField] EnemyManager enemyManager; //ToDo: Hardcoding, fix

    static GameController instance;
    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("GameController");
                instance = go.GetComponent<GameController>();
            }

            return instance;
        }
    }

    GameStateEvent stateEvent = new GameStateEvent();
    public GameStateEvent StateChange { get{ return stateEvent; } }

    GameState state;

    public GameState State
    {
        get { return state; }
        set
        {
            state = value;
            if (state == GameState.Paused)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;

            StateChange.Invoke(state);
        }
    }

    void Start()
    {
        State = GameState.Playing;
    }

    public EnemyManager GetEnemyManager() { return enemyManager; }

    public void OnResumePressed()
    {
        State = GameState.Playing;
    }

    public void OnMenuPressed()
    {
        //load menu
    }

    public void OnRetryPressed()
    {
        //reload this scene
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex); //ToDo: Check this
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Joystick1Button7) || Input.GetKeyUp(KeyCode.Joystick1Button2))
        {
            if (State == GameState.Playing)
                State = GameState.Paused;
            else if (State == GameState.Paused)
                State = GameState.Playing;

                //Should escape on game over cause a restart or menu?
        }
    }
}
