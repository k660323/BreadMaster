using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using Object = UnityEngine.Object;

public class ResourceManager
{
    Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, UnityEngine.Object>();

    int resourcesUnLoadCnt = 0;
    const int resocuresUnLoadMaxCnt = 20;


    public T Load<T>(string key, bool legacy = false) where T : Object
    {
        if (_resources.TryGetValue(key, out Object resource))
            return resource as T;
        if(legacy)
        {
            T obj = Resources.Load<T>(key);
            _resources.Add(key, obj);
            return obj;
        }

        return null;
    }

    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>($"{key}");
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {key}");
            return null;
        }

        // Pooling
        if (pooling)
            return Managers.Pool.Pop(prefab);

        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        if (Managers.Pool.Push(go))
            return;

        Object.Destroy(go);
        resourcesUnLoadCnt++;

        if(resourcesUnLoadCnt == resocuresUnLoadMaxCnt)
        {
            resourcesUnLoadCnt = 0;
            Resources.UnloadUnusedAssets();
        }
    }

    #region 어드레서블
    public void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        // 캐시 확인.
        if (_resources.TryGetValue(key, out Object resource))
        {
            callback?.Invoke(resource as T);
            return;
        }

        string loadKey = key;
        if (key.Contains(".sprite"))
            loadKey = $"{key}[{key.Replace(".sprite", "")}]";

        // 리소스 비동기 로딩 시작.
        var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
        asyncOperation.Completed += (op) =>
        {
            _resources.Add(key, op.Result);
            callback?.Invoke(op.Result);
        };
    }

    public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
    {
        var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        opHandle.Completed += (op) =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;

            foreach (var result in op.Result)
            {
                LoadAsync<T>(result.PrimaryKey, (obj) =>
                {
                    loadCount++;
                    callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                });
            }
        };
    }

    public void LoadSync<T>(string key) where T : UnityEngine.Object
    {
        // 캐시 확인.
        if (_resources.TryGetValue(key, out Object resource))
        {
            return;
        }

        string loadKey = key;
        if (key.Contains(".sprite"))
            loadKey = $"{key}[{key.Replace(".sprite", "")}]";

        // 리소스 동기 로딩 시작.
        var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
        T obj = asyncOperation.WaitForCompletion();
        _resources.Add(key, obj);
        Addressables.Release(asyncOperation);
    }

    public void LoadAllSync<T>(string label) where T : UnityEngine.Object
    {
        var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        var list = opHandle.WaitForCompletion();

        foreach (var result in list)
        {
            LoadSync<T>(result.PrimaryKey);
        }

        Addressables.Release(opHandle);
    }
    #endregion
}
