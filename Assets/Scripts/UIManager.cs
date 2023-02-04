using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Manage player's HUD and other UI elements
    [SerializeField] TMP_Text waveCountDisplay;

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

    public void UpdateWaveCounter (int wave)
    {
        waveCountDisplay.text = $"Wave {wave}";
    }
}
