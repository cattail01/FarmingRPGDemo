
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enums;

///<summary>
/// 游戏时间，是游戏时间系统的监听类
///</summary>
public class GameClock : MonoBehaviour
{
	#region 显示组件，用于显示游戏时间

	[SerializeField] private TextMeshProUGUI timeText;
	[SerializeField] private TextMeshProUGUI dateText;
	[SerializeField] private TextMeshProUGUI seasonText;
	[SerializeField] private TextMeshProUGUI yearText;

	#endregion

	private void OnEnable()
	{
		// 监听游戏时间的变化
		// 这样可以根据游戏时间的变化更改时钟显示
		EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
		EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
	}

	private void OnDisable()
	{
		// 退出监听
		EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
	}


	/// <summary>
	/// 给窗口中的数据更新游戏时间
	/// </summary>
	/// <remarks>
	/// 调整时间，使其更加符合游戏要求，并将时间显示到游戏指定区域
	/// </remarks>
	private void UpdateGameTime(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
		int gameHour, int gameMinute, int gameSecond)
	{
		#region 调整时间的相关参数，用于时间显示

		// 在游戏中，我们每10分钟变化一次分钟
		// 意思就是，只显示10分钟的增量
		gameMinute = gameMinute - (gameMinute % 10);

		// 显示早上还是晚上的字符串
		string ampm = string.Empty;
		if (gameHour < 12)
		{
			ampm = "am";
		}
		else
		{
			ampm = "pm";
		}

		// 将24小时的显示模式转换为12小时的显示模式
		gameHour %= 12;

		#endregion

		#region 时间字符串格式化与显示

		// 将字符串格式化与组合，生成时间字符串
		string time = string.Format("{0:00}:{1:00} {2}", gameHour, gameMinute, ampm);
		string date = string.Format("{0}. {1}", gameDayOfWeek, gameDay);
		string season = gameSeason.ToString();
		string year = $"Year {gameYear}";

		// 将时间投放到显示区域
		timeText.text = time;
		dateText.text = date;
		seasonText.text = season;
		yearText.text = year;

		#endregion
	}
}
