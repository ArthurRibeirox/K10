using UnityEngine;

public abstract class CodeTimingMeasuresExhibitor : MonoBehaviour
{
	protected abstract void SetLog( string log );
	protected abstract void OnEnableChange( bool enabled );

	void OnEnable()
	{
		CodeTimingMeasures.Enable();
	}

	void OnDisable()
	{
		CodeTimingMeasures.Disable();
	}

	void LateUpdate()
	{
		SetLog( CodeTimingMeasures.GetLog() );
		CodeTimingMeasures.Clear();
	}

	void OnApplicationQuit()
	{
		Debug.Log( "CodeTiming Measures:\n" + CodeTimingMeasures.Specific.GetLog() );
	}

	bool debugSpecificMesure = false;
	const string KEY = "Fake Measure";
	void Update()
	{
		if( Input.GetKeyDown( KeyCode.F10 ) )
		{
			if( debugSpecificMesure ) CodeTimingMeasures.Specific.LogStart( KEY );
			else CodeTimingMeasures.Specific.LogEnd( KEY );
			debugSpecificMesure = !debugSpecificMesure;
		}
	}
}
