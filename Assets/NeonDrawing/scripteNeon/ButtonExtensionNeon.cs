using UnityEngine;
using UnityEngine.UI;
using System;

public static class ButtonExtensionNeon
{
	public static void AddEventListenerNeon<T>(this Button button, T param, Action<T> OnClick)
	{
		button.onClick.AddListener(delegate () {
			OnClick(param);
		});
	}

	
}

