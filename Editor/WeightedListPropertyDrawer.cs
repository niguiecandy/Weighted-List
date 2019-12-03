using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditorInternal;
using System;

namespace NGC6543
{
	[CustomPropertyDrawer(typeof(AbstractWeightedList), true)]
	public class WeightedListPropertyDrawer : PropertyDrawer
	{
		const float c_helpboxHeight = 30f;
		
		SerializedProperty _items;
		SerializedProperty _weights;
		SerializedProperty _sumOfWeights;
		
		SerializedProperty _foldOut;
		SerializedProperty _showProgressBar;
		
		ReorderableList _list;
		float _propertyHeight;
		
		//=== Flags
		bool _isHelpboxShown = false;
		
		
		//--------------------------------------------------- PROPERTIYDRAWER_OVERRIDES
		#region PROPERTYDRAWER_OVERRIDES
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_items = property.FindPropertyRelative("_items");
			_weights = property.FindPropertyRelative("_weights");
			_sumOfWeights = property.FindPropertyRelative("_sumOfWeights");
			
			_foldOut = property.FindPropertyRelative("_foldOut");
			_showProgressBar = property.FindPropertyRelative("_showProgressBar");
			
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();
			
			// FoldOut Label
			Rect foldOutRect = position;
			foldOutRect.height = EditorGUIUtility.singleLineHeight;
			_foldOut.boolValue = EditorGUI.Foldout(foldOutRect, _foldOut.boolValue, label, true);
			
			position.y += EditorGUIUtility.singleLineHeight;
			float listHeight = 0f;
			
			// ReorderableList
			if (_foldOut.boolValue)
			{
				ReorderableList list = GetReorderableList(property);
				list.DoList(position);
				listHeight = list.GetHeight();
			}
			
			// Inform user the critical warning.
			if (_sumOfWeights.intValue == 0)
			{
				Rect helpboxRect = position;
				helpboxRect.y += listHeight;
				helpboxRect.height = c_helpboxHeight;
				EditorGUI.HelpBox(helpboxRect, "The sum of weights is 0! Nothing will be picked!", MessageType.Error);
				_isHelpboxShown = true;
			}
			else
			{
				_isHelpboxShown = false;
			}
			
			if (EditorGUI.EndChangeCheck())
			{
				UpdateSumOfWeights();
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}
			
			EditorGUI.EndProperty();
			
			return;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			_propertyHeight = EditorGUIUtility.singleLineHeight;   // PrefixLabel
			if (_isHelpboxShown) _propertyHeight += c_helpboxHeight;
			_foldOut = _foldOut = property.FindPropertyRelative("_foldOut");
			if (_foldOut.boolValue)
			{
				_propertyHeight += GetReorderableList(property).GetHeight();			
			}

			return _propertyHeight;
		}

		#endregion // PROPERTYDRAWER_OVERRIDES

		void UpdateSumOfWeights()
		{
			int sum = 0;
			for (int i = 0; i < _weights.arraySize; i++)
			{
				if (_weights.GetArrayElementAtIndex(i).intValue < 0)
				{
					Debug.LogError("Weight should be equal or greater than 0!");
					_weights.GetArrayElementAtIndex(i).intValue = 0;
				}
				sum += _weights.GetArrayElementAtIndex(i).intValue;
			}
			_sumOfWeights.intValue = sum;
		}


		//--------------------------------------------------- REORDERABLELIST
		#region REORDERABLELIST

		ReorderableList GetReorderableList(SerializedProperty property)
		{
			if (_list == null)
			{
				_items = property.FindPropertyRelative("_items");
				_weights = property.FindPropertyRelative("_weights");

				_list = new ReorderableList(property.serializedObject, _items, true, true, true, true);
				
				_list.drawHeaderCallback = DrawHeader;
				_list.drawElementCallback = DrawElement;
				_list.onAddCallback = AddElement;
				_list.onRemoveCallback = RemoveElement;
				_list.onReorderCallbackWithDetails = ReorderDetailed;
				_list.showDefaultBackground = true;
				_list.elementHeight = GetItemElementHeight();
				
			}
			return _list;
		}

		private void ReorderDetailed(ReorderableList list, int oldIndex, int newIndex)
		{
			//UNDONE _items is controlled by ReorderableList.
			// _items.MoveArrayElement(oldIndex, newIndex);
			_weights.MoveArrayElement(oldIndex, newIndex);
		}

