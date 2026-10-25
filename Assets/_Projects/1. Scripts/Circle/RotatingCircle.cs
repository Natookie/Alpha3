using UnityEngine;

public class RotatingCircle : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public Vector3 rotationAxis = Vector3.forward;

    void Update() => transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
}