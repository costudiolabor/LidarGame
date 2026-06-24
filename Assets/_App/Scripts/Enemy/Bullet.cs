using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private int damage = 1;
    [SerializeField] private int timeLife = 2;
    
    public void Initialize(Vector3 shootDirection, float speed)
    {
        transform.rotation = Quaternion.LookRotation(shootDirection);
        transform.SetParent(null);
        rigidBody.linearVelocity = shootDirection.normalized * speed;
       
        Invoke(nameof(Dead), timeLife);
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
        Destroy(gameObject);
    }
}
