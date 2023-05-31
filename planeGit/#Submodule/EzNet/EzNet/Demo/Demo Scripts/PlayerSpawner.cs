using EzNetLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform worldTransform;
    public GameObject pref;
    public void SpawnNewPlayer()
    {
        Debug.Log("Spawn New Player!");
        GameObject ins = Instantiate(pref, worldTransform);
        ins.transform.position = EzNet.Read<Vector3>("pos");
        ins.transform.eulerAngles = EzNet.Read<Vector3>("rot");
        ins.GetComponent<PlayerController>().isOwn = false;
        ins.GetComponent<EzNetObject>().id = EzNet.ReadPacketBase().from_id;
        EzNet.RegisterNetObject(ins.GetComponent<EzNetObject>());
        ins.SetActive(true);
    }
}
