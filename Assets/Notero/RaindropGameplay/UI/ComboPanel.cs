using UnityEngine;
using UnityEngine.UI;

namespace Notero.RaindropGameplay.UI
{
    public class ComboPanel : MonoBehaviour
    {
        public Image healthbar;
        public Image[] healthPoints;
        public Image BG; // พื้นหลังเดิม
        public Image FadeBG; // พื้นหลังที่ต้องการเปลี่ยน

        float health;
        float maxHealth = 0; // ค่าเริ่มต้นของ maxHealth

        void Start()
        {
            health = 0;
        }

        void HealthBarFiller()
        {
            // โค้ดส่วนนี้เกี่ยวกับการเติมแถบสุขภาพ
            // ไม่ได้มีการเปลี่ยนแปลงจากตอนก่อน

            for (int i = 0; i < healthPoints.Length; i++)
            {
                healthPoints[i].enabled = !DisplayHealthPoint(health, i);
            }
        }

        // โค้ดอื่นๆที่เกี่ยวกับการทำงานของแถบสุขภาพ

        bool DisplayHealthPoint(float _health, int pointNumber)
        {
            return ((pointNumber * 3900) >= _health);
        }

        public void Heal(float healingPoints)
        {
            health = healingPoints;

            HealthBarFiller();
            UpdateBackground();
        }

        void UpdateBackground()
        {
            if (maxHealth == 110000) // เมื่อ maxHealth เต็ม 110000
            {
                BG.gameObject.SetActive(false); // ปิดการใช้งาน BG
                FadeBG.gameObject.SetActive(true); // เปิดการใช้งาน FadeBG
            }
        }

        // เพิ่มฟังก์ชันเพื่ออัปเดตค่า maxHealth และเรียกใช้ในที่ที่ต้องการเปลี่ยนค่า
        public void SetMaxHealth(float newMaxHealth)
        {
            maxHealth = newMaxHealth;
            UpdateBackground();
        }
    }
}

