using UnityEngine;
using UnityEngineInternal;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
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
    public Rigidbody rib;
    public float mv = 20f;
    public GameObject bulletPrefab;

    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //target = GameObject.FindGameObjectWithTag("PlayerBEAN").GetComponent<Transform>();
        timer = wanderTimer;
        goal = target.position;

        StartCoroutine(FireCoroutine());

    }

    IEnumerator FireCoroutine() {
    while(true) {
        yield return new WaitForSeconds(2);

        Instantiate(bulletPrefab);

    }
}

    // Update is called once per frame  
    void Update()
    {

        transform.eulerAngles = new Vector3(0, 0, 0);
        goal = target.position;
        transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.deltaTime);


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
