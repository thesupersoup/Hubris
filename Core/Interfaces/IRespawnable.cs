﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Represents an Entity which can respawn into the world
    /// </summary>
    public interface IRespawnable
    {
        void Respawn();
    }
}
