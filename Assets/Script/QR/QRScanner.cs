using UnityEngine;
using ZXing;
using UnityEngine.UI;

public class QRScanner : MonoBehaviour
{
    private WebCamTexture camTexture;
    public RawImage cameraView;
    public Button restartScanButton;

    private bool scanned = false;

    void Start()
    {
        StartCamera();
        restartScanButton.onClick.AddListener(RestartScan);
        restartScanButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (camTexture != null && camTexture.isPlaying && !scanned)
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);

            if (result != null)
            {
                scanned = true;

                // ⚠️ QR verisini oyuncuya gösterme!
                QRDataManager.Instance?.ParseQRData(result.Text);
                camTexture.Stop();
                restartScanButton.gameObject.SetActive(true);
            }
        }
    }

    void StartCamera()
    {
        camTexture = new WebCamTexture();
        cameraView.texture = camTexture;
        cameraView.material.mainTexture = camTexture;
        camTexture.Play();
        scanned = false;
        restartScanButton.gameObject.SetActive(false);
    }

    public void RestartScan()
    {
        if (camTexture != null && !camTexture.isPlaying)
        {
            camTexture.Play();
            scanned = false;
            restartScanButton.gameObject.SetActive(false);
        }
    }
}
