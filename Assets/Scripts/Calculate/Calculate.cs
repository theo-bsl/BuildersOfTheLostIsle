using UnityEngine;

namespace Calcul
{
    public static class Calculate
    {
        /*public static float Distance(Vector2 a, Vector2 b, float powerDistance = 1f)
        {
            // distance de Minkowski
            return Mathf.Pow(
                Mathf.Pow(Mathf.Abs(a.x - b.x), powerDistance) + 
                Mathf.Pow(Mathf.Abs(a.y - b.y), powerDistance), 1f / powerDistance);
        }*/
        
        public static float Distance(Vector2 a, Vector2 b, float powerDistance = 1f)
        {
            // Calcul des différences en valeur absolue pour chaque axe
            float deltaX = Mathf.Abs(a.x - b.x);
            float deltaY = Mathf.Abs(a.y - b.y);

            // Élève les différences à la puissance spécifiée
            float deltaXPowered = Mathf.Pow(deltaX, powerDistance);
            float deltaYPowered = Mathf.Pow(deltaY, powerDistance);

            // Somme des puissances
            float sumPoweredDeltas = deltaXPowered + deltaYPowered;

            // Racine de la somme pour obtenir la distance de Minkowski
            float distance = Mathf.Pow(sumPoweredDeltas, 1f / powerDistance);

            return distance;
        }

    
        public static float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            // Calcul du facteur de conversion
            float scale = (newMax - newMin) / (oldMax - oldMin);
        
            // Conversion de la valeur dans le nouvel intervalle
            float newValue = newMin + (value - oldMin) * scale;
        
            // Clamp la valeur dans les bornes du nouvel intervalle
            return Mathf.Max(newMin, Mathf.Min(newMax, newValue));
        }

        public static float PerlinNoise(float x, float y, float frequency, int octaveCount, float persistence, float lacunarity, long seed)
        {
            float value = 0f;
            float amplitude = 1f;
            float currentFrequency = frequency;

            for (int i = 0; i < octaveCount; i++)
            {
                float perlinValue = Mathf.PerlinNoise(x * currentFrequency + seed, y * currentFrequency + seed) * amplitude;
                if (perlinValue == 0 || Mathf.Approximately(perlinValue, 1f)) Debug.Log("Perlin value is 0 or 1");
                value += perlinValue;
                currentFrequency *= lacunarity;
                amplitude *= persistence;
            }
            return value;
        }
        
        public static float GetHeight(Vector3 position, LayerMask layerMask)
        {
            if (Physics.Raycast(position + Vector3.up * 100, Vector3.down, out RaycastHit hit, 200, layerMask.value))
            {
                return hit.point.y;
            }

            return 0;
        }
    }
}
