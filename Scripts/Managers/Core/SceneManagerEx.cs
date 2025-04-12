using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public LoadSceneMode loadSceneMode { get; private set; }

    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene type, LoadSceneMode mode = LoadSceneMode.Single)
    {
        loadSceneMode = mode;

        Managers.Clear();

        SceneManager.LoadScene(GetSceneName(type), mode);
    }

    // (비동기 로딩)[비동기] 씬 초기화, 씬 추가
    public AsyncOperation AsyncLoadScene(Define.Scene type, LoadSceneMode mode = LoadSceneMode.Single, bool isCompletedAndActvie = false)
    {
        loadSceneMode = mode;

        // 로딩 바로 시작
        AsyncOperation aSync = SceneManager.LoadSceneAsync(GetSceneName(type), mode);
        // 로딩 끝나후 사후처리
        aSync.allowSceneActivation = isCompletedAndActvie;
        return aSync;
    }

    public AsyncOperation AsyncUnLoadScene(Define.Scene type)
    {
        AsyncOperation aSync = SceneManager.UnloadSceneAsync(GetSceneName(type));
        aSync.allowSceneActivation = true;
        return aSync;
    }

    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        if (loadSceneMode == LoadSceneMode.Single)
            CurrentScene.Clear();
    }
}
