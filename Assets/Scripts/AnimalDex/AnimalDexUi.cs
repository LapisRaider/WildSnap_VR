using UnityEngine;

public class AnimalDexUi : MonoBehaviour
{
    private AnimalDex m_animalDexInfo;

    [Header("UI info")]
    public GameObject m_animalDexSlotPrefab;
    public GameObject m_animalDexSlotParent;

    public AnimalEntryUi m_animalEntryUi;

    // Start is called before the first frame update
    void Awake()
    {
        m_animalDexInfo = AnimalDex.Instance;

        foreach (var dexEntry in m_animalDexInfo.GetDexEntries())
        {
            GameObject dexSlotUi = Instantiate(m_animalDexSlotPrefab, m_animalDexSlotParent.transform);
            AnimalDexSlotUi slotUi = dexSlotUi.GetComponent<AnimalDexSlotUi>();

            slotUi.Init(dexEntry.Key, dexEntry.Value, AnimalSelected);
        }

        AnimalDeselected();
    }

    public void AnimalSelected(AnimalType m_animalType)
    {
        m_animalDexSlotParent.SetActive(false);

        m_animalEntryUi.gameObject.SetActive(true);
        m_animalEntryUi.InitInfo(m_animalDexInfo.GetDexEntries()[m_animalType]);
    }

    public void AnimalDeselected()
    {
        m_animalDexSlotParent.SetActive(true);
        m_animalEntryUi.gameObject.SetActive(false);
    }
}
