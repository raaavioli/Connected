using UnityEngine;

private abstract class GeneralComponent : MonoBehaviour
{
    public Wire ref positive { get; set; };
    public Wire ref negative { get; set; };
    public float current;
    public float resistance {get; protected set;};

    public static bool CheckConnection(Wire positive)
    {
        if (positive == NULL) return false;
        else return positive.positive != null;
    }
}

public class PowerSource : GeneralComponent
{
    protected float voltage;

}

public class Battery : PowerSource
{

    public void Battery(float volt, float ampere) 
    {
        voltage = volt;
        current = ampere;
    }

    public List<PowerSource> CheckCircuit() // Returns other batteries that is in the circuit for gamemanager
    {
        GeneralComponent nextComponent;
        List<PowerSource> foundPowerSources = new List<PowerSource>;
        float resistanceSum, voltageSum;

        if (CheckConnection(positive)) nextComponent = positive.positive;

    
        while(nextComponent != this) {
            resistanceSum += nextComponent.resistance;
            if (nextComponent.GetType() == typeof(PowerSource)) {
                foundPowerSources.Add(nextComponent);
                voltageSum += nextComponent.voltage;
            }

            if (CheckConnection(nextComponent.positive)) nextComponent = positive.positive;
            else break;
        }
        voltageSum += this.voltage;
        if (nextComponent == this) ResolveCircuit(resistanceSum, voltageSum);

        return foundPowerSources;
    }

    private void ResolveCircuit(float resistance, float voltage) // Can safely assume closed circuit
    {
        float current = voltage / (resistance + Mathf.Epsilon; //TODO: Explode batteries or whatever
        this.current = current;
        GeneralComponent currentComponent = this.positive.positive;

        while(currentComponent  != this) {
            currentComponent.current = current;
            currentComponent = currentComponent.positive.positive;
        }
    }
}

public class Light : GeneralComponent
{
    private float requiredPower;
    public void Light(float ohm, float watt)
    {
        resistance = ohm;
        requiredPower = watt;
    }

    void Update()
    {
        //TODO: Animate if  watt is high enough
    }
}

public class Wire : Monobehaviour
{
    public Component ref positive { get; set; };
    public Component ref negative { get; set; };
}