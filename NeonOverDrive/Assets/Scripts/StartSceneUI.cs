using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartSceneUI : MonoBehaviour
{
    public TMP_Text moneyText;
    void Start()
    {

    }
    void Update()
    {
        moneyText.text = GameManager.Instance.money.ToString();
    }
}
