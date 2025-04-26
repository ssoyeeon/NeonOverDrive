using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OutReward : MonoBehaviour
{
    public TMP_Text moneyTextt;
    // Update is called once per frame
    void Update()
    {
        moneyTextt.text = GameManager.Instance.reward.ToString();
    }
}
