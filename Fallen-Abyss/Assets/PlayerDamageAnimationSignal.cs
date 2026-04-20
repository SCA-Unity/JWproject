using UnityEngine;
using TwoBitMachines.FlareEngine.ThePlayer;

namespace TwoBitMachines.FlareEngine
{
    [DisallowMultipleComponent]
    public class PlayerDamageAnimationSignal : MonoBehaviour
    {
        [Header("Refs")]
        public Player player;

        [Header("Signals")]
        public string hurtSignal = "hurt";
        public string deathSignal = "death";

        [Header("Timing")]
        [Min(0f)] public float hurtStartDelay = 0.1f; // 피격 후 hurt 시작 지연시간
        [Min(0f)] public float hurtDuration = 0.2f;   // hurt 유지 시간

        private float hurtStartAt;
        private float hurtUntil;
        private bool dead;

        private void Awake()
        {
            if (player == null)
            {
                player = GetComponent<Player>();
            }
        }

        private void Update()
        {
            if (player == null)
            {
                return;
            }

            if (dead)
            {
                player.signals.Set(deathSignal, true);
                player.signals.Set(hurtSignal, false);
                return;
            }

            float now = Time.time;
            bool inHurtWindow = now >= hurtStartAt && now < hurtUntil;
            player.signals.Set(hurtSignal, inHurtWindow);
        }

        public void OnDamaged(ImpactPacket impact)
        {
            if (dead || player == null || impact.damageValue >= 0)
            {
                return;
            }

            float startTime = Time.time + hurtStartDelay;
            float endTime = startTime + hurtDuration;

            // 연속 피격 시 hurt 창을 자연스럽게 연장
            if (startTime > hurtStartAt)
            {
                hurtStartAt = startTime;
            }
            if (endTime > hurtUntil)
            {
                hurtUntil = endTime;
            }
        }

        public void OnDeath(ImpactPacket impact)
        {
            if (player == null)
            {
                return;
            }

            dead = true;
            hurtStartAt = 0f;
            hurtUntil = 0f;
            player.signals.Set(deathSignal, true);
            player.signals.Set(hurtSignal, false);
        }
    }
}