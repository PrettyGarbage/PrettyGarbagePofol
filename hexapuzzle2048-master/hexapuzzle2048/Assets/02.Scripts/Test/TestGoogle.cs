using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGoogle : MonoBehaviour {

	
	public Button loginButton;
	public Button logoutButton;
	public Button leaderboardButton;
	public Text userId;

	// Use this for initialization
	IEnumerator Start () {
		yield return StartCoroutine(BuildManager.Instance.Init());
		yield return StartCoroutine(NetworkManager.Instance.Init());

		Setting();

	}
	
	public void Setting(){
		loginButton.onClick.AddListener(()=>{
			NetworkManager.Instance.SignIn((bool s)=>{
				if(s){
					userId.text = NetworkManager.Instance.GetUserId();
					logoutButton.gameObject.SetActive(true);
					loginButton.gameObject.SetActive(false);
				}
			});
		});

		logoutButton.onClick.AddListener(()=>{
			NetworkManager.Instance.SignOut();
			logoutButton.gameObject.SetActive(false);
			loginButton.gameObject.SetActive(true);
			userId.text = "";
		});
		logoutButton.gameObject.SetActive(false);

		leaderboardButton.onClick.AddListener(()=>{
			NetworkManager.Instance.ShowLeaderBoard();
		});
	}
	

}