		void AddElement(ReorderableList list)
		{
			_items.arraySize++;
			_weights.arraySize++;
			if (_items.arraySize != _weights.arraySize)
			{
				Debug.Log("Items count : " + _items.arraySize + "\nWeights Count : " + _weights.arraySize);
				Debug.LogError("Items count and Weights count does not match! Fixing...");
				_weights.arraySize = _items.arraySize;
			}
		}

		private void RemoveElement(ReorderableList list)
		{	
			_items.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
			_items.DeleteArrayElementAtIndex(list.index);
			_weights.DeleteArrayElementAtIndex(list.index);
			if (_items.arraySize != _weights.arraySize)
			{
				Debug.Log("Items count : " + _items.arraySize + "\nWeights Count : " + _weights.arraySize);
				Debug.LogError("Items count and Weights count does not match! Fixing...");
				_weights.arraySize = _items.arraySize;
			}
		}


		void DrawHeader(Rect rect)
		{
			Rect itemRect = rect;
			Rect weightRect = rect;
			Rect probabilityRect = rect;

			itemRect.height = weightRect.height = probabilityRect.height = EditorGUIUtility.singleLineHeight;
			itemRect.x += 10;
			itemRect.width = rect.width * 0.5f - 10;
			weightRect.width = rect.width * 0.2f;
			probabilityRect.width = rect.width * 0.3f;

			weightRect.x = itemRect.x + itemRect.width;
			probabilityRect.x = weightRect.x + weightRect.width;
			
			DrawListHeader(itemRect, weightRect, probabilityRect);
		}
				
		void DrawListHeader(Rect itemRect, Rect weightRect, Rect probabilityRect)
		{
			EditorGUIUtility.labelWidth = 100f;
			EditorGUI.LabelField (itemRect, "Items");
			EditorGUI.LabelField (weightRect, "Weights");
			_showProgressBar.boolValue = EditorGUI.ToggleLeft(probabilityRect, "Probability(%)", _showProgressBar.boolValue);
		}

		void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			Rect itemRect = rect;
			Rect weightRect = rect;
			Rect probabilityRect = rect;

			itemRect.height = weightRect.height = probabilityRect.height = EditorGUIUtility.singleLineHeight;

			itemRect.width = rect.width * 0.5f - 5;
			weightRect.width = rect.width * 0.2f;
			probabilityRect.width = rect.width * 0.3f;

			weightRect.x = itemRect.x + itemRect.width + 5;
			probabilityRect.x = weightRect.x + weightRect.width;
			
			DrawListElement(itemRect, weightRect, probabilityRect, _items.GetArrayElementAtIndex(index), _weights.GetArrayElementAtIndex(index));
		}
		
		void DrawListElement(Rect itemRect, Rect weightRect, Rect probabilityRect, SerializedProperty itemElement, SerializedProperty weightElement)
		{
			// EditorGUI.PropertyField(itemRect, itemElement, GUIContent.none, true);
			DrawItemElement(itemRect, itemElement);
			EditorGUI.PropertyField(weightRect, weightElement, GUIContent.none);
			
			if (_sumOfWeights.intValue != 0)
			{
				if (_showProgressBar.boolValue)
				{
					EditorGUI.ProgressBar(probabilityRect, (float)weightElement.intValue / _sumOfWeights.intValue, "");
				}
				else
				{
					EditorGUI.LabelField(probabilityRect, ((float)weightElement.intValue / _sumOfWeights.intValue * 100f).ToString() + " %");
				}
			}
		}
		
		#endregion	// REORDERABLELIST
		

		//--------------------------------------------------- VIRTUAL_METHODS
		/*
			Override these methods if you need to extend a custom WeightedListPropertyDrawer.
		*/
		#region VIRTUAL_METHODS

		/// <summary>
		/// Override this method if a custom drawer for an Item is needed.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="itemProperty"></param>
		protected virtual void DrawItemElement(Rect rect, SerializedProperty itemProperty)
		{
			EditorGUI.PropertyField(rect, itemProperty, GUIContent.none);
		}

		/// <summary>
		/// Override this method if a custom element height is needed.
		/// Default : EditorGUIUtility.singleLineHeight + 2f
		/// </summary>
		/// <returns></returns>
		protected virtual float GetItemElementHeight()
		{
			return EditorGUIUtility.singleLineHeight + 2f;
		}

		#endregion // VIRTUAL_METHODS
	}
}
