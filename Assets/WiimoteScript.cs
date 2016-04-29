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
	public Vector3 MotionPlusOffset = new Vector3 (-2.0f, 2.2f, 14.2f);

	// minimal value to detect a motion
	[Range (0.0001f, 1)]
	public float detectionThreshold = 0.2f;

	// default rotation of the wiimote
	private Quaternion defaultRotation;

	private Wiimote wiimote;

	private bool wmpActivated = false;

	void Start ()
	{
		// store default rotation quaternion
		defaultRotation = transform.rotation;

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
			ReadWiimoteEvents ();
			ReadMotionPlus ();
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
			Vector3 rawRotation = new Vector3 (data.PitchSpeed,
				                      data.YawSpeed,
				                      data.RollSpeed);

			Debug.Log ("raw rotation: " + rawRotation);

			Vector3 offsetRotation = MotionPlusOffset + rawRotation;
			Debug.Log ("offsetRotation :" + offsetRotation);
			Vector3 rotationDegrees = offsetRotation / coefficient;
			Debug.Log ("rotationdegrees :" + rotationDegrees);
			Vector3 finalRotation = checkDeadZone (rotationDegrees, detectionThreshold);
			Debug.Log ("finalRotation :" + finalRotation);

			transform.Rotate (-finalRotation, Space.Self);
		}
	}

	private void ReadWiimoteEvents ()
	{
		int nbOfEvents;
		do {
			nbOfEvents = wiimote.ReadWiimoteData ();
		} while (nbOfEvents > 0);

		if (wiimote.Button.a) {
			Debug.Log ("Button a was pressed");
			ResetWiimoteRotation ();
		}
	}

	private void ResetWiimoteRotation ()
	{
		transform.rotation = defaultRotation;
	}

	private static Vector3 checkDeadZone (Vector3 input, float threshold)
	{
		float x = Mathf.Abs (input.x) < threshold ? 0 : input.x;
		float y = Mathf.Abs (input.y) < threshold ? 0 : input.y;
		float z = Mathf.Abs (input.z) < threshold ? 0 : input.z;
		return new Vector3 (x, y, z);
	}
}
