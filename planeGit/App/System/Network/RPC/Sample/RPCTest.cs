using Library.Network;
using UnityEngine;

[RequireComponent(typeof(RPCView))]
public class RPCTest : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RPCView.Of(this).RPC(nameof(Test), "Hello World!!");
        }
    }

    [NetworkRPC]
    void Test(string message) => Logger.Log($"Test : {message}");
}
