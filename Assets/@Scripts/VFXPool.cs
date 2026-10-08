using UnityEngine;

public class VFXPool : ObjectPool<VFXController>
{
    public static VFXPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
