#if GRIFFIN
using UnityEngine;
using UnityEngine.Rendering;
using Pinwheel.Griffin.Physic;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public class GTreePrototype
    {
        [SerializeField]
        internal GameObject prefab;
        public GameObject Prefab
        {
            get
            {
                return prefab;
            }
            set
            {
                GameObject oldValue = prefab;
                GameObject newValue = value;
                prefab = newValue;
                if (oldValue != newValue)
                {
                    Refresh();
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        internal Mesh sharedMesh;
        public Mesh SharedMesh
        {
            get
            {
                return sharedMesh;
            }
            private set
            {
                sharedMesh = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        internal Material[] sharedMaterials;
        public Material[] SharedMaterials
        {
            get
            {
                return sharedMaterials;
            }
            private set
            {
                sharedMaterials = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        internal Mesh sharedMeshLod1;
        public Mesh SharedMeshLod1
        {
            get
            {
                return sharedMeshLod1;
            }
            private set
            {
                sharedMeshLod1 = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        internal Material[] sharedMaterialsLod1;
        public Material[] SharedMaterialsLod1
        {
            get
            {
                return sharedMaterialsLod1;
            }
            private set
            {
                sharedMaterialsLod1 = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        internal ShadowCastingMode shadowCastingMode;
        public ShadowCastingMode ShadowCastingMode
        {
            get
            {
                return shadowCastingMode;
            }
            set
            {
                shadowCastingMode = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        internal bool receiveShadow;
        public bool ReceiveShadow
        {
            get
            {
                return receiveShadow;
            }
            set
            {
                receiveShadow = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        internal ShadowCastingMode billboardShadowCastingMode;
        public ShadowCastingMode BillboardShadowCastingMode
        {
            get
            {
                return billboardShadowCastingMode;
            }
            set
            {
                billboardShadowCastingMode = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        internal bool billboardReceiveShadow;
        public bool BillboardReceiveShadow
        {
            get
            {
                return billboardReceiveShadow;
            }
            set
            {
                billboardReceiveShadow = value;
            }
        }

        [SerializeField]
        internal int layer;
        public int Layer
        {
            get
            {
                return layer;
            }
            set
            {
                layer = value;
            }
        }

        [SerializeField]
        private bool keepPrefabLayer;
        public bool KeepPrefabLayer
        {
            get
            {
                return keepPrefabLayer;
            }
            set
            {
                keepPrefabLayer = value;
            }
        }

        [SerializeField]
        internal BillboardAsset billboard;
        public BillboardAsset Billboard
        {
            get
            {
                return billboard;
            }
            set
            {
                billboard = value;
            }
        }

        [SerializeField]
        internal bool hasCollider;
        public bool HasCollider
        {
            get
            {
                return hasCollider;
            }
            set
            {
                hasCollider = value;
            }
        }

        [SerializeField]
        internal GTreeColliderInfo colliderInfo;
        public GTreeColliderInfo ColliderInfo
        {
            get
            {
                return colliderInfo;
            }
            set
            {
                colliderInfo = value;
            }
        }

        [SerializeField]
        private float pivotOffset;
        public float PivotOffset
        {
            get
            {
                return pivotOffset;
            }
            set
            {
                pivotOffset = value;
            }
        }

        [SerializeField]
        private Quaternion baseRotation = Quaternion.identity;
        public Quaternion BaseRotation
        {
            get
            {
                if (baseRotation.x == 0 &&
                    baseRotation.y == 0 &&
                    baseRotation.z == 0 &&
                    baseRotation.w == 0)
                {
                    baseRotation = Quaternion.identity;
                }
                return baseRotation;
            }
            set
            {
                baseRotation = value;
                if (baseRotation.x == 0 &&
                     baseRotation.y == 0 &&
                     baseRotation.z == 0 &&
                     baseRotation.w == 0)
                {
                    baseRotation = Quaternion.identity;
                }
            }
        }

        [SerializeField]
        private Vector3 baseScale = Vector3.one;
        public Vector3 BaseScale
        {
            get
            {
                return baseScale;
            }
            set
            {
                baseScale = value;
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        private string editor_PrefabAssetPath;
        public string Editor_PrefabAssetPath
        {
            get
            {
                return editor_PrefabAssetPath;
            }
            set
            {
                editor_PrefabAssetPath = value;
            }
        }
#endif

        public Bounds BaseBounds
        {
            get
            {
                if (Prefab != null)
                {
                    MeshFilter mf = Prefab.GetComponentInChildren<MeshFilter>();
                    if (mf != null)
                    {
                        return mf.sharedMesh.bounds;
                    }
                }
                return new Bounds();
            }
        }

        public bool IsValid
        {
            get
            {
                return Prefab != null && SharedMesh != null && SharedMaterials != null;
            }
        }

        public bool HasLod0
        {
            get
            {
                return sharedMesh != null && sharedMaterials != null && sharedMaterials.Length != 0;
            }
        }

        public bool HasLod1
        {
            get
            {
                return sharedMeshLod1 != null && sharedMaterialsLod1 != null && sharedMaterialsLod1.Length != 0;
            }
        }

        public static GTreePrototype Create(GameObject g)
        {
            GTreePrototype prototype = new GTreePrototype();
            prototype.Prefab = g;
            prototype.PivotOffset = 0;
            prototype.BaseRotation = Quaternion.identity;
            prototype.BaseScale = Vector3.one;
            return prototype;
        }

        public void Refresh()
        {
            if (Prefab == null)
            {
                SharedMesh = null;
                SharedMaterials = null;
                SharedMeshLod1 = null;
                SharedMaterialsLod1 = null;
                ShadowCastingMode = ShadowCastingMode.Off;
                ReceiveShadow = false;
                Layer = 0;
            }
            else
            {
                MeshFilter[] meshFilters = Prefab.GetComponentsInChildren<MeshFilter>();
                MeshRenderer[] meshRenderers = Prefab.GetComponentsInChildren<MeshRenderer>();

                if (meshFilters.Length > 0)
                {
                    SharedMesh = meshFilters[0].sharedMesh;
                }

                if (meshRenderers.Length > 0)
                {
                    SharedMaterials = meshRenderers[0].sharedMaterials;
                    ShadowCastingMode = meshRenderers[0].shadowCastingMode;
                    ReceiveShadow = meshRenderers[0].receiveShadows;
                }

                if (meshFilters.Length > 1)
                {
                    SharedMeshLod1 = meshFilters[1].sharedMesh;
                }

                if (meshRenderers.Length > 1)
                {
                    SharedMaterialsLod1 = meshRenderers[1].sharedMaterials;
                }

                CapsuleCollider col = Prefab.GetComponentInChildren<CapsuleCollider>();
                hasCollider = col != null;
                if (col != null)
                {
                    ColliderInfo = new GTreeColliderInfo(col);
                }

                if (KeepPrefabLayer)
                {
                    Layer = Prefab.layer;
                }
            }

            if (BaseScale == Vector3.zero)
                BaseScale = Vector3.one;
        }

        public static explicit operator GTreePrototype(TreePrototype p)
        {
            return Create(p.prefab);
        }

        public static explicit operator TreePrototype(GTreePrototype p)
        {
            TreePrototype prototype = new TreePrototype();
            prototype.prefab = p.Prefab;
            return prototype;
        }

        public bool Equals(TreePrototype treePrototype)
        {
            return Prefab == treePrototype.prefab;
        }

        public BoundingSphere GetBoundingSphere()
        {
            if (SharedMesh == null)
            {
                return new BoundingSphere(Vector3.zero, 0);
            }
            else
            {
                Bounds b = SharedMesh.bounds;
                Vector3 pos = b.center;
                float radius = Vector3.Distance(b.max, b.center);
                return new BoundingSphere(pos, radius);
            }
        }
    }
}
#endif
