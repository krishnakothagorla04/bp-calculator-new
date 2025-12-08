using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace BPCalculator
{
    // BP categories
    public enum BPCategory
{
    [Display(Name = "Low Blood Pressure")] Low,
    [Display(Name = "Ideal Blood Pressure")] Ideal,
    [Display(Name = "Pre-High Blood Pressure")] PreHigh,
    [Display(Name = "High Blood Pressure")] High
}

public class BloodPressure
{
    public const int SystolicMin = 70;
    public const int SystolicMax = 190;
    public const int DiastolicMin = 40;
    public const int DiastolicMax = 100;

    [Range(SystolicMin, SystolicMax, ErrorMessage = "Invalid Systolic Value")]
    public int Systolic { get; set; }

    [Range(DiastolicMin, DiastolicMax, ErrorMessage = "Invalid Diastolic Value")]
    public int Diastolic { get; set; }

    public BPCategory Category =>
        (Systolic, Diastolic) switch
        {
            var (s, d) when s >= 140 || d >= 90 => BPCategory.High,
            var (s, d) when s >= 120 || d >= 80 => BPCategory.PreHigh,
            var (s, d) when s >= 90 && d >= 60 => BPCategory.Ideal,
            _ => BPCategory.Low
        };

    public string CalculateCategory() => Category.ToString();

    public static string CalculateCategory(int systolic, int diastolic) =>
        (systolic, diastolic) switch
        {
            var (s, d) when s >= 140 || d >= 90 => "High",
            var (s, d) when s >= 120 || d >= 80 => "PreHigh",
            var (s, d) when s >= 90 && d >= 60 => "Ideal",
            _ => "Low"
        };
}


}








// NEW FEATURE: BP Classification Explanation (Exactly 30 lines)
public class BPCategoryExplainer
{
    public static string GetExplanation(string category)
    {
        // Handle null or empty input
        if (string.IsNullOrWhiteSpace(category))
            return "Invalid category. Use: Low, Ideal, PreHigh, High";
        
        return category.ToLower() switch
        {
            "low" => "Low BP (<90/60 mmHg): May cause dizziness. Usually not concerning.",
            "ideal" => "Ideal BP (90/60-120/80 mmHg): Optimal for heart health.",
            "prehigh" => "Pre-High BP (121/81-139/89 mmHg): Lifestyle changes needed.",
            "high" => "High BP (≥140/90 mmHg): Consult doctor. Increases heart risks.",
            _ => "Invalid category. Use: Low, Ideal, PreHigh, High"
        };
    }
    
    public static string GetRecommendations(string category)
    {
        // Handle null or empty input
        if (string.IsNullOrWhiteSpace(category))
            return "No recommendations for invalid category.";
        
        return category.ToLower() switch
        {
            "low" => "Drink more water. Add slight salt. Avoid sudden movements.",
            "ideal" => "Maintain lifestyle. Exercise regularly. Eat balanced diet.",
            "prehigh" => "Reduce salt. Exercise more. Manage stress. Limit alcohol.",
            "high" => "See doctor. Monitor weekly. May need medication.",
            _ => "No recommendations for invalid category."
        };
    }
    
    public static string GetRange(string category)
    {
        // Handle null or empty input
        if (string.IsNullOrWhiteSpace(category))
            return "Unknown range";
        
        return category.ToLower() switch
        {
            "low" => "< 90/60 mmHg",
            "ideal" => "90/60 - 120/80 mmHg", 
            "prehigh" => "121/81 - 139/89 mmHg",
            "high" => "≥ 140/90 mmHg",
            _ => "Unknown range"
        };
    }
}
