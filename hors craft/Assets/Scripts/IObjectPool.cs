// DecompilerFi decompiler from Assembly-CSharp.dll class: IObjectPool
public interface IObjectPool
{
	bool GetObject(int id, out ObjectPoolItem newObject);

	bool ReturnObject(int id, ObjectPoolItem objectToReturn);
}
