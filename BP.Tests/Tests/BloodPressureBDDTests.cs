using Xunit;
using BPCalculator;

namespace BPCalculator.Tests.BDD
{
    public class BloodPressureBDDTests
    {
        // Only the 4 scenarios from your .md file
        [Theory]
        [InlineData(150, 95, "High", "Increases heart risks", "See doctor")]
        [InlineData(110, 70, "Ideal", "Optimal for heart health", "Maintain lifestyle")]
        [InlineData(130, 85, "PreHigh", "Lifestyle changes needed", "Reduce salt")]
        [InlineData(80, 50, "Low", "May cause dizziness", "Drink more water")]
        public void Blood_Pressure_Scenarios(
            int systolic, 
            int diastolic, 
            string expectedCategory,
            string expectedExplanationContains,
            string expectedRecommendationsContains)
        {
            var category = BloodPressure.CalculateCategory(systolic, diastolic);
            var explanation = BPCategoryExplainer.GetExplanation(category);
            var recommendations = BPCategoryExplainer.GetRecommendations(category);
            
            Assert.Equal(expectedCategory, category);
            Assert.Contains(expectedExplanationContains, explanation);
            Assert.Contains(expectedRecommendationsContains, recommendations);
        }
    }
}