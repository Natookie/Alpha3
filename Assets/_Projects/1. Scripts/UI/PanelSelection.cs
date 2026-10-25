using UnityEngine;
using NaughtyAttributes;

public class PanelSelection : MonoBehaviour
{
    [System.Serializable]
    public struct Entry
    {
        public CoreButton button;
        public GameObject panel;
    }

    [Header("ENTRIES")]
    [SerializeField] private Entry[] entries;

    [Header("STATE")]
    [SerializeField] private int defaultIndex = 0;

    private int selectedIndex = -1;

    void Start() => Select(defaultIndex);
    public void Select(int index){
        if(index < 0 || index >= entries.Length) return;
        if(index == selectedIndex) return;

        for(int i = 0; i < entries.Length; i++){
            bool isTarget = i == index;

            if(entries[i].button != null) entries[i].button.SetSelected(isTarget);
            if(entries[i].panel != null) entries[i].panel.SetActive(isTarget);
        }

        selectedIndex = index;
    }
}