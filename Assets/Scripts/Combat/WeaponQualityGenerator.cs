using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WeaponProperties
{
    public float armourValue;
    public float weaponValue;
    public float speed;

    public WeaponProperties(float armour = 0f, float weapon = 0f, float spd = 0f)
    {
        armourValue = armour;
        weaponValue = weapon;
        speed = spd;
    }

    public override string ToString()
    {
        return $"Armour: {armourValue:F2}, Weapon: {weaponValue:F2}, Speed: {speed:F2}";
    }
}

[System.Serializable]
public class QualityWeights
{
    [Header("Linear Quality Function Weights")]
    public float armourWeight = 1.0f;
    public float weaponWeight = 1.2f;
    public float speedWeight = 0.8f;
    public float constantTerm = 0f;

    [Header("Property Bounds")]
    public Vector2 armourBounds = new Vector2(0f, 100f);
    public Vector2 weaponBounds = new Vector2(0f, 100f);
    public Vector2 speedBounds = new Vector2(0f, 100f);
}

public class WeaponQualityGenerator : MonoBehaviour
{
    [Header("Quality Configuration")]
    public QualityWeights qualityWeights = new QualityWeights();

    [Header("XP to Quality Mapping")]
    public AnimationCurve xpToQualityCurve = AnimationCurve.Linear(0f, 10f, 100f, 100f);
    public float qualityStandardDeviation = 3f;

    [Header("Quality Constraints")]
    public float qualityVariancePercentage = 0.3f;
    public bool enforceQualityBounds = true;

    [Header("Generation Settings")]
    public int numPropertiesToRandomize = 2;
    public float minimumQuality = 2f;
    public bool debugOutput = true;

    [Header("Player XP Input")]
    public float playerXP = 50f;

    [Header("Dynamic Property Bounds")]
    public bool useScaledBounds = true;
    public AnimationCurve boundScaleCurve = AnimationCurve.Linear(0f, 0.2f, 100f, 1f);
    
    [Header("Minimum Property Values")]
    public float minimumArmourValue = 5f;
    public float minimumWeaponValue = 5f;
    public float minimumSpeedValue = 5f;

    [Header("XP-Scaled Minimum Values")]
    public AnimationCurve minimumArmourCurve = AnimationCurve.Linear(0f, 1f, 100f, 15f);
    public AnimationCurve minimumWeaponCurve = AnimationCurve.Linear(0f, 1f, 100f, 15f);
    public AnimationCurve minimumSpeedCurve = AnimationCurve.Linear(0f, 1f, 100f, 15f);
    public bool useXPScaledMinimums = true;

    [HideInInspector]
    public WeaponProperties currentWeapon;

    public void Start()
    {
        if (xpToQualityCurve.keys.Length <= 2)
        {
            SetupDefaultQualityCurve();
        }

        if (boundScaleCurve.keys.Length <= 2)
        {
            SetupDefaultBoundCurve();
        }
        
        currentWeapon = GenerateWeapon(playerXP);
        //TestWeaponGeneration();
    }

    private void SetupDefaultQualityCurve()
    {
        Keyframe[] keys = new Keyframe[]
        {
            new Keyframe(0f, 5f),
            new Keyframe(25f, 25f),
            new Keyframe(50f, 50f),
            new Keyframe(75f, 75f),
            new Keyframe(100f, 95f),
        };
        xpToQualityCurve = new AnimationCurve(keys);
    }

    private void SetupDefaultBoundCurve()
    {
        Keyframe[] keys = new Keyframe[]
        {
            new Keyframe(0f, 0.1f),
            new Keyframe(30f, 0.4f),
            new Keyframe(60f, 0.7f),
            new Keyframe(100f, 1.0f)
        };
        boundScaleCurve = new AnimationCurve(keys);
    }

private Vector2 GetScaledBounds(Vector2 baseBounds, float xpLevel)
    {
        if (!useScaledBounds) return baseBounds;

        float scale = boundScaleCurve.Evaluate(xpLevel);
        return new Vector2(baseBounds.x, baseBounds.y * scale);
    }

    /// <summary>
    /// Define the quality function: q(p1, p2, p3) = w1*p1 + w2*p2 + w3*p3 + c
    /// </summary>
    public float CalculateQuality(WeaponProperties props)
    {
        return qualityWeights.armourWeight * props.armourValue +
               qualityWeights.weaponWeight * props.weaponValue +
               qualityWeights.speedWeight * props.speed +
               qualityWeights.constantTerm;
    }

    /// <summary>
    /// Get mean quality value for given XP Level
    /// </summary>
    public float GetMeanQuality(float xpLevel)
    {
        return xpToQualityCurve.Evaluate(xpLevel);
    }

