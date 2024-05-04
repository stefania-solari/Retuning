namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;
	using UnityEngine.Events;
	using TMPro;


	public class SpawnOnMap : MonoBehaviour
	{
			
			
			 // Create a list to store strings
		public string allMarkers;

			
			[SerializeField]
		public TextMeshProUGUI cardPrompt;

		
		
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		[Geocode]
		string[] _locationStrings;
		Vector2d[] _locations;

		[SerializeField]
		float _spawnScale = 100f;

		[SerializeField]
		GameObject _markerPrefab;


		[System.Serializable]
		public class Marker
		{
			public string id;
			public Vector3 coordinates;
			public GameObject prefab;

			// Constructor to initialize the marker
			public Marker(string _id, Vector3 _coordinates, GameObject _prefab)
			{
				id = _id;
				coordinates = _coordinates;
				prefab = _prefab;
			}
		}

		List<GameObject> _spawnedObjects;

		 [SerializeField]
    public string hotspotRestaurant = "glass and forks clicking, clean recording";
        
    [SerializeField]
    public string hotspotClinic = "heartbeat upbeat, bouncy";

    [SerializeField]
    public string hotspotSchool = "kid song, xylophone rhythm, bouncy, playful";

    [SerializeField]
    public string hotspotChurch = "melodic piano organ religious piano organ";

     [SerializeField]
    public string hotspotMarket = "wheels grinding";

    [SerializeField]
    public string hotspotLibrary = "alone shoe walks";

    [SerializeField]
    public string hotspotHotel = "hotel call bell ringing";

     [SerializeField]
    public string hotspotBus = "bus doors opening";

		void Start()
		{
			_locations = new Vector2d[_locationStrings.Length];
			_spawnedObjects = new List<GameObject>();
			for (int i = 0; i < _locationStrings.Length; i++)
			{
				var locationString = _locationStrings[i];
				_locations[i] = Conversions.StringToLatLon(locationString);
				var instance = Instantiate(_markerPrefab);

				 // Assign unique name to the marker
				 if (i == 0)
				 {
					instance.name = hotspotRestaurant;
				 }
				 else if(i == 1)
				 {
					instance.name = hotspotClinic;

				 }else if(i == 2)
				 {
					instance.name = hotspotClinic;

				 }
				 else if(i == 3)
				 {
					instance.name = hotspotRestaurant;

				 }
				  else if(i == 3)
				 {
					instance.name = hotspotSchool;

				 }
				 else if(i == 4)
				 {
					instance.name = hotspotSchool;

				 }
				 else if(i == 5)
				 {
					instance.name = hotspotChurch;

				 }
                else if(i == 6)
				 {
					instance.name = hotspotChurch;

				 }
				 else if(i == 7)
				 {
					instance.name = hotspotLibrary;

				 }
				 else if(i == 8)
				 {
					instance.name = hotspotHotel;

				 }
				  else if(i == 9)
				 {
					instance.name = hotspotRestaurant;

				 }
				 else if(i == 10)
				 {
					instance.name = hotspotRestaurant;

				 }
				  else if(i == 11)
				 {
					instance.name = hotspotHotel;

				 }
                
                
                
				


				instance.GetComponent<EventPointer>().eventPose =  _locations[i];

			// Add click handler to the marker
            var collider = instance.GetComponent<Collider>();
            if (collider != null)
            {
				var clickHandler = collider.gameObject.AddComponent<MarkerClickHandler>();
				
				clickHandler.OnMarkerClick.AddListener(() => OnMarkerClicked(instance.name));
				allMarkers = allMarkers+" "+instance.name;

				Debug.Log("clicked marker name: " + allMarkers);
				
            }
            else
            {
                Debug.LogWarning("No collider found on marker: " + instance.name);
            }

				instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
				instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_spawnedObjects.Add(instance);
			}
		}

    // Method to handle marker click event
    void OnMarkerClicked(string markerName)
    {
		cardPrompt.text = markerName;
        Debug.Log("Clicked Marker: " + markerName);
        // Add your event handling logic here
    }
 // MarkerClickHandler class to handle marker click events
 
		private void Update()
		{
			int count = _spawnedObjects.Count;
			for (int i = 0; i < count; i++)
			{
				var spawnedObject = _spawnedObjects[i];
				var location = _locations[i];
				spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
				spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			}
		}
	}



	 // MarkerClickHandler class to handle marker click events
    public class MarkerClickHandler : MonoBehaviour
    {
        public UnityEvent OnMarkerClick = new UnityEvent();

        void OnMouseDown()
        {
            OnMarkerClick.Invoke();
        }
    }
}