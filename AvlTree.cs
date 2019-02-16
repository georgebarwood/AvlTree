using Collections = System.Collections;
using Generic = System.Collections.Generic;
using Console = System.Console; // For example usage.

class AvlExample
{
  public static void Usage()
  {
    SortedSet<long> set = new SortedSet<long>();

    int testSize = 5 * 1000 * 1000;

    // Insert elements 0, 10, 20 ... into the set.
    for ( int i = 0; i < testSize; i += 10 ) 
    {
      set[ i ] = true; 
    }

    Console.WriteLine( "Should print 50,60..100" );
    foreach ( long x in set.Range( 50, 100 ) ) Console.WriteLine( x );

    // Remove 4/5 of the elements from the set.
    for ( int i = 0; i < testSize; i += 10 ) if ( i % 50 != 0 ) 
    { 
      set[ i ] = false;
    }

    Console.WriteLine( "Should print 50,100..250" );
    foreach ( long x in set.Range( 50, 250 ) ) Console.WriteLine( x );   

    // Test set union and intersection.
    SortedSet<int> s1 = new SortedSet<int>(); 
    SortedSet<int> s2 = new SortedSet<int>(); 
    s1[ 1 ] = true; s1[ 3 ] = true; s1[ 4 ] = true; s1[ 6 ] = true;
    s2[ 2 ] = true; s2[ 3 ] = true; s2[ 5 ] = true; s2[ 6 ] = true;

    Console.WriteLine( "Should print 3, 6" );
    foreach ( int e in s1 & s2 ) Console.WriteLine( e );  
  
    Console.WriteLine( "Should print 1..6" );
    foreach ( int e in s1 | s2 ) Console.WriteLine( e );  

    Console.WriteLine( "Should print 1, 4" );
    foreach ( int e in s1 - s2 ) Console.WriteLine( e ); 

    SortedDictionary<int,string> dict = new SortedDictionary<int,string>( "" );

    dict[ 100 ] = "There";
    dict[ 50 ] = "Hello";
    dict[ 100 ] = "there";

    Console.WriteLine( "Should print Hello there" );
    foreach ( int i in dict.Keys ) Console.WriteLine( dict[ i ] );
  }
}


class SortedSet<T> : AvlTree<T>, Generic.IEnumerable<T>
// Generic sorted set implemented as  height-balanced binary search tree. 
{
  public SortedSet() : base() 
  // Initialise with default ordering for T.
  { 
  } 

  public SortedSet( DCompare compare ) : base( compare ) 
  // Initialise with a specific ordering.
  { 
  } 

  public bool this [ T element ]
  // Include or Remove an element or check whether an element is in the set.
  { 
    set
    {
      if ( value ) Insert( element ); else Remove( element );
    }
    get
    {
      return Lookup( element ) != null;
    } 
  }

  public Generic.IEnumerator<T> GetEnumerator() 
  // Iterate over the set elements.
  {   
    if ( Root != null ) foreach( T key in Root ) yield return key;
  }

  public Generic.IEnumerable<T> Range( T start, T end )
  // Interate over the set elements in the specified range.
  {
    if ( Root != null ) foreach( T key in Root.Range( start, end, Compare ) ) yield return key;
  }

  public static SortedSet<T> operator & ( SortedSet<T> a, SortedSet<T> b )
  // Set intersection.
  {
    SortedSet<T> result = new SortedSet<T>( a.Compare );
    Generic.IEnumerator<T> ea = a.GetEnumerator();
    Generic.IEnumerator<T> eb = b.GetEnumerator();
    bool aok = ea.MoveNext();
    bool bok = eb.MoveNext();
    while ( aok && bok )
    {
      int compare = a.Compare( ea.Current,  eb.Current );
      if ( compare == 0 )
      {
        result.Append( ea.Current );
        aok = ea.MoveNext();
        bok = eb.MoveNext();
      }
      else if ( compare < 0 )
      {
        aok = ea.MoveNext();
      }
      else
      {
        bok = eb.MoveNext();
      }        
    }
    return result;
  }

  public static SortedSet<T> operator | ( SortedSet<T> a, SortedSet<T> b )
  // Set union.
  {
    SortedSet<T> result = new SortedSet<T>( a.Compare );
    Generic.IEnumerator<T> ea = a.GetEnumerator();
    Generic.IEnumerator<T> eb = b.GetEnumerator();
    bool aok = ea.MoveNext();
    bool bok = eb.MoveNext();
    while ( aok && bok )
    {
      int compare = a.Compare( ea.Current,  eb.Current );
      if ( compare == 0 )
      {
        result.Append( ea.Current );
        aok = ea.MoveNext();
        bok = eb.MoveNext();
      }
      else if ( compare < 0 )
      {
        result.Append( ea.Current );
        aok = ea.MoveNext();
      }
      else
      {
        result.Append( eb.Current );
        bok = eb.MoveNext();
      }        
    }
    while ( aok )
    {
      result.Append( ea.Current );
      aok = ea.MoveNext();
    }
    while ( bok )
    {
      result.Append( eb.Current );
      bok = eb.MoveNext();
    }
    return result;
  }

  public static SortedSet<T> operator - ( SortedSet<T> a, SortedSet<T> b )
  // Set difference.
  {
    SortedSet<T> result = new SortedSet<T>( a.Compare );
    Generic.IEnumerator<T> ea = a.GetEnumerator();
    Generic.IEnumerator<T> eb = b.GetEnumerator();
    bool aok = ea.MoveNext();
    bool bok = eb.MoveNext();
    while ( aok && bok )
    {
      int compare = a.Compare( ea.Current,  eb.Current );
      if ( compare == 0 )
      {
        aok = ea.MoveNext();
        bok = eb.MoveNext();
      }
      else if ( compare < 0 )
      {
        result.Append( ea.Current );
        aok = ea.MoveNext();
      }
      else
      {
        bok = eb.MoveNext();
      }        
    }
    while ( aok )
    {
      result.Append( ea.Current );
      aok = ea.MoveNext();
    }
    return result;
  }

  Collections.IEnumerator Collections.IEnumerable.GetEnumerator() 
  // This is required by IEnumerable<T>. 
  { 
    return GetEnumerator(); 
  }

  protected override Node NewNode( T key )
  { 
    return new Node( key );
  }
}