    /// <summary>
    /// Sample quality from normal distribution around mean
    /// </summary>
    public float SampleQualityFromDistribution(float meanQuality, float xpLevel)
    {
        float sampledQuality;
        int attempts = 0;
        const int maxAttempts = 15;
        
        float minAcceptableQuality = minimumQuality;
        float maxAcceptableQuality = float.MaxValue;

        if (enforceQualityBounds)
        {
            float variance = meanQuality * qualityVariancePercentage;
            minAcceptableQuality = Mathf.Max(minimumQuality, meanQuality - variance);
            maxAcceptableQuality = meanQuality + variance;
            
            // Additional XP-based constraints
            if (xpLevel < 20f)
            {
                maxAcceptableQuality = Mathf.Min(maxAcceptableQuality, meanQuality * 1.2f);
            }
            else if (xpLevel > 80f)
            {
                minAcceptableQuality = Mathf.Max(minAcceptableQuality, meanQuality * 0.8f);
            }
        }

        do
        {
            // Box-Muller Transform for normal distribution
            float u1 = UnityEngine.Random.Range(0.001f, 0.999f);
            float u2 = UnityEngine.Random.Range(0.001f, 0.999f);

            float z0 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
            sampledQuality = meanQuality + z0 * qualityStandardDeviation;

            attempts++;
        } 
        while ((sampledQuality < minAcceptableQuality || sampledQuality > maxAcceptableQuality) && attempts < maxAttempts);

        // Fallback if couldn't get a positive value
        if (sampledQuality < minAcceptableQuality || sampledQuality > maxAcceptableQuality)
        {
            sampledQuality = Mathf.Clamp(sampledQuality, minAcceptableQuality, maxAcceptableQuality);
        }

        if (debugOutput)
        {
            Debug.Log($"Sample Quality: {sampledQuality:F2} (Range: {minAcceptableQuality:F2} - {maxAcceptableQuality:F2})");
        }

        return sampledQuality;
    }

    public WeaponProperties GenerateWeapon(float xpLevel)
    {
        // Step 1: Get target quality
        float meanQuality = GetMeanQuality(xpLevel);
        float targetQuality = SampleQualityFromDistribution(meanQuality, xpLevel);
        
        if (debugOutput)
            Debug.Log($"XP Level: {xpLevel:F1}, Target Quality: {targetQuality:F2}, (Mean: {meanQuality:F2})");
        
        // Step 2: Randomly set subset of properties
        WeaponProperties props = new WeaponProperties();
        List<PropertyType> availableProperties = new List<PropertyType>
        {
            PropertyType.Armour,
            PropertyType.Weapon,
            PropertyType.Speed
        };

        List<PropertyType> fixedProperties = new List<PropertyType>();
        for (int i = 0; i < numPropertiesToRandomize && availableProperties.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableProperties.Count);
            PropertyType selectedProp = availableProperties[randomIndex];
            fixedProperties.Add(selectedProp);
            availableProperties.RemoveAt(randomIndex);
            
            // Set random value within bounds
            SetRandomPropertyValue(props, selectedProp, xpLevel);
        }
        
        // Step 3: Solve for remaining properties using linear solver
        SolveRemainingProperties(props, targetQuality, fixedProperties, availableProperties, xpLevel);
        
        // Clamp all values to bounds
        ClampPropertiesToBounds(props, xpLevel);

        if (debugOutput)
        {
            float actualQuality = CalculateQuality(props);
            Debug.Log($"Generated Weapon: {props}");
            Debug.Log($"Actual Quality: {actualQuality:F2} (Target: {targetQuality:F2})");
            
        }

