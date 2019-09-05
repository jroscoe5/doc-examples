// Jonathon Roscoe
// Magnifier.cs
//
// version 1.4
// -converted magnifier class from c++ to c#
// -implemented a MagnifierInterface to be inherited from
// -added Magnify and Set_Scalar to provide more functionality to class
// -a magnifier may now be magnified repeatedly
//
// Implementation for Magnifier class. A Magnifier encapsulates an
// integer value and an integer scalar and returns a scaled value. 
// Using programming-by-contract

using magnifierInterface;

namespace magnifier
{
  class Magnifier : MagnifierInterface
  {
    // CLASS INVARIANT: All implemented methods are originally defined in
    // MagnifierInterface.

    // The stored initial value of Magnifier
    private int value;

    // The scalar the value is magnified by.
    private int scale;

    // The Magnifier's state. True = active.
    private bool is_active;

    // INTERFACE INVARIANTS
    // A Magnifier can either be active or inactive.
    // All Magnifiers are initialized as active.
    // A Magnifier will become inactive when Deactivate is called.
    // When a Magnifier is deactive, Magnify() Set_Scalar() and Deactivate
    // should no longer be called.
    // A Magnifier accepts both an initial value and a scalar. It then will
    // return a magnified value - i.e. the value multiplied by the scalar.
    // When constructing a Magnifier, the passed value and scalar must be
    // non-zero integers. The scalar must be a positive integer.
    // A Magnifier's value and state cannot be reset except by
    // reconstructing the object.

    // Constructor
    // inc_val is the value the Magnifier will store, while inc_scalar
    // will be the stored scalar.
    //
    // PRECONDITIONS: inc_val and inc_scalar must be non-zero integers.
    //
    // POSTCONDITIONS: Magnifier is now active.
    public Magnifier(int inc_val, int inc_scalar)
    {
      value = inc_val;
      scale = inc_scalar;
      is_active = true;
    }

    // Magnify()
    // Magnifies the current value by the current scalar.
    //
    // PRECONDITIONS: Magnifier must be active.
    //
    // POSTCONDITIONS: value is now value * scale
    public void Magnify()
    {
      value = value * scale;
    }

    // Set_Scalar()
    // Sets the encapsulated scale
    //
    // PRECONDITIONS: Magnifier must be active.
    // inc_scalar must be a positive integer.
    //
    // POSTCONDITIONS: scale = inc_scalar
    public void Set_Scalar(int inc_scalar)
    {
      scale = inc_scalar;
    }

    // Get_Value()
    // Returns the magnified value.
    //
    // PRECONDITIONS: NONE
    //
    // POSTCONDITIONS: NONE
    public int Get_Value()
    {
      return value;
    }

    // Get_Active()
    // Returns current state of Magnifier
    //
    // PRECONDITIONS: NONE
    //
    // POSTCONDITIONS: NONE
    public bool Get_Active()
    {
      return is_active;
    }

    // Deactivate()
    // Deactivates the Magnifier
    //
    // PRECONDITIONS: Magnifier must be active.
    //
    // POSTCONDITIONS: Magnifier is now inactive.
    public void Deactivate()
    {
      is_active = false;
    }

  }
}
