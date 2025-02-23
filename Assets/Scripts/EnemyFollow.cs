using UnityEngine;
using UnityEngineInternal;

public class EnemyFollow : MonoBehaviour
{
    public Transform target;
    public float speed = 6f;
    private bool triggered = false;
    private bool hit = false;

    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //target = GameObject.FindGameObjectWithTag("PlayerBEAN").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered && !hit)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy Update Running");
        if (other.CompareTag("PlayerTrigger"))
        {
            Debug.Log("Triggered");
            triggered = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            triggered = false;
        }
    }

    public void Hit()
    {
        hit = true;
        Invoke(nameof(ResetHit), 1f);
    }

    void ResetHit()
    {
        hit = false;
    }
}
