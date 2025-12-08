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
