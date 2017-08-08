using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TowerDefense {
    /// <summary>
    /// Sorting comparer that sorts objects first by lowest Y, then by lowest X.  By using this comparer, objects will be drawn
    /// top to bottom, left to right, like reading English.
    /// </summary>
    public class EnglishSort : IComparer<object> {
        public int Compare(object a, object b) {
            // Assertions and type identification
            Debug.Assert(a != null, "Violation of : a is not null.");
            Debug.Assert(b != null, "Violation of : b is not null.");

            Type[] types = { typeof(GameplayObject) };
            Type typeOfA = null;
            Type typeOfB = null;

            foreach (Type t in types) {
                if (a.GetType().IsSubclassOf(t)) {
                    typeOfA = t;
                }

                if (b.GetType().IsSubclassOf(t)) {
                    typeOfB = t;
                }
            }

            Debug.Assert(typeOfA != null, "Violation of : a is of drawable type.");
            Debug.Assert(typeOfB != null, "Violation of : b is of drawable type.");

            int aX = 0, aY = 0, bX = 0, bY = 0;

            // Get coordinates of a.
            if (typeOfA == typeof(GameplayObject)) {
                GameplayObject temp = (GameplayObject)a;
                aX = temp.Pos.X;
                aY = temp.Pos.Y;
            }

            // Get coordinates of b.
            if (typeOfB == typeof(GameplayObject)) {
                GameplayObject temp = (GameplayObject)b;
                bX = temp.Pos.X;
                bY = temp.Pos.Y;
            }

            // Compare.

            if (aY > bY) {
                return 1;
            } else if (aY < bY) {
                return -1;
            } else // aY == bY
              {
                if (aX > bX) {
                    return 1;
                } else if (aX < bX) {
                    return -1;
                } else // aX == bX
                  {
                    return 0;
                }
            }
        }
    }
}
