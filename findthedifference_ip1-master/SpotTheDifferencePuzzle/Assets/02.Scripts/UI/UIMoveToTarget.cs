using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoveToTarget : MonoBehaviour {

    [SerializeField]
    private Image _image;

    [SerializeField]
    private Transform _targetTransform;
    
    [SerializeField]
    bool _isFly;

    public float speed = 1;
    public float rValue = 0.1f;

    private void Start()
    {
        transform.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

        speed = Random.Range(speed, speed*1.5f);
        rValue = Random.Range(rValue, rValue*2f);
        _image.transform.eulerAngles = Vector3.zero;

    }

    private void Update()
    {

        if (_isFly)
        {
            transform.position += transform.forward * Time.deltaTime * 1000 * speed;
            Quaternion toRotation = Quaternion.LookRotation(_targetTransform.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rValue);

            _image.transform.eulerAngles = Vector3.zero;


            if(Vector3.Distance(_targetTransform.transform.position, transform.position) < 10f)
            {
                _isFly = false;
                gameObject.SetActive(false);
            }

        }
    }

}
