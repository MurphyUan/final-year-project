using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TestControls : NetworkBehaviour
{
    private NetworkVariable<NetworkData> networkVariable = new NetworkVariable<NetworkData>(
        new NetworkData {
            _int = 1,
            _bool = true
        },
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct NetworkData : INetworkSerializable{
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    public override void OnNetworkSpawn()
    {
        networkVariable.OnValueChanged += (NetworkData previousValue, NetworkData newValue) => {
            Debug.Log($"{OwnerClientId}; {networkVariable.Value._int}; {networkVariable.Value._bool}; {networkVariable.Value.message}");
        };
    }

    private void Update() 
    {
        if(!IsOwner) return;

        if(Input.GetKeyDown(KeyCode.T)) {
            // TestServerRpc();
            TestClientRpc();

            networkVariable.Value = new NetworkData{
                _int = Random.Range(0, 100),
                _bool = !networkVariable.Value._bool,
                message = "TestString"
            };
        }

        Vector3 moveDir = new Vector3(0,0,0);

        if(Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        if(Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if(Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if(Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    // Used if only the server should recieve update, all clients can send this message including host/server
    [ServerRpc]
    private void TestServerRpc()
    {
        Debug.Log($"TestServerRpc {OwnerClientId}");
    }

    // Used if all clients should recieve update, only server can send this message
    // Can add ClientRpcParams to send this call along with a message to select clients
    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log($"TestClientRpc {OwnerClientId}");
    }
}
