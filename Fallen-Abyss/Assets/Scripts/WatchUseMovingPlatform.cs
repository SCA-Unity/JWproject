using System;
using System.Reflection;
using UnityEngine;

public class WatchUseMovingPlatform : MonoBehaviour
{
    // (선택) 0.5초마다 상태 로그
    public float heartbeatInterval = 0.5f;

    object _characterInstance;      // Player : Character 인스턴스
    FieldInfo _worldField;          // Character.world
    FieldInfo _useMPField;          // WorldCollision.useMovingPlatform

    bool _hasInit;
    bool _lastValue;
    float _t;

    void Awake()
    {
        TryBind();
        if (_hasInit)
        {
            _lastValue = ReadUseMP();
            Debug.Log($"[WatchUseMP:{name}] Awake useMovingPlatform={_lastValue}");
        }
        else
        {
            Debug.LogWarning($"[WatchUseMP:{name}] Failed to bind. (Character/world/useMovingPlatform not found)");
        }
    }

    void OnEnable()
    {
        if (_hasInit)
        {
            bool v = ReadUseMP();
            Debug.Log($"[WatchUseMP:{name}] OnEnable useMovingPlatform={v}");
            _lastValue = v;
        }
    }

    void Start()
    {
        if (_hasInit)
        {
            bool v = ReadUseMP();
            Debug.Log($"[WatchUseMP:{name}] Start useMovingPlatform={v}");
            _lastValue = v;
        }
    }

    void Update()
    {
        if (!_hasInit) return;

        bool v = ReadUseMP();

        // 값이 바뀌는 순간 "누가 바꿨는지" 콜스택 출력
        if (v != _lastValue)
        {
            Debug.LogWarning(
                $"[WatchUseMP:{name}] CHANGED {_lastValue} -> {v}\nCALLSTACK:\n{Environment.StackTrace}"
            );
            _lastValue = v;
        }

        // 주기적으로 상태 출력(옵션)
        if (heartbeatInterval > 0 && Time.time >= _t)
        {
            _t = Time.time + heartbeatInterval;
            Debug.Log($"[WatchUseMP:{name}] heartbeat useMovingPlatform={v}");
        }
    }

    bool ReadUseMP()
    {
        object worldObj = _worldField.GetValue(_characterInstance);
        // WorldCollision이 struct일 수 있으니 박싱된 worldObj에서 필드를 읽는다
        return (bool)_useMPField.GetValue(worldObj);
    }

    void TryBind()
    {
        // 이 오브젝트에 붙은 MonoBehaviour 중에서 "Character"를 상속한 컴포넌트를 찾는다.
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var behaviours = GetComponents<MonoBehaviour>();

        foreach (var b in behaviours)
        {
            if (b == null) continue;

            Type t = b.GetType();
            Type scan = t;
            while (scan != null)
            {
                if (scan.Name == "Character")
                {
                    _characterInstance = b;

                    // Character.world 필드 찾기
                    _worldField = scan.GetField("world", flags);
                    if (_worldField == null) return;

                    // WorldCollision.useMovingPlatform 필드 찾기
                    object worldObj = _worldField.GetValue(_characterInstance);
                    if (worldObj == null) return;

                    _useMPField = worldObj.GetType().GetField("useMovingPlatform", flags);
                    if (_useMPField == null) return;

                    _hasInit = true;
                    return;
                }
                scan = scan.BaseType;
            }
        }
    }
}
