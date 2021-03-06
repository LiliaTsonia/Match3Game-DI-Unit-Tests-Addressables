using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
	[SerializeField] private Camera _camera;
    [SerializeField] private bool _keepAspectRatio;

    private void Awake()
    {
		BoardController.OnBoardPositionSet += SetScale;
	}

	private void SetScale(int size)
    {
#if UNITY_ANDROID
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            _camera.orthographicSize = size;
        }
#endif

		var topRightCorner = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _camera.transform.position.z));
		var worldSpaceWidth = topRightCorner.x * 2;
		var worldSpaceHeight = topRightCorner.y * 2;

		var spriteSize = GetComponent<SpriteRenderer>().bounds.size;

		var scaleFactorX = worldSpaceWidth / spriteSize.x;
		var scaleFactorY = worldSpaceHeight / spriteSize.y;

		if (_keepAspectRatio)
		{
			if (scaleFactorX > scaleFactorY)
			{
				scaleFactorY = scaleFactorX;
			}
			else
			{
				scaleFactorX = scaleFactorY;
			}
		}

		transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1);
	}
}
