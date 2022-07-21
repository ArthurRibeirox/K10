using UnityEngine;

[System.Serializable]
public class IntState : INumericValueState<int>, ISerializationCallbackReceiver
{
    [SerializeField] int _value;
    [System.NonSerialized] EventSlot<int> _onChange;

    public int Value { get { return _value; } set { Setter( value ); } }
    public int Get() { return _value; }

    public void Setter( int value )
    {
        if( _value == value ) return;
        _value = value;
        _onChange?.Trigger( value );
	}

	public void Increment( int value = 1 )
	{
		if( value == 0 ) return;
		Setter( _value + value );
	}

	public void PlusOne() => Setter( _value + 1 );
	public void MinusOne() => Setter( _value - 1 );

	public void Clear()
	{
		_onChange?.Clear();
		_onChange = null;
	}

    public IEventRegister<int> OnChange => _onChange ?? (_onChange = new EventSlot<int>() );

    public IntState( int initialValue = default( int ) ) { _value = initialValue; Init(); }
	void Init()
	{
		if( _onChange == null ) _onChange = new EventSlot<int>();
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize() { }
	void ISerializationCallbackReceiver.OnAfterDeserialize() { Init(); }
	

	public override string ToString() { return $"IS({_value})"; }
}