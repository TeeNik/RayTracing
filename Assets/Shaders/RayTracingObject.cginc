struct Material
{
    float3 albedo;
    float3 specular;
    float smoothness;
    float3 emission;
};

struct MeshObject
{
    float4x4 localToWorldMatrix;
    int indicesOffset;
    int indicesCount;
    Material material;
};