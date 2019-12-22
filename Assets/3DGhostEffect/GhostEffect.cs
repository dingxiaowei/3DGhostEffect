using UnityEngine;
using System.Collections.Generic;

public class GhostEffect : MonoBehaviour
{
    #region ghostEffectPool
    public class GhostEffectPool
    {
        private List<GhostShowEffect> mLtUsed;
        private Stack<GhostShowEffect> mUnUsedStack;
        public GhostEffectPool()
        {
            mLtUsed = new List<GhostShowEffect>();
            mUnUsedStack = new Stack<GhostShowEffect>();
        }

        public GhostShowEffect GetOneUnUsedObj()
        {
            GhostShowEffect gse = null;
            if (mUnUsedStack.Count > 0)
                gse = mUnUsedStack.Pop();

            if (gse == null)
            {
                GameObject go = new GameObject();
                //go.hideFlags = HideFlags.HideAndDontSave;
                gse = go.AddComponent<GhostShowEffect>();
            }

            return gse;
        }

        public void AddUsedObj(GhostShowEffect gse)
        {
            mLtUsed.Add(gse);
        }

        public void Release()
        {
            for (int i = 0; i < mLtUsed.Count; ++i)
            {
                if (mLtUsed[i] != null)
                    DestroyImmediate(mLtUsed[i].gameObject);
            }

            while (mUnUsedStack.Count > 0)
            {
                GhostShowEffect gse = mUnUsedStack.Pop();
                if (gse != null)
                    DestroyImmediate(gse.gameObject);
            }

            mLtUsed.Clear();
            mUnUsedStack.Clear();
        }

        public void Update(float time)
        {
            for (int i = mLtUsed.Count - 1; i >= 0; --i)
            {
                GhostShowEffect gse = mLtUsed[i];
                if (gse.isEffectOver)
                {
                    mUnUsedStack.Push(mLtUsed[i]);
                    mLtUsed.RemoveAt(i);
                }
            }
        }
    }
    #endregion
    public Color color = new Color(0.2f, 0.2f, 1f);
    public float intervalTime = 1f;
    public float liveTime = 4f;
    public float showTime = 3f;//showTime need < liveTime
    public Shader shader;
    public string colorPropName = "_TintColor";

    private float mLastEffectTime = 0f;

    private SkinnedMeshRenderer[] mSkinnedMeshRenders = null;
    private MeshFilter[] mMeshFilters = null;
    private Material mEffectMaterial = null;
    private GhostEffectPool mGhostEffectPool = null;

    // Use this for initialization
    void Start()
    {
        mSkinnedMeshRenders = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        mMeshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        mGhostEffectPool = new GhostEffectPool();
        mLastEffectTime = Time.time;
        CreateMaterial();
    }

    void OnDestroy()
    {
        if (mGhostEffectPool != null)
            mGhostEffectPool.Release();
        if (mEffectMaterial != null)
            Destroy(mEffectMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time;
        if(time - mLastEffectTime > intervalTime)
        {
            mLastEffectTime = time;
            OnDoEffect();
        }

        mGhostEffectPool.Update(time);
    }

    private void CreateMaterial()
    {
        if (mEffectMaterial != null)
            return;

        mEffectMaterial = new Material(shader);
        mEffectMaterial.SetColor(colorPropName, color);
    }

    private void OnDoEffect()
    {
        for(int i = 0; mSkinnedMeshRenders != null && i < mSkinnedMeshRenders.Length; ++i)
        {
            SkinnedMeshRenderer skinnedMeshRender = mSkinnedMeshRenders[i];

            GhostShowEffect gse = mGhostEffectPool.GetOneUnUsedObj();
            Mesh mesh = gse.CurMesh;
            skinnedMeshRender.BakeMesh(mesh);

            CreateEffect(gse, mesh, skinnedMeshRender.transform);
        }

        for(int i = 0; mMeshFilters != null && i < mMeshFilters.Length; ++i)
        {
            GhostShowEffect gse = mGhostEffectPool.GetOneUnUsedObj();
            Mesh mesh = gse.CurMesh;
            mesh = mMeshFilters[i].mesh;

            CreateEffect(gse, mesh, mMeshFilters[i].transform);
        }
    }

    private void CreateEffect(GhostShowEffect gse, Mesh mesh, Transform trans)
    {
        MeshFilter meshFilter = gse.gameObject.AddMissingComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
        MeshRenderer meshRender = gse.gameObject.AddMissingComponent<MeshRenderer>();
        if (!gse.HasEffectMaterial)
            meshRender.material = mEffectMaterial;
        gse.StartEffect(liveTime, showTime, colorPropName);
        gse.transform.position = trans.position;
        gse.transform.rotation = trans.rotation;
        mGhostEffectPool.AddUsedObj(gse);
    }
}
