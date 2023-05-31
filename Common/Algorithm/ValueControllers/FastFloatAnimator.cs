using UnityEngine;

public struct FastFloatAnimator
{
	public float _min;
    public float _max;
    public float _acceleration;
    public float _deacceleration;
    public float _currentValue;
    public float _desiredValue;
    public float _currentSpeed;
    public float _maximumSpeed;
    
    public FastFloatAnimator( float baseValue ) : this( 0, baseValue, baseValue, baseValue ) { }
    public FastFloatAnimator( float initialValue, float accel, float deaccel, float maximumSpeed, float min = float.MinValue, float max = float.MaxValue )
    {
        _acceleration = accel;
        _deacceleration = deaccel;
        _maximumSpeed = maximumSpeed;
        _desiredValue = _currentValue = initialValue;
		_currentSpeed = 0;
	    _min = min;
        _max = max;
    }

    public void SetDesire( float desired )
    {
		var diff = desired - _desiredValue;
		if( diff < float.Epsilon && diff > FloatHelper.NegativeEpsilon )
			return;

        if( desired > _max ) _desiredValue = _max;
        else if( desired < _min ) _desiredValue = _min;
        else _desiredValue = desired;
    }

    public void Update( float deltaTime )
    {
		if( deltaTime < float.Epsilon ) return;

        var diff = _desiredValue - _currentValue;

        if( diff < float.Epsilon && diff > FloatHelper.NegativeEpsilon ) return;

		var diffSign = 1f;
		if( diff < float.Epsilon ) diffSign = -1f;

        var acceleratedSpeed = _currentSpeed + diffSign * _acceleration * deltaTime;
        var deacceleratedSpeed = Mathf.Sqrt( 2 * _deacceleration * diff * diffSign );

		var speedDirectionSign = 1f;
		if( acceleratedSpeed < float.Epsilon ) speedDirectionSign = -1f;

		var speedAbs = acceleratedSpeed * speedDirectionSign;
		if( speedAbs > deacceleratedSpeed ) speedAbs = deacceleratedSpeed;
		if( speedAbs > _maximumSpeed ) speedAbs = _maximumSpeed;
        _currentSpeed = speedDirectionSign * speedAbs;

		var step = _currentSpeed * deltaTime;

		if( step > float.Epsilon ) // Not 0 == !Mathf.Approximately( step, 0 )
		{
			if( step + float.Epsilon > diff )
	        {
	            _currentValue = _desiredValue;
	            _currentSpeed = 0;
	        }
	        else
	        {
				_currentValue += step;

        		var newDiff = _desiredValue - _currentValue;
				if( newDiff < float.Epsilon && newDiff > FloatHelper.NegativeEpsilon ) //if( Mathf.Approximately( _current, _desired ) )
				{
					_currentValue = _desiredValue;
					_currentSpeed = 0;
	            }
	        }
		}
		else if( step < FloatHelper.NegativeEpsilon )
		{
			if( step - float.Epsilon < diff )
	        {
	            _currentValue = _desiredValue;
	            _currentSpeed = 0;
	        }
	        else
	        {
				_currentValue += step;

        		var newDiff = _desiredValue - _currentValue;
				if( newDiff < float.Epsilon && newDiff > FloatHelper.NegativeEpsilon ) //if( Mathf.Approximately( _current, _desired ) )
				{
					_currentValue = _desiredValue;
	            	_currentSpeed = 0;
	            }
	        }
		}
    }
}
