using UnityEngine;
using System.Collections;
using WiimoteApi;

public class WiimoteScript : MonoBehaviour {
    private Wiimote wiimote;

    void Start () {
        WiimoteManager.FindWiimotes ();

        if (WiimoteManager.HasWiimote ()) {
            Debug.Log ("Wiimote is connected");
            wiimote = WiimoteManager.Wiimotes [0];
            wiimote.SendPlayerLED (true, false, false, true);

            // activate wiimotionplus
            if (wiimote.RequestIdentifyWiiMotionPlus ()) {
                if (wiimote.ActivateWiiMotionPlus ()) {
                    Debug.Log ("Wiimotion plus activated");
                }
            }
        }
    }

    void Update () {
    }

    void OnApplicationQuit () {
        WiimoteManager.Cleanup (wiimote);
    }
}
