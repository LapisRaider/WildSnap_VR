using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDexUi : MonoBehaviour
{
    public AnimalDex m_animalDexInfo;

    [Header("UI info")]
    public GameObject m_animalDexSlotPrefab;
    public GameObject m_animalDexSlotParent;

    

    // Start is called before the first frame update
    void Awake()
    {
        m_animalDexInfo.Init();

        foreach (var dexEntry in m_animalDexInfo.GetDexEntries())
        {
            GameObject dexSlotUi = Instantiate(m_animalDexSlotPrefab, m_animalDexSlotParent.transform);
            AnimalDexSlotUi slotUi = dexSlotUi.GetComponent<AnimalDexSlotUi>();

            slotUi.Init(dexEntry.Key, dexEntry.Value, SlotClicked);
        }
    }

    public void SlotClicked(AnimalType m_animalType)
    {
        m_animalDexSlotParent.SetActive(false);

    }
}
