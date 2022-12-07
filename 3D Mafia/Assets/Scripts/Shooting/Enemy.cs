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
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    public float restartDelay = 10f;

    public CharacterController cc;
    public Canvas c1;
    public Canvas c2;
    public Canvas c3;
    public Canvas deathCanvas;

    public void TakeDamage (float amount)
    {
        health -= amount;

        if (health <= 0f) {
            Invoke("Die", restartDelay);
            cc.enabled = false;
            c1.enabled = false;
            c2.enabled = false;
            c3.enabled = false;
            deathCanvas.enabled = true;
        }
    }

    void Die() {
        //Destroy(gameObject);

        //gameObject.transform.position = new Vector3(-66, 0, 10);
        health = 50f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("YOU DIED");
    }
}
