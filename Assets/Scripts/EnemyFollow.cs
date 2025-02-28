using UnityEngine;
using UnityEngineInternal;
using System.Collections;

public class EnemyFollow : MonoBehaviour
{
    public Transform target;
    public float speed = 6f;
    private bool triggered = false;
    private bool hit = false;
    public float wanderTimer = 5f;
    public float timer;
    public float wanderRadius = 20f;
    private Vector3 goal;
    private int health = 3;
    private bool dead = false;

    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //target = GameObject.FindGameObjectWithTag("PlayerBEAN").GetComponent<Transform>();
        timer = wanderTimer;
        Vector3 goal = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 goal = transform.position;
        timer += Time.deltaTime;
        //while (health > 0)
        //{
            if (triggered && !hit && health > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            }
            else if (timer >= wanderTimer && !triggered && !hit && health > 0)
            {
                Vector3 pos = transform.position;
                float x = Random.Range(pos.x - wanderRadius, pos.x + wanderRadius);
                float z = Random.Range(pos.z - wanderRadius, pos.z + wanderRadius);
                goal = new Vector3(x, pos.y, z);

                timer = 0;
            }
            if (!triggered && !hit && health > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.deltaTime);
            }
        //}
        /*
        if (!dead)
        {
            transform.Rotate(0, 0, 90);
            dead = true;
        }
        
        if (health <= 0)
        {
            //Destroy(gameObject);
            
        }
        */
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
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
        health--;
        Debug.Log("health" + health);
        Invoke(nameof(ResetHit), 1f);
    }

    void ResetHit()
    {
        hit = false;
    }
    
}
