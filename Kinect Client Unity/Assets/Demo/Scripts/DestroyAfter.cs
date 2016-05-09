using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour
{
    public float lifeTime = 2.5f;

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
