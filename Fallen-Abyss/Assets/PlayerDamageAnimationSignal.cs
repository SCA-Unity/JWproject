using UnityEngine;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine.Events;

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

        [Header("Hurt")]
        [Min(0f)] public float hurtDuration = 0.2f;

        [Header("Death Sequence (PlayerDeath-style)")]
        [Min(0f)] public float deathTime = 3f;
        [Min(0f)] public float transitionTime = 0.9f;
        public bool disableColliderOnDeath = true;
        public bool blockInputOnDeath = true;
        public bool resetHealthOnTransition = true;
        public float resetHealthValue = 3f;
        public UnityEvent onDeathBegin;
        public UnityEvent onDeathTransitionBegin;

        private float hurtUntil;
        private float deathCounter;
        private bool deadSequence;
        private bool inTransition;
        private BoxCollider2D cachedCollider;
        private Health health;

        private void Awake()
        {
            if (player == null)
            {
                player = GetComponent<Player>();
            }
            if (cachedCollider == null)
            {
                cachedCollider = GetComponent<BoxCollider2D>();
            }
            if (health == null)
            {
                health = GetComponent<Health>();
            }
        }

        private void Update()
        {
            if (player == null)
            {
                return;
            }

            if (deadSequence)
            {
                player.signals.Set(deathSignal, true);
                player.signals.Set(hurtSignal, false);
                RunDeathSequence();
            }
            else if (Time.time < hurtUntil)
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
            if (deadSequence || player == null || impact.damageValue >= 0)
            {
                return;
            }

            hurtUntil = Time.time + hurtDuration;
            player.signals.Set(hurtSignal, true);
        }

        public void OnDeath(ImpactPacket impact)
        {
            if (player == null || deadSequence)
            {
                return;
            }

            deathCounter = 0f;
            inTransition = false;
            deadSequence = true;

            if (blockInputOnDeath)
            {
                player.BlockInput(true);
            }
            if (disableColliderOnDeath && cachedCollider != null)
            {
                cachedCollider.enabled = false;
            }

            player.signals.Set(deathSignal, true);
            player.signals.Set(hurtSignal, false);
            onDeathBegin?.Invoke();
        }

        private void RunDeathSequence()
        {
            deathCounter += Time.deltaTime;

            if (!inTransition)
            {
                if (deathCounter < deathTime)
                {
                    return;
                }

                if (transitionTime <= 0f)
                {
                    FinishDeathSequence();
                    return;
                }

                inTransition = true;
                deathCounter = 0f;
                onDeathTransitionBegin?.Invoke();
                return;
            }

            if (deathCounter >= transitionTime)
            {
                FinishDeathSequence();
            }
        }

        private void FinishDeathSequence()
        {
            deadSequence = false;
            inTransition = false;
            deathCounter = 0f;

            if (blockInputOnDeath)
            {
                player.BlockInput(false);
            }
            if (disableColliderOnDeath && cachedCollider != null)
            {
                cachedCollider.enabled = true;
            }
            if (resetHealthOnTransition && health != null)
            {
                health.SetValue(resetHealthValue);
            }
        }
    }
}
