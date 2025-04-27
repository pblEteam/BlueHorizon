#if GRIFFIN
using UnityEngine;
using UnityEngine.Rendering;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public class GGrassPrototype
    {
        [SerializeField]
        private Texture2D texture;
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
            }
        }

        [SerializeField]
        private GameObject prefab;
        public GameObject Prefab
        {
            get
            {
                return prefab;
            }
            set
            {
                prefab = value;
                RefreshDetailObjectSettings();
            }
        }

        [SerializeField]
        internal Vector3 size;
        public Vector3 Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
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
        private GGrassShape shape;
        public GGrassShape Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;
            }
        }

        [SerializeField]
        private Mesh customMesh;
        public Mesh CustomMesh
        {
            get
            {
                return customMesh;
            }
            set
            {
                customMesh = value;
            }
        }

        [SerializeField]
        private Mesh customMeshLod1;
        public Mesh CustomMeshLod1
        {
            get
            {
                return customMeshLod1;
            }
            set
            {
                customMeshLod1 = value;
            }
        }

        [SerializeField]
        private Mesh detailMesh;
        public Mesh DetailMesh
        {
            get
            {
                return detailMesh;
            }
            private set
            {
                detailMesh = value;
            }
        }

        [SerializeField]
        private Mesh detailMeshLod1;
        public Mesh DetailMeshLod1
        {
            get
            {
                return detailMeshLod1;
            }
            private set
            {
                detailMeshLod1 = value;
            }
        }

        [System.Obsolete]
        [SerializeField]
        private Material detailMaterial;
        [System.Obsolete]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public Material DetailMaterial
        {
            get
            {
                return detailMaterial;
            }
            private set
            {
                detailMaterial = value;
            }
        }

        [SerializeField]
        private Material[] detailMaterials;
        public Material[] DetailMaterials
        {
            get
            {
                return detailMaterials;
            }
            private set
            {
                detailMaterials = value;
            }
        }

        [SerializeField]
        private Material[] detailMaterialsLod1;
        public Material[] DetailMaterialsLod1
        {
            get
            {
                return detailMaterialsLod1;
            }
            private set
            {
                detailMaterialsLod1 = value;
            }
        }

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

        [SerializeField]
        private bool alignToSurface;
        public bool AlignToSurface
        {
            get
            {
                return alignToSurface;
            }
            set
            {
                alignToSurface = value;
            }
        }

        [SerializeField]
        internal float pivotOffset;
        public float PivotOffset
        {
            get
            {
                return pivotOffset;
            }
            set
            {
                pivotOffset = Mathf.Clamp(value, -1, 1);
            }
        }

        [SerializeField]
        private float bendFactor = 1;
        public float BendFactor
        {
            get
            {
                return bendFactor;
            }
            set
            {
                bendFactor = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private Color color = Color.white;
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        [SerializeField]
        private bool isBillboard;
        public bool IsBillboard
        {
            get
            {
                return isBillboard;
            }
            set
            {
                isBillboard = value;
            }
        }

        public bool HasLod0
        {
            get
            {
                if (shape != GGrassShape.DetailObject)
                {
                    return true;
                }
                else
                {
                    return DetailMesh != null && DetailMaterials != null && DetailMaterials.Length > 0;
                }
            }
        }

        public bool HasLod1
        {
            get
            {
                if (shape != GGrassShape.DetailObject)
                {
                    return true;
                }
                else
                {
                    return DetailMeshLod1 != null && DetailMaterialsLod1 != null && DetailMaterialsLod1.Length > 0;
                }
            }
        }

        public static GGrassPrototype Create(Texture2D tex)
        {
            GGrassPrototype prototype = new GGrassPrototype();
            prototype.Shape = GGrassShape.Quad;
            prototype.Texture = tex;
            prototype.ShadowCastingMode = ShadowCastingMode.On;
            prototype.ReceiveShadow = true;
            prototype.Size = Vector3.one;
            prototype.Layer = LayerMask.NameToLayer("Default");
            prototype.AlignToSurface = false;
            prototype.PivotOffset = 0;
            prototype.BendFactor = 1;
            prototype.Color = Color.white;
            return prototype;
        }

        public static GGrassPrototype Create(GameObject prefab)
        {
            GGrassPrototype prototype = new GGrassPrototype();
            prototype.Shape = GGrassShape.DetailObject;
            prototype.Prefab = prefab;
            prototype.Size = Vector3.one;
            prototype.Layer = LayerMask.NameToLayer("Default");
            prototype.AlignToSurface = false;
            prototype.PivotOffset = 0;
            prototype.BendFactor = 1;
            prototype.Color = Color.white;
            return prototype;
        }

        public Mesh GetBaseMesh()
        {
            if (Shape == GGrassShape.DetailObject)
            {
                return DetailMesh;
            }
            if (Shape == GGrassShape.CustomMesh)
            {
                return CustomMesh != null ? CustomMesh : GRuntimeSettings.Instance.foliageRendering.GetGrassMesh(GGrassShape.Quad);
            }
            else
            {
                return GRuntimeSettings.Instance.foliageRendering.GetGrassMesh(Shape);
            }
        }

        public Mesh GetBaseMeshLod1()
        {
            if (Shape == GGrassShape.DetailObject)
            {
                return DetailMeshLod1;
            }
            if (Shape == GGrassShape.CustomMesh)
            {
                return CustomMeshLod1 != null ? CustomMeshLod1 :
                    CustomMesh != null ? CustomMesh :
                    GRuntimeSettings.Instance.foliageRendering.GetGrassMeshLod1(GGrassShape.Quad);
            }
            else
            {
                return GRuntimeSettings.Instance.foliageRendering.GetGrassMeshLod1(Shape);
            }
        }

        public void RefreshDetailObjectSettings()
        {
            if (Prefab == null)
            {
                DetailMesh = null;
                DetailMaterials = null;
                DetailMeshLod1 = null;
                DetailMaterialsLod1 = null;
            }
            else
            {
                MeshFilter[] meshFilters = Prefab.GetComponentsInChildren<MeshFilter>();
                MeshRenderer[] meshRenderers = Prefab.GetComponentsInChildren<MeshRenderer>();

                if (meshFilters.Length > 0)
                {
                    DetailMesh = meshFilters[0].sharedMesh;
                }

                if (meshRenderers.Length > 0)
                {
                    DetailMaterials = meshRenderers[0].sharedMaterials;
                    ShadowCastingMode = meshRenderers[0].shadowCastingMode;
                    ReceiveShadow = meshRenderers[0].receiveShadows;
                }

                if (meshFilters.Length > 1)
                {
                    DetailMeshLod1 = meshFilters[1].sharedMesh;
                }

                if (meshRenderers.Length > 1)
                {
                    DetailMaterialsLod1 = meshRenderers[1].sharedMaterials;
                }
            }
        }

        public static explicit operator GGrassPrototype(DetailPrototype p)
        {
            GGrassPrototype proto = new GGrassPrototype();
            proto.Color = p.healthyColor;
            proto.Shape = p.usePrototypeMesh ? GGrassShape.DetailObject : GGrassShape.Quad;
            proto.Texture = p.prototypeTexture;
            proto.Prefab = p.prototype;
            proto.Size = new Vector3(p.maxWidth, p.maxHeight, p.maxWidth);
            proto.Layer = LayerMask.NameToLayer("Default");
            proto.AlignToSurface = false;
            proto.BendFactor = 1;
            proto.IsBillboard = p.renderMode == DetailRenderMode.GrassBillboard;
            return proto;
        }

        public static explicit operator DetailPrototype(GGrassPrototype p)
        {
            DetailPrototype proto = new DetailPrototype();
            proto.usePrototypeMesh = p.Shape == GGrassShape.DetailObject;
            proto.prototypeTexture = p.Texture;
            proto.prototype = p.Prefab;
            proto.minWidth = p.size.x;
            proto.maxWidth = p.size.x * 2;
            proto.minHeight = p.size.y;
            proto.maxHeight = p.size.y * 2;
            proto.healthyColor = p.color;
            if (p.IsBillboard && p.Shape != GGrassShape.DetailObject)
                proto.renderMode = DetailRenderMode.GrassBillboard;

            return proto;
        }

        public bool Equals(DetailPrototype detailPrototype)
        {
            bool modeEqual =
                (Shape == GGrassShape.Quad && !detailPrototype.usePrototypeMesh) ||
                (Shape == GGrassShape.DetailObject && detailPrototype.usePrototypeMesh);
            return
                modeEqual &&
                Texture == detailPrototype.prototypeTexture &&
                Prefab == detailPrototype.prototype &&
                Size == new Vector3(detailPrototype.maxWidth, detailPrototype.maxHeight, detailPrototype.maxWidth);
        }
    }
}
#endif
