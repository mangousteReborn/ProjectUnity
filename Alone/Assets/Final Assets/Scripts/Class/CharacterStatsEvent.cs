using System;

public enum CharacterStatsEvent
{
	currentLifeChange,
	lifeChange,

	vignetteBonusAdded,
	vignetteBonusRemoved,

	actionAdded,
	actionRemoved,

	effectAdded,
	effectRemove,

	currentActionPointChanged,
	maxActionPointChanged, // TODO

	hotActionPushed,
	lastHotActionRemoved,

	gameModeChanged,
	change,
}



