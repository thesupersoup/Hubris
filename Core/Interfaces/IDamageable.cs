namespace Hubris
{
	public interface IDamageable
	{
		// int nDmg is the amount of damage total, bool nDirect is whether the damage should be directly applied or obey restrictions
		bool TakeDmg( LiveEntity damager, int nType, int nDmg, bool nDirect);
	}
}