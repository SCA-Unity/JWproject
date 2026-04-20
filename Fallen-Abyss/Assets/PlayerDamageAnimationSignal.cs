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

        [Min(0f)] public float hurtDuration = 0.2f;

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

            if (Time.time < hurtUntil)
            {
                player.signals.Set(hurtSignal, true);
            }
            else
            {
                player.signals.Set(hurtSignal, false);
            }
        }

        public void OnDamaged(ImpactPacket impact)
        {
            if (dead || player == null || impact.damageValue >= 0)
            {
                return;
            }

            hurtUntil = Time.time + hurtDuration;
            player.signals.Set(hurtSignal, true);
        }

        public void OnDeath(ImpactPacket impact)
        {
            if (player == null)
            {
                return;
            }

            dead = true;
            player.signals.Set(deathSignal, true);
            player.signals.Set(hurtSignal, false);
        }
    }
}
