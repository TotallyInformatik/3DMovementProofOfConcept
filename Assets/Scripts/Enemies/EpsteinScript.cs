using System;
using System.Collections;
using UnityEngine;

public class Epstein : MonoBehaviour
{

    public Transform target;
    public float idletime = 0.3f;
    public float chillSpeed = 4f;
    public float attackSpeed = 6f;
    private bool triggered = false;
    private bool hit = false;
    private int health = 3;
    [Header("P is the probability of moving towards the player")]
    private int pNumerator = 30;
    private int pDenominator = 100;

    private Rigidbody _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(AnnoyingMovementCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
    }


    IEnumerator AnnoyingMovementCoroutine() {

        while (true) {
            yield return new WaitForSeconds(idletime);

            System.Random rand = new System.Random();
            int randomnum = rand.Next(1, pDenominator + 1);

            if (randomnum <= pNumerator) {
                // move towards the player
                _rb.AddForce((target.position - transform.position) * attackSpeed, ForceMode.Impulse);
            } else {
                // move towards some random fuckass direction

                Vector3 randomPosition = new Vector3(
                    (float)(-10d + rand.NextDouble() * 20d),  // X-axis range
                    (float)(-10d + rand.NextDouble() * 20d),     // Y-axis range
                    (float)(-10d + rand.NextDouble() * 20d)   // Z-axis range
                );

                _rb.AddForce(randomPosition * chillSpeed, ForceMode.Impulse);
                

            }

        }

    }
}
