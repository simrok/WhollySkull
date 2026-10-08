using UnityEngine;

public class FaceMainCamera : MonoBehaviour
{
    public enum Axis { Forward, Back, Up, Down, Left, Right }

    [Header("设置")]
    public Axis faceAxis = Axis.Forward; // 选择哪个轴对着相机
    public Vector3 upDir = Vector3.up;   // 自定义向上向量
    public bool m_yawOnly = false;      // 是否只绕Y轴旋转

    private Transform _mainCamTransform;

    void Start()
    {
        if (Camera.main != null)
        {
            _mainCamTransform = Camera.main.transform;
        }
		
		upDir = transform.up;
    }

    void LateUpdate()
    {
        if (_mainCamTransform == null) return;

        // 计算从物体指向相机的向量
        Vector3 direction = transform.position - _mainCamTransform.position;

        if (m_yawOnly)
        {
            direction.y = 0;
        }

        if (direction != Vector3.zero)
        {
            // 默认 LookRotation 让 Forward 轴指向 direction
            Quaternion targetRotation = Quaternion.LookRotation(direction, upDir);

            // 根据选择的轴进行修正偏移
            transform.rotation = targetRotation * GetOffset(faceAxis);
        }
    }

    private Quaternion GetOffset(Axis axis)
    {
        switch (axis)
        {
            case Axis.Back:    return Quaternion.Euler(0, 180, 0);
            case Axis.Up:      return Quaternion.Euler(90, 0, 0);
            case Axis.Down:    return Quaternion.Euler(-90, 0, 0);
            case Axis.Left:    return Quaternion.Euler(0, 90, 0);
            case Axis.Right:   return Quaternion.Euler(0, -90, 0);
            default:           return Quaternion.identity; // Forward 不需要偏移
        }
    }
}