using UnityEngine;
using System.Collections;

public class VignetteEntity : Vignette {

    public enum EntityType { Base };

    private EntityType _type;

    public VignetteEntity(EntityType type, string key, string name, string desc = null, string imagePath = "Vignettes/default_bonus")
	: base(key, name, desc == null ? name : desc, VignetteType.enemy, imagePath){
        this._type = type;
    }

    public EntityType entityType
    {
        get { return this._type; }
    }
}
