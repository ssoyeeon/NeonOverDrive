using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartSceneUI : MonoBehaviour
{
    public TMP_Text moneyText;
    public GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        moneyText.text = manager.money.ToString();
    }
}
