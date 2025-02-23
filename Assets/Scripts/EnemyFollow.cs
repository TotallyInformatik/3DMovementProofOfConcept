using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    private Transform target;
    public float speed = 6f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("PlayerBEAN").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
}
