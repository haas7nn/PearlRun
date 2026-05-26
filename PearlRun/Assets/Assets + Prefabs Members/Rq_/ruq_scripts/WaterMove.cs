using UnityEngine;

public class WaterWave : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;

    public float waveSpeed = 1f;
    public float waveHeight = 0.2f;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
    }

    void Update()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];

            v.y = Mathf.Sin(Time.time * waveSpeed + v.x + v.z) * waveHeight;

            vertices[i] = v;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}