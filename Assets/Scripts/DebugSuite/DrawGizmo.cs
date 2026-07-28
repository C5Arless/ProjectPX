using UnityEngine;

public class DrawGizmo : MonoBehaviour {
    [SerializeField] public Shapes shape;
    [SerializeField] public Color color;
    [SerializeField] public bool wireFrame;
    [SerializeField] public bool hasLabel;
    [SerializeField] public Mesh mesh;

    private void OnDrawGizmos() {
        Gizmos.color = color;

        if (hasLabel) UnityEditor.Handles.Label(transform.position, transform.name);
        
        switch (shape) {
            case Shapes.Sphere: {
                if (wireFrame) Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
                else Gizmos.DrawSphere(transform.position, transform.localScale.x);
                break;
            }
            case Shapes.Cube: {
                if (wireFrame) Gizmos.DrawWireCube(transform.position, transform.localScale);
                else Gizmos.DrawCube(transform.position, transform.localScale);
                break;
            }
            case Shapes.Mesh: {
                if (mesh == null) {
                    if (wireFrame) Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
                    else Gizmos.DrawSphere(transform.position, transform.localScale.x);
                } else {
                    if (wireFrame) Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation, transform.localScale);
                    else Gizmos.DrawMesh(mesh, transform.position, transform.rotation, transform.localScale);
                }
                break;
            }
        }
    }
}
