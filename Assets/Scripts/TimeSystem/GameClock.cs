
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enums;

///<summary>
/// 
///</summary>
public class GameClock : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI timeText;
	[SerializeField] private TextMeshProUGUI dateText;
	[SerializeField] private TextMeshProUGUI seasonText;
	[SerializeField] private TextMeshProUGUI yearText;

	/// <summary>
	/// 
	/// </summary>
	private void OnEnable()
	{
		EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
	}

	private void OnDisable()
	{
		EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
	}



	private void UpdateGameTime(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
		int gameHour, int gameMinute, int gameSecond)
	{

	}
}
