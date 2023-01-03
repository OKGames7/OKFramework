/*
 The MIT License (MIT)

Copyright (c) 2013 yamamura tatsuhiko

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using OKGamesLib;
using Cysharp.Threading.Tasks;

public class Fade : MonoBehaviour, IFader {
    private IFade fade;

    private float cutoutRange;

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init() {
        fade = GetComponent<IFade>();
        fade.Init();
        fade.Range = cutoutRange;
    }

    private void OnValidate() {
        Init();
        fade.Range = cutoutRange;
    }

    private async UniTask FadeoutTask(float time, System.Action action) {
        float endTime = Time.timeSinceLevelLoad + time * (cutoutRange);
        while (Time.timeSinceLevelLoad <= endTime) {
            cutoutRange = (endTime - Time.timeSinceLevelLoad) / time;
            fade.Range = cutoutRange;
            // yield return new WaitForEndOfFrameとほぼ同じ.
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }
        cutoutRange = 0;
        fade.Range = cutoutRange;

        action?.Invoke();
    }

    private async UniTask FadeinTask(float time, System.Action action) {
        float endTime = Time.timeSinceLevelLoad + time * (1 - cutoutRange);
        while (Time.timeSinceLevelLoad <= endTime) {
            cutoutRange = 1 - ((endTime - Time.timeSinceLevelLoad) / time);
            fade.Range = cutoutRange;
            // yield return new WaitForEndOfFrameとほぼ同じ.
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }
        cutoutRange = 1;
        fade.Range = cutoutRange;

        action?.Invoke();
    }

    public async UniTask FadeOut(float time, System.Action action) {
        await FadeoutTask(time, action);
    }

    public async UniTask FadeOut(float time) {
        await FadeoutTask(time, null);
    }

    public async UniTask FadeIn(float time, System.Action action) {
        await FadeinTask(time, action);
    }

    public async UniTask FadeIn(float time) {
        await FadeinTask(time, null);
    }
}
