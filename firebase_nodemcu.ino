
//FirebaseESP8266.h must be included before ESP8266WiFi.h
#include "FirebaseESP8266.h"	// Install Firebase ESP8266 library
#include <ESP8266WiFi.h>
#include <DHT.h>		// Install DHT11 Library and Adafruit Unified Sensor Library


#define FIREBASE_HOST "https://fir-ar-83859.firebaseio.com/"                          // the project name address from firebase id
#define FIREBASE_AUTH "VunXf7Cgg2lwkc5AJBZjd9ukLTzrWpGPX7L770Fd"            // the secret key generated from firebase
#define WIFI_SSID "AndroidAPY"
#define WIFI_PASSWORD "gpou7061"

#define DHTPIN 0		// Connect Data pin of DHT to D2
int led =5;			// Connect LED to D5

#define DHTTYPE    DHT11
DHT dht(DHTPIN, DHT11);

//Define FirebaseESP8266 data object
FirebaseData firebaseData;
FirebaseData ledData;

FirebaseJson json;


void setup()
{

  Serial.begin(115200);
  delay(5000);

  dht.begin();
  pinMode(led,OUTPUT);
  
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  Serial.print("Connecting to Wi-Fi");
  while (WiFi.status() != WL_CONNECTED)
  {
    Serial.print(".");
    delay(500);
  }
  Serial.println();
  Serial.print("Connected with IP: ");
  Serial.println(WiFi.localIP());
  Serial.println();

  Firebase.begin(FIREBASE_HOST, FIREBASE_AUTH);
  Firebase.reconnectWiFi(true);

}

void sensorUpdate(){
  
  float h = dht.readHumidity();
  // Read temperature as Celsius (the default)
  float t = dht.readTemperature();
  // Read temperature as Fahrenheit (isFahrenheit = true)
  float f = dht.readTemperature(true);

  // Check if any reads failed 
  if (isnan(h) || isnan(t) || isnan(f)) {
    Serial.println(F("Failed to read from DHT sensor!"));
    return;
  }

  Serial.print(F("Humidity: "));
  Serial.print(h);
  Serial.print(F("%  Temperature: "));
  Serial.print(t);
  Serial.print(F("C  ,"));
  Serial.print(f);
  Serial.println(F("F  "));

  if (Firebase.setFloat(firebaseData, "/FirebaseIOT/temperature", t))
  {
    Serial.println("PASSED");
    
  }
  else
  {
    Serial.println("FAILED");
    
  }

  if (Firebase.setFloat(firebaseData, "/FirebaseIOT/humidity", h))
  {
    Serial.println("PASSED");
  }
  else
  {
    Serial.println("FAILED");
    
  }
}
void loop() {
  sensorUpdate();
  
  if (Firebase.getString(ledData, "/FirebaseIOT/led")){
    Serial.println(ledData.stringData());
    if (ledData.stringData() == "1") {
    digitalWrite(led, HIGH);
    }
  else if (ledData.stringData() == "0"){
    digitalWrite(led, LOW);
    }
  }
  delay(2000);
}
