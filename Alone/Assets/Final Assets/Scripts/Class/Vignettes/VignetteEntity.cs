using UnityEngine;
using System.Collections;

public class VignetteEntity : Vignette {

    public enum EntityType { Base };

    public VignetteEntity(EntityType type, string key, string name, string desc = null, string imagePath = "Vignettes/default_bonus")
	: base(key, name, desc == null ? name : desc, VignetteType.enemy, imagePath){}

    public EntityType type
    {
        get { return this.type; }
    }
}
