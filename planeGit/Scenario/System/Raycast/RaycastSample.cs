using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastSample : MonoBehaviour
{
    //Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ITest());
        
    }

    private IEnumerator ITest()
    {
        yield return new WaitForSeconds(1f);
        RaycastSystem.Instance.StartRaycast();
    }

}
