using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerSpawnPoint : MonoBehaviour
{
    const string Text = "PLAYER";

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;
        Gizmos.color = new Color(0, 0, 255, 128);
        Gizmos.DrawSphere(position, 1f);
        Vector3 labelPosition = new(position.x, position.y + 2f, position.z);
        Handles.Label(labelPosition, Text);
    }
}