using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Serializable : MonoBehaviour
{
    public abstract byte[] Serialize();
    public abstract void Deserialize(byte[] data);
    public abstract void Smooth(byte[] _oldData, byte[] _newData);
}

