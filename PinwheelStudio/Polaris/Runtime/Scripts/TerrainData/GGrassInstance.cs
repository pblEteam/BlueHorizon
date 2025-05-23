#if GRIFFIN
using UnityEngine;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public struct GGrassInstance
    {
        [SerializeField]
        public int prototypeIndex;
        public int PrototypeIndex
        {
            get
            {
                return prototypeIndex;
            }
            set
            {
                prototypeIndex = value;
            }
        }

        [SerializeField]
        public Vector3 position;
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        [SerializeField]
        public Quaternion rotation;
        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        [SerializeField]
        public Vector3 scale;
        public Vector3 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        public static GGrassInstance Create(int prototypeIndex)
        {
            GGrassInstance instance = new GGrassInstance();
            instance.PrototypeIndex = prototypeIndex;
            instance.Position = Vector3.zero;
            instance.Rotation = Quaternion.identity;
            instance.Scale = Vector3.one;
            return instance;
        }
    }
}
#endif
