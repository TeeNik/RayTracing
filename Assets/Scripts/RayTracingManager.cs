using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RayTracingManager : MonoBehaviour
{
    public ComputeShader RayTracingShader;
    public Texture SkyboxTexture;
    public Texture CubeTexture;
    public Light DirectionalLight;

    public BloomEffect BloomEffect;
    public FogEffect FogEffect;

    [Header("Raytracing Settings")] 
    public int RayBouncesCount = 8;
    public int RaysPerPixelCount = 1;
    
    [Header("Environment")] 
    public Color HorizonSkyColor = Color.cyan;
    public Color ZenithSkyColor = Color.white;
    public float EnvLightIntensity = 1.0f;
    
    private RenderTexture _target;
    private RenderTexture _converged;
    private Camera _camera;
    private int _currentSample = 0;
    private Material _addMaterial;

    private static bool _meshObjectsNeedRebuilding = false;
    private static List<RayTracingObject> _rayTracingObjects = new List<RayTracingObject>();
    
    private Dictionary<RayTracingObject, BVH> _bvhByObject = new();

    struct MeshInfo
    {
        public Matrix4x4 localToWorldMatrix;
        public Matrix4x4 worldToLocalMatrix;
        public int nodeOffset;
        public int triangleOffset;
        public RayTracingMaterial material;
    }

    private static List<MeshInfo> _meshObjects = new List<MeshInfo>();
    private static List<Vector3> _vertices = new List<Vector3>();
    private static List<int> _indices = new List<int>();
    
    private ComputeBuffer _meshObjectBuffer;
    private ComputeBuffer _vertexBuffer;
    private ComputeBuffer _indexBuffer;
    private ComputeBuffer _spheresBuffer;

    private static List<Vector2> _uvs = new List<Vector2>();
    private ComputeBuffer _uvBuffer;
    
    private ComputeBuffer _nodesBuffer;
    private ComputeBuffer _trianglesBuffer;

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

    public static void RequestObjectsRebuild()
    {
        _meshObjectsNeedRebuilding = true;
    }

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
        
        _uvBuffer?.Release();
        
        _nodesBuffer.Release();
        _trianglesBuffer.Release();
    }

    private void SetUpScene()
    {
        
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
        
        RayTracingShader.SetInt("_RayBouncesCount", RayBouncesCount);
        RayTracingShader.SetInt("_RaysPerPixelCount", RaysPerPixelCount);
        
        RayTracingShader.SetVector("_HorizonSkyColor", HorizonSkyColor);
        RayTracingShader.SetVector("_ZenithSkyColor", ZenithSkyColor);
        RayTracingShader.SetFloat("_EnvLightIntensity", EnvLightIntensity);

        Vector3 LightDir = DirectionalLight.transform.forward;
        RayTracingShader.SetVector("_DirectionalLight", new Vector4(LightDir.x, LightDir.y, LightDir.z, DirectionalLight.intensity));

        SetComputeBuffer("_Spheres", _spheresBuffer);
        SetComputeBuffer("_MeshObjects", _meshObjectBuffer);
        SetComputeBuffer("_Vertices", _vertexBuffer);
        SetComputeBuffer("_Indices", _indexBuffer);
        
        SetComputeBuffer("_Nodes", _nodesBuffer);
        SetComputeBuffer("_Triangles", _trianglesBuffer);
        
        SetComputeBuffer("_Uvs", _uvBuffer);
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

        if (BloomEffect)
        {
            var bloomRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, _converged.format);
            BloomEffect.ApplyBloomToRenderTexture(_converged, bloomRT);

           //if (FogEffect)
           //{
           //    var fogRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, _converged.format);
           //    FogEffect.ApplyFogToRenderTexture(bloomRT, fogRT);
           //    Graphics.Blit(fogRT, destination);
           //    RenderTexture.ReleaseTemporary(fogRT);
           //}
            
            Graphics.Blit(bloomRT, destination);
            RenderTexture.ReleaseTemporary(bloomRT);
        }
        else
        {
            Graphics.Blit(_converged, destination);
        }
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
        
        _uvs.Clear();
        
        List<Sphere> spheres = new List<Sphere>();
        foreach (var rayTracingSphere in Resources.FindObjectsOfTypeAll<RayTracingSphere>())
        {
            if (rayTracingSphere.gameObject.activeSelf)
            {
                Sphere sphere = new Sphere();
                sphere.pos = rayTracingSphere.transform.position;
                sphere.radius = rayTracingSphere.Radius;
                sphere.material = rayTracingSphere.Material.GetMaterialForShader();
                spheres.Add(sphere);
            }
        }
        ShaderUtils.CreateComputeBuffer(ref _spheresBuffer, spheres);

        List<NodeData> AllNodes = new();
        List<TriangleData> AllTriangles = new();

        foreach (RayTracingObject obj in _rayTracingObjects)
        {
            Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            
            int firstVertex = _vertices.Count;
            _vertices.AddRange(mesh.vertices);

            int firstIndex = _indices.Count;
            var indices = mesh.GetIndices(0);
            _indices.AddRange(indices.Select(index => index + firstVertex));

            var uvs = mesh.uv.Length > 0 ? mesh.uv : new Vector2[_vertices.Count];
            _uvs.AddRange(uvs);

            if (!_bvhByObject.ContainsKey(obj))
            {
                _bvhByObject.Add(obj, new BVH(mesh.vertices, mesh.triangles));
            }
            BVH bvh = _bvhByObject[obj];
            
            _meshObjects.Add(new MeshInfo()
            {
                localToWorldMatrix = obj.transform.localToWorldMatrix,
                worldToLocalMatrix = obj.transform.worldToLocalMatrix,
                nodeOffset = AllNodes.Count,
                triangleOffset = AllTriangles.Count,
                material = obj.Material.GetMaterialForShader(),
            });
            
            AllNodes.AddRange(bvh.NodesForBuffer);
            AllTriangles.AddRange(bvh.TrianglesForBuffer);
        }

        ShaderUtils.CreateComputeBuffer(ref _meshObjectBuffer, _meshObjects);
        ShaderUtils.CreateComputeBuffer(ref _vertexBuffer, _vertices);
        ShaderUtils.CreateComputeBuffer(ref _indexBuffer, _indices);
        
        ShaderUtils.CreateComputeBuffer(ref _uvBuffer, _uvs);
        
        ShaderUtils.CreateComputeBuffer(ref _nodesBuffer, AllNodes);
        ShaderUtils.CreateComputeBuffer(ref _trianglesBuffer, AllTriangles);
    }

    private void SetComputeBuffer(string name, ComputeBuffer buffer)
    {
        if (buffer != null)
        {
            RayTracingShader.SetBuffer(0, name, buffer);
        }
    }

}
