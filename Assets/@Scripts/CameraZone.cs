using Unity.Cinemachine;
using UnityEngine;

// 플레이어가 이 트리거 안에 들어오면 지정한 카메라로 전환, 나가면 기본 카메라로 복귀
// (전환 연출은 Main Camera의 CinemachineBrain 블렌드 설정을 따름)
[RequireComponent(typeof(Collider))]
public class CameraZone : MonoBehaviour
{
    [SerializeField] private CinemachineCamera zoneCamera;
    [SerializeField] private int activePriority = 20;   // 기본 카메라(10)보다 높으면 됨

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Player>() != null) zoneCamera.Priority = activePriority;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Player>() != null) zoneCamera.Priority = 0;
    }
}
