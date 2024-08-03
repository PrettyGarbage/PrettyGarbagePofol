using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestSaveFile : MonoBehaviour {

	Rect loadRect  = new Rect(0,0, 200, 100);
	Rect saveRect  = new Rect(300,0, 200, 100);
	Rect saveGameResultRect  = new Rect(600,0, 200, 100);
	Rect savePlayDataRect  = new Rect(900,0, 200, 100);
	Rect textRect  = new Rect(600,600, 500, 300);
	UserData userdata;
	StringBuilder sb = new StringBuilder();
	// Use this for initialization
	IEnumerator Start () {
		
        yield return StartCoroutine(BuildManager.Instance.Init());
        yield return StartCoroutine(DataManager.Instance.Init());
        yield return StartCoroutine(SoundManager.Instance.Init());
        yield return StartCoroutine(NetworkManager.Instance.Init());
	}
	
	/// <summary>
	/// OnGUI is called for rendering and handling GUI events.
	/// This function can be called multiple times per frame (one call per event).
	/// </summary>
	void OnGUI()
	{
		if(GUI.Button(loadRect, "load file")){
			DataManager.Instance.LoadUserDataFile();
		}

		if(GUI.Button(saveRect, "save file")){
			DataManager.Instance.SaveUserDataFile();
		}
		if(GUI.Button(saveGameResultRect, "SaveGameResult")){
			DataManager.Instance.SaveGameResult(DataManager.Instance.UserData.Score+1, DataManager.Instance.UserData.Coin+1, new int[] { 0, 0, 0 });
		}

		if(GUI.Button(savePlayDataRect, "SavePlayData")){
			int[,] cells = new int[GameConstants.CELL_ARRAY_SIZE,GameConstants.CELL_ARRAY_SIZE];
			int index = 0;
			for (int i = 0; i < GameConstants.CELL_ARRAY_SIZE; i++)
			{
				for (int j = 0; j < GameConstants.CELL_ARRAY_SIZE; j++)
				{
					cells[i,j] = index;
					index++;
				}	
			}
			ItemUseCountInfo itemUseCountInfo = new ItemUseCountInfo()
													.AddCount(ItemType.TRASH, 1)
													.AddCount(ItemType.BREAK, 4)
													.AddCount(ItemType.UNDO, 2);
			DataManager.Instance.SavePlayData(10, 101, cells, new LastBlocks(new int[2]{1,3}, 9) , new int[] { 1, 4, 2 }, 2);
		}

		if(DataManager.Instance.UserData){
			userdata = DataManager.Instance.UserData;
			sb.AppendLine("score - ");
			sb.Append(userdata.Score);

			GUI.TextArea(textRect, sb.ToString());

		}

	}
}
