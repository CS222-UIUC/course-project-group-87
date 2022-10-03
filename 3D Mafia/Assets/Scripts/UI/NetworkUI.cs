using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class NetworkUI : MonoBehaviour
{
    
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private GameObject interact;

    private string password;

    // Start is called before the first frame update
    private void Awake()
    {
        serverBtn.onClick.AddListener(Server);
        hostBtn.onClick.AddListener(Host);
        clientBtn.onClick.AddListener(Client);
        
    }

    public void Server()
    {
        // Most likely won't use
        // NetworkManager.Singleton.StartServer();
        GameObject go = Instantiate(interact, Vector3.zero, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();

    }
    
    public void Host()
    {
        password = passwordField.text;
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(password);
        NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        password = passwordField.text;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(password);
        NetworkManager.Singleton.StartClient();
    }

    private void spawn()
    {
        // float x;
        // float y;
        // for (int i = 0; i < 4; i++)
        // {
        //     x = Random.Range(-20.0f, 20.0f);
        //     
        // }
        Instantiate(interact, new Vector3(3.0f, 4.0f, 5.0f), Quaternion.identity);
    }
    
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientId = request.ClientNetworkId;
        var connectionData = request.Payload;
        
        Debug.Log(password);
        
        string pass = Encoding.ASCII.GetString(connectionData);
        Debug.Log(pass);

        if (pass == password)
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
        }
        else
        {
            response.Approved = false;
        }

    }
}
