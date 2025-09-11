// CharacterPlacer.cs
using UnityEngine;
using Unity.Netcode;

public class CharacterPlacer : MonoBehaviour
{
    public static CharacterPlacer Instance;


    public LayerMask battlefieldLayer; // Inspector'dan Battlefield layer'�n� se�ece�iz

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

        // Sol t�klad���nda yerle�tirme i�lemini tetikle
        if (Input.GetMouseButtonDown(0))
        {
            PlaceCharacter();
        }

        // Sa� t�k ile yerle�tirmeyi iptal et
        if (Input.GetMouseButtonDown(1))
        {
            isPlacementMode = false;
            Debug.Log("Yerle�tirme iptal edildi.");
        }
    }

    // HandUIManager'daki kart UI'lar� bu fonksiyonu �a��racak
    public void EnterPlacementMode(CardData card)
    {
        if (card == null) return;
        cardToPlace = card;
        isPlacementMode = true;
        Debug.Log(card.cardName + " i�in yerle�tirme modu aktif.");
    }

    private void PlaceCharacter()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, battlefieldLayer))
        {
            Vector3 placementPosition = hit.point;

            // BattleManager'a spawn iste�i g�nder
            // Bu RPC'yi bir sonraki ad�mda olu�turaca��z
            BattleManager.Instance.SpawnCharacterAtPositionServerRpc(cardToPlace.id, placementPosition);

            // Yerle�tirme modundan ��k
            isPlacementMode = false;
            cardToPlace = null;
        }
        else
        {
            Debug.Log("Ge�ersiz yer. L�tfen sava� alan�na t�klay�n.");
        }
    }
}