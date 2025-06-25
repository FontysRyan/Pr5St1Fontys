using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UI_handler : MonoBehaviour
{
    public TextMeshProUGUI Player_HP;
    public TextMeshProUGUI Player_DMG;
    public TextMeshProUGUI Enemy_Counter;
    public TextMeshProUGUI Wave_Counter;
    public TextMeshProUGUI Score_Counter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayerStats(int HP, int DMG)
    {
        Player_HP.text = HP.ToString();
        Player_DMG.text = HP.ToString();
    }

    public void UpdateLevelStats(int Wave, int Enemies)
    {
        Wave_Counter.text = Wave.ToString();
        Enemy_Counter.text = Enemies.ToString();
    }

    internal void UpdateScore(int score)
    {
        Score_Counter.text = score.ToString();
    }
}
