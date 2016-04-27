using UnityEngine;
using System.Collections;
using WiimoteApi;
using System.Runtime.Remoting;
using System.Threading;

public class WiimoteScript : MonoBehaviour
{
	private Wiimote wiimote;
	private bool wmp_activated = false;

	void Start ()
	{
		WiimoteManager.FindWiimotes ();

		if (WiimoteManager.HasWiimote ()) {
			Debug.Log ("Wiimote is connected");
			wiimote = WiimoteManager.Wiimotes [0];
			wiimote.SendPlayerLED (true, false, false, true);

			// set output data format
			wiimote.SendDataReportMode (InputDataType.REPORT_BUTTONS_ACCEL_EXT16);

			wiimote.RequestIdentifyWiiMotionPlus ();
		}
	}

	void Update ()
	{
		ReadWiimoteData ();
		ActivateMotionPlus ();
	}

	void OnApplicationQuit ()
	{
		if (wiimote != null) {
			WiimoteManager.Cleanup (wiimote);
		}
	}

	void ReadWiimoteData ()
	{
		if (wiimote != null) {
			int nbWiimoteEvents;
			do {
				nbWiimoteEvents = wiimote.ReadWiimoteData ();

				if (nbWiimoteEvents > 0 && wiimote.current_ext == ExtensionController.MOTIONPLUS) {
					Vector3 rotation = new Vector3 (-wiimote.MotionPlus.PitchSpeed,
						                   wiimote.MotionPlus.YawSpeed,
						                   wiimote.MotionPlus.RollSpeed) / 95f;

					Debug.Log ("offset is equal to: " + rotation);
				}
			} while (nbWiimoteEvents > 0);
		}
	}

	void ActivateMotionPlus ()
	{
		if (wiimote != null) {
			if (wiimote.wmp_attached && !wmp_activated) {
				// activate wiimotionplus
				if (wiimote.ActivateWiiMotionPlus ()) {
					Debug.Log ("Wiimotion plus activated");
					wmp_activated = true;
				}
			}
		}
	}
}
