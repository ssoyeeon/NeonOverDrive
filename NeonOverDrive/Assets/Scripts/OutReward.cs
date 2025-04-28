using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OutReward : MonoBehaviour
{
    public TMP_Text moneyText;
    public TMP_Text timeText;
    // Update is called once per frame
    void Update()
    {
        //moneyText.text = GameManager.Instance.reward.ToString();
        timeText.text = GameManager.Instance.gameTimer.ToString();
    }
}
