using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Mapbox library
using Mapbox.Examples;
using Mapbox.Utils;
using Unity.VisualScripting;

public class EventPointer : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float amplitude = 2.0f;
    [SerializeField] private float frequency = 0.5f;


    LocationStatus playerLocation;
    [SerializeField] public Vector2d eventPose;
    MenuUIManager menuUiManager;

      [SerializeField] public int publicDistance;

    
    
    // Start is called before the first frame update
    void Start()
    {
       menuUiManager = GameObject.Find("Canvas").GetComponent<MenuUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        floatandRotatePointer(); 
    
    }
    private void floatandRotatePointer()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.fixedTime*Mathf.PI*frequency)*amplitude)+15, transform.position.z);
    }

    private void OnMouseDown()
    {
        playerLocation = GameObject.Find("Canvas").GetComponent<LocationStatus>();
        var currentPlayerLocation = new GeoCoordinatePortable.GeoCoordinate(playerLocation.GetLocationLat(), playerLocation.GetLocationLon());
        var eventLocation = new GeoCoordinatePortable.GeoCoordinate(eventPose[0], eventPose[1]);
        var distance = currentPlayerLocation.GetDistanceTo(eventLocation);
        Debug.Log("Distance is: " + distance);
        distance = publicDistance;

        if (distance < 500)
        {
            menuUiManager.DisplayStartEventPanel();


        }else
        {
            menuUiManager.DisplayUserNotInRangePanel();
        }

    }
}
