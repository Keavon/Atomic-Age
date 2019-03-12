using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//step 1: raise pool water level to top of caution tape
//step 2: raise secondary water on right to top of caution tape
//step 3: delete secondary, widen pool water
//step 4: raise pool water to halfway up left slope
// tune the timing right
// edit script and shader to remove lines that aren't used 
// rename variables, customize to code style
public class DisplacementBehaviour : MonoBehaviour 
{
    public Material _mat;
    private RenderTexture _screenTex;
    private RenderTexture _waterMaskTex;

    public Texture _displacementTex;
    public float _movement = 5f;
    public Color _waterColor = Color.white;

    private GameObject _postRenderCamObj;
    private Camera _postRenderCam;
    private Camera _screenCam;

    void Awake() 
    {
        _screenTex = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Default);
        _waterMaskTex = new RenderTexture(Screen.width / 4, Screen.height / 4, 24, RenderTextureFormat.Default);
        _waterMaskTex.wrapMode = TextureWrapMode.Repeat;

        _screenCam = GetComponent<Camera>();
        _screenCam.SetTargetBuffers(_screenTex.colorBuffer, _screenTex.depthBuffer);
        CreatePostRenderCam();

        var shader = Shader.Find("Unlit/DisplacementShader");
        _mat = new Material(shader);
    }

    void OnPostRender()
	{
        _postRenderCam.CopyFrom(_screenCam);
        _postRenderCam.clearFlags = CameraClearFlags.SolidColor;
        _postRenderCam.backgroundColor = Color.black;
        _postRenderCam.cullingMask = 1 << LayerMask.NameToLayer("Water");
        _postRenderCam.targetTexture = _waterMaskTex;
        _postRenderCam.Render();

        _mat.SetTexture("_MaskTex", _waterMaskTex);
        _mat.SetTexture("_DisplacementTex", _displacementTex);
        _mat.SetFloat("_Movement", _movement);
        _mat.SetColor("_WaterColor", _waterColor);
        Graphics.Blit(_screenTex, null, _mat);
	}

    private void CreatePostRenderCam() 
    {
        _postRenderCamObj = new GameObject("PostRenderCam");
        _postRenderCamObj.transform.position = transform.position;
        _postRenderCamObj.transform.rotation = transform.rotation;
        _postRenderCam = _postRenderCamObj.AddComponent<Camera>();
        _postRenderCam.enabled = false;
    }
}
