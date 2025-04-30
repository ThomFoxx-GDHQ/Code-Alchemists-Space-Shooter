using UnityEngine;

[CreateAssetMenu(fileName = "WaveEnemySpawns", menuName = "Scriptable Objects/WaveEnemySpawns")]
public class WaveEnemySpawns : ScriptableObject
{
    public int[] spawnWeights;

    public float[] SpawnRates()
    {
        float[] rates = new float[spawnWeights.Length];
        int total = 0;

        foreach (int i in spawnWeights)
            total += i;

        for (int j = 0; j < spawnWeights.Length; j++)
            rates[j] = (float)spawnWeights[j] / (float)total;

        return rates;
    }
}
