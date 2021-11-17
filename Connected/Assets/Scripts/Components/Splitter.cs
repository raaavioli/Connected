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
            return (0, null);
        }

        // Actual formula
        totRes = 1/((1/resistance1) + (1/resistance2));
        ratio = resistance1 / (resistance1+resistance2);

        return (totRes, combiner1);
    }

    private (float, GeneralComponent) CheckSplitWire(GeneralComponent nextComponent)
    {
        float res, resistanceSum = 0.0f;
        while(nextComponent.GetType() != typeof(Combiner)) 
        {
            if(nextComponent.GetType() == typeof(Splitter)) {
                // Recasting the splitter to use CheckSplitter()
                Splitter splitter = (Splitter)nextComponent;
                (res, nextComponent) = splitter.CheckSplitter();
                resistanceSum += res;
            } else if(nextComponent == null) {
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
}