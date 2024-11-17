using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSphereGeneratior : MonoBehaviour
{
    public int SphereSeed = 7658;
    public Vector2 SphereRadius = new Vector2(3.0f, 8.0f);
    public uint SpheresMax = 100;
    public float SpherePlacementRadius = 100.0f;
    
    public RayTracingSphere SpherePrefab;

    public void Start()
    {
        GenerateSpheres();
    }

    public void GenerateSpheres()
    {
        Random.InitState(SphereSeed);
        List<RayTracingSphere> spheres = new List<RayTracingSphere>();
        for (int i = 0; i < SpheresMax; ++i)
        {
            RayTracingSphere sphere = Instantiate(SpherePrefab);
            sphere.Radius = Random.Range(SphereRadius.x, SphereRadius.y);

            int tries = 0;
            while (true)
            {
                bool collided = false;
                Vector2 randomPos = Random.insideUnitCircle * SpherePlacementRadius;
                sphere.transform.position = new Vector3(randomPos.x, sphere.Radius, randomPos.y);

                foreach (var other in spheres)
                {
                    float minDist = sphere.Radius + other.Radius;
                    if (Vector3.SqrMagnitude(sphere.transform.position - other.transform.position) < minDist * minDist)
                    {
                        collided = true;
                        break;
                    }
                }

                ++tries;
                if (tries >= 1000)
                {
                    Debug.LogError("Failed try to place a sphere without colliding with others");
                    break;
                }

                if (!collided)
                {
                    break;
                }
            }

            Color color = Random.ColorHSV();
            float chance = Random.value;
            if (chance < 0.8f)
            {
                bool metal = Random.value < 0.4f;
                sphere.Material.albedo = metal ? Color.black : new Color(color.r, color.g, color.b);
                sphere.Material.specular = metal ? new Color(color.r, color.g, color.b) : Color.white * 0.04f;
                sphere.Material.specularChance = Random.value;
                sphere.Material.smoothness = Random.value;
            }
            else
            {
                sphere.Material.emission = new Color(color.r, color.g, color.b);
                sphere.Material.emissionStrength = Random.Range(5, 10);
            }
            spheres.Add(sphere);
        }
    }

}
