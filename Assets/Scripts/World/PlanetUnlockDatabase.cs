using UnityEngine;

[CreateAssetMenu(fileName = "PlanetUnlockDatabase", menuName = "Progression/Planet Unlock Database")]
public class PlanetUnlockDatabase : ScriptableObject
{
    public PlanetUnlockRule[] rules;
}
