# Augmented-Reality-and-IoT-for-Machines-Monitoring - Phase II

After the first feedback on ![AR App Phase 1](https://github.com/yudhisteer/Augmented-Reality-for-Preventive-Maintenance), I wanted to go a step ahead and explore how we could work with AR and the other components of Industry 4.0. 

The Internet of Things(IoT) is a major and important component in implementing the Smart Factory concept. The **Industrial Internet of Things (IIoT)** refers to interconnected sensors, instruments, and other devices networked together with computers' industrial applications, including manufacturing and energy management. This connectivity allows for data collection, exchange, and analysis, potentially facilitating improvements in productivity and efficiency as well as other economic benefits.

RT Knits have a project of implementing IoT using the **LoRaWan** or **Sigfox** communication protocol to gather data for its electricity and water consumption and to drive efficiency in its other processes. Having IoT means data collection which means data visualisation. While they will be having a central Dashboard in order to work with the data collected, they will still need a proper channel to allow these data to be accessed by other people(operators, maintenance,...). 
A dashboard that could be accessed at the endpoint - right where the data is being collected. Without the need to login each time or to install computers to have access to the dashboards. The users would be able to view the data with a push of a button using their phone.

The second phase of the project will be to combine Augmented Reality and IoT. I would need to create a dashboard using AR that would be accessible at the endpoint of the system. The app will take readings from the IoT sensors and display the data live in the app. AR will help me in making the data visualisation and application user-friendly and easy to understand by anyone - Mauritians or not.

## Point of Attack

At the time of starting the project, I didn’t even know if this would be possible as I would need to be able to read the data collected from the Cloud in Unity and display it with AR using Vuforia Engine and all this should happen in real-time. In order to tackle this problem, I divided the project in two parts: 

1. Read and display the data using Bluetooth
2. Read and display the data from the Cloud

Reading data from Bluetooth seemed more feasible as everything would happen locally and we would not need to have internet connection. If this was possible, then reading data from the Cloud should not be a problem.

To start simple, we would set up a temperature sensor with a microcontroller and then work with the data collected to create the dashboard.

## Action plan for Phase I

1. Setup Arduino and sensors
2. Setup  ArduinoBluetoothAPI plugin
3. Write function to process sensor values
4. Write code to change color of flange
5. Creating the Dashboard Scene
6. Create function to display real-time values from sensor
7. Setup up Stream Live Graph using a Virtual Button
8. Troubleshooting
9. Evaluation

## Phase I: AR with Bluetooth

The scenario would be that we have a temperature sensor in the pump and we would need to get the data in real-time and display it in a dashboard. 

The benefit of working with AR is that it helps us in displaying the data in a more intuitive way. On top of displaying the temperature values are important for engineers, we also need to model the data such that it is comprehensible by operators and maintenance workers who are not engineers. The purpose of showing temperature is to get a sense of whether we exceeded a certain limit which we should be aware of. For example, if we say the pump is operating at 50 degrees Celsius, is it safe or not? Or is 30 degrees safer? How should someone who is not an engineer know that? 

The **principle of Human Computer Interaction(HCI)** says that : “Displays are human-made artifacts designed to support the perception of relevant system variables and to facilitate further processing of that information. Before a display is designed, the task that the display is intended to support must be defined (e.g. navigating, controlling, decision making, learning, entertaining, etc.). A user or operator must be able to process whatever information that a system generates and displays; therefore, the information must be displayed according to principles in a manner that will support perception, situation awareness, and understanding.”

The answer to show situation awareness in the context of knowing whether a certain temperature is safe is **Color**. Color helps us distinguish whether we reached a safe level or a critical one. A Red color will show the temperature is high hence, should be reduced and a Blue one will show inactivity or a safe level.

### 1. Setup Arduino and sensors

The components needed are:

- Arduino Uno
- HC-06 Bluetooth module
- DHT11 temperature sensor

![Copy of Internship 2020 (3)](https://user-images.githubusercontent.com/59663734/134490936-6188949a-9170-4ffa-89e4-519de2b23f6a.jpg)

And wrote its code to read the temperature and humidity:

```
#include <dht11.h>
#define dht_apin A0 // Analog Pin sensor is connected to

dht11 DHT11;
void setup(){
Serial.begin(9600);
delay(500);//Delay to let system boot
Serial.println("DHT11 Humidity & temperature Sensor\n\n");
delay(1000);//Wait before accessing Sensor
}//end "setup()"

void loop(){
//Start of Program
DHT11.read(dht_apin);
Serial.print("Current humidity = ");
Serial.print(DHT11.humidity);
Serial.print("% ");
Serial.print("temperature = ");
Serial.print(DHT11.temperature);
Serial.println("C ");
delay(1000);//Wait 5 seconds before accessing sensor again.
//Fastest should be once every two seconds.
}// end loop(

```

When running the code, the result would display in the Arduino IDE as such:

![image](https://user-images.githubusercontent.com/59663734/134491727-d204919e-dada-48e6-8926-504d9a869083.png)


### 2. Setup  ArduinoBluetoothAPI plugin

Using the ArduinoBluetoothAPI plugin I created a BluetoothManager.cs script that would allow me to decrypt the data from the bluetooth and parse it using a BluetoothHelper. The data received are not stored as variables but instead auto-updated as a string called “OnMessageReceived”. When pressing the “Connect” button in the GameScene, the data starts being read in the Console of Unity:

![image](https://user-images.githubusercontent.com/59663734/134621984-e9a8eea9-3c79-449e-aa58-b708d797c23f.png)

Similar to the AR project in phase one, we will have two scenes: Live-Data Scene and Dashboard Scene. The former will show the temperature using the Model Target of the pump and will change the color of the flange depending on the temperature.

We start by importing a 3D model of the pump and position and scale it as done before. We choose a Grey color for the flange and insert a TextMeshPro UI object and place it just above the flange. The UI object is a placeholder for the temperature value which will be updated in real-time from the temperature sensor.

![image](https://user-images.githubusercontent.com/59663734/139116291-137db1ca-dc30-4117-acad-498c95674b76.png)


Now, we modify the BluetoothManager script to add the temperature values to the scene. 

We start by initializing our public class ```BluetoothManager```:

```
public class BluetoothManager : MonoBehaviour
{

	// Use this for initialization
	BluetoothHelper bluetoothHelper;
	string deviceName;
	public Text text;
	public TextMeshPro tempText;
	public TextMeshPro humidityText;
	public Text tempText2;
	public Text humidityText2;
	public int humidityVal;
	public int temperatureVal;
    private float X;
    private GraphChart chart;

	public GameObject flange;

	//public GameObject sphere;

	string received_message;

	public int testTemp;
	public bool pump_visual;
```

### 3. Write function to process sensor values

We then write a function that will process the temperature and humidity values:

```
	//Function to process temperature and humidity
	//could be moved to independent script for clarity
	void parseTempHumidity (string message){
		//message format is "Current humidity = []% temperature = []C
		//18th index is humidity
		//temp is last 4 characters. Remove the C and the = if applicable

		int humidityIndex = 18;
		string humidityTruncated = message.Substring(18, 3);
		//Debug.Log(humidityTruncated);
		string tempTrunctated = message.Substring(message.Length - 7);
        //Debug.Log(tempTrunctated);
		string humidity = string.Empty;
		//process humidity string to get just the number
		for(int i=0; i<humidityTruncated.Length; i++){
			if(Char.IsDigit(humidityTruncated[i])){
				humidity +=humidityTruncated[i];
			}

		}
		humidityVal = int.Parse(humidity);

		//parse temperature similarly
		string temperature = string.Empty;
		for(int i=0; i<tempTrunctated.Length; i++){
			if(Char.IsDigit(tempTrunctated[i])){
				temperature +=tempTrunctated[i];
			}
		}
		temperatureVal = int.Parse(temperature);

		//Debug.Log("Temp is: " + temperatureVal);		
		//Debug.Log("Humidity is: " + humidityVal);
```

The message received from the arduino is in the format: ```"Current humidity = [ ] % temperature = [ ] C.``` However, we only need the integers in the square brackets. To truncate a string we would check its length, then use substring to limit it's length from ```0``` to the ideal length.

### 4. Write code to change color of flange

Next, we write the code that will change the ```color``` of the flange according to certain temperatures:

We initialized the ```Red```, ```Blue``` and ```Green``` color and then used the ```Mathf.Clamp``` function which constrains the given value between the given minimum float and maximum float values. It returns the min value if the given float value is less than the min and returns the max value if the given value is greater than the max value.  

Hence if the temperature is ```greater than 37``` degrees, then the flange turns to Red color. ```Between 22 and 37``` degrees it is Green Color and ```below 22``` degrees it is Blue color.

```
        Scene scene = SceneManager.GetActiveScene();

		//true if pump scene. False for just dashboard
		if (scene.name=="PumpScene"){
			//change color
			float RedColor;
			float GreenColor;
			float BlueColor;


			//borrowed from https://github.com/augmentedstartups/IoTAR/blob/master/Lab_8_Hot_Drink_Sensor/Unity%20Scripts/ColorChange.cs
			RedColor =   Mathf.Clamp((temperatureVal-22)/(37-22),0,1);
			if(temperatureVal >= 22)
			{
				GreenColor =   Mathf.Clamp((37 - temperatureVal)/(37-22),0,1);
			}
			else
			{
				GreenColor =   Mathf.Clamp((temperatureVal-15)/(22-15),0,1);
			}

			BlueColor =  Mathf.Clamp((22-temperatureVal)/(22-15),0,1);

            flange = GameObject.Find("Flange");
			flange.GetComponent<Renderer>().material.color = new Color(RedColor, GreenColor, BlueColor);
			//Debug.Log("updated Pump flange color");
            tempText = GameObject.Find("TempText").GetComponent<TextMeshPro>();
			tempText.text = temperatureVal + "C";
            //Debug.Log("//////////");
            //Debug.Log(tempText.text);
```

The result is as such:

![image](https://user-images.githubusercontent.com/59663734/139117347-f3029cc0-c79f-4313-958f-f8132223fd9d.png)


### 5. Creating the Dashboard Scene

Next, I created a second scene for the Dashboard and I used an **Image Target**. I used an image of pebbles given by Vuforia that is easily processed with a rating of ```5``` stars as it has many features as shown below:

![image](https://user-images.githubusercontent.com/59663734/139117528-a8820811-95f4-4d3a-a02e-a2b1bc73f273.png)

The yellow points shows the points that will be tracked by the Vuforia Engine Image Recognition.

We then create 2 buttons for the Humidity and Temperature Values and with their placeholders for the values. 

![image](https://user-images.githubusercontent.com/59663734/139117578-296aafb5-ad4b-41f1-bc83-f11e468b2b59.png)

### 6. Create function to display real-time values from sensor

We then write the code in the same BluetoothManager script to update the values for the two variables:

```
		else{
			//update dashboard
			//create graph
            humidityText2 = GameObject.Find("HumidityValueText").GetComponent<Text>();
            tempText2 = GameObject.Find("TemptValueText").GetComponent<Text>();
			humidityText2.text = humidityVal + "%";
			//Debug.Log("Updating Dashboard");
			tempText2.text = temperatureVal + "C";

            chart = GameObject.Find("GraphChart").GetComponent<GraphChart>();
            
            chart.DataSource.AddPointToCategoryRealtime("Temperature", X, temperatureVal, 1f);
            chart.DataSource.AddPointToCategoryRealtime("Humidity", X, humidityVal, 1f);
            X++;
```

We attach the ```‘temperatureVal’``` and ```‘humidityVal’``` in the following placeholders.

### 7. Setup up Stream Live Graph using a Virtual Button

Next, we also want to show how the temperature and humidity changes with time, hence, we need to show their Graph in real-time. For this, we used a plugin called ```GraphandChart``` and modified their ```Stream Live Graph``` example to fit our context.

We want to show the graph only when the user presses on the ```Virtual Button``` hence, we create an instance of a virtual button and places it right above the image of the button on the ```Image Target```. We create an ```OnButtonPressed``` function that would set the Live graph true when the button is pressed.


```

public class VirtualButtonScript : MonoBehaviour
{
    public GameObject sphereGo, cubeGo;
    public GameObject LiveStream;
    private bool curActive;
    VirtualButtonBehaviour[] vbs;
    // Start is called before the first frame update
    void Start()
    {
        vbs = GetComponentsInChildren<VirtualButtonBehaviour>();
        for (int i = 0; i < vbs.Length; ++i)
        {
            vbs[i].RegisterOnButtonPressed(OnButtonPressed);
            vbs[i].RegisterOnButtonReleased(OnButtonReleased);
        }
        //sphereGo.SetActive(false);
        //cubeGo.SetActive(true);

        curActive = false;
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        //sphereGo.SetActive(true);
        //cubeGo.SetActive(false);


        Debug.Log("Button Pressed");
        if(curActive == true){
            LiveStream.SetActive(false);
            curActive = false;
        }

        if(curActive == false){
            LiveStream.SetActive(true);
            curActive = true;
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        //cubeGo.SetActive(true);
        //sphereGo.SetActive(false);
    }
}
```

When testing, the result shown be as follows:

![image](https://user-images.githubusercontent.com/59663734/139118131-7bfe8a9f-c49a-4f7e-9d89-67a0884f5607.png)

### 8. Troubleshooting
One problem I was getting when playing both scenes is that the values for the temperature and humidity would get carried from one scene to the other and I would get an error message as such:

![image](https://user-images.githubusercontent.com/59663734/139118225-0fa33c93-67a4-4c41-993d-7608c97573be.png)

Although using the LoadSceneManagement changes the scene, the data is still being carried from one scene to the other. What we need to do is to Disconnect the streaming of data when switching scenes and Reconnect again manually. We do so by adding this code to the BluetoothManager Script:

```
#Update Scene
Scene scene = SceneManager.GetActiveScene();
```

### 9. Evaluation

Working with the bluetooth module at first validated the hypothesis that we can set up a communication protocol between these platforms - ```Unity/Vuforia``` and ```Bluetooth/Cloud```. I designed the functions that will be needed - ```Live Data``` and ```Dashboard```. Now, we will need to do the same thing using ```Cloud``` to retrieve the data and without any plugins. 




## Phase II: AR with IoT
This second sub-phase will perform the same functions as the Bluetooth one except it will not read the data locally but from the Cloud using a WiFi module.

## Action plan for Phase II

1. Setup Arduino and Sensors
2. Setting the MQTT Protocol
3. Coding the NodeMCU
4. Display Values in Real-Time
5. Troubleshooting
6. Setting Firebase
7. Parse Humidity and Temperature values
8. Creating live-data scene
9. Add Dynamic Scale

### 1. Setup Arduino and Sensors

### Setting the Hardware

The components needed are:

- NodeMCU
- DHT11 temperature sensor

I set up the following connections :

![Copy of Internship 2020 (4)](https://user-images.githubusercontent.com/59663734/134491196-b56106c4-11a5-48ee-bf03-a7c060393abe.jpg)

### 2. Setting the MQTT Protocol

Next, we need to send the data to the Cloud so we will need a MQTT broker which is a server that receives all messages from the clients and then routes the messages to the appropriate destination clients. An MQTT client is any device (from a micro controller up to a full-fledged server) that runs an MQTT library and connects to an MQTT broker over a network.

MQTT is a simple messaging protocol, designed for constrained devices with low-bandwidth. So, it's the perfect solution for Internet of Things applications. MQTT allows us to send commands to control outputs, read and publish data from sensor nodes and much more. Since most of these MQTT brokers have a publish-based-subscribe price, I would need an open-source one which is free for testing. 

ThingSpeak is an open-source Internet of Things (IoT) application and API to store and retrieve data from things using the HTTP and MQTT protocol over the Internet or via a Local Area Network. ThingSpeak enables the creation of sensor logging applications, location tracking applications, and a social network of things with status updates.

![image](https://user-images.githubusercontent.com/59663734/139120646-9468ff20-07af-4dc4-92cc-4bdcce47337c.png)

We start by creating a Channel and add the Field Values which we will need. We will get a Dashboard as such where we will view the data live:

![image](https://user-images.githubusercontent.com/59663734/139120728-eed4e9d6-d325-4a19-8fd4-782e704dbc9d.png)

### 3. Coding the NodeMCU

To code the MCU we will need the ```internet SSID```, ```password```, and the ```READ API``` key from ThingSpeak. 

```
#include <DHT.h>  // Including library for dht 
#include <ESP8266WiFi.h>
 
String apiKey = "***************8";     //  Enter your Write API key from ThingSpeak
 
const char *ssid =  "Telecom-2acb";     // replace with your wifi ssid and wpa2 key
const char *pass =  "***************";
const char* server = "api.thingspeak.com";
 
#define DHTPIN 0          //pin where the dht11 is connected
 
DHT dht(DHTPIN, DHT11);
 
WiFiClient client;
 
void setup() 
{
       Serial.begin(115200);
       delay(5000);
       dht.begin();
 
       Serial.println("Connecting to ");
       Serial.println(ssid);
 
 
       WiFi.begin(ssid, pass);
 
      while (WiFi.status() != WL_CONNECTED) 
     {
            delay(500);
            Serial.print(".");
     }
      Serial.println("");
      Serial.println("WiFi connected");
 
}
 
void loop() 
{
  
      float h = dht.readHumidity();
      float t = dht.readTemperature();
      
              if (isnan(h) || isnan(t)) 
                 {
                     Serial.println("Failed to read from DHT sensor!");
                      return;
                 }
 
                         if (client.connect(server,80))   //   "184.106.153.149" or api.thingspeak.com
                      {  
                            
                             String postStr = apiKey;
                             postStr +="&field1=";
                             postStr += String(t);
                             postStr +="&field2=";
                             postStr += String(h);
                             postStr += "\r\n\r\n";
 
                             client.print("POST /update HTTP/1.1\n");
                             client.print("Host: api.thingspeak.com\n");
                             client.print("Connection: close\n");
                             client.print("X-THINGSPEAKAPIKEY: "+apiKey+"\n");
                             client.print("Content-Type: application/x-www-form-urlencoded\n");
                             client.print("Content-Length: ");
                             client.print(postStr.length());
                             client.print("\n\n");
                             client.print(postStr);
 
                             Serial.print("Temperature: ");
                             Serial.print(t);
                             Serial.print(" degrees Celcius, Humidity: ");
                             Serial.print(h);
                             Serial.println("%. Send to Thingspeak.");
                        }
          client.stop();
 
          Serial.println("Waiting...");
  
  // thingspeak needs minimum 15 sec delay between updates
  delay(15000);
}

```

### 4. Display Values in Real-Time

The data will be viewed in the Arduino IDE as such:

![image](https://user-images.githubusercontent.com/59663734/139121710-69c6afa7-7152-45f7-9950-657dd90ad71e.png)

And the dashboard in ThingSpeak will be like this:

![image](https://user-images.githubusercontent.com/59663734/139121799-c0deab10-3f2f-4bf5-bc1c-8a98d0a62dad.png)

### 5. Troubleshooting

However, it was not possible to set up a connection between ThingSpeak and Unity with the UnityWebRequest function. Since ThingSpeak converts the real-time data into Json, we will need to use a Json parser to decrypt the data, truncate the parts we will need and then compile it into Unity. Another solution could be to save the data and use XML to fetch the data, alas, the deadline was close and I needed to present the project soon. I needed another solution and I decided to use **Google Firebase** instead.

### 6. Setting Firebase

The difference between Firebase and ThingSpeak is that Firebase is a web application backend and ThingSpeak-MQTT is standard for publish-subscribe messages. Firebase Cloud Messaging  uses HTTP and XMPP Server Protocol serving JSON and Plain Text both.

In our case, we are interested in creating a real-time database and Firebase offers just that. The Firebase Realtime Database is a cloud-hosted NoSQL database that lets us store and sync data between users in real time. Also, Firebase does not store data but only displays the current values being registered.

![image](https://user-images.githubusercontent.com/59663734/139122669-d016364e-b84f-48a2-9551-412ee1ec979c.png)

So instead of using HTTP API to fetch data as would have done in ThingSpeak, we will now use the SDK for Unity that Firebase offers us to set up communication between our real-time database and Unity.

Similar to ThingSpeak we set up a project in Firebase and create two variables to store our data with an initial value of ```0```. Our dashboard will look like this:

![image](https://user-images.githubusercontent.com/59663734/139122774-55d191b5-4713-48d2-b716-8cb13cfb7270.png)

We then need to copy our Bundle Identifier tag to Firebase and download the google-services.json to upload into Unity. We will use a RestAPI to directly send and retrieve data from Firebase.


We create a public class named ```temperature``` to store the values fetched from the database. Then in ```Temperature.cs``` script we write the following code:

```
public class Temperature
{
    public string temperature;
    public string humidity;    
}
```

### 7. Parse Humidity and Temperature values

In another file named Main.cs script we install our libraries:

```
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
```



We will then need our Firebase Host link and Firebase project name to set up the RestAPI. We write the following code to parse the value of humidity and temperature:

```
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
```

### 8. Creating live-data scene

Similar to the Bluetooth project, we need to change the color of the flange. We do so by using the ```Mathf.Clamp``` function and get activated when ```SceneManager.GetActiveScene().name == "LiveScene"```.

```
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
```


Now, to display the temperature value above the flange:

```
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
```

### 9. Add Dynamic Scale

Now, compared to the Bluetooth Project we also want to show a dynamic scale of how the temperature changes. We start by creating scale in PNG format and upload it to Unity. Then for the dynamic slider, we use the in-built slider option in Unity. We set up the maximum to be ```50°C``` and minimum ```15°C```.

![image](https://user-images.githubusercontent.com/59663734/139228074-2d7f36d6-34e7-4846-8fa2-5520e2bf29fa.png)


```
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
```

### 10. Create Dashboard Scene

The Dashboard scene will be similar to the Bluetooth Project except instead of having the two graphs - temperature and humidity - in one chart, this time we will separate. So we add two graphs next to each other as such:

![image](https://user-images.githubusercontent.com/59663734/139228870-284e2a1d-354f-4d71-8118-6e215ee18863.png)

We will still have a Virtual Button to display the graphs on when pressing on it. We initialize two charts and append one to** Temperature Values** and the other to **Humidity values**:

```
    public GraphChart chart,chart2;
    //public BluetoothManager btManager;
    private float Timer;
    private float X;
    public int i;
    // Start is called before the first frame update
    void Start()
    {
        //chart.DataSource.ClearCategory("humidity");
        //chart2.DataSource.ClearCategory("temperature");
        chart2.DataSource.ClearCategory("humidity");
        chart.DataSource.ClearCategory("temperature");
        Timer = 0.5f;
        X = 4f;
    }
```

We then need to fetch data from Firebase and feed it into the graphs:

```
 // Update is called once per frame
    void Update()
    {

        Timer -= Time.deltaTime;        
        if (Timer < 0f)
        {
            
             
            //Debug.Log("Updating GRAPH");

            Timer = 1f;

            // values fetched from firebase
            int temperature = this.gameObject.GetComponent<MainScript>().t;
            int humidity = this.gameObject.GetComponent<MainScript>().h;
            //chart.DataSource.
            chart.DataSource.AddPointToCategoryRealtime("Temperature", X, temperature, 1f);
            //chart.DataSource.AddPointToCategoryRealtime("Humidity", X, i, 1f);
            //chart2.DataSource.AddPointToCategoryRealtime("Temperature", X, i, 1f);
            chart2.DataSource.AddPointToCategoryRealtime("Humidity", X, humidity, 1f);            
            
            X++;
            chart2.DataSource.StartBatch();
            chart.DataSource.StartBatch();

            chart.DataSource.EndBatch();
            chart2.DataSource.EndBatch();
        }

        

    }
```

## Conclusion



