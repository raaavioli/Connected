using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combiner : GeneralComponent
{
        public Wire secondNegative { get; set; }

        public GeneralComponent ResetCombiner()
        {
            GeneralComponent firstComponent, secondComponent;
            if (negative != null) {
                negative.HideCurrent();
                firstComponent = ResetCombWire(negative)
            } else {
                firstComponent = null;
            }
            if (secondNegative != null) {
                secondComponent.HideCurrent();
                secondComponent = ResetCombWire(secondNegative);
            } else {
                secondComponent = null;
            }

            if (firstComponent ==  secondComponent) { // Assumes only one break per reset call.
                return firstComponent;
            } else {
                return null;
            }
        }
        private GeneralComponent ResetCombWire(wire negative) 
        {
        GeneralComponent nextComponent
        nextComponent = this;

        while (nextComponent != null) 
        {
            nextComponent.current = 0.0f;

            if (nextComponent.negative != null) {
                nextComponent.negative.HideCurrent();
            }
            if (CheckConnection(nextComponent.negative, false)) {
                nextComponent = nextComponent.negative.negative;
            } else {
                return null;
            }

            if (nextComponent.GetType() == typeof(Combiner)) {
                Combiner combiner = (Combiner)nextComponent;
                nextComponent = combiner.ResetCombiner();
            } else if (nextComponent.GetType() == typeof(Splitter)) {
                return nextComponent;
            } 
        }
        return nextComponent;
        }
}
