using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour {

	public bool   bShow = true;
	public float  updateInterval = 0.5f;
	
	private float mLastInterval;
	private int   mFrames = 0;
	private float mFps;
	
	void Start() 
	{
		mLastInterval = Time.realtimeSinceStartup;
		
		mFrames = 0;
	}
	
	void OnGUI() 
	{
		if(bShow)
			GUI.Label(new Rect(0, 100, 200, 200), "FPS:" + mFps.ToString("f2"));
	}
	
	void Update() 
	{
		if(!bShow)
			return;

		++mFrames;
		
		if (Time.realtimeSinceStartup > mLastInterval + updateInterval) 
		{
			mFps = mFrames / (Time.realtimeSinceStartup - mLastInterval);
			
			mFrames = 0;
			
			mLastInterval = Time.realtimeSinceStartup;
		}
	}
}
