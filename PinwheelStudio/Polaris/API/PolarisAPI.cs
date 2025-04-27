#if GRIFFIN_3
using Pinwheel.Griffin.Physic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin
{
    public static partial class Polaris
    {
        public static void GetVersion(ref int major, ref int minor, ref int patch)
        {
            major = GVersionInfo.Major;
            minor = GVersionInfo.Minor;
            patch = GVersionInfo.Patch;
        }

        public static float GetVersionNumber()
        {
            return GVersionInfo.Number;
        }

        public static GTerrainData CreateAndInitTerrainData(GTexturingModel texturingModel)
        {
            GTerrainData data = ScriptableObject.CreateInstance<GTerrainData>();

            if (Application.isPlaying) //Reset() only called in edit mode
            {
                data.Reset();
                data.Geometry.Reset();
                data.Shading.Reset();
                data.Rendering.Reset();
                data.Foliage.Reset();
                data.Mask.Reset();
            }

            if (texturingModel == GTexturingModel.VertexColor)
            {
                data.Geometry.AlbedoToVertexColorMode = GAlbedoToVertexColorMode.Sharp;
            }

            if (texturingModel == GTexturingModel.GradientLookup)
            {
                data.Shading.UpdateLookupTextures();
                data.Shading.SplatControlResolution = 32;
            }
            if (texturingModel == GTexturingModel.ColorMap ||
                texturingModel == GTexturingModel.VertexColor)
            {
                data.Shading.SplatControlResolution = 32;
            }
            if (texturingModel == GTexturingModel.Splat)
            {
                data.Shading.AlbedoMapResolution = 32;
                data.Shading.MetallicMapResolution = 32;
            }
            return data;
        }

        public static bool InitTerrainMaterial(Material mat, GLightingModel lightingModel, GTexturingModel texturingModel, GSplatsModel splatModel = GSplatsModel.Splats4)
        {
            Shader shader = GRuntimeSettings.Instance.terrainRendering.GetTerrainShader(GCommon.CurrentRenderPipeline, lightingModel, texturingModel, splatModel);
            if (shader != null)
            {
                mat.shader = shader;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SetTerrainMaterial(GTerrainData data, Material mat)
        {
            data.Shading.CustomMaterial = mat;
            data.Shading.UpdateMaterials();
        }

        public static GStylizedTerrain CreateTerrain(GTerrainData data)
        {
            GameObject g = new GameObject();
            GStylizedTerrain terrain = g.AddComponent<GStylizedTerrain>();
            terrain.TerrainData = data;

            return terrain;
        }

        public static GTreeCollider AttachTreeCollider(GStylizedTerrain t)
        {
            GameObject colliderGO = new GameObject("Tree Collider");
            colliderGO.transform.parent = t.transform;
            colliderGO.transform.localPosition = Vector3.zero;
            colliderGO.transform.localRotation = Quaternion.identity;
            colliderGO.transform.localScale = Vector3.one;

            GTreeCollider collider = colliderGO.AddComponent<GTreeCollider>();
            collider.Terrain = t;

            return collider;
        }

        public static GTerrainData DeepCloneTerrainData(GTerrainData src)
        {
            GTerrainData des = GTerrainData.CreateInstance<GTerrainData>();
            src.CopyTo(des);
            src.GeometryData = null;

            des.Geometry.HeightMap.SetPixelData(src.Geometry.HeightMap.GetRawTextureData(), 0, 0);
            if (src.Shading.HasAlbedoMap)
            {
                des.Shading.AlbedoMap.SetPixelData(src.Shading.AlbedoMap.GetRawTextureData(), 0, 0);
            }
            if (src.Shading.HasMetallicMap)
            {
                des.Shading.MetallicMap.SetPixelData(src.Shading.MetallicMap.GetRawTextureData(), 0, 0);
            }
            if (src.Shading.SplatControlMapCount > 0)
            {
                for (int i = 0; i < src.Shading.SplatControlMapCount; ++i)
                {
                    Texture2D srcControl = src.Shading.GetSplatControl(i);
                    Texture2D desControl = des.Shading.GetSplatControl(i);
                    desControl.SetPixelData(srcControl.GetRawTextureData(), 0, 0);
                }
            }
            des.Foliage.TreeInstances.AddRange(src.Foliage.TreeInstances);
            des.Foliage.AddGrassInstances(src.Foliage.GetGrassInstances());
            if (src.Mask.HasMaskMap)
            {
                des.Mask.MaskMap.SetPixelData(src.Mask.MaskMap.GetRawTextureData(), 0, 0);
            }

            return des;
        }

        public static int GetDominantTextureIndex(GStylizedTerrain t, Vector3 worldPos)
        {
            Vector2 uv = t.WorldPointToUV(worldPos);
            int splatControlCount = t.TerrainData.Shading.SplatControlMapCount;
            float max = -1;
            int index = 0;
            for (int ti = 0; ti < splatControlCount; ++ti)
            {
                Texture2D controlMap = t.TerrainData.Shading.GetSplatControl(ti);
                Color c = controlMap.GetPixelBilinear(uv.x, uv.y);
                for (int ci = 0; ci < 4; ++ci)
                {
                    if (c[ci] > max)
                    {
                        max = c[ci];
                        index = ti * 4 + ci;
                    }
                }
            }
            return index;
        }

        public static Vector2 WorldPositionToUV(GStylizedTerrain t, Vector3 worldPos)
        {
            return t.WorldPointToUV(worldPos);
        }

        public static Texture2D GetHeightMap(GTerrainData data)
        {
            return data.Geometry.HeightMap;
        }

        public static Color EncodeHeightMapSample(float height01, float additionalSubdiv01, float visibility01)
        {
            Vector4 enc = GCommon.EncodeTerrainHeight(height01);
            enc.z = additionalSubdiv01;
            enc.w = visibility01;
            return enc;
        }

        public static void DecodeHeightMapSample(Color sample, ref float height01, ref float subdiv01, ref float visibility01)
        {
            height01 = GCommon.DecodeTerrainHeight(new Vector2(sample.r, sample.g));
            subdiv01 = sample.b;
            visibility01 = sample.a;
        }

        public static void UpdateTerrainMesh(GStylizedTerrain terrain, IEnumerable<Rect> regions01 = null)
        {
            terrain.TerrainData.Geometry.SetRegionDirty(regions01);
            terrain.TerrainData.SetDirty(GTerrainData.DirtyFlags.Geometry);
            terrain.TerrainData.Geometry.ClearDirtyRegions();
        }

        public static Texture2D GetAlbedoMap(GTerrainData data)
        {
            return data.Shading.AlbedoMap;
        }

        public static Texture2D GetAlbedoMapForReadingBilinear(GTerrainData data)
        {
            return data.Shading.AlbedoMapOrDefault;
        }

        public static Texture2D GetMetallicMap(GTerrainData data)
        {
            return data.Shading.MetallicMap;
        }

        public static Texture2D GetMetallicMapForReadingBilinear(GTerrainData data)
        {
            return data.Shading.MetallicMapOrDefault;
        }

        public static int GetControlMapCount(GTerrainData data)
        {
            return data.Shading.SplatControlMapCount;
        }

        public static Texture2D GetControlMap(GTerrainData data, int index)
        {
            return data.Shading.GetSplatControl(index);
        }

        public static Texture2D GetControlMapForReadingBilinear(GTerrainData data, int index)
        {
            return data.Shading.GetSplatControlOrDefault(index);
        }

        public static int GetTreeInstanceCount(GTerrainData data)
        {
            return data.Foliage.TreeInstances.Count;
        }

        public static void AddTreeInstance(GTerrainData data, IEnumerable<GTreeInstance> instances)
        {
            data.Foliage.AddTreeInstances(instances);
        }

        public static void RemoveTreeInstance(GTerrainData data, System.Predicate<GTreeInstance> condition)
        {
            data.Foliage.RemoveTreeInstances(condition);
        }

        public static void ClearTreeInstances(GTerrainData data)
        {
            data.Foliage.ClearTreeInstances();
        }

        public static void AddGrassInstances(GTerrainData data, List<GGrassInstance> instances)
        {
            data.Foliage.AddGrassInstances(instances);
        }

        public static void RemoveGrassInstance(GTerrainData data, System.Predicate<GGrassInstance> condition)
        {
            GGrassPatch[] patches = data.Foliage.GrassPatches;
            foreach(GGrassPatch p in patches)
            {
                p.RemoveInstances(condition);
            }
        }

        public static void ClearGrassInstances(GTerrainData data)
        {
            data.Foliage.ClearGrassInstances();
        }

        public static void UpdateMaterial(GTerrainData data)
        {
            data.Shading.UpdateMaterials();
        }
    }
}
#endif