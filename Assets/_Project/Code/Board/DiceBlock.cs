using UnityEngine;

/// <summary>
/// Generates dice roll values for board movement
///
/// Responsibilities:
/// - Simulate a die roll.
/// Return a random value for movement
///
/// This class currently generates values between 1 and 10.
///
/// FUTURE SUPPORT:
/// - Dice roll animations
/// - Custom dice blocks
/// - special item dice
/// </summary>

public class DiceBlock : MonoBehaviour
{
    
    // generate a random dice value
    // return an interget between 1 and 10 inclusive. This value is used by TurnManager to determine how many spaces the active player may move.
    public int Roll()
    {
        return Random.Range(1, 11);
    }
}
