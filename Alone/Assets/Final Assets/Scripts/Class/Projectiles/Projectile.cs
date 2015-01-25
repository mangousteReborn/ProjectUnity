using UnityEngine;
using System.Collections;

public abstract class Projectile  {




	protected Projectile(){

	}


	public abstract void onCollision(Collision col);
}
