using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NGC6543
{
	public class WeightedListTest : MonoBehaviour
	{
		public WeightedTransformList _weightedTransformList;
		
		[Header("Test")]
		[SerializeField, Range(10, 100000)] int _maxPickCount = 100;
		
		List<int> _pickedIndexes = new List<int>();
		
		public void PickTest()
		{
			_pickedIndexes.Clear();
			for (int i = 0; i < _maxPickCount; i++)
			{
				_weightedTransformList.PickWeightedRandom();
				_pickedIndexes.Add(_weightedTransformList.LastPickIndex);
			}
			
			// Evaluate the result
			
			// # of unable-to-pick count
			int error = 0;
			// Number of picked indexes
			int[] pickedResult = new int[_weightedTransformList.Count];
			
			for (int i = 0; i < _pickedIndexes.Count; i++)
			{
				if (_pickedIndexes[i] == -1)
				{
					error++;
				}
				else
				{
					pickedResult[_pickedIndexes[i]]++;
				}
			}
			string result = "Test finished. Total # of Trial : " + _maxPickCount;
			result += "\n# of errors : " + error.ToString();
			result += "\nPick counts for each items : ";
			for (int i = 0; i < pickedResult.Length; i++)
			{
				result += "\n[" + i + "] : " + pickedResult[i].ToString();
			}
			
			Debug.Log(result);
		}
	}
	
	
	#if UNITY_EDITOR
	[CustomEditor(typeof(WeightedListTest))]
	public class WeightedListTestEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			EditorGUILayout.Space();
			
			if (GUILayout.Button("Pick"))
			{
				((WeightedListTest)target).PickTest();
			}
		}
	}
	
	#endif	// UNITY_EDITOR	
}