class SortedDictionary<TKey,TValue> : AvlTree<TKey>
// Generic sorted dictionary implemented as a height-balanced binary search tree.
{
  public SortedDictionary( TValue defaultValue ) : base() 
  // Initialise with default order for TKey.
  // defaultValue is the value returned if an unassigned key is accessed.
  { 
    DefaultValue = defaultValue; 
  }

  public SortedDictionary( DCompare compare, TValue defaultValue ) : base( compare ) 
  // Initialise with specific order.
  // defaultValue is the value returned if an unassigned key is accessed.
  { 
    DefaultValue = defaultValue; 
  }

  public TValue this [ TKey key ]
  // Set or get an indexed dictionary value. 
  { 
    set
    {
      AssignValue = value;
      Insert( key );
    }
    get
    {
      Pair p = (Pair) Lookup( key );
      return p != null ? p.Value : DefaultValue;
    } 
  }

  public Generic.IEnumerable<TKey> Keys 
  // Iterate over all the dictionary keys.
  {
    get
    {   
      if ( Root != null ) foreach( TKey key in Root ) yield return key;
    }
  }

  public Generic.IEnumerable<TKey> KeyRange( TKey start, TKey end )
  // Iterate over the specified range of dictionary keys.
  {
    if ( Root != null ) foreach( TKey key in Root.Range( start, end, Compare ) ) yield return key;
  }

  private readonly TValue DefaultValue;

  private TValue AssignValue;

  private class Pair : AvlTree<TKey>.Node
  {
    public TValue Value;

    public Pair( TKey key, TValue value ) : base( key ) 
    { 
      Value = value; 
    }
  }

  protected override Node NewNode( TKey key )
  {
    return new Pair( key, AssignValue );
  }

  protected override void Update( Node x )
  {
    Pair p = (Pair) x;
    p.Value = AssignValue;
  }
}


