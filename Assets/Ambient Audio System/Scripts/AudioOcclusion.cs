using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))] // Require a Low Pass Filter component
public class AudioOcclusion : MonoBehaviour
{
    [Header("Occlusion Settings")]
    [Tooltip("LayerMask for objects that can occlude sound.")]
    public LayerMask occlusionLayerMask;

    [Tooltip("Maximum distance to check for occlusion.")]
    public float maxOcclusionDistance = 20f;

    [Tooltip("Volume reduction when fully occluded.")]
    [Range(0, 1)]
    public float occlusionVolumeReduction = 0.5f;

    [Tooltip("Smoothness of volume transition when occlusion changes.")]
    public float volumeSmoothness = 5f;

    [Header("Low Pass Filter Settings")]
    [Tooltip("Cutoff frequency when not occluded (full sound).")]
    public float fullFrequency = 22000f; // Default high frequency (no filter)

    [Tooltip("Cutoff frequency when fully occluded (muffled sound).")]
    public float occludedFrequency = 1000f; // Lower frequency for muffled sound

    [Tooltip("Smoothness of frequency transition when occlusion changes.")]
    public float frequencySmoothness = 5f;

    private AudioSource audioSource;
    private AudioLowPassFilter lowPassFilter;
    private Transform playerTransform;
    private float originalVolume;
    private float targetVolume;
    private float targetFrequency;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
        originalVolume = audioSource.volume;
        targetVolume = originalVolume;
        targetFrequency = fullFrequency;

        // Find the player (assuming it has a "Player" tag)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found. Make sure the player has the 'Player' tag.");
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Calculate direction and distance to the player
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Only check occlusion if the player is within the max distance
        if (distanceToPlayer <= maxOcclusionDistance)
        {
            // Cast a ray from the audio source to the player
            RaycastHit hit;
            bool isOccluded = Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, distanceToPlayer, occlusionLayerMask);

            // Adjust target volume and frequency based on occlusion
            if (isOccluded)
            {
                // Reduce volume and lower frequency if occluded
                targetVolume = originalVolume * occlusionVolumeReduction;
                targetFrequency = occludedFrequency;
            }
            else
            {
                // Restore original volume and frequency if not occluded
                targetVolume = originalVolume;
                targetFrequency = fullFrequency;
            }
        }
        else
        {
            // Restore original volume and frequency if the player is too far away
            targetVolume = originalVolume;
            targetFrequency = fullFrequency;
        }

        // Smoothly interpolate the volume and frequency to avoid sudden changes
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * volumeSmoothness);
        lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, targetFrequency, Time.deltaTime * frequencySmoothness);
    }

    private void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            // Draw a line from the audio source to the player for debugging
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
}