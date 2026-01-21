using UnityEngine;
using TwoBitMachines.FlareEngine.ThePlayer;

namespace TwoBitMachines.FlareEngine
{
    /// <summary>
    /// Timed block without adding a new Ability.
    /// Mirrors how Abilities lock player input (inputs.block) so other abilities
    /// do not keep generating locomotion/jump signals that override the block animation.
    /// </summary>
    [DisallowMultipleComponent]
    public class TimedBlockSignal : MonoBehaviour
    {
        [Header("Refs")]
        public Player player;                 // Must be the same object that has Player (Character)
        [Header("Input")]
        public KeyCode blockKey = KeyCode.Q;

        [Header("Settings")]
        [Min(0.01f)] public float blockDuration = 0.5f;

        [Tooltip("If true, blocks ALL player inputs during block (inputs.block = true).")]
        public bool lockInputsDuringBlock = true;

        [Tooltip("If true, forces X velocity to 0 during block via ApplyVelocityX(0).")]
        public bool forceStopXDuringBlock = true;

        private bool blocking;
        private float endTime;

        // In case something else also blocks input, we restore only what we changed.
        private bool weLockedInputs;

        private void Awake()
        {
            if (player == null)
                player = GetComponent<Player>();
        }

        private void OnDisable()
        {
            // Safety: if object disables mid-block, restore state.
            EndBlock(force: true);
        }

        private void Update()
        {
            if (player == null || !player.enabled || !player.gameObject.activeInHierarchy)
                return;

            // Start timed block on key down (not hold)
            if (!blocking && Input.GetKeyDown(blockKey))
            {
                StartBlock();
            }

            // Maintain block effects every frame while active
            if (blocking)
            {
                MaintainBlock();

                if (Time.time >= endTime)
                {
                    EndBlock(force: false);
                }
            }
        }

        private void StartBlock()
        {
            blocking = true;
            endTime = Time.time + blockDuration;

            // 1) Signal for SpriteEngineUA state
            player.signals.Set("block", true);

            // 2) Lock inputs similarly to how abilities gate locomotion/other actions
            if (lockInputsDuringBlock)
            {
                // Only lock if not already locked by something else
                // (Player exposes BlockInput which sets inputs.block)
                if (!player.inputs.block)
                {
                    player.BlockInput(true);
                    weLockedInputs = true;
                }
            }

            // 3) Remove horizontal drift so velX/Run signals don't keep triggering
            if (forceStopXDuringBlock)
            {
                player.ApplyVelocityX(0f);
            }
        }

        private void MaintainBlock()
        {
            // Keep signal asserted for the whole duration
            player.signals.Set("block", true);

            if (forceStopXDuringBlock)
            {
                player.ApplyVelocityX(0f);
            }
        }

        private void EndBlock(bool force)
        {
            if (!blocking && !force)
                return;

            blocking = false;

            // Clear signal
            if (player != null)
                player.signals.Set("block", false);

            // Restore input lock only if we set it
            if (player != null && weLockedInputs)
            {
                player.BlockInput(false);
                weLockedInputs = false;
            }
        }
    }
}
