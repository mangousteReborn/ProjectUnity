using UnityEngine;
using System.Collections;

public class VignetteEntity : Vignette {

    public enum EntityType { Base };

    private EntityType _type;
    private int _cost;

    public VignetteEntity(EntityType type,int cost, string key, string name, string desc = null, string imagePath = "Vignettes/default_bonus")
	: base(key, name, desc == null ? name : desc, VignetteType.enemy, imagePath){
        this._type = type;
        this._cost = cost;
    }

    public EntityType entityType
    {
        get { return this._type; }
    }

    public int cost
    {
        get { return this._cost; }
    }
}
