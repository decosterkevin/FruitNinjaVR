using System;
using UnityEngine;
using Random = UnityEngine.Random;

    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private MouseLook m_MouseLook;
        private Camera m_Camera;
        private float m_YRotation;
        private Vector2 m_Input;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private Vector3 m_OriginalCameraPosition;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
			m_MouseLook.Init(transform , m_Camera.transform);
        }


        // Update is called once per frame
        private void Update()
        {
            RotateView();
            // the jump state needs to read here to make sure it is not missed
           
        }


      


        private void FixedUpdate()
        {
           
           
            UpdateCameraPosition();
        }


        
        private void UpdateCameraPosition()
        {
            Vector3 newCameraPosition;
            
                newCameraPosition = m_Camera.transform.localPosition;
            
            m_Camera.transform.localPosition = newCameraPosition;
        }


       

        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    
}