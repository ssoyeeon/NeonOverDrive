using KartGame.KartSystems;
using TMPro;
using UnityEngine;

namespace KartGame.UI
{
    // 게임 내에서 카트의 속도 정보를 UI에 표시하는 클래스
    public class InGameInfo : MonoBehaviour
    {
        // 속도를 표시할 UI 텍스트 요소
        public TextMeshProUGUI Speed;

        // 자동으로 카트를 찾을지 여부 설정
        public bool AutoFindKart = true;

        // 카트 컨트롤러 참조
        public ArcadeKart KartController;

        // 시작 시 초기화
        void Start()
        {
            // 자동 찾기가 활성화되어 있으면 씬에서 카트 컨트롤러 찾기
            if (AutoFindKart)
            {
                ArcadeKart kart = FindObjectOfType<ArcadeKart>();
                KartController = kart;
            }

            // 카트 컨트롤러가 없으면 이 게임 오브젝트 비활성화
            if (!KartController)
            {
                gameObject.SetActive(false);
            }
        }

        // 매 프레임마다 업데이트
        void Update()
        {
            // 카트의 현재 속도 계산
            float speed = KartController.Rigidbody.velocity.magnitude;

            // 시속(km/h)으로 속도 표시
            Speed.text = string.Format($"{Mathf.FloorToInt(speed * 3.6f)} km/h");

            // 초속(m/s)도 함께 표시
            Speed.text += string.Format($"\n{speed:0.0} m/s");
        }
    }
}