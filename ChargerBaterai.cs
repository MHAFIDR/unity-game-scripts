using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ChargingStation : MonoBehaviour
{
    public ItemData unchargedBattery;
    public ItemData chargedBattery;
    public AudioClip chargeSound;

    [Header("UI Pengisian Baterai")]
    public GameObject chargingPanel;
    public Slider chargeSlider;
    public TextMeshProUGUI percentageText;
    public float chargeDuration = 3f;

    [Header("Model 3D")]
    public GameObject batteryModel; // Tambahkan baris ini

    private bool isCharging = false;

    void Start()
    {
        // Pastikan model baterai disembunyikan di awal
        if (batteryModel != null)
        {
            batteryModel.SetActive(false);
        }
    }

    public void ChargeBattery()
    {
        if (!isCharging && InventoryManager.instance.playerInventory.items.Contains(unchargedBattery))
        {
            StartCoroutine(ChargeBatteryCoroutine());
            SphereCollider collider = GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.enabled = false; // Nonaktifkan collider untuk mencegah interaksi ganda
            }
        }
    }

    private IEnumerator ChargeBatteryCoroutine()
    {
        isCharging = true;

        if (batteryModel != null)
        {
            batteryModel.SetActive(true); // Munculkan model baterai
        }

        if (chargeSound != null)
        {
            AudioSource.PlayClipAtPoint(chargeSound, transform.position);
        }

        float elapsedTime = 0f;
        while (elapsedTime < chargeDuration)
        {
            if (GameManager.instance.isUiOpen)
            {
                if (chargingPanel.activeSelf)
                {
                    chargingPanel.SetActive(false);
                }
            }
            else
            {
                if (!chargingPanel.activeSelf)
                {
                    chargingPanel.SetActive(true);
                }
            }

            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / chargeDuration;
            chargeSlider.value = progress;
            percentageText.text = (progress * 100f).ToString("F0") + "%";
            yield return null;
        }

        chargeSlider.value = 1f;
        percentageText.text = "100%";

        yield return new WaitForSeconds(0.5f);

        InventoryManager.instance.playerInventory.Remove(unchargedBattery);
        InventoryManager.instance.playerInventory.Add(chargedBattery);
        InventoryManager.instance.UpdateUI();

        Debug.Log("Baterai berhasil diisi!");

        chargingPanel.SetActive(false);
        if (batteryModel != null)
        {
            batteryModel.SetActive(false); // Sembunyikan lagi model baterai
        }
        isCharging = false;
    }
}
