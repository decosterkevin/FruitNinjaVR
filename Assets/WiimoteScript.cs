using UnityEngine;
using System.Collections;
using WiimoteApi;

public class WiimoteScript : MonoBehaviour {
    // minimal value to detect a motion
    [Range (0.0001f, 1)]
    public float detectionThreshold = 0.2f;

    // accelerometer coefficient
    public float coefficient = 10f;

    // experimental acceleration calibration data
    private int[,] accelCalibrationData = {{ 529, 529, 617 },
        { 522, 633, 512 },
        { 630, 528, 513 }
    };

    // default rotation of the wiimote
    private Quaternion defaultRotation;

    // initial position of the sword
    private Vector3 initialPosition;

    private Wiimote wiimote;

    private bool wmpActivated = false;

    void Start () {
        // store default rotation quaternion
        defaultRotation = transform.rotation;
        initialPosition = transform.position;

        WiimoteManager.FindWiimotes ();

        if (WiimoteManager.HasWiimote ()) {
            Debug.Log ("Wiimote is connected");
            wiimote = WiimoteManager.Wiimotes [0];
            wiimote.SendPlayerLED (true, false, false, false);

            // set output data format
            wiimote.SendDataReportMode (InputDataType.REPORT_BUTTONS_ACCEL_EXT16);

            wiimote.RequestIdentifyWiiMotionPlus ();

            wiimote.Accel.accel_calib = accelCalibrationData;
        }
    }

    void Update () {
        if (wiimote != null) {
            ActivateMotionPlus ();
            ReadWiimoteEvents ();
            ReadMotionPlus ();
        }
    }

    void OnApplicationQuit () {
        if (wiimote != null) {
            WiimoteManager.Cleanup (wiimote);
        }
    }

    private void ActivateMotionPlus () {
        if (!wmpActivated && wiimote.wmp_attached) {
            wiimote.ActivateWiiMotionPlus ();
            wmpActivated = true;
            Debug.Log ("WiiMotionPlus activated");
        }
    }

    private IEnumerator CalibrateMotionPlus () {
        if (wmpActivated && wiimote.current_ext == ExtensionController.MOTIONPLUS) {
            Debug.Log ("Calibrating...");
            MotionPlusData data = wiimote.MotionPlus;
            data.SetZeroValues ();
            Debug.Log ("MotionPlus was calibrated");
        } else {
            yield return null;
        }
    }

    private void ReadMotionPlus () {
        if (wmpActivated && wiimote.current_ext == ExtensionController.MOTIONPLUS) {
            MotionPlusData data = wiimote.MotionPlus;
            Vector3 rawRotation = new Vector3 (data.PitchSpeed,
                                      data.YawSpeed,
                                      data.RollSpeed);

            //Debug.Log ("raw rotation: " + rawRotation);

            Vector3 rotationDegrees = rawRotation * Time.deltaTime;
            //Debug.Log ("rotationdegrees :" + rotationDegrees);
            Vector3 finalRotation = checkDeadZone (rotationDegrees, detectionThreshold);
            //Debug.Log ("finalRotation :" + finalRotation);

            transform.Rotate (-finalRotation, Space.Self);
        }
    }

    public void StartAccelerometerCalibration () {
        StartCoroutine (CalibrateAccel ());
    }

    public void StartMotionPlusCalibration () {
        StartCoroutine (CalibrateMotionPlus ());
    }

    private IEnumerator CalibrateAccel () {
        Debug.Log ("Starting calibration process..");
        for (int i = 0; i < 3; i++) {
            AccelCalibrationStep calibrationStep = (AccelCalibrationStep)i;
            Debug.Log ("Please put wiimote in " + (AccelCalibrationStep)i + " position");
            yield return new WaitForSeconds (3f);
            Debug.Log ("Calibrating...");
            wiimote.Accel.CalibrateAccel (calibrationStep);
        }
        Debug.Log ("Calibration done");
        accelCalibrationData = wiimote.Accel.accel_calib;
        PrintAccelCalibration ();
    }

    private void ReadWiimoteEvents () {
        int nbOfEvents;
        do {
            nbOfEvents = wiimote.ReadWiimoteData ();
        } while (nbOfEvents > 0);

        if (wiimote.Button.a) {
            Debug.Log ("Button a was pressed");
            ResetWiimoteRotation ();
            transform.position = initialPosition;
        }
    }

    private void ResetWiimoteRotation () {
        transform.rotation = defaultRotation;
    }

    private Vector3 GetAccelVector () {
        float accel_x;
        float accel_y;
        float accel_z;

        float[] accel = wiimote.Accel.GetCalibratedAccelData ();
        accel_x = accel [0];
        accel_y = -accel [2];
        accel_z = -accel [1];

        return new Vector3 (accel_x, accel_y, accel_z).normalized;
    }

    private static Vector3 checkDeadZone (Vector3 input, float threshold) {
        float x = Mathf.Abs (input.x) < threshold ? 0 : input.x;
        float y = Mathf.Abs (input.y) < threshold ? 0 : input.y;
        float z = Mathf.Abs (input.z) < threshold ? 0 : input.z;
        return new Vector3 (x, y, z);
    }

    private void PrintAccelCalibration () {
        foreach (int i in accelCalibrationData) {
            Debug.Log (i);
        }
    }
}
