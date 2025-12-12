using System.Collections.Generic;
using TwoBitMachines.FlareEngine;
using UnityEngine;

public class DebugVelXSignals : MonoBehaviour
{
    public Character character;
    Dictionary<string, bool> signals;

    void Awake()
    {
        if (character == null)
            character = GetComponent<Character>();

        signals = character.signals.signals;
    }

    void LateUpdate()
    {
        bool r = signals.TryGetValue("velXRight", out var vr) && vr;
        bool l = signals.TryGetValue("velXLeft", out var vl) && vl;

        Debug.Log($"velXRight={r}, velXLeft={l}");

        //signals.Clear(); // 문서에서 권장하는 방식
    }
}