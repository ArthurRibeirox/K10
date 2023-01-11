using UnityEngine;


[System.Serializable]
public abstract class BaseCollectionElementSoftReference
{
    public abstract void DisposeAsset();
    public abstract IHashedSO GetBaseReference();
}

[System.Serializable]
public class CollectionElementSoftReference<T> : BaseCollectionElementSoftReference where T : UnityEngine.ScriptableObject, IHashedSO
{
#if UNITY_EDITOR
    [SerializeField] T _assetHardReference;
    [SerializeField] EAssetReferenceState _referenceState = EAssetReferenceState.Empty;
#endif //UNITY_EDITOR
    T _assetRuntimeReference;
    [SerializeField] int _id;

	private static T _dummyInstance = null;
	private static T Dummy => _dummyInstance ?? ( _dummyInstance = ScriptableObject.CreateInstance<T>() );

	public override void DisposeAsset()
	{
		_assetRuntimeReference = null;
		_referenceState = EAssetReferenceState.Empty;
	}

	private void EDITOR_UpdateDataFromRef()
	{
		_id = _assetHardReference?.HashID ?? -1;
	}

	public override IHashedSO GetBaseReference() => GetReference();

	public T GetReference()
	{
		_assetRuntimeReference = (T)Dummy.GetCollection().GetElementBase( _id );
		_referenceState = _assetRuntimeReference != null ? EAssetReferenceState.Loaded : EAssetReferenceState.LoadedNull;
		return _assetRuntimeReference;
	}
}
