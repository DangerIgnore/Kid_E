using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Clap {

    [System.Serializable] public class FloatEvent : UnityEvent<float> { }
    [System.Serializable] public class DualFloatEvent : UnityEvent<float, float> { }
    [System.Serializable] public class FloatArrayEvent : UnityEvent<float[]> { }

}
