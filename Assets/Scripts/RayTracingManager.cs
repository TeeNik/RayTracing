using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracingManager : MonoBehaviour
{
    public ComputeShader RayTracingShader;
    public Texture SkyboxTexture;
    public Light DirectionalLight;

    public Vector2 SphereRadius = new Vector2(3.0f, 8.0f);
    public uint SpheresMax = 100;
    public float SpherePlacementRadius = 100.0f;
    private ComputeBuffer _spheresBuffer;

    private RenderTexture _target;
    private Camera _camera;
    private int _currentSample = 0;
    private Material _addMaterial;

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
        if (_spheresBuffer != null)
        {
            _spheresBuffer.Release();
        }
    }

    private void SetUpScene()
    {
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
            bool metal = Random.value < 0.5f;
            sphere.albedo = metal ? Vector3.zero : new Vector3(color.r, color.g, color.b);
            sphere.specular = metal ? new Vector3(color.r, color.g, color.b) : Vector3.one * 0.04f;
            spheres.Add(sphere);
        }

        _spheresBuffer = new ComputeBuffer(spheres.Count, 40);
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
        SetShaderParameters();
        Render(destination);
    }

    private void SetShaderParameters()
    {
        RayTracingShader.SetBuffer(0, "_Spheres", _spheresBuffer);
        RayTracingShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
        RayTracingShader.SetVector("_PixelOffset", new Vector2(Random.value, Random.value));
        RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);

        Vector3 LightDir = DirectionalLight.transform.forward;
        RayTracingShader.SetVector("_DirectionalLight", new Vector4(LightDir.x, LightDir.y, LightDir.z, DirectionalLight.intensity));

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

        Graphics.Blit(_target, destination, _addMaterial);
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            if (_target != null)
            {
                _target.Release();
            }

            _target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat,
                RenderTextureReadWrite.Linear);

            _target.enableRandomWrite = true;
            _target.Create();
        }
    }
    
}
