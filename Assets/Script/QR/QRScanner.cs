using UnityEngine;
using ZXing;
using UnityEngine.UI;

public class QRScanner : MonoBehaviour
{
    private WebCamTexture camTexture;
    public RawImage cameraView;
    public Button restartScanButton;
    public GameObject placeholderImage;

    private bool scanned = false;

    void Start()
    {
        StartCamera();
        restartScanButton.onClick.AddListener(RestartScan);
        restartScanButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (camTexture != null && camTexture.isPlaying && camTexture.didUpdateThisFrame && !scanned)
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);

            if (result != null)
            {
                scanned = true;
                Debug.Log("✅ QR kodu okundu: " + result.Text);

                QRDataManager.Instance?.ParseQRData(result.Text);
                StopCamera();
                restartScanButton.gameObject.SetActive(true);
            }
        }
    }


    void StartCamera()
    {
        if (camTexture != null)
        {
            camTexture.Stop();
            camTexture = null;
        }

        camTexture = new WebCamTexture();
        cameraView.texture = camTexture;
        cameraView.material.mainTexture = camTexture;

        camTexture.Play();
        scanned = false;
        restartScanButton.gameObject.SetActive(false);

        // 👇 Placeholder gizle
        if (placeholderImage != null)
            placeholderImage.SetActive(false);

        Debug.Log("📷 Kamera başlatıldı");
    }

    void StopCamera()
    {
        if (camTexture != null)
        {
            camTexture.Stop();
            cameraView.texture = null;
            cameraView.material.mainTexture = null;
            Debug.Log("🛑 Kamera durduruldu");
        }

        // 👇 Placeholder göster
        if (placeholderImage != null)
            placeholderImage.SetActive(true);
    }


    public void RestartScan()
    {
        Debug.Log("🔁 Tarama yeniden başlatılıyor...");
        StopCamera();       // Önce temiz durdur
        StartCamera();      // Sonra tekrar başlat
    }
}
