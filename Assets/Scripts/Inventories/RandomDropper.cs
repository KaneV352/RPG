using RPG.Inventories;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        // CONFIG DATA
        [Tooltip("How far can the pickups be scattered from the dropper.")]
        [SerializeField] float scatterDistance = 1;
        [SerializeField] DropLibrary dropLibrary;

        // CONSTANTS
        const int Attempts = 30;

        public void RandomDrop()
        {
            var baseStats = GetComponent<BaseStats>();

            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach (var drop in drops)
            {
                DropItem(drop.Item, drop.Number);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            // We might need to try more than once to get on the NavMesh
            for (int i = 0; i < Attempts; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return transform.position;
        }
    }
}