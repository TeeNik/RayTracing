using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RayTracingManager : MonoBehaviour
{
    public ComputeShader RayTracingShader;
    public Texture SkyboxTexture;
    public Light DirectionalLight;

    [Header("Scene SetUp")] 
    public int SphereSeed = 7658;
    public Vector2 SphereRadius = new Vector2(3.0f, 8.0f);
    public uint SpheresMax = 100;
    public float SpherePlacementRadius = 100.0f;
    private ComputeBuffer _spheresBuffer;
    private const int SphereStructSize = 56;

    private RenderTexture _target;
    private RenderTexture _converged;
    private Camera _camera;
    private int _currentSample = 0;
    private Material _addMaterial;

    private static bool _meshObjectsNeedRebuilding = false;
    private static List<RayTracingObject> _rayTracingObjects = new List<RayTracingObject>();

    public static void RegisterObject(RayTracingObject obj)
    {
        _rayTracingObjects.Add(obj);
        _meshObjectsNeedRebuilding = true;
    }

    public static void UnregisterObject(RayTracingObject obj)
    {
        _rayTracingObjects.Remove(obj);
        _meshObjectsNeedRebuilding = true;
    }

    struct MeshInfo
    {
        public Matrix4x4 localToWorldMatrix;
        public int indicesOffset;
        public int indicesCount;
        public RayTracingMaterial material;
    }

    private static List<MeshInfo> _meshObjects = new List<MeshInfo>();
    private static List<Vector3> _vertices = new List<Vector3>();
    private static List<int> _indices = new List<int>();
    private ComputeBuffer _meshObjectBuffer;
    private ComputeBuffer _vertexBuffer;
    private ComputeBuffer _indexBuffer;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        _currentSample = 0;
        SetUpScene();
    }

    private void OnDisable()
    {
        _spheresBuffer?.Release();
        _meshObjectBuffer?.Release();
        _vertexBuffer?.Release();
        _indexBuffer?.Release();
    }

    private void SetUpScene()
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
                sphere.albedo = metal ? Vector3.zero : new Vector3(color.r, color.g, color.b);
                sphere.specular = metal ? new Vector3(color.r, color.g, color.b) : Vector3.one * 0.04f;
                sphere.smoothness = Random.value;
            }
            else
            {
                Color emission = Random.ColorHSV(0, 1, 0, 1, 3.0f, 8.0f);
                sphere.emission = new Vector3(emission.r, emission.g, emission.b);
            }
            spheres.Add(sphere);
        }

        foreach (var Sphere in Resources.FindObjectsOfTypeAll<RayTracingSphere>())
        {
            Sphere sphere = new Sphere();
            sphere.pos = Sphere.transform.position;
            sphere.radius = Sphere.Radius;
            sphere.albedo = Sphere.Material.albedo;
            sphere.specular = Sphere.Material.specular;
            sphere.smoothness = Sphere.Material.smoothness;
            sphere.emission = Sphere.Material.emission;
            spheres.Add(sphere);
        }

        _spheresBuffer = new ComputeBuffer(spheres.Count, SphereStructSize);
        _spheresBuffer.SetData(spheres);
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            _currentSample = 0;
            transform.hasChanged = false;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RebuildMeshObjectBuffers();
        SetShaderParameters();
        Render(destination);
    }

    private void SetShaderParameters()
    {
        RayTracingShader.SetFloat("_Seed", Random.value);
        RayTracingShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
        RayTracingShader.SetVector("_PixelOffset", new Vector2(Random.value, Random.value));
        RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);

        Vector3 LightDir = DirectionalLight.transform.forward;
        RayTracingShader.SetVector("_DirectionalLight", new Vector4(LightDir.x, LightDir.y, LightDir.z, DirectionalLight.intensity));

        SetComputeBuffer("_Spheres", _spheresBuffer);
        SetComputeBuffer("_MeshObjects", _meshObjectBuffer);
        SetComputeBuffer("_Vertices", _vertexBuffer);
        SetComputeBuffer("_Indices", _indexBuffer);
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            if (_target != null)
            {
                _target.Release();
            }

            if (_converged != null)
            {
                _converged.Release();
            }

            _target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat,
                RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();

            _converged = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat,
                RenderTextureReadWrite.Linear);
            _converged.enableRandomWrite = true;
            _converged.Create();

            _currentSample = 0;
        }
    }

    private void Render(RenderTexture destination)
    {
        InitRenderTexture();

        RayTracingShader.SetTexture(0, "Result", _target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RayTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        if (_addMaterial == null)
        {
            _addMaterial = new Material(Shader.Find("Hidden/AddSampleShader"));
        }
        _addMaterial.SetFloat("_Sample", _currentSample);
        ++_currentSample;

        Graphics.Blit(_target, _converged, _addMaterial);
        Graphics.Blit(_converged, destination);
    }

    private void RebuildMeshObjectBuffers()
    {
        if (!_meshObjectsNeedRebuilding)
        {
            return;
        }

        _meshObjectsNeedRebuilding = false;
        _currentSample = 0;

        _meshObjects.Clear();
        _vertices.Clear();
        _indices.Clear();

        foreach (RayTracingObject obj in _rayTracingObjects)
        {
            Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;

            int firstVertex = _vertices.Count;
            _vertices.AddRange(mesh.vertices);

            int firstIndex = _indices.Count;
            var indices = mesh.GetIndices(0);
            _indices.AddRange(indices.Select(index => index + firstVertex));

            _meshObjects.Add(new MeshInfo()
            {
                localToWorldMatrix = obj.transform.localToWorldMatrix,
                indicesOffset = firstIndex,
                indicesCount = indices.Length,
                material = obj.Material
            });
        }

        ShaderUtils.CreateComputeBuffer(ref _meshObjectBuffer, _meshObjects);
        ShaderUtils.CreateComputeBuffer(ref _vertexBuffer, _vertices);
        ShaderUtils.CreateComputeBuffer(ref _indexBuffer, _indices);
    }

    private void SetComputeBuffer(string name, ComputeBuffer buffer)
    {
        if (buffer != null)
        {
            RayTracingShader.SetBuffer(0, name, buffer);
        }
    }

}
