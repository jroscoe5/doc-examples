// Jonathon Roscoe
// Oscillator.cs
//
// version 1.3
// - seperated oscillators.cs into multiple files
// - added constants
// 
// The Oscillator class encapsulates a non-zero value and 
// oscillates it back and forth i.e. 4 -> -4 -> 4 -> 4.
// Using Programming by Contract.

namespace oscillator
{
  class Oscillator
  {
    // CLASS INVARIANT: Oscillate() is a virtual function intended to be
    // overridden in child classes.

    // IMPLEMENTATION INVARIANT:
    // These constants are used for arithmetic operations within Oscillate()
    // and SignHelper()
    protected const int NEGATIVE_ONE = -1;
    protected const int POSITIVE_ONE = 1;
    protected const int ZERO = 0;

    // The value the oscillator is currently at. Should never be 0.
    protected int value;

    // The Oscillator's state. True = active.
    protected bool is_active;

    // SignHelper()
    // Returns the sign of the current value as either 1 or -1.
    // Not used in Oscillator but valuable in more complex descendants
    // to calculate the next integer stored in value.
    //
    // PRECONDITIONS: NONE
    //
    // POSTCONDITIONS: NONE 
    protected int SignHelper()
    {
      if (value > ZERO)
        return POSITIVE_ONE;
      else
        return NEGATIVE_ONE;
    }


    // INTERFACE INVARIANTS:
    // 
    // An Oscillator can either be active or inactive.
    // All Oscillators are initialized as active.
    // When an Oscillator is active it may use all public functions.
    // An Oscillator will become inactive when Deactive() is called.
    // When an Oscillator is inactive it may no longer Oscillate() or
    // Deactive(). Get_Value() may still be called.
    // Once an Oscillator is inactive it will remain inactive until a new
    // Oscillator is constructed.
    // An Oscillator's value changes whenever Oscillate() is called.
    // However, an Oscillator's value will never increase or decrease.
    // The value will only be the one given at construction or its opposite.
    // An Oscillator's value can only be reset by calling the constructor.
    // An Oscillator must be initialized with a non-zero value.


    // Constructor
    // inc_val is the value the oscillator will initially store.
    // 
    // PRECONDITIONS: inc_val must be non-zero integer.
    //
    // POSTCONDITIONS: Oscillator is now active and holding inc_value.
    public Oscillator(int inc_val)
    {
      value = inc_val;
      is_active = true;
    }

    // Oscillate()
    // Changes the sign on value. Ex: 5 becomes -5 becomes 5...
    //
    // PRECONDITIONS: Oscillator must be active.
    //
    // POSTCONDITIONS: value is now the opposite of what it was.
    public virtual void Oscillate()
    {
      value = NEGATIVE_ONE * value;
    }

    // Get_Value()
    // Returns the current value held in the Oscillator.
    //
    // PRECONDITIONS: NONE
    //
    // POSTCONDITIONS: NONE
    public int Get_Value()
    {
      return value;
    }

    // Get_Active()
    // Returns the current state of the Oscillator. True = active.
    //
    // PRECONDITIONS: NONE
    //
    // POSTCONDITIONS: NONE
    public bool Get_Active()
    {
      return is_active;
    }

    // Deactivate()
    // Deactivates the Oscillator
    //
    // PRECONDITIONS: Oscillator must be active.
    //
    // POSTCONDITIONS: Oscillator is now inactive.
    public void Deactivate()
    {
      is_active = false;
    }
  }
}
