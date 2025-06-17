using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutReward : MonoBehaviour
{
    [Header("�⺻ UI ��ҵ�")]
    public TMP_Text moneyText;
    public TMP_Text timeText;

    [Header("������ ��Ʈ �̹����� (Inspector���� ����)")]
    public Image frontImage;    
    public Image bodyImage;     
    public Image backImage;     
    public Image leftFrontWheelImage;  
    public Image leftBackWheelImage;   
    public Image rightFrontWheelImage; 
    public Image rightBackWheelImage;  

    void Start()
    {
        // ������ ���¿� ���� �̹��� ���� ����
        UpdateDamageDisplay();
    }

    void Update()
    {
        // �⺻ UI ������Ʈ
        if (timeText != null)
            timeText.text = GameManager.Instance.gameTimer.ToString("F1");

        if (moneyText != null)
            moneyText.text = GameManager.Instance.money.ToString();
    }

    void UpdateDamageDisplay()
    {
        var damage = GameManager.Instance.GetCurrentVehicleDamage();

        // VehicleController�� ������ ���� ����
        // �պκ� ������ (���� ����)
        if (frontImage != null)
        {
            frontImage.color = GetDamageColor(damage.engineDamage);
        }

        // ��ü ������
        if (bodyImage != null)
        {
            bodyImage.color = GetDamageColor(damage.bodyDamage);
        }

        // �޺κ� ������
        if (backImage != null)
        {
            backImage.color = GetDamageColor(damage.bodyDamage);
        }

        // �� ������ (��� �ٿ� �����ϰ� ����)
        Color wheelColor = GetDamageColor(damage.wheelDamage);

        if (leftFrontWheelImage != null)
        {
            leftFrontWheelImage.color = wheelColor;
        }

        if (leftBackWheelImage != null)
        {
            leftBackWheelImage.color = wheelColor;
        }

        if (rightFrontWheelImage != null)
        {
            rightFrontWheelImage.color = wheelColor;
        }

        if (rightBackWheelImage != null)
        {
            rightBackWheelImage.color = wheelColor;
        }
    }

    // VehicleController�� ������ ������ ���� ����
    Color GetDamageColor(float damagePercent)
    {
        if (damagePercent >= 60f)
            return Color.red;        // �ɰ��� �ջ�
        else if (damagePercent >= 30f)
            return Color.yellow;     // ����� �ջ�
        else
            return Color.white;      // ����
    }
}