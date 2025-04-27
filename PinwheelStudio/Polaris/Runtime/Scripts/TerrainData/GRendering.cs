#if GRIFFIN
using UnityEngine;
using UnityEngine.Serialization;

namespace Pinwheel.Griffin
{
    public class GRendering : ScriptableObject
    {
        [SerializeField]
        private GTerrainData terrainData;
        public GTerrainData TerrainData
        {
            get
            {
                return terrainData;
            }
            internal set
            {
                terrainData = value;
            }
        }

        [SerializeField]
        private bool castShadow;
        public bool CastShadow
        {
            get
            {
                return castShadow;
            }
            set
            {
                castShadow = value;
            }
        }

        [SerializeField]
        private bool receiveShadow;
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

        [FormerlySerializedAs("drawFoliage")]
        [SerializeField]
        private bool drawTrees;
        public bool DrawTrees
        {
            get
            {
                return drawTrees;
            }
            set
            {
                drawTrees = value;
            }
        }

        [SerializeField]
        private bool drawGrasses = true;
        public bool DrawGrasses
        {
            get
            {
                return drawGrasses;
            }
            set
            {
                drawGrasses = value;
            }
        }

        [SerializeField]
        private bool enableInstancing;
        [System.Obsolete]
        public bool EnableInstancing
        {
            get
            {
                if (!SystemInfo.supportsInstancing)
                    enableInstancing = false;
                return enableInstancing;
            }
            set
            {
                if (SystemInfo.supportsInstancing)
                {
                    enableInstancing = value;
                }
                else
                {
                    enableInstancing = false;
                }
            }
        }

        [SerializeField]
        private float treeLod1Start;
        public float TreeLod1Start
        {
            get
            {
                return treeLod1Start;
            }
            set
            {
                treeLod1Start = Mathf.Clamp(value, 0, billboardStart);
            }
        }

        [SerializeField]
        private float billboardStart;
        public float BillboardStart
        {
            get
            {
                return billboardStart;
            }
            set
            {
                billboardStart = Mathf.Clamp(value, 0, treeDistance);
            }
        }

        [SerializeField]
        private float treeDistance;
        public float TreeDistance
        {
            get
            {
                return treeDistance;
            }
            set
            {
                treeDistance = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float grassDistance;
        public float GrassDistance
        {
            get
            {
                return grassDistance;
            }
            set
            {
                grassDistance = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float grassLod1Start;
        public float GrassLod1Start
        {
            get
            {
                return grassLod1Start;
            }
            set
            {
                grassLod1Start = Mathf.Clamp(value, 0, grassDistance);
            }
        }

        [SerializeField]
        private float grassFadeStart;
        public float GrassFadeStart
        {
            get
            {
                return grassFadeStart;
            }
            set
            {
                grassFadeStart = Mathf.Clamp01(value);
            }
        }

        public void Reset()
        {
            name = "Rendering";
            CastShadow = GRuntimeSettings.Instance.renderingDefault.terrainCastShadow;
            ReceiveShadow = GRuntimeSettings.Instance.renderingDefault.terrainReceiveShadow;
            DrawTrees = GRuntimeSettings.Instance.renderingDefault.drawTrees;
            TreeDistance = GRuntimeSettings.Instance.renderingDefault.treeDistance;
            BillboardStart = GRuntimeSettings.Instance.renderingDefault.billboardStart;
            TreeLod1Start = GRuntimeSettings.Instance.renderingDefault.treeLod1Start;
            DrawGrasses = GRuntimeSettings.Instance.renderingDefault.drawGrasses;
            GrassDistance = GRuntimeSettings.Instance.renderingDefault.grassDistance;
            GrassLod1Start = GRuntimeSettings.Instance.renderingDefault.grassLod1Start;
            GrassFadeStart = GRuntimeSettings.Instance.renderingDefault.grassFadeStart;
        }

        public void ResetFull()
        {
            Reset();
        }

        public void CopyTo(GRendering des)
        {
            des.CastShadow = CastShadow;
            des.ReceiveShadow = ReceiveShadow;
            des.DrawTrees = DrawTrees;
            des.TreeLod1Start = TreeLod1Start;
            des.BillboardStart = BillboardStart;
            des.TreeDistance = TreeDistance;
            des.GrassDistance = GrassDistance;
            des.GrassLod1Start = GrassLod1Start;
            des.GrassFadeStart = GrassFadeStart;
        }
    }
}
#endif
