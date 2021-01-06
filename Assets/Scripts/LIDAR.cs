using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LIDAR : MonoBehaviour {

	public GameObject Scanner; // Reference `Scanner` gameobject
	public GameObject Head; // Reference `Head` gameobject
	public GameObject Laser; // Reference `Laser` gameobject
	public Toggle EnableSensorVisualization; // Toggle button to control sensor visualization
	public Slider ScanRate; // Slider to set LIDAR scanning rate (Hz)
	public Text ScanRateValue; // Text to display the LIDAR scanning rate (Hz)
	public Text Range; // Text to display the range value of laser scan

	public float MinimumRange = 0.15f; // LIDAR minimum range (m)
	public float MaximumRange = 12f; // LIDAR maximum range (m)
	public int MeasurementsPerScan = 360; // Number of measurements per scan
	public float Intensity = 47.0f; // Intensity of the laser ray

	private List<string> RangeArray = new List<string>(); // List storing range values of a scan
	private float timer = 0f; // Timer to synchronize laser scan updates

	void Update()
	{
		// SENSOR VISUALIZATION
		if(EnableSensorVisualization.isOn) Laser.SetActive(true); // Enable visualization
		else Laser.SetActive(false); // Disable visualization

		// DISPLAY SCAN RATE
		ScanRateValue.text = ScanRate.value.ToString(); // Update the `ScanRateValue` text

		// LASER SCAN
		Scanner.transform.Rotate(Vector3.up, Time.deltaTime*360*ScanRate.value); // Spin the scanner

		Vector3 LaserVector = Scanner.transform.TransformDirection(Vector3.forward) * 12; // 12 m long vector pointing in the forward direction (i.e. local Z-axis)
		Ray LaserRay = new Ray(Scanner.transform.position, LaserVector); // Initialize a ray w.r.t. the vector at the origin of the `Scanner` transform
		//Debug.DrawRay(Scanner.transform.position, LaserVector, Color.red); // Visually draw the raycast in scene for debugging purposes
		RaycastHit VisualizationRayHit; // Initialize a raycast hit object
		if(Physics.Raycast(LaserRay, out VisualizationRayHit))
		{
				Range.text = "Range (m): " + (VisualizationRayHit.distance+MinimumRange).ToString(); // Update the `Range` value to the `hit distance` if the ray is colliding
				Laser.transform.localScale = new Vector3(1,1,VisualizationRayHit.distance); // Update length of the `Laser` line
		}
		else
		{
				Range.text = "Range (m): inf"; // Update the `Range` value to `inf`, otherwise
				Laser.transform.localScale = new Vector3(1,1,12); // Update length of the `Laser` line
		}

		timer = timer + Time.deltaTime; // Update timer
		if(timer < 1/ScanRate.value)
		{
				// Scan
		}
		if(timer >= 1/ScanRate.value)
		{
				LaserScan(); // Report the scan
				timer = 0; // Reset timer
		}
	}

	void LaserScan()
	{
			float angle = (Head.transform.eulerAngles.y)*(Mathf.PI/180); // Reset angle to align w.r.t. `Head`

			// LASER SCAN
			for(int i=0; i<MeasurementsPerScan; i++)
			{
					float x = Mathf.Sin(angle); // sin(angle)
					float z = Mathf.Cos(angle); // cos(angle)
					angle -= 2*Mathf.PI/MeasurementsPerScan; // Update angle
					Vector3 LaserRayDirection = new Vector3(x*MaximumRange, 0, z*MaximumRange); // Compute direction of the raycast
					RaycastHit hit; // Instantiate a raycast hit object
					/*
					if(i==0)
					{
							Debug.DrawRay(Head.transform.position, LaserRayDirection, Color.green); // Visually draw the raycast in scene for debugging purposes (green color to indicate 1st ray)
					}
					else if(i==89)
					{
							Debug.DrawRay(Head.transform.position, LaserRayDirection, Color.blue); // Visually draw the raycast in scene for debugging purposes (blue color to indicate 90th ray)
					}
					else
					{
							Debug.DrawRay(Head.transform.position, LaserRayDirection, Color.red); // Visually draw the raycast in scene for debugging purposes (red color to indicate other rays)
					}
					*/
					if(Physics.Raycast(Head.transform.position, LaserRayDirection, out hit, MaximumRange) && hit.distance>MinimumRange) RangeArray.Add((hit.distance).ToString()); // Update the range measurement to the `hit distance` if the ray is colliding
					else RangeArray.Add("inf"); // Update the range measurement to `inf`, otherwise
			}

			// LOG LASER SCAN

			// Range Array
			string RangeArrayString = "Range Array: "; // Initialize `RangeArrayString`
			foreach(var item in RangeArray)
			{
					RangeArrayString += item + " "; // Concatenate the `RangeArrayString` with all the elements in `RangeArray`
			}
			Debug.Log(RangeArrayString); // Log the `RangeArrayString` to Unity Console

			// Intensity Array
			string IntensityArrayString = "Intensity Array: "; // Initialize `RangeArrayString`
			for(int i=0; i<MeasurementsPerScan; i++)
			{
					IntensityArrayString += Intensity + " "; // Concatenate the `RangeArrayString` with all the elements in `RangeArray`
			}
			Debug.Log(IntensityArrayString); // Log the `RangeArrayString` to Unity Console

			RangeArray.Clear(); // Clear the `RangeArray`
	}
}
