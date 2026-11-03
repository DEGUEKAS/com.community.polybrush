
using UnityEngine;
using UnityEngine.Polybrush;

namespace UnityEditor.Polybrush
{
    [InitializeOnLoad]
    static class HierarchyChanged
    {
        static HierarchyChanged()
        {
            EditorApplication.hierarchyChanged += () =>
            {
                foreach (var gameObject in Selection.gameObjects)
                {
                    var mesh = Util.GetMesh(gameObject);
                    long id = EditableObject.GetMeshId(mesh);

                    // if the mesh is an instance managed by polybrush check that it's not a duplicate.
                    if (id != EditableObject.k_InvalidMeshId)
                    {
                        // Everything is compared as long: GetMeshId() returns long so the same
                        // comparisons compile under both the legacy int API and the 64-bit EntityId API.
                        if (id != EditableObject.GetObjectId(gameObject)
                            && EditableObject.ObjectFromId(id) != null)
                        {
                            mesh = PolyMeshUtility.DeepCopy(mesh);
                            mesh.name = EditableObject.GetMeshInstanceName(gameObject);

                            var mf = gameObject.GetComponent<MeshFilter>();
                            var sf = gameObject.GetComponent<SkinnedMeshRenderer>();
                            var polyMesh = gameObject.GetComponent<PolybrushMesh>();

                            if (polyMesh != null)
                            {
                                polyMesh.SetMesh(mesh);
                                PrefabUtility.RecordPrefabInstancePropertyModifications(polyMesh);
                            }
                            else if (mf != null)
                                mf.sharedMesh = mesh;
                            else if (sf != null)
                                sf.sharedMesh = mesh;
                        }
                    }
                }
            };
        }
    }
}
