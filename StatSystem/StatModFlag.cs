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

		Unarmed,
		Dagger,
		Axe,
		Sword,
		Mace,
		Spear,
		Bow,
		Staff,
		Wand,

		Physical,
		Slashing,
		Piercing,
		Crushing,

		Elemental,
		Lightning,
		Ice,
		Fire,

		Melee,
		Ranged,
		OneHand,
		TwoHand,
		DualWield,

		Health,
		Mana,
		Shield,

		Max,
		Regen,
	}
}