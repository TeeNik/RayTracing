static const int CheckerPattern = 1;
static const int HasTexture = 2;
static const int Invisible = 3;

struct Material
{
    float3 albedo;
    float3 specular;
    float smoothness;
    float3 emission;
    float emissionStrength;
    float specularChance;
    int flag;
};

struct Sphere
{
    float3 pos;
    float radius;
    Material material;
};

struct MeshObject
{
    float4x4 localToWorldMatrix;
    float4x4 worldToLocalMatrix;
    int indicesOffset;
    int indicesCount;
    Material material;
    float3 boundMin;
    float3 boundMax;
};

struct Ray
{
    float3 origin;
    float3 dir;
    float3 energy;
};

Ray CreateRay(float3 origin, float3 dir)
{
    Ray ray;
    ray.origin = origin;
    ray.dir = dir;
    ray.energy = float3(1, 1, 1);
    return ray;
}

Ray CreateCameraRay(float2 uv, in float4x4 cameraToWorld, in float4x4 cameraInverseProjection)
{
    float3 origin = mul(cameraToWorld, float4(0, 2, 0, 1)).xyz;
    float3 dir = mul(cameraInverseProjection, float4(uv, 0, 1)).xyz;
    dir = mul(cameraToWorld, float4(dir, 0)).xyz;
    dir = normalize(dir);
    return CreateRay(origin, dir);
}

struct RayHit
{
    float3 pos;
    float dist;
    float3 normal;
    Material material;
    float2 uv;
};

Material CreateMaterial()
{
    Material material;
    material.albedo = float3(0, 0, 0);
    material.specular = float3(0, 0, 0);
    material.smoothness = 0;
    material.emission = float3(0, 0, 0);
    material.emissionStrength = 0;
    material.specularChance = 0;
    material.flag = 0;
    return material;
}

RayHit CreateRayHit()
{
    RayHit hit;
    hit.pos = float3(0, 0, 0);
    hit.dist = 1.#INF;
    hit.normal = float3(0, 0, 0);
    hit.material = CreateMaterial();
    hit.uv = float2(0,0);
    return hit;
}

struct BVHNode
{
    float3 BoundsMin;
    float3 BoundsMax;
    int TriangleIndex;
    int TriangleCount;
    int ChildIndex;
};

struct BVHTriangle
{
    float3 VertexA;
    float3 VertexB;
    float3 VertexC;
};