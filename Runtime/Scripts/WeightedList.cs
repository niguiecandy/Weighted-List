using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    WeightedList<T>
    
    A List with a weight for each items.
	
 */

namespace NGC6543
{
	/// <summary>
	/// DON'T EXTEND THIS : Extend WeightedList<T> INSTEAD!
	/// </summary>
	[System.Serializable]
	public abstract class AbstractWeightedList
	{}

	public class WeightedList<T> : AbstractWeightedList
	{
		[SerializeField] List<T> _items;
		[SerializeField] List<int> _weights;
		
		[SerializeField] int _sumOfWeights;
		
		// Used for WeightedListPropertyDrawer
		[SerializeField] bool _foldOut;
		[SerializeField] bool _showProgressBar;
		
		/// <summary>
		/// Last pick index.
		/// -1 : pick wasn't performed before, or pick failed
		/// </summary>
		int _lastPickIndex = -1;
		
		public T this [int index]
		{
			get
			{
				return _items[index];
			}
			set
			{
				_items[index] = value;
			}
		}
		
		public int Count
		{
			get
			{
				return _items.Count;
			}
		}
		
		/// <summary>
		/// Sum of all the weights. Should be equal or greater than 0.
		/// </summary>
		/// <value></value>
		public int SumOfWeights
		{
			get
			{
				if (_sumOfWeights <= 0) UpdateSumOfWeights();
				return _sumOfWeights;
			}
		}
		
		/// <summary>
		/// Returns the last pick index.
		/// </summary>
		/// <value>-1 : pick failed.</value>
		public int LastPickIndex
		{
			get
			{
				return _lastPickIndex;
			}
		}
		
		public WeightedList()
		{
			_items = new List<T>();
			_weights = new List<int>();
		}
		
		~WeightedList()
		{
			_items.Clear();
			_weights.Clear();
		}
		
		/// <summary>
		/// Adds an item and set its weight.
		/// </summary>
		/// <param name="item"></param>
		public void Add(T item, int weight = 1)
		{
			_items.Add(item);
			_weights.Add(weight);
			UpdateSumOfWeights();
		}
		
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= _items.Count)
			{
				return;
			}
			_items.RemoveAt(index);
			_weights.RemoveAt(index);
			UpdateSumOfWeights();
		}
		
		public void Clear()
		{
			_items.Clear();
			_weights.Clear();
			UpdateSumOfWeights();
		}
		
		/*
			Updates sum of weights.
			If items count is 0, set it to 0.
			If a weight is negative, set it to zero.
		 */
		void UpdateSumOfWeights()
		{
			if (_weights.Count == 0)
			{
				_sumOfWeights = 0;
				return;
			}
			
			for (int i = 0; i < _weights.Count; i++)
			{
				if (_weights[i] < 0)
				{
					Debug.LogWarning("Item " + i + "'s Weight is negative! Force it to 0.");
					_weights[i] = 0;
				}
				_sumOfWeights += _weights[i];
			}
			Debug.Log("Sum of Weights : " + _sumOfWeights);
			
			// HACK if the sum of weights is zero, inform user!
			if (_sumOfWeights == 0)
			{
				Debug.LogError("The sum of weights is zero! Nothing will be picked!");
			}
		}
		
		/// <summary>
		/// Returns an unweighted randomly selected item.
		/// </summary>
		/// <returns></returns>
		public T PickEvenRandom()
		{
			int index = PickEvenRandomIndex();
			if (index == -1)
			{
				return default(T);
			}
			return _items[index];
		}
		
		/// <summary>
		/// Returns an unweighted randomly selected item index. 
		/// </summary>
		/// <returns></returns>
		public int PickEvenRandomIndex()
		{
			if (_items.Count == 0)
			{
				_lastPickIndex = -1;
				return -1;
			}
			_lastPickIndex = UnityEngine.Random.Range(0, _items.Count);
			return _lastPickIndex;
		}
		
		/// <summary>
		/// Returns a weighted-randomly selected item.
		/// </summary>
		/// <returns></returns>
		public T PickWeightedRandom()
		{
			int index = PickWeightedRandomIndex();
			if (index == -1)
			{
				return default(T);
			}
			return _items[index];
		}
		
		/// <summary>
		/// Returns an index of a weighted-randomly selected item.
		/// </summary>
		/// <returns>Index of a picked item</returns>
		public int PickWeightedRandomIndex()
		{	
			if (_items.Count == 0 || _sumOfWeights == 0)
			{
				// Debug.LogError("The sum of weights is 0! Nothing can be picked!");
				_lastPickIndex = -1;
				return -1;
			}
			
			// HACK random value should be bigger than 0. Otherwise, an item of weight 0 might be picked!
			int randomValue = UnityEngine.Random.Range(1, _sumOfWeights + 1);
			// Debug.Log("RandomValue : " + randomValue);

			// HACK randomize the starting index to reduce index order influence.
			int resultIndex = UnityEngine.Random.Range(0, _items.Count);

			while (true)
			{
				randomValue -= _weights[resultIndex];
				if (randomValue <= 0) break;
				resultIndex++;
				resultIndex = (resultIndex == _items.Count) ? 0 : resultIndex;
			}
			// Debug.Log("sumOfWeights : " + _sumOfWeights + ", picked Index : " + resultIndex);
			_lastPickIndex = resultIndex;
			return resultIndex;
		}
		
		/// <summary>
		/// Returns a weighted probability of a given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns>-1 if index is out of range.</returns>
		public float GetProbability (int index)
		{
			float result = -1;
			try
			{
				result = _weights[index] / (float)SumOfWeights;
			}
			catch (System.ArgumentOutOfRangeException e)
			{
				return result;
			}
			return result;
		}
	}
}