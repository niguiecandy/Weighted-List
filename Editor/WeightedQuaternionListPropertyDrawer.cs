using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace NGC6543
{
	[CustomPropertyDrawer(typeof(WeightedQuaternionList))]
	public class WeightedQuaternionListPropertyDrawer : WeightedListPropertyDrawer 
	{
		protected override void DrawItemElement(Rect rect, SerializedProperty itemProperty)
		{
			float cachedLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 15f;

			Quaternion q = itemProperty.quaternionValue;
			Rect r = rect;
			r.width /= 4f;
			q.x = EditorGUI.FloatField(r, "X", itemProperty.quaternionValue.x);
			r.x += r.width;
			q.y = EditorGUI.FloatField(r, "Y", itemProperty.quaternionValue.y);
			r.x += r.width;
			q.z = EditorGUI.FloatField(r, "Z", itemProperty.quaternionValue.z);
			r.x += r.width;
			q.w = EditorGUI.FloatField(r, "W", itemProperty.quaternionValue.w);
			itemProperty.quaternionValue = q;

			EditorGUIUtility.labelWidth = cachedLabelWidth;
		}
	}	
}
