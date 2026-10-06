using UnityEngine;

// 문 잠금 표시: 잠겨 있으면 포탈 VFX와 옆 화로 불을 끄고, 해금되면 켬
public class DoorLock : MonoBehaviour
{
    [SerializeField] private bool unlocked;
    [SerializeField] private GameObject portalVfx;   // Portal Type A
    [SerializeField] private GameObject unlockFire;  // 문 옆 화로의 불 + 조명

    public bool Unlocked => unlocked;

    private void Awake() => Apply();

    // 인장을 얻었을 때 호출 (예: 망각의 인장 획득 → 미련의 문 SetUnlocked(true))
    public void SetUnlocked(bool value)
    {
        unlocked = value;
        Apply();
    }

    private void Apply()
    {
        if (portalVfx != null) portalVfx.SetActive(unlocked);
        if (unlockFire != null) unlockFire.SetActive(unlocked);
    }
}
