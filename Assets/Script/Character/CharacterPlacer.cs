// CharacterPlacer.cs
using UnityEngine;
using Unity.Netcode;

public class CharacterPlacer : MonoBehaviour
{
    public static CharacterPlacer Instance;
    

    public LayerMask battlefieldLayer; // Inspector'dan Battlefield layer'ýný seçeceðiz

    private Camera mainCamera;
    private CardData cardToPlace;
    private bool isPlacementMode = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!isPlacementMode) return;

        // Sol týkladýðýnda yerleþtirme iþlemini tetikle
        if (Input.GetMouseButtonDown(0))
        {
            PlaceCharacter();
        }

        // Sað týk ile yerleþtirmeyi iptal et
        if (Input.GetMouseButtonDown(1))
        {
            isPlacementMode = false;
            Debug.Log("Yerleþtirme iptal edildi.");
        }
    }

    // HandUIManager'daki kart UI'larý bu fonksiyonu çaðýracak
    public void EnterPlacementMode(CardData card)
    {
        if (card == null) return;
        cardToPlace = card;
        isPlacementMode = true;
        Debug.Log(card.cardName + " için yerleþtirme modu aktif.");
    }

    private void PlaceCharacter()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, battlefieldLayer))
        {
            Vector3 placementPosition = hit.point;

            // BattleManager'a spawn isteði gönder
            // Bu RPC'yi bir sonraki adýmda oluþturacaðýz
            BattleManager.Instance.SpawnCharacterAtPositionServerRpc(cardToPlace.cardId, placementPosition);

            // Yerleþtirme modundan çýk
            isPlacementMode = false;
            cardToPlace = null;
        }
        else
        {
            Debug.Log("Geçersiz yer. Lütfen savaþ alanýna týklayýn.");
        }
    }
}