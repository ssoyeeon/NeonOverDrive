using KartGame.KartSystems;
using TMPro;
using UnityEngine;

namespace KartGame.UI
{
    // ���� ������ īƮ�� �ӵ� ������ UI�� ǥ���ϴ� Ŭ����
    public class InGameInfo : MonoBehaviour
    {
        // �ӵ��� ǥ���� UI �ؽ�Ʈ ���
        public TextMeshProUGUI Speed;

        // �ڵ����� īƮ�� ã���� ���� ����
        public bool AutoFindKart = true;

        // īƮ ��Ʈ�ѷ� ����
        public ArcadeKart KartController;

        // ���� �� �ʱ�ȭ
        void Start()
        {
            // �ڵ� ã�Ⱑ Ȱ��ȭ�Ǿ� ������ ������ īƮ ��Ʈ�ѷ� ã��
            if (AutoFindKart)
            {
                ArcadeKart kart = FindObjectOfType<ArcadeKart>();
                KartController = kart;
            }

            // īƮ ��Ʈ�ѷ��� ������ �� ���� ������Ʈ ��Ȱ��ȭ
            if (!KartController)
            {
                gameObject.SetActive(false);
            }
        }

        // �� �����Ӹ��� ������Ʈ
        void Update()
        {
            // īƮ�� ���� �ӵ� ���
            float speed = KartController.Rigidbody.velocity.magnitude;

            // �ü�(km/h)���� �ӵ� ǥ��
            Speed.text = string.Format($"{Mathf.FloorToInt(speed * 3.6f)} km/h");

            // �ʼ�(m/s)�� �Բ� ǥ��
            Speed.text += string.Format($"\n{speed:0.0} m/s");
        }
    }
}