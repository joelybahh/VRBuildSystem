using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBuildSystem {
    public class BuildEvents : MonoBehaviour {

        #region Singleton
        private static BuildEvents m_instance;
        public static BuildEvents Instance {
            get {
                return m_instance;
            }
        }
        #endregion

        #region Delegates / Events

        // Event for hitting the ground
        public delegate void PlaceBuildObject( RaycastHit a_hit, Quaternion a_rotation );
        public static event PlaceBuildObject OnGroundHit;

        // Event for hitting an object
        public delegate void DestroyBuildObject( RaycastHit a_hit );
        public static event DestroyBuildObject OnObjectHit;

        #endregion

        #region Private Variables

        private RaycastHit m_hit;

        #endregion

        #region Unity Methods

        private void Awake() {
            m_instance = this;
        }

        private void Update() {
            // TODO: Update this to be more dynamic, allow it to work with VIVE and OCULUS Controlllers
            if ( Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out m_hit) ) {
                UpdateGroundHit();
                UpdateObjectHit();
            }
        }

        private void UpdateObjectHit() {
            // If what we hit is the a type of Object
            if ( m_hit.transform.tag == "Foundation" || 
                 m_hit.transform.tag == "Roof" ||
                 m_hit.transform.tag == "Pillar" ||
                 m_hit.transform.tag == "Doorway") 
            {
                // Check we have something subscribed to this event
                if ( OnObjectHit != null ) {
                    // If so execute all associated logic
                    OnObjectHit(m_hit);
                }
            }
        }

        private void UpdateGroundHit() {
            // If what we hit is the floors
            if ( m_hit.transform.tag == "Floor" ) {
                // Check we have something subscribed to this event
                if ( OnGroundHit != null ) {
                    // If so execute all associated logic
                    OnGroundHit(m_hit, Quaternion.identity);
                }
            }
        }

        #endregion
    }
}