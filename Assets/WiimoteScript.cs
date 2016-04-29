using UnityEngine;
using System.Collections;
using WiimoteApi;
using System.Runtime.Remoting;
using System.Threading;
using System;

public class WiimoteScript : MonoBehaviour
{
	[Range (1, 150)]
	public float coefficient = 1f;

	// the number of samples needed for wiimotion plus calibration
	[Range (100, 1000)]
	public int numberOfSamplesForCalibration = 100;

	private Wiimote wiimote;

	private bool wmpActivated = false;
	private bool calibrated = false;
	private Vector3 wmpOffset = Vector3.zero;

	void Start ()
	{
		WiimoteManager.FindWiimotes ();

		if (WiimoteManager.HasWiimote ()) {
			Debug.Log ("Wiimote is connected");
			wiimote = WiimoteManager.Wiimotes [0];
			wiimote.SendPlayerLED (true, false, false, false);

			// set output data format
			wiimote.SendDataReportMode (InputDataType.REPORT_BUTTONS_ACCEL_EXT16);

			wiimote.RequestIdentifyWiiMotionPlus ();
		}
	}

	void Update ()
	{
		if (wiimote != null) {
			ActivateMotionPlus ();
			if (!calibrated && wmpActivated) {
				Debug.Log ("Starting calibration coroutine");
				StartCoroutine (calibrateMotionPlus ());
				calibrated = true;
			}
			ReadWiimoteEvents ();
			//ReadMotionPlus ();
		}
	}

	void OnApplicationQuit ()
	{
		if (wiimote != null) {
			WiimoteManager.Cleanup (wiimote);
		}
	}

	private void ActivateMotionPlus ()
	{
		if (!wmpActivated && wiimote.wmp_attached) {
			wiimote.ActivateWiiMotionPlus ();
			wmpActivated = true;
			Debug.Log ("WiiMotionPlus activated");
		}
	}

	private void ReadMotionPlus ()
	{
		if (wmpActivated && wiimote.current_ext == ExtensionController.MOTIONPLUS) {
			MotionPlusData data = wiimote.MotionPlus;
			Vector3 rotation = new Vector3 (-data.PitchSpeed,
				                   -data.YawSpeed,
				                   -data.RollSpeed) / coefficient;
			Debug.Log (rotation);
			transform.Rotate (rotation);
		}
	}

	private void ReadWiimoteEvents ()
	{
		int nbOfEvents;
		do {
			nbOfEvents = wiimote.ReadWiimoteData ();
		} while (nbOfEvents > 0);
	}

	private IEnumerator calibrateMotionPlus ()
	{
		if (wmpActivated && wiimote.current_ext == ExtensionController.MOTIONPLUS) {
			int numberOfSamples = 0;
			Vector3 sum = Vector3.zero;
			while (numberOfSamples < numberOfSamplesForCalibration) {
				MotionPlusData data = wiimote.MotionPlus;
				sum += new Vector3 (data.PitchSpeed, data.YawSpeed, data.RollSpeed);
				numberOfSamples++;
				yield return null;
			}
			wmpOffset = -(sum / numberOfSamples);
			Debug.Log ("The offset of the wiimotion plus is equal to:" + wmpOffset);
		}
	}
}
