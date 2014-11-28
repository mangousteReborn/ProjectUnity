using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIScript : MonoBehaviour {

	[SerializeField]
	GameObject _mainContainerObject;
	
	[SerializeField]
	GameObject _mainMenuObject;

	[SerializeField]
	GameObject _actionMenuObject;

	[SerializeField]
	GameObject _squareComponentObject;

	[SerializeField]
	GameObject _optionMenuObject;

	// TODO : Top content components


	private int _screenHeight;
	private int _screenWidth;



	// MainCanvas
	private Canvas _mainCanvas;
		
		/* Top Content : TODO
			- Last action, Timers ...
	 	*/
		private Canvas _topContent;
		
		/* Bottom Content :
			- Action bar, Menu (Options, Vignettes, Actions)
	 	*/
		private Canvas _bottomContent;

			//MainMenu
			private Canvas _mainMenu;


	void Start() {
		_screenHeight = Screen.height;
		_screenWidth = Screen.width;

		_mainCanvas = (Canvas) Instantiate(_mainObject);
		RectTransform rt = _mainCanvas.GetComponent<RectTransform>();
		rt.rect.height = _screenHeight;
		rt.rect.width = _screenWidth;



	}
}
