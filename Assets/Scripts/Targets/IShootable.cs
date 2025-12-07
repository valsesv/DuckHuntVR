/// <summary>
/// Interface for objects that can be hit by projectiles.
/// Allows projectiles to interact with any target type (bombs, targets, etc.)
/// </summary>
public interface IShootable
{
    /// <summary>
    /// Called when a projectile hits this object.
    /// </summary>
    /// <param name="projectileData">The projectile data containing information about the projectile that hit this object.</param>
    void CheckHit(ProjectileData projectileData);
}
