namespace Exanite.StatSystem
{
	/// <summary>
	/// Flags used in the StatSystem to differentiate between different types of mods
	/// </summary>
	[System.Serializable]
	public enum StatModFlag
	{
		Base,

		Damage,

		Attack,
		Spell,
		Hit,
		DamageOverTime,

		Unarmed,
		Dagger,
		Axe,
		Sword,
		Mace,
		Spear,
		Bow,
		Crossbow,
		Staff,
		Wand,

		Physical,
		Lightning,
		Ice,
		Fire,
		Chaos,
		Elemental,

		Melee,
		Ranged,
		OneHand,
		TwoHand,
		DualWield,

		Projectile,
		AreaOfEffect,

		Health,
		Mana,
		Shield,

		Max,
		Regen,
	}
}