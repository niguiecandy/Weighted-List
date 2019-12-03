using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Some of the implemented weighted lists.
 */

namespace NGC6543
{
    [System.Serializable]
    public class WeightedIntList : WeightedList<int> {}

    [System.Serializable]
    public class WeightedFloatList : WeightedList<float> {}
    
    [System.Serializable]
    public class WeightedVector2List : WeightedList<Vector2> {}

    [System.Serializable]
    public class WeightedVector3List : WeightedList<Vector3> {}

    [System.Serializable]
    public class WeightedVector4List : WeightedList<Vector4> {}

    [System.Serializable]
    public class WeightedQuaternionList : WeightedList<Quaternion> {}

    [System.Serializable]
    public class WeightedColorList : WeightedList<Color> {}

    [System.Serializable]
    public class WeightedGameObjectList : WeightedList<GameObject> {}
    
    [System.Serializable]
    public class WeightedTransformList : WeightedList<Transform> {}
    
	[System.Serializable]
	public class WeightedAnimationCurveList : WeightedList<AnimationCurve> {}
	
}