using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    [SerializeField] private float _globalShakeSource = 1f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void Shake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(_globalShakeSource);
    }
}
