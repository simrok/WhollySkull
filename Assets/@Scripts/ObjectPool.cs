using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <T> Type으로 제한하기 위해 objectPool<T>로 명시
// 오브젝트 풀을 제네릭 하기 위해 Component를 상속받는 타입으로 제한
public class ObjectPool<T> : MonoBehaviour where T : Component
{
    // 초기 오브젝트를 담는다.
    [SerializeField] private T[] mOrigin;
    // 오브젝트 풀 시스템을 초기화한다.
    protected List<T>[] mPool;

    void Start()
    {
        mPool = new List<T>[mOrigin.Length];
        for (int i = 0; i < mOrigin.Length; i++)
        {
            mPool[i] = new List<T>();
        }
    }
    // 풀(mPool)로부터 오브젝트를 가져온다.
    public T GetFromPool(int id = 0)
    {
        for (int i = 0; i < mPool[id].Count; ++i)
        {
            if (!mPool[id][i].gameObject.activeInHierarchy)
            {
                mPool[id][i].gameObject.SetActive(true);
                return mPool[id][i];
            }
        }
        return MakeNewInstance(id);
    }
    // 풀(mPool)이 비어있거나, 현재 모든 오브젝트가 활성화 상태일 경우 인스턴스한다.
    public virtual T MakeNewInstance(int id)
    {
        T newObj = Instantiate(mOrigin[id]);
        mPool[id].Add(newObj);
        return newObj;
    }
}