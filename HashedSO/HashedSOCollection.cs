using System;
using System.Collections.Generic;
using System.Linq;
using GitHub.Unity;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;





public abstract class HashedSOCollection<T> : BaseHashedSOCollection, IEnumerable<T> where T : HashedScriptableObject
{
	//[Serializable] public class StringStringDictionary : SerializableDictionary<int, T> { }

	
	[FormerlySerializedAs("_list")][SerializeField] List<T> _listOld = new List<T>();
	
	[SerializeField]
	private SerializableDictionary<int, T> objDic;	
	
//	public T this[int index] => objDic[index];
//	public T this[int index] => objDic.GetValuesList()[index];  // USING POS IN VALUE  TODO: SEE IF NEED TO CREATE A METHOD TO GET BY ID
	public T this[int hashId] => objDic[hashId];   // USING POS IN VALUE  TODO: SEE IF NEED TO CREATE A METHOD TO GET BY ID
	public override int Count => objDic.Count;

	public T GetElement(int hashId) => this[hashId];  //TODO: SEE IF NEED TO CREATE A METHOD TO GET BY ID

	public T GetRandomElement()
	{
		return  objDic.ElementAt(Random.Range(0, objDic.Count)).Value;
	}
	protected override bool DicHasIDKey(int hashID)
	{
		return objDic.ContainsKey(hashID);
	}

	//public T GetElement(int hashId) => objDic[hashId];  
	protected override IHashedSO GetIHashSOByKey(int hashID)
	{
		return objDic[hashID];
	}

	public override int GetDicIDbyElement(IHashedSO element)
	{
		return objDic.FirstOrDefault(x => x.Value == element).Key;
	}
	public T GetElementOrDefault(int hashId)
	{
		if (ContainsHashID(hashId)) return objDic[hashId];  
		return default(T);
	}
	public override System.Type GetElementType() => typeof(T);

	public override IHashedSO GetElementBase(int hashId)
	{
		//Debug.Log("<><>< hashId: "+hashId + " Count: "+objDic.GetValuesList().Count);

		//if (hashId >= objDic.GetValuesList().Count) return null;
		//if (!ContainsHashID(hashId)) return null;
		//return	objDic.GetValuesList()[hashId]; //   this[hashId];


		if (!ContainsHashID(hashId))
		{
			Debug.LogError($" HASH ID DO NOT CONTAINS ON DICTIONARY, WILL RETURN NULL - Check why it is asking for this ID");
			return null;
		}
		
		return objDic[hashId];
		
		
	}

	//public override bool ContainsHashID(int hashID) => (hashID < objDic.Count || objDic[hashID] != null);

	public override bool ContainsHashID(int hashID) => (objDic.ContainsKey(hashID) && objDic[hashID] != null);
	public bool TryGetElement(int hashID, out T element) => objDic.TryGetValue(hashID, out element);
	public override bool Contains( IHashedSO obj ) => objDic.ContainsKey( (obj as T).HashID );

	public IEnumerator<T> GetEnumerator() => objDic.Values.GetEnumerator();  //TODO: VALIDATE FOR REAL

#if UNITY_EDITOR

	protected override void Clear() 
	{ 
		objDic.Clear();
		UnityEditor.EditorUtility.SetDirty( this );
	}
	
	public override bool EditorCanChangeIDsToOptimizeSpace => true;

	public override void Editor_HACK_Remove(int id)
	{
	//	objDic[id] = null;
		Debug.Log($"<><><><> HACK REMOVE ID: {id}");// {obj.ToString()} id: {obj.HashID}");

		objDic.Remove(id);
	}

	protected override bool AddElement(IHashedSO obj)
	{
		if( obj is T t )
			objDic.Add( t.HashID, t );

		return ( obj is T );
	}


	protected override bool ResolveConflictedFile( IHashedSO t, string assetPath ) => true;
	
	public override bool TryResolveConflict( int i )
	{
		if( i >= Count ) return false;
		var element = objDic[i];
		var realId = element.HashID;
		if( realId == i ) return false;
		if( objDic[realId] != null ) return false;
		objDic[realId] = element;
		objDic[i] = null;
		UnityEditor.EditorUtility.SetDirty( this );
		return true;
	}
	
	protected override bool SetRealPosition( IHashedSO obj ) //TODO: why this change id
	{
//		Debug.Log("!!!!!TRYING ADDING NULL" );
		var t = obj as T;
		if( t == null ) return false;
		var id = obj.HashID;
		if( id < 0 ) return false;
		Debug.Log($"!!!!!ADICIONAR SetRealPosition obj: {obj.ToString()} id: {id}, objDic.Count: {objDic.Count} " );
		while (id >= objDic.Count)
		{
	//	Debug.Log($"!!!!!<>< while id: {id}, >= objDic.Count: {objDic.Count} " );
			objDic.Add( Random.Range(-10000000,-1),null);
		}
		objDic[id] = t;
		UnityEditor.EditorUtility.SetDirty( this );
		
		return true;
	}
	
#endif

	public override string ToString()// TODO: NEED FIX
	{
		//string s = "";
		//for( int i = 0; i < objDic.Count; i++ )
		//{
		//	
		//	//var element = objDic[i];  // THIS SHOULD BE ID AND NOT POS
		//	if( element == null ) s += $"[{i}] => NULL\n";
		//	else s += $"[{i}] => {objDic[i].NameOrNull()}[{objDic[i].HashID}]\n";
		//}
		//return $"{this.GetType()}:\n{s}";

		
		//string s = "";
		//for( int i = 0; i < objDic.Count; i++ )
		//{
		//	Debug.Log($"<><>Dicionario count : {objDic.GetValuesList().Count}  objdic count: {objDic.Count}");
		//	var element = GetElementBase(i);// objDic.GetValuesList()[i];  // THIS SHOULD BE ID AND NOT POS
		//	if( element == null ) s += $"[{i}] => NULL\n";
		//	else s += $"[{i}] => {objDic.GetValuesList()[i].NameOrNull()}[{objDic.GetValuesList()[i].HashID}]\n";
		//}
		//return $"{this.GetType()}:\n{s}";

		string s = "";
		int currentElementIteration = 0;
		foreach (KeyValuePair<int,T> entry in objDic)
		{
//			Debug.Log($"<><>Quantidade de values count : {objDic.GetValuesList().Count}  objdic count: {objDic.Count}");
			var element = entry.Value;
			if (element == null)

			{
				Debug.Log("ACHOU NULL.");
				s += $"[{currentElementIteration}] => NULL\n";
			}
			else s += $"[{currentElementIteration}] => {element.NameOrNull()}[{element.HashID}]\n";

			currentElementIteration++;
		}
		//string s = "";
		//s += $"NULL\n";
		return $"{this.GetType()}:\n{s}";

	}



	public override List<IHashedSO> CheckNullInDic()
	{
		List<IHashedSO> _elementsToRemove = new List<IHashedSO>();
		List<int> _keysToRemove = new List<int>();

		
		foreach (KeyValuePair<int, T> pair in objDic)
		{
			T value = pair.Value;
			try
			{
				var name =	pair.Value.ToString();
			}
			catch (Exception e)
			{
			//	Console.WriteLine(e);
				_keysToRemove.Add(pair.Key);
			//	throw;
			}
		
		}

		foreach (var key in _keysToRemove)
		{
			Editor_HACK_Remove( key  );
		}

	


		return _elementsToRemove;
 
	}
//	private int SetRandomID()
//	{
//		Debug.Log($"<><>PlaceholderCount Before : {PlaceholderCount}");
//		return Random.Range(PlaceholderCount, int.MaxValue);
//	
//	}
}