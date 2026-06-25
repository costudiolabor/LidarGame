using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private int damage = 1;
    [SerializeField] private int timeLife = 2;

    private WaitForSeconds _waitForSeconds;
    public void Initialize(Vector3 shootDirection, float speed)
    {
        _waitForSeconds = new WaitForSeconds(timeLife);
        transform.rotation = Quaternion.LookRotation(shootDirection);
        transform.SetParent(null);
        rigidBody.linearVelocity = shootDirection.normalized * speed;
        StartCoroutine(TimeLife());
    }

    private IEnumerator TimeLife()
    {
        yield return _waitForSeconds;
        Dead();
    }
    

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
        }
        Dead();
    }

    private void Dead()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
