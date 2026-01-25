using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMask : BasePanel
{
    public Image maskImage;
    public override void Init()
    {
        maskImage.enabled = false;
    }
    public async Task BlackImageFadeIn(float duration)
    {
        maskImage.enabled = true;
        if (maskImage == null) return;
        Debug.Log("kkk");
        // maskImage.color = Color.black;
        maskImage.fillMethod = Image.FillMethod.Horizontal;
        maskImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        maskImage.fillAmount = 0;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            maskImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            Debug.Log($"当前进度: {maskImage.fillAmount}");
            await Task.Yield();
        }
        maskImage.fillAmount = 1;
        Debug.Log("jjj");
    }
    public async Task BlackImageFadeOut(float duration)
    {
        if (maskImage == null) return;

        maskImage.fillOrigin = (int)Image.OriginHorizontal.Right;
        maskImage.fillAmount = 1;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            maskImage.fillAmount = 1 - Mathf.Clamp01(elapsed / duration);
            await Task.Yield();
        }
        maskImage.fillAmount = 0;
    }

}
