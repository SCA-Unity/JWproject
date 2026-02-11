using UnityEngine;
using TwoBitMachines.FlareEngine;

public class FlareMPQuickDiag : MonoBehaviour
{
    float t;
    Character ch;

    void Awake() => ch = GetComponent<Character>();

    void Update()
    {
        if (Time.time < t) return;
        t = Time.time + 0.5f;

        if (ch == null)
        {
            Debug.Log($"[MPDiag:{name}] Character component MISSING");
            return;
        }

        Debug.Log(
            $"[MPDiag:{name}] type={ch.type} useMovingPlatform={ch.world.useMovingPlatform} " +
            $"passengersListCount={Character.passengers.Count} movingPlatformsCount={Character.movingPlatforms.Count}"
        );
    }
}
