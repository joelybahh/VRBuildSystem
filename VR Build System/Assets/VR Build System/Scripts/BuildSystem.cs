using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBuildSystem {
    /// <summary>
    /// Auth: Joel Gabriel
    /// Desc: This class handles the logic for building/placing objects. It is setup so it can hold a list of objects that can
    ///       be placed, and can correctly handle the swapping of these objects. It subsribes to events that are called on the
    ///       BuildEvents script.
    /// </summary>
    public class BuildSystem : MonoBehaviour {

        // A list of all the objects you can build/place.
        [SerializeField] private List<BuildObject> m_buildObjects;

        
        private GameObject        m_previewObject;  // The current preview object.
        private FoundationSnapHandler       m_snapHandler;    // A reference to the SnapHandler.     
        private int               m_selectionIndex; // The current selection index.      
        private bool              m_systemActive;   // Whether or not the system is on or off. (off means no building logic runs).
        private eObjectType       m_curObjType;

        void Start() {
            // Set out intial index to 0
            m_selectionIndex = 0;

            // Intialize the first object preview
            InitGhostPreview();

            // Set the system to be active by default
            m_systemActive = true;

            m_curObjType = m_buildObjects[m_selectionIndex].ObjectType;
        }

        void OnEnable() {
            // Subscribe to the on ground hit event so we can place objects
            BuildEvents.OnGroundHit += PlaceObject;
            BuildEvents.OnGroundHit += UpdateGhost;
            BuildEvents.OnObjectHit += DestroySelectedObject;
        }

        void OnDisable() {
            // Un-Subscribe to the on ground hit event so we can place objects
            BuildEvents.OnObjectHit -= DestroySelectedObject;
            BuildEvents.OnGroundHit -= UpdateGhost;
            BuildEvents.OnGroundHit -= PlaceObject;
        }

        /// <summary>
        /// Instantiates, and sets up the ghost prievew for the first item
        /// </summary>
        private void InitGhostPreview() {
            m_previewObject = Instantiate(m_buildObjects[m_selectionIndex].PreviewObject, Vector3.zero, Quaternion.identity);
            m_snapHandler = m_previewObject.GetComponent<FoundationSnapHandler>();
            m_buildObjects[m_selectionIndex].hasInstantiatedPreview = true;
        }

        /// <summary>
        /// places the currently selected item as long as it is viable
        /// </summary>
        /// <param name="a_hit">the hit object returned from the raycast</param>
        /// <param name="a_rotation">the rotation to instantiate the object at</param>
        private void PlaceObject( RaycastHit a_hit, Quaternion a_rotation ) {
            // Is the surface too steep? If so don't bother placing, hence return.
            if ( SurfaceTooSteep(a_hit.normal) ) return;

            // We know the surface isn't too steep, so lets check for a left click
            // TODO: UPDATED FOR VIVE CONTROLLERS
            if ( Input.GetMouseButtonDown(0) ) {
                // Are we currently snapping to an object?
                if ( m_snapHandler.m_inSnapZone ) {
                    // YES, Setup the temp variables for spawning the object in the snapped position.
                    var obj = m_buildObjects[m_selectionIndex].FinalObject;
                    float x = m_snapHandler.GetSnappedPos().x;
                    float y = 0.25f;
                    float z = m_snapHandler.GetSnappedPos().z;

                    Instantiate(obj, new Vector3(x, y, z), a_rotation);
                } else {
                    // NO, instantiate the object wherever the raycast is hitting instead.
                    Instantiate(m_buildObjects[m_selectionIndex].FinalObject, a_hit.point, a_rotation);
                }
            }
        }
       
        /// <summary>
        /// Updates the ghost of the object you are about to place.
        /// </summary>
        /// <param name="a_hit">the hit information from the raycast.</param>
        /// <param name="a_rotation">the rotation information for the ghost.</param>
        private void UpdateGhost( RaycastHit a_hit, Quaternion a_rotation ) {

            // If you move too far away from the snap point
            if ( Vector3.Distance(m_snapHandler.GetSnappedPos(), a_hit.point) > 1f ) {
                // Disconnect.
                m_snapHandler.m_inSnapZone = false;
            }

            // Update position and rotation.
            // HACK: fix hard coded y for testing, need to mathematically sort this llogic out.
            m_previewObject.transform.position = new Vector3(a_hit.point.x, 0.25f, a_hit.point.z);
            m_previewObject.transform.rotation = a_rotation;
        }

        /// <summary>
        /// Destroys the selected object (the object you are hovering).
        /// </summary>
        /// <param name="a_hit"></param>
        private void DestroySelectedObject(RaycastHit a_hit) {
            // Simply destroys the currently hovered object.
            // HACK: fix this, hard coded for testing.
            if ( Input.GetMouseButtonDown(1) ) {
                Destroy(a_hit.transform.gameObject);
            }
        }

        /// <summary>
        /// Updates the selected object, handles the swapping between foundations, pillars, etc.
        /// </summary>
        /// <param name="a_newIndex">The index of the new item you would like to swap to.</param>
        private void UpdateSelectedObject( int a_newIndex ) {
            // Make sure we turn this last one off to prevent incorrect ghost displaying for the new item
            m_buildObjects[m_selectionIndex].hasInstantiatedPreview = false;
            // Update the preview object to match the new index.
            m_previewObject = m_buildObjects[a_newIndex].PreviewObject;

            m_curObjType = m_buildObjects[m_selectionIndex].ObjectType;
        }

        /// <summary>
        /// Checks if the surface is too steep
        /// </summary>
        /// <param name="a_surfNormal">The normal of the surface we just hit.</param>
        /// <returns>true if it is, false if it isn't</returns>
        private bool SurfaceTooSteep(Vector3 a_surfNormal) {
            // get the angle from the surface normal, to the global up vector
            float angle = Vector3.Angle(a_surfNormal, Vector3.up);

            if ( angle > 45 )
                return true;
            else
                return false;
        }
    }
}