abstract class AvlTree<T> 
// Height-balanced binary search tree.
{
  public delegate int DCompare( T key1, T key2 );

  protected AvlTree() 
  // Initialise with default compare function.
  {    
    Compare = Generic.Comparer<T>.Default.Compare;
  }

  protected AvlTree( DCompare compare ) 
  // Initialise with specific compare function.
  {    
    Compare = compare;
  }

  protected void Insert( T key ) 
  // Insert key into the tree. If key is already in tree, Update is called.
  {    
    bool heightIncreased;
    Root = Insert( Root, key, out heightIncreased );
  }

  protected void Append( T key )
  // Append a key to the tree.
  {
    bool heightIncreased;
    Root = Append( Root, key, out heightIncreased );
  }

  protected void Remove( T key ) 
  // Remove key from the tree. If key is not present, has no effect. 
  {
    bool heightIncreased;
    Root = Remove( Root, key, out heightIncreased );
  }

  protected abstract Node NewNode( T key ); 
  // Factory method called by Insert if key not found.

  protected virtual void Update( Node x )
  // Called by Insert when an existing key is found.
  {    
  }

  protected virtual void FreeNode( Node x )
  // Called by Remove when a Node is removed.
  {    
  }

  protected Node Lookup( T key )
  {
    // Search tree for Node with Key equal to key.
    Node x = Root;
    while ( x != null )
    {
      int cf = Compare( key, x.Key );
      if ( cf < 0 ) x = x.Left;
      else if ( cf > 0 ) x = x.Right;
      else return x;
    }
    return null;
  }

  protected class Node
  {
    public Node Left, Right;
    public readonly T Key;
    public sbyte Balance;
    public Node( T key ) 
    { 
      Key = key;
    }

    public Generic.IEnumerator<T> GetEnumerator() 
    {
      if ( Left != null ) foreach ( T key in Left ) yield return key;
      yield return Key;
      if ( Right != null ) foreach ( T key in Right ) yield return key;      
    }

    public Generic.IEnumerable<T> Range( T start, T end, DCompare compare )
    {
      int cstart = compare( start, Key );
      int cend = compare( end, Key );
      if ( cstart < 0 && Left != null )
      {
        foreach ( T key in Left.Range( start, end, compare ) ) yield return key;
      }
      if ( cstart <= 0 && cend >= 0 ) yield return Key;
      if ( cend > 0 && Right != null )
      {
        foreach ( T key in Right.Range( start, end, compare ) ) yield return key;
      }
    }

  } // Node

  // Fields.

  protected readonly DCompare Compare;
  protected Node Root;

  // Constant values for Node.Balance.
  private const int LeftHigher = -1, Balanced = 0, RightHigher = 1;

  // Private methods used to implement key insertion and removal.

  private Node Insert( Node x, T key, out bool heightIncreased )
  {
    if ( x == null )
    {
      x = NewNode( key );
      heightIncreased = true;
    }
    else 
    {
      int compare = Compare( key, x.Key );
      if ( compare < 0 )
      {
        x.Left = Insert( x.Left, key, out heightIncreased );
        if ( heightIncreased )
        {
          if ( x.Balance == Balanced )
          {
            x.Balance = LeftHigher;
          }
          else
          {
            heightIncreased = false;
            if ( x.Balance == LeftHigher )
            {
              bool heightDecreased;
              return RotateRight( x, out heightDecreased );
            }
            x.Balance = Balanced;
          }
        }
      }
      else if ( compare > 0 )
      {
        x.Right = Insert( x.Right, key, out heightIncreased );
        if ( heightIncreased )
        {
          if ( x.Balance == Balanced )
          {
            x.Balance = RightHigher;
          }
          else
          {
            heightIncreased = false;
            if ( x.Balance == RightHigher )
            {
              bool heightDecreased;
              return RotateLeft( x, out heightDecreased );
            }
            x.Balance = Balanced;
          }
        }
      }
      else // compare == 0
      {
        Update( x );
        heightIncreased = false;
      }
    }
    return x;
  }

  private Node Append( Node x, T key, out bool heightIncreased )
  {
    if ( x == null )
    {
      x = NewNode( key );
      heightIncreased = true;
    }
    else 
    {
      x.Right = Insert( x.Right, key, out heightIncreased );
      if ( heightIncreased )
      {
        if ( x.Balance == Balanced )
        {
          x.Balance = RightHigher;
        }
        else
        {
          heightIncreased = false;
          if ( x.Balance == RightHigher )
          {
            bool heightDecreased;
            return RotateLeft( x, out heightDecreased );
          }
          x.Balance = Balanced;
        }
      }
    }
    return x;
  }

  private Node Remove( Node x, T key, out bool heightDecreased )
  {
    if ( x == null ) // key not found.
    {
      heightDecreased = false;
      return x;
    }
    int compare = Compare( key, x.Key );
    if ( compare == 0 )
    {
      Node deleted = x;
      if ( x.Left == null )
      {
        heightDecreased = true;
        x = x.Right;
      }
      else if ( x.Right == null )
      {
        heightDecreased = true;
        x = x.Left;
      }
      else
      {
        // Remove the smallest element in the right sub-tree and substitute it for x.
        Node right = RemoveLeast( deleted.Right, out x, out heightDecreased );
        x.Left = deleted.Left;
        x.Right = right;
        x.Balance = deleted.Balance;
        if ( heightDecreased )
        {
          if ( x.Balance == LeftHigher )
          {
            x = RotateRight( x, out heightDecreased );
          }
          else if ( x.Balance == RightHigher )
          {
            x.Balance = Balanced;
          }
          else
          {
            x.Balance = LeftHigher;
            heightDecreased = false;
          }
        }
      }
      FreeNode( deleted );
    }
    else if ( compare < 0 )
    {
      x.Left = Remove( x.Left, key, out heightDecreased );
      if ( heightDecreased )
      {
        if ( x.Balance == RightHigher )
        {
          return RotateLeft( x, out heightDecreased );
        }
        if ( x.Balance == LeftHigher )
        {
          x.Balance = Balanced;
        }
        else
        {
          x.Balance = RightHigher;
          heightDecreased = false;
        }
      }
    }
    else
    {
      x.Right = Remove( x.Right, key, out heightDecreased );
      if ( heightDecreased )
      {
        if ( x.Balance == LeftHigher )
        {
          return RotateRight( x, out heightDecreased );
        }
        if ( x.Balance == RightHigher )
        {
          x.Balance = Balanced;
        }
        else
        {
          x.Balance = LeftHigher;
          heightDecreased = false;
        }
      }
    }
    return x;
  }

  private static Node RemoveLeast( Node x, out Node least, out bool heightDecreased )
  {
    if ( x.Left == null )
    {
      heightDecreased = true;
      least = x;
      return x.Right;
    }
    else
    {
      x.Left = RemoveLeast( x.Left, out least, out heightDecreased );
      if ( heightDecreased )
      {
        if ( x.Balance == RightHigher )
        {
          return RotateLeft( x, out heightDecreased );
        }
        if ( x.Balance == LeftHigher )
        {
          x.Balance = Balanced;
        }
        else
        {
          x.Balance = RightHigher;
          heightDecreased = false;
        }
      }
      return x;
    }
  }

  private static Node RotateRight( Node x, out bool heightDecreased )
  {
    // Left is 2 levels higher than Right.
    heightDecreased = true;
    Node z = x.Left;
    Node y = z.Right;
    if ( z.Balance != RightHigher ) // Single rotation.
    {
      z.Right = x;
      x.Left = y;
      if ( z.Balance == Balanced ) // Can only occur when deleting values.
      {
        x.Balance = LeftHigher;
        z.Balance = RightHigher;
        heightDecreased = false;
      }
      else // z.Balance = LeftHigher
      {
        x.Balance = Balanced;
        z.Balance = Balanced;
      }
      return z;
    }
    else // Double rotation.
    {
      x.Left = y.Right;
      z.Right = y.Left;
      y.Right = x;
      y.Left = z;
      if ( y.Balance == LeftHigher )
      {
        x.Balance = RightHigher;
        z.Balance = Balanced;
      }
      else if ( y.Balance == Balanced )
      {
        x.Balance = Balanced;
        z.Balance = Balanced;
      }
      else // y.Balance == RightHigher
      {
        x.Balance = Balanced;
        z.Balance = LeftHigher;
      }
      y.Balance = Balanced;
      return y;
    }
  }

  private static Node RotateLeft( Node x, out bool heightDecreased )
  {
    // Right is 2 levels higher than Left.
    heightDecreased = true;
    Node z = x.Right;
    Node y = z.Left;
    if ( z.Balance != LeftHigher ) // Single rotation.
    {
      z.Left = x;
      x.Right = y;
      if ( z.Balance == Balanced ) // Can only occur when deleting values.
      {
        x.Balance = RightHigher;
        z.Balance = LeftHigher;
        heightDecreased = false;
      }
      else // z.Balance = RightHigher
      {
        x.Balance = Balanced;
        z.Balance = Balanced;
      }
      return z;
    }
    else // Double rotation
    {
      x.Right = y.Left;
      z.Left = y.Right;
      y.Left = x;
      y.Right = z;
      if ( y.Balance == RightHigher )
      {
        x.Balance = LeftHigher;
        z.Balance = Balanced;
      }
      else if ( y.Balance == Balanced )
      {
        x.Balance = Balanced;
        z.Balance = Balanced;
      }
      else // y.Balance == LeftHigher
      {
        x.Balance = Balanced;
        z.Balance = RightHigher;
      }
      y.Balance = Balanced;
      return y;
    }
  }
}
