using UnityEngine;

public enum DisplayMode{ Windowed, BorderlessWindow }

public class DisplayModeManager : MonoBehaviour
{
    public static DisplayModeManager Instance{ get; private set; }

    private const string PREF_KEY = "display_mode";

    [SerializeField] private int borderlessWidthFallback = 1920;
    [SerializeField] private int borderlessHeightFallback = 1080;

    private DisplayMode currentMode;

    void Awake(){
        if(Instance != null && Instance != this){ 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
    }

    void Start() => LoadAndApplySaved();
    public DisplayMode CurrentMode => currentMode;

    public DisplayMode GetOppositeMode(){
        return currentMode == DisplayMode.Windowed
            ? DisplayMode.BorderlessWindow
            : DisplayMode.Windowed;
    }

    public void RequestToggle(){
        if(DisplayModeConfirmUI.Instance != null) DisplayModeConfirmUI.Instance.RequestModeChange(GetOppositeMode());
        else{
            ApplyImmediate(GetOppositeMode());
            SaveMode(GetOppositeMode());
        }
    }

    public void RequestMode(DisplayMode mode){
        if(DisplayModeConfirmUI.Instance != null) DisplayModeConfirmUI.Instance.RequestModeChange(mode);
        else{
            ApplyImmediate(mode);
            SaveMode(mode);
        }
    }

    public void LoadAndApplySaved(){
        int saved = PlayerPrefs.GetInt(PREF_KEY, (int)DisplayMode.Windowed);
        currentMode = (DisplayMode)saved;
        ApplyImmediate(currentMode);
    }

    public void ApplyImmediate(DisplayMode mode){
        if(mode == DisplayMode.Windowed) Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        else{
            Resolution native = GetNativeResolution();
            Screen.SetResolution(native.width, native.height, FullScreenMode.FullScreenWindow);
        }
        currentMode = mode;
    }

    public void SaveMode(DisplayMode mode){
        PlayerPrefs.SetInt(PREF_KEY, (int)mode);
        PlayerPrefs.Save();
        currentMode = mode;
    }

    Resolution GetNativeResolution(){
        Resolution[] all = Screen.resolutions;
        if(all.Length == 0) return new Resolution{ width = borderlessWidthFallback, height = borderlessHeightFallback };
        Resolution best = all[0];
        int bestScore = 0;
        for(int i = 0; i < all.Length; i++){
            int score = all[i].width * all[i].height;
            if(score > bestScore){
                bestScore = score;
                best = all[i];
            }
        }
        return best;
    }
}