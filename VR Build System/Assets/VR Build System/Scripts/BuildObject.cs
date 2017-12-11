using UnityEngine;

namespace VRBuildSystem {
    [System.Serializable]
    public class BuildObject {

        [SerializeField] private GameObject   m_previewObj;
        [SerializeField] private GameObject   m_finalObj;
        [SerializeField] private eObjectType  m_objType;
        [SerializeField] private eObjectStyle m_objStyle;
        [SerializeField] private int          m_quantity;

        public bool hasInstantiatedPreview { get; set; }

        public int Quantity {
            get { return m_quantity; }
            set { m_quantity = value; }
        }

        public GameObject FinalObject {
            get { return m_finalObj; }
        }

        public GameObject PreviewObject {
            get { return m_previewObj; }
        }

        public eObjectType ObjectType {
            get { return m_objType; }
        }
    }

    public enum eObjectType {
        FOUNDATION,
        WALL,
        PILLAR,
        ROOF, 
        DOOR_WAY,
        DOOR
    }
    public enum eObjectStyle {
        HAY,
        WOOD,
        STONE,
        STEEL,
        REINFORCED_STEEL
    }
}