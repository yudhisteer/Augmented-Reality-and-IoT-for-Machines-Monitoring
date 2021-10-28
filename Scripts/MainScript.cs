using UnityEngine;
using UnityEditor;
using Models;
using Proyecto26;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Net;
using System;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour {

	public static string temperature, humidity;			
	public Slider cubeScale;
	public Temperature TM = new Temperature(); // Class which stores the value fetched from firebase
	public TextMeshPro txt;
	public Text temp, humi;
	public float val, temp2;
	public int t,h;	
	public GameObject flange;
	private void LogMessage(string title, string message) {
#if UNITY_EDITOR
		EditorUtility.DisplayDialog (title, message, "Ok");
#else
		Debug.Log(message);
#endif
	}
    private void Update()
    {
		if (SceneManager.GetActiveScene().name == "LiveScene")
		{
			if (cubeScale.value < t)
			{
				cubeScale.value += 0.2f;
			}
			if (cubeScale.value > t)
			{
				cubeScale.value -= 0.2f;
			}
		}
		//if(t >= 35)
		//      {
		//	temp2 = (t / 100f) - 0.2f;
		//      }
		//if (t >= 22 && t <= 34)
		//{
		//	temp2 = (t / 100f) - 0.2f;
		//}
		//if (t <= 21 && t > 15)
		//{
		//	temp2 = (t / 100f) - 0.2f;
		//}
		//if (t <= 15)
		//{
		//	temp2 = (t / 100f) - 0.09f;
		//}
		//if (SceneManager.GetActiveScene().name == "LiveScene")
		//	cubeScale.transform.position = Vector3.MoveTowards(cubeScale.transform.position, new Vector3(cubeScale.transform.position.x, temp2, cubeScale.transform.position.z), 6f*Time.deltaTime) ;
	}
    private void Start()
    {
		Getvalue();
	}
	

	// Method is used to fetch value from firebase and change the flange color according to the temperature
	public void Getvalue()
	{		
		RestClient.Get<Temperature>("https://fir-ar-83859.firebaseio.com/" + "FirebaseIOT" + ".json").Then(onResolved: dq =>
		{
			TM = dq;
			string temperature = string.Empty;
			for (int i = 0; i < dq.temperature.Length; i++)
			{
				if (Char.IsDigit(dq.temperature[i]))
				{
					temperature += dq.temperature[i];
				}
			}
			string humidity = string.Empty;
			for (int i = 0; i < dq.humidity.Length; i++)
			{
				if (Char.IsDigit(dq.humidity[i]))
				{
					humidity += dq.humidity[i];
				}
			}
			
			h = int.Parse(humidity);
			t = int.Parse(temperature);

			//change color
			float RedColor;
			float GreenColor;
			float BlueColor;

			//borrowed from https://github.com/augmentedstartups/IoTAR/blob/master/Lab_8_Hot_Drink_Sensor/Unity%20Scripts/ColorChange.cs
			if(SceneManager.GetActiveScene().name == "LiveScene")
            {
				RedColor = Mathf.Clamp((t - 22f) / (40f - 22f), 0f, 1f);
				Debug.Log(RedColor);
				if (t >= 22)
				{
					GreenColor = Mathf.Clamp((40f - t) / (40f - 22f), 0f, 1f);
					Debug.Log(GreenColor);
				}
				else
				{
					GreenColor = Mathf.Clamp((t - 15f) / (22f - 15f), 0f, 1f);
					Debug.Log(GreenColor);
				}

				BlueColor = Mathf.Clamp((22f - t) / (22f - 15f), 0f, 1f);
				Debug.Log(BlueColor);
				flange = GameObject.Find("Flange");
				flange.GetComponent<MeshRenderer>().materials[0].color = new Color(RedColor, GreenColor, BlueColor);
			}
							
			if(txt != null)
            {
				txt.text = t + "C";
			}
            else
            {
				
				//if (t > 50)
				//{
				//	t = 40;
				//}
				//if (t < 15)
				//{
				//	t = 15;
				//}
				temp.text = t.ToString();
                
				humi.text = h.ToString();
            }
			Debug.Log("printing");
			Getvalue();
		});		

	}
	
}