using Xunit;
using BPCalculator;

namespace BPCalculator.Tests
{
    public class BloodPressureTests
    {
        // Existing BP Tests
        [Fact]
        public void Category_Is_High_When_Sys150_Dia95()
        {
            var result = BloodPressure.CalculateCategory(150, 95);
            Assert.Equal("High", result);
        }

        [Fact]
        public void Category_Is_PreHigh_When_Sys130_Dia85()
        {
            var result = BloodPressure.CalculateCategory(130, 85);
            Assert.Equal("PreHigh", result);
        }

        [Fact]
        public void Category_Is_Ideal_When_Sys110_Dia70()
        {
            var result = BloodPressure.CalculateCategory(110, 70);
            Assert.Equal("Ideal", result);
        }

        [Fact]
        public void Category_Is_Low_When_Sys80_Dia50()
        {
            var result = BloodPressure.CalculateCategory(80, 50);
            Assert.Equal("Low", result);
        }

        // NEW: BP Explainer Tests (30-line feature tests)
        [Theory]
        [InlineData("Low", "Low BP (<90/60 mmHg): May cause dizziness. Usually not concerning.")]
        [InlineData("Ideal", "Ideal BP (90/60-120/80 mmHg): Optimal for heart health.")]
        [InlineData("PreHigh", "Pre-High BP (121/81-139/89 mmHg): Lifestyle changes needed.")]
        [InlineData("High", "High BP (≥140/90 mmHg): Consult doctor. Increases heart risks.")]
        [InlineData("invalid", "Invalid category. Use: Low, Ideal, PreHigh, High")]
        [InlineData("", "Invalid category. Use: Low, Ideal, PreHigh, High")]
        [InlineData("LOW", "Low BP (<90/60 mmHg): May cause dizziness. Usually not concerning.")] // Case insensitive
        [InlineData("IDEAL", "Ideal BP (90/60-120/80 mmHg): Optimal for heart health.")]
        [InlineData("prehigh", "Pre-High BP (121/81-139/89 mmHg): Lifestyle changes needed.")]
        [InlineData("HIGH", "High BP (≥140/90 mmHg): Consult doctor. Increases heart risks.")]
        public void BPCategoryExplainer_GetExplanation_ReturnsCorrectText(string category, string expected)
        {
            // Act
            var result = BPCategoryExplainer.GetExplanation(category);
            
            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Low", "Drink more water. Add slight salt. Avoid sudden movements.")]
        [InlineData("Ideal", "Maintain lifestyle. Exercise regularly. Eat balanced diet.")]
        [InlineData("PreHigh", "Reduce salt. Exercise more. Manage stress. Limit alcohol.")]
        [InlineData("High", "See doctor. Monitor weekly. May need medication.")]
        [InlineData("invalid", "No recommendations for invalid category.")]
        public void BPCategoryExplainer_GetRecommendations_ReturnsCorrectAdvice(string category, string expected)
        {
            // Act
            var result = BPCategoryExplainer.GetRecommendations(category);
            
            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Low", "< 90/60 mmHg")]
        [InlineData("Ideal", "90/60 - 120/80 mmHg")]
        [InlineData("PreHigh", "121/81 - 139/89 mmHg")]
        [InlineData("High", "≥ 140/90 mmHg")]
        [InlineData("invalid", "Unknown range")]
        public void BPCategoryExplainer_GetRange_ReturnsCorrectRange(string category, string expected)
        {
            // Act
            var result = BPCategoryExplainer.GetRange(category);
            
            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void BPCategoryExplainer_AllMethodsForValidCategory_ReturnsConsistentData()
        {
            // Arrange
            var category = "Ideal";
            
            // Act
            var explanation = BPCategoryExplainer.GetExplanation(category);
            var recommendations = BPCategoryExplainer.GetRecommendations(category);
            var range = BPCategoryExplainer.GetRange(category);
            
            // Assert
            Assert.Contains("Optimal for heart health", explanation);
            Assert.Contains("Maintain lifestyle", recommendations);
            Assert.Contains("90/60 - 120/80 mmHg", range);
        }

        [Fact]
        public void BPCategoryExplainer_EdgeCase_NullCategory_ReturnsInvalid()
        {
            // Act
            var explanation = BPCategoryExplainer.GetExplanation(null);
            
            // Assert
            Assert.Contains("Invalid category", explanation);
        }

        // Also add test for empty string
        [Fact]
        public void BPCategoryExplainer_EdgeCase_EmptyString_ReturnsInvalid()
        {
            // Act
            var explanation = BPCategoryExplainer.GetExplanation("");
            
            // Assert
            Assert.Contains("Invalid category", explanation);
        }

        [Fact]
        public void BPCategoryExplainer_Integration_WithBloodPressureClass()
        {
            // Arrange
            var bp = new BloodPressure { Systolic = 120, Diastolic = 80 };
            
            // Act
            var category = bp.CalculateCategory(); // Returns "PreHigh"
            var explanation = BPCategoryExplainer.GetExplanation(category);
            var recommendations = BPCategoryExplainer.GetRecommendations(category);
            
            // Assert
            Assert.Equal("PreHigh", category);
            Assert.Contains("Lifestyle changes needed", explanation);
            Assert.Contains("Reduce salt", recommendations);
        }

        // Test for code coverage - all four valid categories
        [Theory]
        [InlineData("Low")]
        [InlineData("Ideal")]
        [InlineData("PreHigh")]
        [InlineData("High")]
        public void BPCategoryExplainer_ValidCategories_DoNotReturnInvalid(string category)
        {
            // Act
            var explanation = BPCategoryExplainer.GetExplanation(category);
            
            // Assert
            Assert.DoesNotContain("Invalid category", explanation);
        }
    }
}