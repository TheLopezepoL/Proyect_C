using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveJumpItem : MonoBehaviour
{
    [SerializeField] public GameObject Object;
    public HeroBehaviour hero;
    // Start is called before the first frame update
    void Start()
    {
        hero = Object.GetComponent<HeroBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Evolve");
        // Check if the collision is with the enemy's disjointed trigger collider
        if (other.CompareTag("Hero"))
        {
            hero.UnlockDobleJump();
        }
    }
}
