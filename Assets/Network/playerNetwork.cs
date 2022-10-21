using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class playerNetwork : NetworkBehaviour
{
    //PLAYER CONTROLLER

    //private network variables sync data across the network
    //the example below is for the health of each player                  //read permission is set to everyone, write permission is set to owner 
                                                                          //this is so only the owner's inputs affect individual values
                                             //"1" is the default value  //but both can see each others values
    private NetworkVariable<int> HealthP1 = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> HealthP2 = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    //spawn object reference
    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;




    //OnNetworkSpawn is the Network version of Start()/Awake()
    public override void OnNetworkSpawn()
    {
        //When health changes display current value
        HealthP1.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log(OwnerClientId + "; HealthP1: " + HealthP1.Value);
            
        };

        HealthP2.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log(OwnerClientId + "; HealthP2: " + HealthP2.Value);

        };
    }

    private void Update()
    {
        //!isOwner stops players from affecting each others inputs
        if (!IsOwner) return;

        //"T" to test P1 Health changing
        if (Input.GetKeyDown(KeyCode.T))
        {
            HealthP1.Value = Random.Range(0, 100);
        }

        //"R" to test P2 Health changing
        if (Input.GetKeyDown(KeyCode.R))
        {
            HealthP2.Value = Random.Range(0, 100);
        }

        //"E" to test server messages
        if (Input.GetKeyDown(KeyCode.E))
        {
            TestServerRpc("message!");
        }

        //"Q" to test spawn object
        if (Input.GetKeyDown(KeyCode.Q))
        {
            spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
            
        }

        //basic movement inputs
        Vector3 moveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;


        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    //Data only gets sent to server/host never the client
    

    [ServerRpc]
    private void TestServerRpc(string message)
    {
        Debug.Log("TestServerRpc" + OwnerClientId + "; " + message);
    }

    //Data gets sent to the host/client
    //Can only be sent from server/host to the client
    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log("TestServerRpc");
    }
}
