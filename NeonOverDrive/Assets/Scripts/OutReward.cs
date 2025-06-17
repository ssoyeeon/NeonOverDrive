using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutReward : MonoBehaviour
{
    [Header("기본 UI 요소들")]
    public TMP_Text moneyText;
    public TMP_Text timeText;

    [Header("데미지 파트 이미지들 (Inspector에서 연결)")]
    public Image frontImage;    
    public Image bodyImage;     
    public Image backImage;     
    public Image leftFrontWheelImage;  
    public Image leftBackWheelImage;   
    public Image rightFrontWheelImage; 
    public Image rightBackWheelImage;  

    void Start()
    {
        // 데미지 상태에 따른 이미지 색상 설정
        UpdateDamageDisplay();
    }

    void Update()
    {
        // 기본 UI 업데이트
        if (timeText != null)
            timeText.text = GameManager.Instance.gameTimer.ToString("F1");

        if (moneyText != null)
            moneyText.text = GameManager.Instance.money.ToString();
    }

    void UpdateDamageDisplay()
    {
        var damage = GameManager.Instance.GetCurrentVehicleDamage();

        // VehicleController와 동일한 로직 적용
        // 앞부분 데미지 (엔진 위주)
        if (frontImage != null)
        {
            frontImage.color = GetDamageColor(damage.engineDamage);
        }

        // 차체 데미지
        if (bodyImage != null)
        {
            bodyImage.color = GetDamageColor(damage.bodyDamage);
        }

        // 뒷부분 데미지
        if (backImage != null)
        {
            backImage.color = GetDamageColor(damage.bodyDamage);
        }

        // 휠 데미지 (모든 휠에 동일하게 적용)
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

    // VehicleController와 동일한 데미지 색상 로직
    Color GetDamageColor(float damagePercent)
    {
        if (damagePercent >= 60f)
            return Color.red;        // 심각한 손상
        else if (damagePercent >= 30f)
            return Color.yellow;     // 경미한 손상
        else
            return Color.white;      // 정상
    }
}