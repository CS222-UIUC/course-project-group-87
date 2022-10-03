using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private TMP_InputField passwordField;

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
        NetworkManager.Singleton.StartServer();

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
