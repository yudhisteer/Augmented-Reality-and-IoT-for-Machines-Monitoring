using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChartAndGraph;

public class GraphSample : MonoBehaviour
{

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
}