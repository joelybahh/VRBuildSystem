using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBuildSystem {
    /// <summary>
    /// This class is attatched to the ghost items, and handles the snapping logic for items.
    /// TODO: Fix the fact it currently works with 4 pre-defined tags! this limits only to
    ///       use with foundation objects which is not ideal. Setup like this for prototyping.
    /// </summary>
    public class FoundationSnapHandler : MonoBehaviour {

        #region Private Variables

        private Collider    m_curCollider;
        private Vector3     m_snapPos;
        private eObjectSide m_sideHit;

        #endregion

        #region Public Properties

        public bool m_inSnapZone { get; set; }

        #endregion

        #region Unity Methods

        private void Start() {
            m_curCollider = null;
            m_inSnapZone = false;
            m_sideHit = eObjectSide.NULL;
        }

        private void Update() {
            // if we are in the snapzone, and we actually have collider info
            if ( m_inSnapZone && m_curCollider != null ) {

                // set up/calculate the temp xyz variables for use in setting the object to its snapped position.
                float x = m_curCollider.transform.parent.position.x + GetOffsetAmount().x;
                float y = 0.25f; // HACK: fix me, just plus half of the objects y scale onto the pos!
                float z = m_curCollider.transform.parent.position.z + GetOffsetAmount().z;

                // set the position of the object to that position.
                transform.position = new Vector3(x, y, z);
            }
        }

        private void OnTriggerEnter( Collider col ) {         
            switch ( col.tag ) {
                case "SnapZoneLeft":  SetSide(col, eObjectSide.LEFT);  break;               
                case "SnapZoneRight": SetSide(col, eObjectSide.RIGHT); break;               
                case "SnapZoneFront": SetSide(col, eObjectSide.FRONT); break;
                case "SnapZoneBack":  SetSide(col, eObjectSide.BACK);  break;               
            }
        }

        private void OnTriggerExit( Collider col ) {
            // if we exit the trigger, we are no longer in the snap zone.
            // this is to ensure the boolean isn't set back to true when you move the
            // mouse too far.
            m_inSnapZone = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method simply sets up the current side correctly, updating
        /// the current collider as well as setting its isInSnapZone bool
        /// to true. Most importanty sets up the sideHit to the correct value.
        /// </summary>
        /// <param name="col">    the collider we are hitting.   </param>
        /// <param name="a_side"> the side we want to set it to. </param>
        private void SetSide( Collider col, eObjectSide a_side) {
            m_curCollider = col;
            m_inSnapZone = true;
            m_sideHit = a_side;
        }

        /// <summary>
        /// Determines the offset amount based on the current side being hit
        /// TODO: create an extension method for the enum to convert its vector3 representation. 
        /// </summary>
        /// <returns></returns>
        private Vector3 GetOffsetAmount() {
            switch ( m_sideHit ) {
                case eObjectSide.FRONT: return new Vector3(0, 0,  ( transform.localScale.z / 2 ));
                case eObjectSide.BACK:  return new Vector3(0, 0, -( transform.localScale.z / 2 ));
                case eObjectSide.LEFT:  return new Vector3(-( transform.localScale.x / 2 ), 0, 0);
                case eObjectSide.RIGHT: return new Vector3( ( transform.localScale.x / 2 ), 0, 0);
                default: return Vector3.zero;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Simply gets the snapped position
        /// </summary>
        /// <returns>Current position</returns>
        public Vector3 GetSnappedPos() {
            return transform.position;
        }

        #endregion
    }

    #region Public Enums

    /// <summary>
    /// HACK: enum setup up for rapid-prototyping
    ///       maybe I leave this and make this the foundation
    ///       snap handler?
    /// </summary>
    public enum eObjectSide {
        FRONT,
        BACK, 
        LEFT,
        RIGHT,
        NULL
    }

    #endregion      
}