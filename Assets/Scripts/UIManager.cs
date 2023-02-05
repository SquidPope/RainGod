using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Manage player's HUD and other UI elements
    [SerializeField] TMP_Text waveCountDisplay;

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject pausePanel;

    [SerializeField] TMP_Text waveGameOver;
    [SerializeField] TMP_Text timeGameOver;

    [SerializeField] Slider chaacHealth;

    //ToDo: Splash image announcing next wave

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
        Chaac.Instance.HealthChange.AddListener(HealthChange);

        chaacHealth.maxValue = Chaac.Instance.GetMaxHealth();
    }

    void GameStateChange(GameState state)
    {
        Debug.Log($"state {state}");
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
            float time = Time.timeSinceLevelLoad;
            string min = Mathf.Floor(time / 60f).ToString("00"); //ToDo: A time of over an hour probaly breaks this, banking on nobody lasting that long
            string sec = Mathf.Floor(time % 60).ToString("00");
            timeGameOver.text = $"Time: {min} : {sec}";
            break;

            default:
            break;
        }
    }

    void HealthChange(float health) { chaacHealth.value = health; }

    void NewWave(int waveCount)
    {
        wave = waveCount;
        waveCountDisplay.text = $"Wave {wave}";
    }
}
