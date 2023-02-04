using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Manage player's HUD and other UI elements
    [SerializeField] TMP_Text waveCountDisplay;

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject pausePanel;

    [SerializeField] TMP_Text waveGameOver;
    [SerializeField] TMP_Text timeGameOver;

    int wave;

    static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("UIManager");
                instance = go.GetComponent<UIManager>();
            }

            return instance;
        }
    }

    void Awake()
    {
        GameController.Instance.StateChange.AddListener(GameStateChange);
        GameController.Instance.GetEnemyManager().NewWave.AddListener(NewWave);
    }

    void GameStateChange(GameState state)
    {
        switch(state)
        {
            case GameState.Playing:
            gameOverPanel.SetActive(false);
            pausePanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            break;

            case GameState.Paused:
            gameOverPanel.SetActive(false);
            pausePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            break;

            case GameState.Over:
            gameOverPanel.SetActive(true);
            pausePanel.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            waveGameOver.text = $"Waves Cleared: {wave}"; //last wave cleared
            timeGameOver.text = $"Time: {Time.timeSinceLevelLoad}"; //ToDo: might not be what we want, also needs to be formated
            break;

            default:
            break;
        }
    }

    void NewWave(int waveCount)
    {
        wave = waveCount;
        waveCountDisplay.text = $"Wave {wave}";
    }
}