        return props;
    }

    private void SetRandomPropertyValue(WeaponProperties props, PropertyType propType, float xpLevel)
    {
        switch (propType)
        {
            case PropertyType.Armour:
                Vector2 armourBounds = GetScaledBounds(qualityWeights.armourBounds, xpLevel);
                props.armourValue = UnityEngine.Random.Range(Mathf.Max(armourBounds.x, GetMinimumPropertyValue(PropertyType.Armour, xpLevel)), armourBounds.y);
                break;
            case PropertyType.Weapon:
                Vector2 weaponBounds = GetScaledBounds(qualityWeights.weaponBounds, xpLevel);
                props.weaponValue = UnityEngine.Random.Range(Mathf.Max(weaponBounds.x, GetMinimumPropertyValue(PropertyType.Weapon, xpLevel)), weaponBounds.y);
                break;
            case PropertyType.Speed:
                Vector2 speedBounds = GetScaledBounds(qualityWeights.speedBounds, xpLevel);
                props.speed = UnityEngine.Random.Range(Mathf.Max(speedBounds.x, GetMinimumPropertyValue(PropertyType.Speed, xpLevel)),speedBounds.y);
                break;
        }
    }

    private void SolveRemainingProperties(WeaponProperties props, float targetQuality,
                                        List<PropertyType> fixedProperties,
                                        List<PropertyType> remainingProperties, float xpLevel)
    {
        // Calculate the quality contribution from fixed properties
        float fixedQualityContribution = qualityWeights.constantTerm;

        foreach (PropertyType fixedProp in fixedProperties)
        {
            switch (fixedProp)
            {
                case PropertyType.Armour:
                    fixedQualityContribution += qualityWeights.armourWeight * props.armourValue;
                    break;
                case PropertyType.Weapon:
                    fixedQualityContribution += qualityWeights.weaponWeight * props.weaponValue;
                    break;
                case PropertyType.Speed:
                    fixedQualityContribution += qualityWeights.speedWeight * props.speed;
                    break;
            }
        }
        
        // Remaining quality to achieve
        float remainingQuality = targetQuality - fixedQualityContribution;

        // Solve for remaining properties
        if (remainingProperties.Count == 1)
        {
            // Single variable case - direct solve
            SolveSingleVariable(props, remainingQuality, remainingProperties[0], xpLevel);
        }
        else if (remainingProperties.Count == 2)
        {
            PropertyType randomProp = remainingProperties[UnityEngine.Random.Range(0, remainingProperties.Count)];
            PropertyType solveProp = remainingProperties.First(p => p != randomProp);
            
            SetRandomPropertyValue(props, randomProp, xpLevel);

            float usedQuality = GetPropertyWeight(randomProp) * GetPropertyValue(props, randomProp);
            float finalRemainingQuality = remainingQuality - usedQuality;
            
            SolveSingleVariable(props, finalRemainingQuality, solveProp, xpLevel);
        }
        else if (remainingProperties.Count == 3)
        {
            // Three variable - distribute quality randomly
            DistributeQualityRandomly(props, remainingQuality, remainingProperties, xpLevel);
        }
    }

    private void SolveSingleVariable(WeaponProperties props, float remainingQuality, PropertyType propType,
        float xpLevel)
    {
        float weight = GetPropertyWeight(propType);
        if (Mathf.Abs(weight) < 0.0001f)
        {
            SetRandomPropertyValue(props, propType, xpLevel);
            return;
        }

        float value = remainingQuality / weight;

        // Get bounds for this property at this XP level
        Vector2 bounds = GetPropertyBounds(propType, xpLevel);
        float minValue = GetMinimumPropertyValue(propType, xpLevel);
        float effectiveMin = Mathf.Max(bounds.x, minValue);

        if (debugOutput)
        {
            Debug.Log($"Solving {propType}: calculated value = {value:F2}, bounds = {bounds.x:F2}-{bounds.y:F2}, effective min = {effectiveMin:F2}");
        }
    

    if (value < effectiveMin)
        {
            value = effectiveMin + UnityEngine.Random.Range(0f, (bounds.y - effectiveMin) * 0.2f);
            if (debugOutput)
                Debug.Log($"Value too low, adjusted to: {value:F2}");
        }
        else if (value > bounds.y)
        {
            value = bounds.y - UnityEngine.Random.Range(0f, (bounds.y - effectiveMin) * 0.2f);
            if (debugOutput) Debug.Log($"Value too high, adjusted to: {value:F2}");
        }
                
        SetPropertyValue(props, propType, value);
    }

    private void DistributeQualityRandomly(WeaponProperties props, float remainingQuality,
        List<PropertyType> properties, float xpLevel)
    {
        float[] ratios = new float[properties.Count];
        float totalRatio = 0f;

        for (int i = 0; i < ratios.Length; i++)
        {
            ratios[i] = UnityEngine.Random.Range(0.1f, 1f);
            totalRatio += ratios[i];
        }
        
        // Normalise ratios
        for (int i = 0; i < ratios.Length; i++)
        {
            ratios[i] /= totalRatio;
        }
        
        // Assign quality proportionally
        for (int i = 0; i < properties.Count; i++)
        {
            PropertyType propType = properties[i];
            float weight = GetPropertyWeight(propType);

            if (Mathf.Abs(weight) > 0.0001f)
            {
                float assignedQuality = remainingQuality * ratios[i];
                float value = assignedQuality / weight;

                Vector2 bounds = GetPropertyBounds(propType, xpLevel);
                float minValue = GetMinimumPropertyValue(propType, xpLevel);
                float effectiveMin = Mathf.Max(bounds.x, minValue);

                if (value < effectiveMin)
                {
                    value = effectiveMin + UnityEngine.Random.Range(0f, (bounds.y - effectiveMin) * 0.2f);
                }
                else if (value > bounds.y)
                {
                    value = bounds.y - UnityEngine.Random.Range(0f, (bounds.y - effectiveMin) * 0.2f);
                }
                
                SetPropertyValue(props, propType, value);
            }
            else
            {
                SetRandomPropertyValue(props, propType, xpLevel);
            }
        }
    }

    private float GetPropertyWeight(PropertyType propType)
    {
        switch (propType)
        {
            case PropertyType.Armour: return qualityWeights.armourWeight;
            case PropertyType.Weapon: return qualityWeights.weaponWeight;
            case PropertyType.Speed: return qualityWeights.speedWeight;
            default: return 0f;
        }
    }

    private float GetPropertyValue(WeaponProperties props, PropertyType propType)
    {
        switch (propType)
        {
            case PropertyType.Armour: return props.armourValue;
            case PropertyType.Weapon: return props.weaponValue;
            case PropertyType.Speed: return props.speed;
            default: return 0f;
        }
    }

    private float GetMinimumPropertyValue(PropertyType propType, float xpLevel)
    {
        if (useXPScaledMinimums)
        {
            switch (propType)
            {
                case PropertyType.Armour: return minimumArmourCurve.Evaluate(xpLevel);
                case PropertyType.Weapon: return minimumWeaponCurve.Evaluate(xpLevel);
                case PropertyType.Speed: return minimumSpeedCurve.Evaluate(xpLevel);
                default: return 0.1f;
            }
        }
        else
        {
            switch (propType)
            {
                case PropertyType.Armour: return minimumArmourValue;
                case PropertyType.Weapon: return minimumWeaponValue;
                case PropertyType.Speed: return minimumSpeedValue;
                default: return 0.1f;
            }
        }
        
    }

    private void SetPropertyValue(WeaponProperties props, PropertyType propType, float value)
    {
        switch (propType)
        {
            case PropertyType.Armour: props.armourValue = value; break;
            case PropertyType.Weapon: props.weaponValue = value; break;
            case PropertyType.Speed: props.speed = value; break;
        }
    }

    private Vector2 GetPropertyBounds(PropertyType propType, float xpLevel)
    {
        switch (propType)
        {
            case PropertyType.Armour:
                return GetScaledBounds(qualityWeights.armourBounds, xpLevel);
            case PropertyType.Weapon:
                return GetScaledBounds(qualityWeights.weaponBounds, xpLevel);
            case PropertyType.Speed:
                return GetScaledBounds(qualityWeights.speedBounds, xpLevel);
            default:
                return new Vector2(0f, 100f);
        }
    }

    private void ClampPropertiesToBounds(WeaponProperties props, float xpLevel)
    {
        Vector2 armourBounds = GetScaledBounds(qualityWeights.armourBounds, xpLevel);
        Vector2 weaponBounds = GetScaledBounds(qualityWeights.weaponBounds, xpLevel);
        Vector2 speedBounds = GetScaledBounds(qualityWeights.speedBounds, xpLevel);
        
        // Ensure minimum values are about 0 to prevent breaking quality calculations
        float effectiveArmourMin = Mathf.Max(armourBounds.x, GetMinimumPropertyValue(PropertyType.Armour, xpLevel));
        float effectiveWeaponMin = Mathf.Max(weaponBounds.x, GetMinimumPropertyValue(PropertyType.Weapon, xpLevel));
        float effectiveSpeedMin = Mathf.Max(speedBounds.x, GetMinimumPropertyValue(PropertyType.Speed, xpLevel));

        props.armourValue = Mathf.Clamp(props.armourValue, effectiveArmourMin, armourBounds.y);
        props.weaponValue = Mathf.Clamp(props.weaponValue, effectiveWeaponMin, weaponBounds.y);
        props.speed = Mathf.Clamp(props.speed, effectiveSpeedMin, speedBounds.y);
        
        // Debug output
        if (debugOutput)
        {
            Debug.Log($"Clamping - Armour: {effectiveArmourMin:F2}-{armourBounds.y:F2}, " +
                      $"Weapon: {effectiveWeaponMin:F2}-{weaponBounds.y:F2}, " +
                      $"Speed: {effectiveSpeedMin:F2}-{speedBounds.y:F2}.");
        }
    }

    private void TestWeaponGeneration()
    {
        Debug.Log("=== Testing Weapon Quality Generation ===");

        for (int i = 0; i < 5; i++)
        {
            float testXP = UnityEngine.Random.Range(0f, 100f);
            float meanQuality = GetMeanQuality(testXP);
            
            Debug.Log($"\n--- Test {i + 1}: XP Level {testXP:F1} ---");
            Debug.Log($"Mean Quality from Curve: {meanQuality:F2}");

            WeaponProperties weapon = GenerateWeapon(testXP);
            float quality = CalculateQuality(weapon);
            
            Debug.Log($"Final Quality: {quality:F2}");
        }
    }
}

public enum PropertyType
{
    Armour,
    Weapon,
    Speed
}