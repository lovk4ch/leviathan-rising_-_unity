using UnityEditor;
using UnityEngine;

public class CrystalMesh : MonoBehaviour
{
    private void Awake()
    {
        // SaveMesh(Octahedron(5), "crystal", false, true);
    }

    public static Mesh Triangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
    {
        var normal = Vector3.Cross((vertex1 - vertex0), (vertex2 - vertex0)).normalized;
        var mesh = new Mesh
        {
            vertices = new[] { vertex0, vertex1, vertex2 },
            normals = new[] { normal, normal, normal },
            uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) },
            triangles = new[] { 0, 1, 2 }
        };
        return mesh;
    }

    public static Mesh Octahedron(float radius)
    {
        // Верх
        var v0 = new Vector3(0, -radius, 0);
        // Многоугольник посередине
        var v1 = new Vector3(-radius, 0, 0);
        var v2 = new Vector3(0, 0, -radius);
        var v3 = new Vector3(+radius, 0, 0);
        var v4 = new Vector3(0, 0, +radius);
        // Низ
        var v5 = new Vector3(0, radius, 0);

        var combine = new CombineInstance[8];
        combine[0].mesh = Triangle(v0, v1, v2);
        combine[1].mesh = Triangle(v0, v2, v3);
        combine[2].mesh = Triangle(v0, v3, v4);
        combine[3].mesh = Triangle(v0, v4, v1);
        combine[4].mesh = Triangle(v5, v2, v1);
        combine[5].mesh = Triangle(v5, v3, v2);
        combine[6].mesh = Triangle(v5, v4, v3);
        combine[7].mesh = Triangle(v5, v1, v4);

        var mesh = new Mesh();
        mesh.CombineMeshes(combine, true, false);
        return mesh;
    }

    /*[MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
    public static void SaveMeshInPlace(MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = mf.sharedMesh;
        SaveMesh(m, m.name, false, true);
    }

    [MenuItem("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
    public static void SaveMeshNewInstanceItem(MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = mf.sharedMesh;
        SaveMesh(m, m.name, true, true);
    }

    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }*/
}