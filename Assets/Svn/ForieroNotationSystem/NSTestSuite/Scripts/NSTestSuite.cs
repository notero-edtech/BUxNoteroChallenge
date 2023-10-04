/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

[Singleton] public partial class NSTestSuite : MonoBehaviour, ISingleton
{
    public static NSTestSuite Instance => Singleton<NSTestSuite>.instance;

    NSBehaviour NSB => NSBehaviour.instance;

    public WindowManager windowManager;

    public Text textModeText;
    public Text canvasRenderModeText;

    public Dropdown systemDropdown;
    public Dropdown directionDropdown;

    public RectTransform fpsRectTransform;
    
    public Dropdown updateCameraDropdown;
    public Dropdown updateTimeDropdown;

    public void OnCloseClick()
    {
        windowManager.gameObject.SetActive(false);
    }
    
    IEnumerator Start()
    {
        yield return new WaitWhile(() => NSBehaviour.instance == null);
        yield return null;
        
        NSPlayback.InitSystem(SystemEnum.RollingLeftRight);
      
        updateCameraDropdown.ClearOptions();
        foreach (var m in System.Enum.GetValues(typeof(NSPlayback.UpdateMode)))
        {
            updateCameraDropdown.options.Add(new Dropdown.OptionData(m.ToString()));
        }
        updateCameraDropdown.value = 0;
        updateCameraDropdown.RefreshShownValue();

        updateTimeDropdown.ClearOptions();
        foreach (var m in System.Enum.GetValues(typeof(NSPlayback.UpdateMode)))
        {
            updateTimeDropdown.options.Add(new Dropdown.OptionData(m.ToString()));
        }
        updateTimeDropdown.value = 0;
        updateTimeDropdown.RefreshShownValue();
               
        yield return null;
        yield return new WaitUntil(() => NSB.ns != null);

        textModeText.text = NSSettingsStatic.textMode.ToString();
        canvasRenderModeText.text = NSSettingsStatic.canvasRenderMode.ToString();

        updateCameraDropdown.value = (int)NSB.ns.nsSystemSettings.updateMode;
        updateCameraDropdown.RefreshShownValue();
 
        directionDropdown.ClearOptions();
        directionDropdown.options.Add(new Dropdown.OptionData(NSPlayback.NSRollingPlayback.RollingMode.Left.ToString()));
        directionDropdown.options.Add(new Dropdown.OptionData(NSPlayback.NSRollingPlayback.RollingMode.Right.ToString()));
        directionDropdown.value = 0;
        directionDropdown.RefreshShownValue();
        OnDirectionDropdownChange();
    }

    public void OnSystemDropdownChange()
    {
        switch (systemDropdown.options[systemDropdown.value].text)
        {
            case "Left Right":
                Debug.Log("Initializing Left Right!");
                directionDropdown.ClearOptions();
                directionDropdown.options.Add(new Dropdown.OptionData(NSPlayback.NSRollingPlayback.RollingMode.Left.ToString()));
                directionDropdown.options.Add(new Dropdown.OptionData(NSPlayback.NSRollingPlayback.RollingMode.Right.ToString()));
                directionDropdown.value = 0;
                directionDropdown.RefreshShownValue();
                OnDirectionDropdownChange();
                NSPlayback.InitSystem(SystemEnum.RollingLeftRight);
                break;
            case "Top Bottom":
                Debug.Log("Initializing Top Bottom!");
                directionDropdown.ClearOptions();
                directionDropdown.options.Add(new Dropdown.OptionData(NSPlayback.NSRollingPlayback.RollingMode.Top.ToString()));
                directionDropdown.options.Add(new Dropdown.OptionData(NSPlayback.NSRollingPlayback.RollingMode.Bottom.ToString()));
                directionDropdown.value = 0;
                directionDropdown.RefreshShownValue();
                OnDirectionDropdownChange();
                NSPlayback.InitSystem(SystemEnum.RollingTopBottom);
                break;
        }
    }

    public void OnDestroyClick()
    {
        NSPlayback.Stop();
        if (NSB && NSB.ns)
        {
            NSB.ns.DestroyChildren();
            NSB.ns.Init();
        }
        NSPlayback.ResetMeasures();
        //manual.Reset();
    }

    public void OnCreateClick()
    {
        OnDestroyClick();
        //manual.Create();
    }
    
    public void OnDirectionDropdownChange()
    {
        OnDestroyClick();

        switch (directionDropdown.options[directionDropdown.value].text)
        {
            case "Left": NSPlayback.NSRollingPlayback.rollingMode = NSPlayback.NSRollingPlayback.RollingMode.Left; break;
            case "Right": NSPlayback.NSRollingPlayback.rollingMode = NSPlayback.NSRollingPlayback.RollingMode.Right; break;
            case "Top": NSPlayback.NSRollingPlayback.rollingMode = NSPlayback.NSRollingPlayback.RollingMode.Top; break;
            case "Bottom": NSPlayback.NSRollingPlayback.rollingMode = NSPlayback.NSRollingPlayback.RollingMode.Bottom; break;
        }
    }

    private void Update()
    {
        if (Keyboard.current[Key.Escape].IsPressed() && windowManager.gameObject.activeSelf) { windowManager.gameObject.SetActive(false); }
    }
 
    public void OnUpdateCameraDropdownChange()
    {
        NSPlayback.updateCameraMode = (NSPlayback.UpdateMode)updateCameraDropdown.value;
    }

    public void OnUpdateTimeDropdownChange()
    {
        NSPlayback.updateTimeMode = (NSPlayback.UpdateMode)updateTimeDropdown.value;
    }
    
    public void OnTextModeButtonClick()
    {
        switch (NSSettingsStatic.textMode)
        {
            case TextMode.Text:
                NSSettingsStatic.textMode = TextMode.TextMeshPRO;
                break;
            case TextMode.TextMeshPRO:
                NSSettingsStatic.textMode = TextMode.Text;
                break;
        }

        textModeText.text = NSSettingsStatic.textMode.ToString();
    }

    public void OnCanvasRenderModeButtonClick()
    {
        switch (NSSettingsStatic.canvasRenderMode)
        {
            case CanvasRenderMode.Screen:
                NSB.SwitchCameraMode(CanvasRenderMode.World);
                break;
            case CanvasRenderMode.World:
                NSB.SwitchCameraMode(CanvasRenderMode.Screen);
                break;
        }

        canvasRenderModeText.text = NSSettingsStatic.canvasRenderMode.ToString();
    }

    public void OnDebugButtonClick()
    {
        NSB.nsDebug.debugRectTransform.gameObject.SetActive(!NSB.nsDebug.debugRectTransform.gameObject.activeSelf);
    }

    public void OnWindowManagerClick() => windowManager.gameObject.SetActive(!windowManager.gameObject.activeSelf);
    public void OnFPSClick() => fpsRectTransform.gameObject.SetActive(!fpsRectTransform.gameObject.activeSelf);
}
