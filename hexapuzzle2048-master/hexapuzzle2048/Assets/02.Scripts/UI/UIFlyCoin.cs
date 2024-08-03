using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class lerpMovement : MonoBehaviour {

	private Vector3 _destinationest = Vector3.zero;
	private Transform _transform;
	[SerializeField]
	float _speed;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		_transform = gameObject.transform;
	}
	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>

	public void SetInfo(Vector3 point, float speed){
		_destinationest = point;
		_speed = speed;
	}

	void Update()
	{
		if(_transform && _destinationest != Vector3.zero){
			_transform.position += _transform.up * Time.deltaTime * _speed;
			Vector3.Angle(_transform.position, _destinationest);
			// Vector3.sl
			// _transform.Rotate()
			// _transform.eulerAngles = new Vector3(0,0,)
			//_transform.eulerAngles = new Vector3(-90, q.eulerAngles.y, q.eulerAngles.z);
			// Vector3.Slerp(_transform.position, _destinationest, 0.1f);

			if(Vector3.Distance(_transform.position, _destinationest) <= 1f){
				_destinationest = Vector3.zero;
			}

		}
	}


}


public class UIFlyCoin : MonoBehaviour {

	public GameObject _coinObject;
	public int coinCount = 10;
	public float flySpeed = 1;
	public float interval = 0.1f;

	[SerializeField]
	Transform _target;

	void Start()
	{
		Fly(_target);
	}

    public void Fly(Transform target){
		_target = target;
		StartCoroutine(FlyCoinCor());
	}

	IEnumerator FlyCoinCor(){
		for (int i = 0; i < coinCount; i++)
		{
			GameObject go =  Object.Instantiate(_coinObject, transform.position, transform.rotation, transform);
			go.AddComponent<lerpMovement>().SetInfo(_target.position, flySpeed);
			go.transform.eulerAngles = new Vector3(0,0, Random.Range(0f, 360f));
			yield return new WaitForSeconds(interval);
		}
	}

	
}
