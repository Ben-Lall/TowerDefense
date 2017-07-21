using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TowerDefense {
    public class DrawComparer : IComparer<object> {
        public int Compare(object a, object b) {
            // Assertions and type identification
            Debug.Assert(a != null, "Violation of : a is not null.");
            Debug.Assert(b != null, "Violation of : b is not null.");

            Type[] types = { typeof(Tower), typeof(Monster) };
            Type typeOfA = null;
            Type typeOfB = null;

            foreach (Type t in types) {
                if (a.GetType() == t) {
                    typeOfA = t;
                }

                if (b.GetType() == t) {
                    typeOfB = t;
                }
            }

            Debug.Assert(typeOfA != null, "Violation of : a is of drawable type.");
            Debug.Assert(typeOfB != null, "Violation of : b is of drawable type.");

            int aX = 0, aY = 0, bX = 0, bY = 0;

            // Get coordinates of a.
            if (typeOfA == typeof(Tower)) {
                Tower temp = (Tower)a;
                aX = temp.Pos.X;
                aY = temp.Pos.Y;
            } else if(typeOfA == typeof(Monster)) {
                Monster temp = (Monster)a;
                aX = temp.Pos.X - temp.SpriteWidth / 2;
                aY = temp.Pos.Y - temp.SpriteWidth / 2;
            }

            // Get coordinates of b.
            if (typeOfB == typeof(Tower)) {
                Tower temp = (Tower)b;
                bX = temp.Pos.X;
                bY = temp.Pos.Y;
            } else if (typeOfB == typeof(Monster)) {
                Monster temp = (Monster)b;
                bX = temp.Pos.X - temp.SpriteWidth / 2;
                bY = temp.Pos.Y - temp.SpriteWidth / 2;
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
