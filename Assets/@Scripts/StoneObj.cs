using UnityEngine;

public class StoneObj : MonoBehaviour
{
    // StoneObj가 비활성화 되는 조건
    private void Update()
    {
        // 해당 오브젝트가 오브젝트 풀에서 리턴되어 활성화 상태가 되면
        // 특정 조건에 의해 다시 SetActive(false)로 비활성화 시켜야 오브젝트 풀을 만든 의미가 있다.
        //if(transform.position.y < mGroundHeight)
        //{
        //    gameObject.SetActive(false);
        //}
    }
}
