using UnityEngine;

public class AmbientArea : MonoBehaviour
{
    public AudioClip audioClip;
    [Range(0, 1)] public float volume = .5f;

    private AudioSource audioSource;
    private BoxCollider area;
    private readonly float followSpeed = .5f;
    private bool isActive = false;

    [SerializeField] private Color boxColor = new Color(0, 1, 0, .5f);

    private void Awake()
    {
        area = GetComponent<BoxCollider>();
        area.isTrigger = true;

        string viewName = "Ambient Audio Source:" + audioClip.name;
        GameObject go = new GameObject(viewName);
        go.transform.position = area.transform.position;

        audioSource = go.AddComponent<AudioSource>();
        audioSource.transform.SetParent(this.transform);
        audioSource.dopplerLevel = 0;
        audioSource.spread = 0;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 8;
        audioSource.loop = true;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.spatialBlend = 1;
        audioSource.clip = audioClip;
    }

    public void SetActive(bool active)
    {
        isActive = active;
        audioSource.enabled = active;
    }

    public void UpdateAmbientAreaAudio(Vector3 playerPos, float masterVolume)
    {
        if (!isActive) return;

        audioSource.volume = volume * masterVolume;

        Vector3 target = area.ClosestPoint(playerPos);
        Vector3 current = audioSource.transform.position;
        audioSource.transform.position = Vector3.Lerp(current, target, followSpeed);
    }

    private void OnDrawGizmos()
    {
        if (area == null)
        {
            area = GetComponent<BoxCollider>();
            return;
        }

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        Gizmos.color = boxColor;
        Gizmos.DrawCube(area.center, area.size);

        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(area.center, area.size);

        Gizmos.matrix = oldMatrix;
        Gizmos.color = Color.white;
    }
}