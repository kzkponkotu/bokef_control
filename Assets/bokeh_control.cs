using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PostFX;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
public class bokeh_control : MonoBehaviour {

	public GameObject FPS_cam;
	public GameObject PostProcess;
	float bokef_area = 1;
	private TiltShift tiltshift;
	bool plus_flag=true;
	/*
		エフェクトなし1
		距離ぼかし＆中央の距離でぼかし量決定2
		距離ぼかし＆キー長押しでぼかし量決定3
		画面周辺ぼかし＆キー長押しでぼかし量決定4		
	*/
	private int mode=1;

	private Camera ray_cam;
	private PostProcessVolume volume;
	private DepthOfField depth = null;
	public Text mode_text;
	public GameObject panel;
	int mode1count=0,mode2count=0,mode3count=0,mode4count=0;
	bool zoom_flag=true;

	// Use this for initialization
	void Start () {
		tiltshift = FPS_cam.GetComponent<TiltShift>();
		ray_cam = FPS_cam.GetComponent<Camera>();
		volume = PostProcess.GetComponent<PostProcessVolume>();
		volume.profile.TryGetSettings(out depth);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1))mode = 1;
		if(Input.GetKeyDown(KeyCode.Alpha2))mode = 2;
		if(Input.GetKeyDown(KeyCode.Alpha3))mode = 3;
		if(Input.GetKeyDown(KeyCode.Alpha4))mode = 4;

		if(mode == 1){
			mode1count++;
			mode_text.enabled = true;
			panel.SetActive(true);
			if(mode1count > 150){
				mode_text.enabled = false;
				panel.SetActive(false);				
			}
			mode_text.text = "ぼかし無し";
			tiltshift.enabled = false;
			PostProcess.SetActive(false);
			mode2count=0;
			mode3count=0;
			mode4count=0;
		}

		if(mode == 2){
			mode2count++;
			mode_text.enabled = true;
			panel.SetActive(true);
			if(mode2count > 150){
				mode_text.enabled = false;
				panel.SetActive(false);				
			}			
			mode_text.text = "距離ぼかし＆画面中央の距離でぼかし量決定";
			PostProcess.SetActive(true);
			Ray ray = ray_cam.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2));

			//Rayが当たったオブジェクトの情報を入れる箱
			RaycastHit hit;

			//Rayの飛ばせる距離
			int distance = 1000;

			volume.profile.TryGetSettings(out depth);

			//Rayの可視化    ↓Rayの原点　　　　↓Rayの方向　　　　　　　　　↓Rayの色
			Debug.DrawLine (ray.origin, ray.direction * distance, Color.red);

			//もしRayにオブジェクトが衝突したら
			//                  ↓Ray  ↓Rayが当たったオブジェクト ↓距離
			if (Physics.Raycast(ray,out hit,distance))
			{
					depth.focusDistance.value = hit.distance;
			}
			mode1count=0;
			mode3count=0;
			mode4count=0;			
		}

		if(mode == 3){
			mode3count++;
			mode_text.enabled = true;
			panel.SetActive(true);
			if(mode3count > 150){
				mode_text.enabled = false;
				panel.SetActive(false);				
			}			
			mode_text.text = "距離ぼかし＆Gキー長押しでぼかし量決定";
			PostProcess.SetActive(true);			
			if(Input.GetKey(KeyCode.G)){
				if(plus_flag){
					depth.focusDistance.value += 0.1f;
					if(depth.focusDistance.value > 20)depth.focusDistance.value=20;
				}else{
					depth.focusDistance.value -= 0.1f;
					if(depth.focusDistance.value < 1)depth.focusDistance.value=1;
				}
			}
			mode1count=0;
			mode2count=0;
			mode4count=0;			
		}

		if(mode == 4){
			mode4count++;
			mode_text.enabled = true;
			panel.SetActive(true);			
			if(mode4count > 150){
				mode_text.enabled = false;
				panel.SetActive(false);				
			}			
			mode_text.text = "画面ぼかし＆Gキー長押しでぼかし量決定";			
			tiltshift.enabled = true;
			if(Input.GetKey(KeyCode.G)){
				if(plus_flag){
					tiltshift.Area += 0.05f;
					if(tiltshift.Area > 3)tiltshift.Area=3f;
				}else{
					tiltshift.Area -= 0.05f;
					if(tiltshift.Area < 0)tiltshift.Area=0f;				
				}
			}
			mode1count=0;
			mode2count=0;
			mode3count=0;
		}
		if(Input.GetKeyUp(KeyCode.G)){
			plus_flag = !plus_flag;
		}		
		if(Input.GetKeyUp(KeyCode.Z)){
			zoom_flag = !zoom_flag;
		}	
		if(Input.GetKey(KeyCode.Z)){
			if(zoom_flag){
				ray_cam.fieldOfView -= 0.5f;
				if(tiltshift.Area < 1f)ray_cam.fieldOfView=1f;
			}else{
				ray_cam.fieldOfView += 0.5f;
				if(tiltshift.Area > 90f)ray_cam.fieldOfView=90f;
			}
		}			
		if(Input.GetKeyDown(KeyCode.R)){
			SceneManager.LoadScene("SampleScene");
		}		
	}
}
