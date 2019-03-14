using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameObject player;
    public GameObject boundsPlane;
    public float zoom;
    public float smoothing;

    private Bounds bounds;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private bool shaking;

    public float shakeAmount;
    public float shakeDuration;

    float shakePercentage;//A percentage (0-1) representing the amount of shake to be applied when setting rotation.
    float startAmount;//The initial shake amount (to determine percentage), set when ShakeCamera is called.
    float startDuration;//The initial shake duration, set when ShakeCamera is called.

    bool isRunning = false; //Is the coroutine running right now?

    public bool smooth;//Smooth rotation?
    public float smoothAmount = 5f;//Amount to smooth

    void Start()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        bounds = boundsPlane.GetComponent<Renderer>().bounds;

        minX = bounds.min.x + horzExtent;
        maxX = bounds.max.x - horzExtent;
        minY = bounds.min.y + vertExtent;
        maxY = bounds.max.y - vertExtent;
    }

    void Update () {
		Vector3 cameraPosition = transform.position;

        Rect screenBounds = BoundsToScreenRect(bounds);

        Vector2 playerPosition = Camera.main.WorldToScreenPoint(player.transform.position);

        float percentX = 0.5f - (screenBounds.xMax - playerPosition.x) / screenBounds.xMax;
        float percentY = 0.5f - (screenBounds.yMax - playerPosition.y) / screenBounds.yMax;

        float newCameraX = Mathf.Lerp(cameraPosition.x, player.transform.position.x, 0.001f + smoothing * Mathf.Pow(percentX, 2));
        float newCameraY = Mathf.Lerp(cameraPosition.y, player.transform.position.y, 0.001f + smoothing * Mathf.Pow(percentY, 2));

        transform.position = new Vector3(newCameraX, newCameraY, transform.position.z);
    }


    // Shaking found at https://wiki.unity3d.com/index.php/Camera_Shake
    public void ShakeCamera(float amount, float duration)
    {

        shakeAmount += amount;//Add to the current amount.
        startAmount = shakeAmount;//Reset the start amount, to determine percentage.
        shakeDuration += duration;//Add to the current time.
        startDuration = shakeDuration;//Reset the start time.

        if (!isRunning) StartCoroutine(Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
    }


    IEnumerator Shake()
    {
        isRunning = true;

        while (shakeDuration > 0.01f)
        {
            Vector3 rotationAmount = Random.insideUnitSphere * shakeAmount;//A Vector3 to add to the Local Rotation
            rotationAmount.z = 0;//Don't change the Z; it looks funny.

            shakePercentage = shakeDuration / startDuration;//Used to set the amount of shake (% * startAmount).

            shakeAmount = startAmount * shakePercentage;//Set the amount of shake (% * startAmount).
            shakeDuration = Mathf.Lerp(shakeDuration, 0, Time.deltaTime);//Lerp the time, so it is less and tapers off towards the end.


            if (smooth)
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime * smoothAmount);
            else
                transform.localRotation = Quaternion.Euler(rotationAmount);//Set the local rotation the be the rotation amount.

            yield return null;
        }
        transform.localRotation = Quaternion.identity;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
        isRunning = false;
    }

    void LateUpdate()
    {
        Vector3 v3 = transform.position;
        v3.x = Mathf.Clamp(v3.x, minX, maxX);
        v3.y = Mathf.Clamp(v3.y, minY, maxY);
        transform.position = v3;
    }

    // Found at https://answers.unity.com/questions/49943/is-there-an-easy-way-to-get-on-screen-render-size.html
    private Rect BoundsToScreenRect(Bounds bounds)
    {
        // Get mesh origin and farthest extent (this works best with simple convex meshes)
        Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
        Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));

        // Create rect in screen space and return - does not account for camera perspective
        return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
    }
}
