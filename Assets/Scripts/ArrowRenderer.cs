using UnityEngine;
using System.Collections;

public class ArrowRenderer : MonoBehaviour
{

    public FoodManager foodManager;
    public GameObject box;
    public GameObject Arrow;
   
    void Start()
    {
        Arrow.GetComponent<CanvasRenderer>().SetAlpha(0);
        box.GetComponent<CanvasRenderer>().SetAlpha(0);
    }
    void Update()
    {
        PositionArrow();
    }

    void PositionArrow()
    {

        int towerIndex = foodManager.getCurrentFiringTowerIndex();
        Rect rect = GetComponent<RectTransform>().rect;
        float width = rect.size.x;
        float heigh = rect.size.y;
        if (towerIndex != -1)
        {
            GameObject goTarget = foodManager.spawnPoints[towerIndex];

            Vector3 screenPos = Camera.main.WorldToViewportPoint(goTarget.transform.position);
            screenPos.x *= width;
            screenPos.y *= heigh;
            if (screenPos.z > 0.0f && screenPos.x > 0.0f && screenPos.x < width && screenPos.y > 0.0f && screenPos.y < heigh)
            {
                Arrow.GetComponent<CanvasRenderer>().SetAlpha(0);
                box.GetComponent<CanvasRenderer>().SetAlpha(1);
                Vector3 screenCenter = new Vector3(width, heigh, 0.0f) / 2.0f;
                screenPos -= screenCenter;
                box.transform.localPosition = screenPos;
                //Debug.Log("inscreen target");
            }
            else {
                Arrow.GetComponent<CanvasRenderer>().SetAlpha(1);
                box.GetComponent<CanvasRenderer>().SetAlpha(0);
                if (screenPos.z < 0.0f)
                {
                    screenPos *= -1.0f;
                }
                
                Vector3 screenCenter = new Vector3(width, heigh, 0.0f) / 2.0f;
                
                screenPos -= screenCenter;
                
                float angle = Mathf.Atan2(screenPos.y, screenPos.x);
                //angle -= 90.0f * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                
                float m = sin / cos;
                Vector3 screenBounds = screenCenter * 0.9f;

                if (sin > 0.0f)
                {
                    screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0.0f);
                }
                else
                {
                    screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0.0f);
                }

                if (screenPos.x > screenBounds.x)
                {

                    screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0.0f);
                }
                else if (screenPos.x < -screenBounds.x)
                {
                    screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0.0f);
                }



                
                Arrow.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle * Mathf.Rad2Deg);
                Arrow.transform.localPosition = screenPos;

            }
        }
    }
}
