using UnityEngine;

public class GhostShowEffect : MonoBehaviour
{
    public bool isEffectOver = false;

    private bool mIsShow = false;
    private Material mMaterial = null;
    private MeshFilter mMeshFilter = null;

    private float mShowTime = 0f;
    private float mLifeTime = 0f;
    private float mStartEffectTime = 0f;
    private string colorPropName = "_TintColor";

    public void StartEffect(float lifeTime, float showTime, string colorPropName)
    {
        this.colorPropName = colorPropName;
        isEffectOver = false;
        mIsShow = false;
        mLifeTime = lifeTime;
        mShowTime = showTime;
        mStartEffectTime = Time.time;

        mMeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshRenderer meshRender = gameObject.GetComponent<MeshRenderer>();
        mMaterial = meshRender.material;
        Color col = mMaterial.GetColor(colorPropName);
        col.a = 0f;
        mMaterial.SetColor(colorPropName, col);
    }

    void OnDestroy()
    {
        if (mMaterial != null)
            Destroy(mMaterial);
    }

    void Update()
    {
        if (isEffectOver || mMaterial == null)
            return;

        float time = Time.time - mStartEffectTime;
        if (!mIsShow && time > mShowTime)
        {
            Color col = mMaterial.GetColor(colorPropName);
            col.a = (mLifeTime - mShowTime) / mLifeTime;
            mMaterial.SetColor(colorPropName, col);
            mIsShow = true;
        }
        else if(mIsShow)
        {
            Color col = mMaterial.GetColor(colorPropName);
            col.a = (mLifeTime - mShowTime) - time / mLifeTime * (mLifeTime - mShowTime);
            mMaterial.SetColor(colorPropName, col);
        }

        if (time >= mLifeTime)
        {
            isEffectOver = true;
        }
    }

    public bool HasEffectMaterial { get { return mMaterial != null; } }

    public Mesh CurMesh { get { return mMeshFilter != null ? mMeshFilter.sharedMesh : new Mesh(); } }
}
