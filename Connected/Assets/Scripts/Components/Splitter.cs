using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : GeneralComponent
{
    private float ratio;
    public Wire secondPositive { get; set; }

    public (float, GeneralComponent) CheckSplitter()
    {
        float resistance1 = 0.0f, resistance2 = 0.0f, totRes;
        GeneralComponent combiner1, combiner2;

        if (CheckConnection(positive) && CheckConnection(secondPositive)) {
            (resistance1, combiner1) = CheckSplitWire(positive.positive);
            (resistance2, combiner2) = CheckSplitWire(secondPositive.positive);
        } else {
            combiner1 = null; combiner2 = null;
        }

        // If combiner1 isnt null and combiner2 is null it will catch after or sign.
        if (combiner1 == null || (combiner1 != combiner2)) {
            // TODO: Add appropriate error message
            return (0, null);
        }

        // From an actual formula
        totRes = 1 / ((1/resistance1) + (1/resistance2));
        ratio = resistance1 / (resistance1+resistance2);

        return (totRes, combiner1);

    }
    // nextComponent is initially the first component AFTER the splitter.
    // Stuff went wrong when (0, null) is returned.
    private (float, GeneralComponent) CheckSplitWire(GeneralComponent nextComponent)
    {
        float res, resistanceSum = 0.0f;
        while (nextComponent.GetType() != typeof(Combiner))
        {
            if (nextComponent.GetType() == typeof(Splitter)) {
                // Recasting the splitter to use CheckSplitter()
                Splitter splitter = (Splitter)nextComponent;
                (res, nextComponent) = splitter.CheckSplitter();
                resistanceSum += res;
            } else if (nextComponent.GetType() == typeof(Battery)) {
                //TODO: Add appropriate error message.
                return (0, null);
            } else if (nextComponent == null) {
                return (0, null);
            } else {
                resistanceSum += nextComponent.resistance;
                if (CheckConnection(nextComponent.positive)) {
                    nextComponent = nextComponent.positive.positive;
                }
            }
        }
        return (resistanceSum, nextComponent);
    }
    public GeneralComponent ResolveSplitter(float current)
    {
        // No controls made due to them being made in the check.
        GeneralComponent returnComponent = ResolveSplitWire(current*ratio, positive.positive);
        returnComponent = ResolveSplitWire(current*(1-ratio), secondPositive.positive);

        // Returns what comes after the combiner.
        returnComponent.positive.ShowCurrent();
        return returnComponent.positive.positive;
    }
    private GeneralComponent ResolveSplitWire(float current, GeneralComponent nextComponent) 
    {
        while (nextComponent.GetType() != typeof(Combiner)) {
            if (nextComponent.GetType() == typeof(Splitter)) {
                nextComponent = ResolveSplitter(current);
            } else {
                nextComponent.current = current;
                nextComponent.positive.ShowCurrent();
                nextComponent = nextComponent.positive.positive;
            }
        }
        return nextComponent;
    }
}