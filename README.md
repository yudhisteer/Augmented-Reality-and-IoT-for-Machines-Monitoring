# Augmented-Reality-and-IoT-for-Machines-Monitoring

After the first feedback on **AR App Phase I**, I wanted to go a step ahead and explore how we could work with AR and the other components of Industry 4.0. 

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
