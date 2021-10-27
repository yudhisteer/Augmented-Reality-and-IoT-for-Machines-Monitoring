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

## 1. AR with Bluetooth

The scenario would be that we have a temperature sensor in the pump and we would need to get the data in real-time and display it in a dashboard. 

The benefit of working with AR is that it helps us in displaying the data in a more intuitive way. On top of displaying the temperature values are important for engineers, we also need to model the data such that it is comprehensible by operators and maintenance workers who are not engineers. The purpose of showing temperature is to get a sense of whether we exceeded a certain limit which we should be aware of. For example, if we say the pump is operating at 50 degrees Celsius, is it safe or not? Or is 30 degrees safer? How should someone who is not an engineer know that? 

The **principle of Human Computer Interaction(HCI)** says that : “Displays are human-made artifacts designed to support the perception of relevant system variables and to facilitate further processing of that information. Before a display is designed, the task that the display is intended to support must be defined (e.g. navigating, controlling, decision making, learning, entertaining, etc.). A user or operator must be able to process whatever information that a system generates and displays; therefore, the information must be displayed according to principles in a manner that will support perception, situation awareness, and understanding.”

The answer to show situation awareness in the context of knowing whether a certain temperature is safe is **Color**. Color helps us distinguish whether we reached a safe level or a critical one. A Red color will show the temperature is high hence, should be reduced and a Blue one will show inactivity or a safe level.

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


# Part II

![Copy of Internship 2020 (4)](https://user-images.githubusercontent.com/59663734/134491196-b56106c4-11a5-48ee-bf03-a7c060393abe.jpg)

