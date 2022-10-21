using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObject/Player Settings", order = 1)]
public class PlayerSettings : ScriptableObject
{
    [Header("Gameplay Settings")]
    public bool quickCast = false;

    [Header("Camera Settings")]
    [Range(10f, 60f)]
    public float cameraPanningSpeed = 40f;
    [Range(0.1f, 1f)]
    public float cameraFloatiness = 0.5f;
    [Range(1f, 20f)]
    public float cameraPanningThreshold = 20f;
    [Range(0.25f, 1f)]
    public float cameraZoomSensitivity = 0.25f;

    [Header("Movement Hotkeys")]
    public KeyCode movePlayer = KeyCode.Mouse1;

    [Header("Camera Hotkeys")]
    public KeyCode lockCamera = KeyCode.L;
    public KeyCode recenterCamera = KeyCode.Space;

    [Header("Ability Hotkeys")]
    public KeyCode damageAbility = KeyCode.Q;
    public KeyCode stunAbility = KeyCode.W;
    public KeyCode slowAbility = KeyCode.E;
    public KeyCode ultimateAbility = KeyCode.R;
}