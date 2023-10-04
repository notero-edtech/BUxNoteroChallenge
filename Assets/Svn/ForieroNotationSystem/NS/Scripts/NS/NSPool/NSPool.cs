/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using PathologicalGames;
using UnityEngine;

[RequireComponent(typeof(SpawnPool))]
public class NSPool : MonoBehaviour
{
    public NSPoolSettings.Pool pool = NSPoolSettings.Pool.Undefined;
    private SpawnPool spawnPool;
    private NSPoolBank poolBank;

    private void Awake()
    {
        spawnPool = GetComponent<SpawnPool>();
        poolBank = NSPoolSettings.GetPoolBank(pool);

        if (poolBank == null) return;
        
        for (int i = 0; i < this.poolBank.prefabPool.Count; i++)
        {
            if (this.poolBank.prefabPool[i].prefab == null)
            {
                Debug.LogWarning(string.Format("Initialization Warning: Pool '{0}' " +
                                               "contains a PrefabPool with no prefab reference. Skipping.",
                    this.pool));
                continue;
            }

            // Init the PrefabPool's GameObject cache because it can't do it.
            //   This is only needed when created by the inspector because the constructor
            //   won't be run.
            this.poolBank.prefabPool[i].inspectorInstanceConstructor();
            spawnPool.CreatePrefabPool(this.poolBank.prefabPool[i]);
        }
    }
}
