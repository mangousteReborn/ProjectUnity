using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/*
 *  author: Thomas P
 * 	created_at:27/11/14
 * 
 * /?\ : Classic Player Interface (whole)
 * General and complete interface for Classic players (Game Master will get
 * another GUI).
 */
public class PlayerGUIScript : MonoBehaviour {

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
		
		_mainCanvas = _mainContainerObject.GetComponent<Canvas>();
		RectTransform rt = _mainCanvas.GetComponent<RectTransform>();
		
		// Must store Rect typed objert in a variable. Dunno why .. !
		Rect r = rt.rect;
		r.height = _screenHeight;
		r.width = _screenWidth;
	}
}
