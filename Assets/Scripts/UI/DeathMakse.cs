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
        if (maskImage == null) return;
        maskImage.enabled = true;
        maskImage.fillMethod = Image.FillMethod.Horizontal;
        maskImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        maskImage.fillAmount = 0;
        await Task.Yield();
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            maskImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            await Task.Yield();
        }
        maskImage.fillAmount = 1;
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
