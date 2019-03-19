using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Vanessa Lopez
//step 1: raise pool water level to top of caution tape
//step 2: raise secondary water on right to top of caution tape
//step 3: delete secondary, widen pool water
//step 4: raise pool water to halfway up left slope
// tune the timing right
public class DisplacementBehaviour : MonoBehaviour {
	public Material ppeMaterial;
	public Texture displacementTexture;
	public float movementSpeed = 5f;
	public Color waterColor = Color.white;

	RenderTexture mainRenderTexture;
	RenderTexture maskWaterTexture;
	GameObject ppeRenderCamera;
	Camera ppeRenderCameraComponent;
	Camera thisCameraComponent;

	void Awake() {
		mainRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Default);
		maskWaterTexture = new RenderTexture(Screen.width / 4, Screen.height / 4, 24, RenderTextureFormat.Default);
		maskWaterTexture.wrapMode = TextureWrapMode.Repeat;

		thisCameraComponent = GetComponent<Camera>();
		thisCameraComponent.SetTargetBuffers(mainRenderTexture.colorBuffer, mainRenderTexture.depthBuffer);
		CreatePostRenderCam();
	}

	void OnPostRender() {
		ppeRenderCameraComponent.CopyFrom(thisCameraComponent);
		ppeRenderCameraComponent.clearFlags = CameraClearFlags.SolidColor;
		ppeRenderCameraComponent.backgroundColor = Color.black;
		ppeRenderCameraComponent.cullingMask = 1 << LayerMask.NameToLayer("Water");
		ppeRenderCameraComponent.targetTexture = maskWaterTexture;
		ppeRenderCameraComponent.Render();

		ppeMaterial.SetTexture("_MaskTex", maskWaterTexture);
		ppeMaterial.SetTexture("_DisplacementTex", displacementTexture);
		ppeMaterial.SetFloat("_Movement", movementSpeed);
		ppeMaterial.SetColor("_WaterColor", waterColor);
		Graphics.Blit(mainRenderTexture, null, ppeMaterial);
	}

	private void CreatePostRenderCam() {
		ppeRenderCamera = new GameObject("PostRenderCam");
		ppeRenderCamera.transform.position = transform.position;
		ppeRenderCamera.transform.rotation = transform.rotation;
		ppeRenderCameraComponent = ppeRenderCamera.AddComponent<Camera>();
		ppeRenderCameraComponent.enabled = false;
	}
}
