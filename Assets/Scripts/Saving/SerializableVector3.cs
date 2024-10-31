using UnityEngine;

namespace RPG.Saving
{
    /// <summary>
    /// A `System.Serializable` wrapper for the `Vector3` class.
    /// </summary>
    [System.Serializable]
    public class SerializableVector3
    {
        float _x, _y, _z;

        /// <summary>
        /// Copy over the state from an existing Vector3.
        /// </summary>
        public SerializableVector3(Vector3 vector)
        {
            _x = vector.x;
            _y = vector.y;
            _z = vector.z;
        }

        /// <summary>
        /// Create a Vector3 from this class' state.
        /// </summary>
        /// <returns></returns>
        public Vector3 ToVector()
        {
            return new Vector3(_x, _y, _z);
        }
    }
}