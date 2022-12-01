using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class Enemy : PlayerController
{
    public float health = 50f;

    public void TakeDamage (float amount)
    {
        health -= amount;

        if (health <= 0f) {
            Die();
        }
    }

    void Die() {
        //Destroy(gameObject);

        int i = 0;

        while (ActivePlayers[i] != this) {
            i++;
        }

        ActivePlayers[i].transform.position = new Vector3(-66, 0, 10);

        //gameObject.transform.position = new Vector3(-66, 0, 10);
        health = 50f;
    }
}