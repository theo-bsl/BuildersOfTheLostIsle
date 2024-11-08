using UnityEngine;

public class SeedManager : Singleton<SeedManager>
{
    [Header("Seed Settings")]
    [SerializeField] private int seed = 0;
    [SerializeField] private string islandName = default;

    protected override void Awake()
    {
        ComputeSeed();
        SetSeed();
    }
    
    private void SetSeed()
    {
        PlayerPrefs.SetInt("seed", seed);
    }
    
    private void ComputeSeed()
    {
        seed = 0;
        foreach (char letter in islandName)
        {
            seed += letter;
        }
    }
}