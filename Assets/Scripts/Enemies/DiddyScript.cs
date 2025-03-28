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
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float bulletTTL;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = wanderTimer;
        goal = target.position;

        StartCoroutine(FireCoroutine());
    }

    IEnumerator FireCoroutine() {
    while(true) {
        yield return new WaitForSeconds(4);

        Vector3 instantiatePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject newBullet = Instantiate(bulletPrefab, instantiatePosition, Quaternion.identity);

        Rigidbody newBulletRB = newBullet.GetComponent<Rigidbody>();
        newBulletRB.AddForce((target.position - instantiatePosition) * 0.00001f * bulletSpeed, ForceMode.Force);

        Destroy(newBullet, bulletTTL); 
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
