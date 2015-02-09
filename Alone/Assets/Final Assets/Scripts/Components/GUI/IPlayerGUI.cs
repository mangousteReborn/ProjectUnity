using System.Collections;

public interface IPlayerGUI
{

	void setOwner (Player owner);

	void updateGUI();

	void changeGameMode(int val);
}


