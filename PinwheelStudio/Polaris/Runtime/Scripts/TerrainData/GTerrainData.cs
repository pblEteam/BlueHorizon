#if GRIFFIN
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.Griffin
{
    [CreateAssetMenu(fileName = "Terrain Data", menuName = "Polaris/Terrain Data")]
    public class GTerrainData : ScriptableObject
    {
        public delegate void GlobalDirtyHandler(GTerrainData data, DirtyFlags flag);
        public static event GlobalDirtyHandler GlobalDirty;

        public delegate void DirtyHandler(DirtyFlags flag);
        public event DirtyHandler Dirty;

        internal delegate void GrassPatchChangedHandler(int index);
        internal event GrassPatchChangedHandler GrassPatchChanged;

        internal delegate void GrassPatchGridSizeChangedHandler();
        internal event GrassPatchGridSizeChangedHandler GrassPatchGridSizeChanged;

        internal delegate void TreeChangedHandler();
        internal event TreeChangedHandler TreeChanged;

        internal delegate void PrototypeGroupChangedHandler();
        internal event PrototypeGroupChangedHandler TreePrototypeGroupChanged;
        internal event PrototypeGroupChangedHandler GrassPrototypeGroupChanged;

        [System.Flags]
        public enum DirtyFlags : byte
        {
            None = 0,
            Geometry = 1,
            GeometryTimeSliced = 2,
            Rendering = 4,
            Shading = 8,
            Foliage = 16,
            Mask = 32,
            All = byte.MaxValue
        }

        [SerializeField]
        private string id;
        public string Id
        {
            get
            {
                return id;
            }
            internal set
            {
                id = value;
            }
        }

        [SerializeField]
        internal GGeometry geometry;
        public GGeometry Geometry
        {
            get
            {
                if (geometry == null)
                {
                    geometry = ScriptableObject.CreateInstance<GGeometry>();
                    geometry.TerrainData = this;
                    //geometry.ResetFull();
                }
                GCommon.TryAddObjectToAsset(geometry, this);
                geometry.TerrainData = this;
                return geometry;
            }
        }

        [SerializeField]
        private GShading shading;
        public GShading Shading
        {
            get
            {
                if (shading == null)
                {
                    shading = ScriptableObject.CreateInstance<GShading>();
                    shading.TerrainData = this;
                    //shading.ResetFull();
                }
                GCommon.TryAddObjectToAsset(shading, this);
                shading.TerrainData = this;
                return shading;
            }
        }

        [SerializeField]
        private GRendering rendering;
        public GRendering Rendering
        {
            get
            {
                if (rendering == null)
                {
                    rendering = ScriptableObject.CreateInstance<GRendering>();
                    rendering.TerrainData = this;
                    //rendering.ResetFull();
                }
                GCommon.TryAddObjectToAsset(rendering, this);
                rendering.TerrainData = this;
                return rendering;
            }
        }

        [SerializeField]
        private GFoliage foliage;
        public GFoliage Foliage
        {
            get
            {
                if (foliage == null)
                {
                    foliage = ScriptableObject.CreateInstance<GFoliage>();
                    foliage.TerrainData = this;
                    foliage.ResetFull();
                }
                GCommon.TryAddObjectToAsset(foliage, this);
                foliage.TerrainData = this;
                return foliage;
            }
        }

        [SerializeField]
        private GMask mask;
        public GMask Mask
        {
            get
            {
                if (mask == null)
                {
                    mask = ScriptableObject.CreateInstance<GMask>();
                    mask.TerrainData = this;
                    //mask.ResetFull();
                }
                GCommon.TryAddObjectToAsset(mask, this);
                mask.TerrainData = this;
                return mask;
            }
        }

        [SerializeField]
        [FormerlySerializedAs("generatedData")]
        private GTerrainGeneratedData geometryData;
        public GTerrainGeneratedData GeometryData
        {
            get
            {
                GGeometry geo = geometry ? geometry : Geometry; //wtf!
                if (geo.StorageMode == GGeometry.GStorageMode.SaveToAsset)
                {
                    if (geometryData == null)
                    {
                        geometryData = GCommon.GetTerrainGeneratedDataAsset(this, "GeneratedGeometry");
                    }
                    geometryData.TerrainData = this;
                    return geometryData;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                geometryData = value;
            }
        }

        public void Reset()
        {
            id = GCommon.GetUniqueID();
        }

        public void SetDirty(DirtyFlags flag)
        {
#if UNITY_EDITOR
            //EditorUtility.SetDirty(this);
            if ((flag == DirtyFlags.All || flag == DirtyFlags.Geometry) && geometry!=null)
            {
                EditorUtility.SetDirty(geometry);
            }
            if ((flag == DirtyFlags.All || flag == DirtyFlags.Shading) && shading!=null)
            {
                EditorUtility.SetDirty(shading);
            }
            if ((flag == DirtyFlags.All || flag == DirtyFlags.Rendering) && rendering!=null)
            {
                EditorUtility.SetDirty(rendering);
            }
            if ((flag == DirtyFlags.All || flag == DirtyFlags.Foliage) && foliage!=null)
            {
                EditorUtility.SetDirty(foliage);
            }
#endif
            if (shading != null)
            {
                shading.UpdateMaterials();
            }
            if (Dirty != null)
            {
                Dirty(flag);
            }

            if (GlobalDirty != null)
            {
                GlobalDirty(this, flag);
            }
        }

        /// <summary>
        /// Copy nummeric settings 
        /// </summary>
        /// <param name="des"></param>
        public void CopyTo(GTerrainData des)
        {
            Geometry.CopyTo(des.Geometry);
            Shading.CopyTo(des.Shading);
            Rendering.CopyTo(des.Rendering);
            Foliage.CopyTo(des.Foliage);
            Mask.CopyTo(des.Mask);
        }

        internal void InvokeGrassChange(Vector2 gridIndex)
        {
            if (GrassPatchChanged != null)
            {
                int index = GUtilities.To1DIndex((int)gridIndex.x, (int)gridIndex.y, Foliage.PatchGridSize);
                GrassPatchChanged.Invoke(index);
            }
        }

        internal void InvokeGrassPatchGridSizeChange()
        {
            if (GrassPatchGridSizeChanged != null)
            {
                GrassPatchGridSizeChanged.Invoke();
            }
        }

        internal void InvokeTreeChanged()
        {
            if (TreeChanged != null)
            {
                TreeChanged.Invoke();
            }
        }
        
        internal void InvokeTreePrototypeGroupChanged()
        {
            if (TreePrototypeGroupChanged != null)
            {
                TreePrototypeGroupChanged.Invoke();
            }
        }
        
        internal void InvokeGrassPrototypeGroupChanged()
        {
            if (GrassPrototypeGroupChanged != null)
            {
                GrassPrototypeGroupChanged.Invoke();
            }
        }

        public void CleanUp()
        {
            if (geometry != null && geometry.subDivisionMap != null)
            {
                GUtilities.DestroyObject(geometry.subDivisionMap);
            }
        }
    }
}
#endif
