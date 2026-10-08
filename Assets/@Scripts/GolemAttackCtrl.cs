using UnityEngine;

public class GolemAttackCtrl : MonoBehaviour
{
    // 돌 오브젝트를 GetFromPool() 한다.
    private void SpawnStone()
    {
        // mStonePool에는 GolemStonePool이 담긴 상태에서 SpawnStone()을 사용하여 풀 기능을 사용한다.
        //StoneObj obj = mStonePool.GetFromPool(Random.Range(0, 2));
        //obj.gameObject.SetActive(true);
        //obj.transform.position = transform.position;
    }
}
