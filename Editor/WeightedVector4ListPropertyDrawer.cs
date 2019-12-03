using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
namespace NGC6543
{
	[CustomPropertyDrawer(typeof(WeightedVector4List), true)]
	public class WeightedVector4ListPropertyDrawer : WeightedListPropertyDrawer
	{
		protected override void DrawItemElement(Rect rect, SerializedProperty itemProperty)
		{
			float cachedLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 15f;
			
			Vector4 v = itemProperty.vector4Value;
			Rect r = rect;
			r.width /= 4f;
			v.x = EditorGUI.FloatField(r, "X", itemProperty.vector4Value.x);
			r.x += r.width;
			v.y = EditorGUI.FloatField(r, "Y", itemProperty.vector4Value.y);
			r.x += r.width;
			v.z = EditorGUI.FloatField(r, "Z", itemProperty.vector4Value.z);
			r.x += r.width;
			v.w = EditorGUI.FloatField(r, "W", itemProperty.vector4Value.w);
			itemProperty.vector4Value = v;
			
			EditorGUIUtility.labelWidth = cachedLabelWidth;
		}
		
	}	
}
