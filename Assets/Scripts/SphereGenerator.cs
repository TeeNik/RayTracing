using System.Collections.Generic;
using UnityEngine;

public class RandomSphereGeneratior : MonoBehaviour
{
    public int SphereSeed = 7658;
    public Vector2 SphereRadius = new Vector2(3.0f, 8.0f);
    public uint SpheresMax = 100;
    public float SpherePlacementRadius = 100.0f;

    public void GenerateSpheres()
    {
        Random.InitState(SphereSeed);
        List<Sphere> spheres = new List<Sphere>();
        for (int i = 0; i < SpheresMax; ++i)
        {
            Sphere sphere = new Sphere();
            sphere.radius = Random.Range(SphereRadius.x, SphereRadius.y);

            int tries = 0;
            while (true)
            {
                bool collided = false;
                Vector2 randomPos = Random.insideUnitCircle * SpherePlacementRadius;
                sphere.pos = new Vector3(randomPos.x, sphere.radius, randomPos.y);

                foreach (var other in spheres)
                {
                    float minDist = sphere.radius + other.radius;
                    if (Vector3.SqrMagnitude(sphere.pos - other.pos) < minDist * minDist)
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
                sphere.material.albedo = metal ? Vector3.zero : new Vector3(color.r, color.g, color.b);
                sphere.material.specular = metal ? new Vector3(color.r, color.g, color.b) : Vector3.one * 0.04f;
                sphere.material.specularChance = Random.value;
                sphere.material.smoothness = Random.value;
            }
            else
            {
                sphere.material.emission = new Vector3(color.r, color.g, color.b);
                sphere.material.emissionStrength = Random.Range(5, 10);
            }
            spheres.Add(sphere);
        }
    }

}
