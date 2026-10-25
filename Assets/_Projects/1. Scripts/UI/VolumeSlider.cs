using Nova;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

[System.Serializable]
public class VolumeSliderEvent : UnityEvent<float> { }

public enum VolumeChannel { Master, SFX, BGM, UI }

[RequireComponent(typeof(Nova.Interactable))]
public class VolumeSlider : MonoBehaviour
{
    [Header("CHANNEL")]
    [SerializeField] private VolumeChannel channel = VolumeChannel.Master;

    [Header("REFERENCES")]
    [SerializeField] private UIBlock2D containerBlock;
    [SerializeField] private UIBlock2D background;
    [SerializeField] private UIBlock2D fill;
    [SerializeField] private UIBlock2D draggable;
    [SerializeField] private TextBlock valueText;

    [Header("VALUE")]
    [Range(0f, 1f)] [SerializeField] private float defaultValue = 0.5f;

    [Header("EVENT")]
    [SerializeField] private VolumeSliderEvent onValueChanged = new VolumeSliderEvent();
    public VolumeSliderEvent OnValueChanged => onValueChanged;

    private float normalizedValue;
    private bool initialized;

    public float NormalizedValue => normalizedValue;

    void Awake(){
        Init();
    }

    void Start(){
        LoadCurrentVolume();
    }

    [Button("Initialize", EButtonEnableMode.Editor)]
    public void Init(){
        if(initialized) return;
        initialized = true;

        if(containerBlock == null) containerBlock = GetComponent<UIBlock2D>();

        containerBlock.AddGestureHandler<Gesture.OnDrag>(HandleDrag);
        containerBlock.AddGestureHandler<Gesture.OnRelease>(HandleRelease);
    }

    void LoadCurrentVolume(){
        if(AudioManager.Instance == null){
            SetNormalizedValue(defaultValue, notify: false);
            return;
        }

        float current = GetVolumeFromAudioManager();
        SetNormalizedValue(current, notify: false);
    }

    float GetVolumeFromAudioManager(){
        if(AudioManager.Instance == null) return defaultValue;
        switch(channel){
            case VolumeChannel.Master: return AudioManager.Instance.GetMasterVolume();
            case VolumeChannel.SFX:    return AudioManager.Instance.GetSfxVolume();
            case VolumeChannel.BGM:    return AudioManager.Instance.GetBgmVolume();
            default: return defaultValue;
        }
    }

    void SetVolumeOnAudioManager(float value){
        if(AudioManager.Instance == null) return;
        switch(channel){
            case VolumeChannel.Master: AudioManager.Instance.SetMasterVolume(value); break;
            case VolumeChannel.SFX:    AudioManager.Instance.SetSfxVolume(value);    break;
            case VolumeChannel.BGM:    AudioManager.Instance.SetBgmVolume(value);    break;
        }
    }

    void HandleDrag(Gesture.OnDrag evt){
        if(background == null) return;
        float bgWidth = background.CalculatedSize.X.Value;
        if(bgWidth <= 0f) return;

        float deltaPercent = evt.DragDeltaLocalSpace.x / bgWidth;
        float newValue = Mathf.Clamp01(normalizedValue + deltaPercent);

        SetNormalizedValue(newValue, notify: true);
    }

    void HandleRelease(Gesture.OnRelease evt){
        if(SaveManager.Instance != null) SaveManager.Instance.Save();
    }

    public void SetNormalizedValue(float value, bool notify = true){
        normalizedValue = Mathf.Clamp01(value);
        UpdateVisuals();

        SetVolumeOnAudioManager(normalizedValue);

        if(notify) onValueChanged.Invoke(normalizedValue);
    }

    void UpdateVisuals(){
        if(fill != null){
            Length3 fillSize = fill.Size;
            fillSize.X.Percent = normalizedValue;
            fill.Size = fillSize;
        }

        if(draggable != null && background != null){
            float bgWidth = background.CalculatedSize.X.Value;
            float handleWidth = draggable.Size.X.Value;
            if(bgWidth > 0f){
                float offset = (normalizedValue - 0.5f) * (bgWidth - handleWidth);
                draggable.Position.X.Value = offset;
            }
        }

        if(valueText != null) valueText.Text = Mathf.RoundToInt(normalizedValue * 100f) + "%";
    }
}