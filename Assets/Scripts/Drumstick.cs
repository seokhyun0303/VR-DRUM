using UnityEngine;
using System.Collections;

public class Drumstick : MonoBehaviour
{
    public OVRInput.Controller whichHand = OVRInput.Controller.RTouch;

    private Vector3 previousPosition;
    private float currentSpeed = 0f;
    public GameObject hitEffectPrefab;
    private float lastHitTime = 0f;
    public float hitCooldown = 0.2f;

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        currentSpeed = (transform.position - previousPosition).magnitude / Time.deltaTime;
        previousPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Drum"))
        {
            if (Time.time - lastHitTime < hitCooldown)
                return;

            lastHitTime = Time.time;

            AudioSource drumSound = other.GetComponent<AudioSource>();
            if (drumSound != null && drumSound.clip != null)
            {
                float impactForce = currentSpeed;
                float volume = Mathf.Clamp(impactForce / 5.0f, 0.3f, 1.0f);

                drumSound.PlayOneShot(drumSound.clip, volume);

                float baseStrength = Mathf.Clamp01(impactForce / 5.0f);

                string drumPartName = other.name;

                float vibrationStrength = Mathf.Clamp01(GetVibrationStrength(drumPartName, baseStrength) * 2.0f);
                float vibrationDuration = GetVibrationDuration(drumPartName, impactForce);

                OVRInput.SetControllerVibration(1.0f, vibrationStrength, whichHand);
                StartCoroutine(StopVibration(vibrationDuration));

                if (hitEffectPrefab != null)
                {
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    Quaternion rotation = Quaternion.LookRotation(Vector3.up);
                    GameObject effect = Instantiate(hitEffectPrefab, hitPoint, rotation);

                    // 🎯 타격 세기 기반으로 축소 (현재 크기가 최대)
                    Vector3 baseScale = hitEffectPrefab.transform.localScale;
                    float scaleRatio = Mathf.Clamp(currentSpeed / 5f, 0.1f, 1.0f);
                    effect.transform.localScale = baseScale * scaleRatio;

                    Destroy(effect, 1f); // 자동 제거
                }
            }
        }
    }

    IEnumerator StopVibration(float delay)
    {
        yield return new WaitForSeconds(delay);
        OVRInput.SetControllerVibration(0, 0, whichHand);
    }

    float GetVibrationStrength(string drumName, float baseStrength)
    {
        if (drumName.Contains("Kick"))
            return baseStrength * 1.0f;
        else if (drumName.Contains("Snare"))
            return baseStrength * 0.7f;
        else if (drumName.Contains("FloorTom"))
            return baseStrength * 0.6f;
        else if (drumName.Contains("HighTom"))
            return baseStrength * 0.5f;
        else if (drumName.Contains("LowTom"))
            return baseStrength * 0.55f;
        else if (drumName.Contains("Tom"))
            return baseStrength * 0.6f;
        else if (drumName.Contains("HiHat"))
            return baseStrength * 0.4f;
        else if (drumName.Contains("LeftCrash"))
            return baseStrength * 0.5f;
        else if (drumName.Contains("RightCrash"))
            return baseStrength * 0.6f;
        else if (drumName.Contains("RideCrash"))
            return baseStrength * 0.7f;
        else if (drumName.Contains("Crash"))
            return baseStrength * 0.55f;
        else
            return baseStrength * 0.3f;
    }

    float GetVibrationDuration(string drumName, float impactForce)
    {
        float baseDuration;

        if (drumName.Contains("Kick"))
            baseDuration = 0.2f;
        else if (drumName.Contains("Snare"))
            baseDuration = 0.15f;
        else if (drumName.Contains("FloorTom"))
            baseDuration = 0.12f;
        else if (drumName.Contains("HighTom"))
            baseDuration = 0.1f;
        else if (drumName.Contains("LowTom"))
            baseDuration = 0.11f;
        else if (drumName.Contains("Tom"))
            baseDuration = 0.12f;
        else if (drumName.Contains("HiHat"))
            baseDuration = 0.08f;
        else if (drumName.Contains("LeftCrash"))
            baseDuration = 0.1f;
        else if (drumName.Contains("RightCrash"))
            baseDuration = 0.11f;
        else if (drumName.Contains("RideCrash"))
            baseDuration = 0.13f;
        else if (drumName.Contains("Crash"))
            baseDuration = 0.1f;
        else
            baseDuration = 0.07f;

        float impactFactor = Mathf.Clamp01(impactForce / 5.0f);
        return baseDuration * (1.0f + impactFactor);
    }
}

