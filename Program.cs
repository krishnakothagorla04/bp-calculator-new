using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using BPCalculator;

// Configure Serilog first
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddControllers();

    var app = builder.Build();

    // TELEMETRY MIDDLEWARE - Logs every request
    app.Use(async (context, next) =>
    {
        var startTime = DateTime.UtcNow;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        await next();
        
        stopwatch.Stop();
        
        Log.Information("API {Method} {Path} -> {StatusCode} in {ResponseTime}ms", 
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    });

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    //  HEALTH ENDPOINT
    app.MapGet("/health", () => 
    {
        Log.Information("Health check called - Status: Healthy");
        return new 
        { 
            status = "Healthy", 
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            service = "BP Calculator + Category Explainer", // Updated service name
            version = "1.0.0",
            environment = app.Environment.EnvironmentName
        };
    });

    //  METRICS ENDPOINT
    app.MapGet("/metrics", () => 
    {
        Log.Information("Metrics checked - Memory: {MemoryMB}MB", 
            GC.GetTotalMemory(false) / 1024 / 1024);
            
        return new 
        {
            timestamp = DateTime.UtcNow,
            memory_used = GC.GetTotalMemory(false),
            uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime()
        };
    });

    //  BP CALCULATOR API ENDPOINT
    app.MapPost("/api/bp/calculate", (BPRequest request) =>
    {
        Log.Information("BP Calculation - Systolic: {Systolic}, Diastolic: {Diastolic}", 
            request.Systolic, request.Diastolic);

        // Manual validation
        if (request.Systolic < 70 || request.Systolic > 190)
        {
            Log.Warning("BP Validation failed - Systolic out of range: {Systolic}", request.Systolic);
            return Results.BadRequest("Systolic must be between 70 and 190");
        }
            
        if (request.Diastolic < 40 || request.Diastolic > 100)
        {
            Log.Warning("BP Validation failed - Diastolic out of range: {Diastolic}", request.Diastolic);
            return Results.BadRequest("Diastolic must be between 40 and 100");
        }
            
        if (request.Systolic <= request.Diastolic)
        {
            Log.Warning("BP Validation failed - Systolic <= Diastolic: {Systolic}/{Diastolic}", 
                request.Systolic, request.Diastolic);
            return Results.BadRequest("Systolic must be greater than Diastolic");
        }

        var bp = new BloodPressure 
        { 
            Systolic = request.Systolic, 
            Diastolic = request.Diastolic 
        };
        
        Log.Information("BP Result - Category: {Category}", bp.Category);
        
        return Results.Ok(new 
        {
            Category = bp.Category.ToString(),
            CategoryDisplayName = bp.Category.GetDisplayName(),
            Systolic = bp.Systolic,
            Diastolic = bp.Diastolic,
            Message = GetBPCategoryMessage(bp.Category),
            // NEW: Include explanation from our new feature
            Explanation = BPCategoryExplainer.GetExplanation(bp.Category.ToString()),
            Recommendations = BPCategoryExplainer.GetRecommendations(bp.Category.ToString())
        });
    });

    //  NEW FEATURE: BP CATEGORY EXPLANATION ENDPOINT (30-line feature)
    app.MapGet("/api/bp/explain/{category}", (string category) =>
    {
        Log.Information("BP Explanation requested for category: {Category}", category);
        
        var explanation = BPCategoryExplainer.GetExplanation(category);
        var recommendations = BPCategoryExplainer.GetRecommendations(category);
        var range = BPCategoryExplainer.GetRange(category);
        
        bool isValid = !explanation.Contains("Invalid category");
        
        return Results.Ok(new 
        {
            Category = category,
            IsValid = isValid,
            BloodPressureRange = range,
            Explanation = explanation,
            Recommendations = recommendations,
            WhenToSeeDoctor = category.ToLower() == "high" ? 
                "Consult doctor as soon as possible" :
                category.ToLower() == "prehigh" ? 
                "Schedule checkup within 3 months" :
                "Regular annual checkups are sufficient",
            MonitoringFrequency = category.ToLower() switch
            {
                "high" => "Weekly or as directed by doctor",
                "prehigh" => "Monthly",
                _ => "Every 6-12 months"
            }
        });
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    app.MapRazorPages();

    Log.Information("Starting web host with BP Calculator and Category Explainer. Environment: {Env}", 
        app.Environment.EnvironmentName);
        
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Helper methods
static string GetBPCategoryMessage(BPCategory category)
{
    return category switch
    {
        BPCategory.Low => "Your blood pressure is lower than normal",
        BPCategory.Ideal => "Your blood pressure is ideal", 
        BPCategory.PreHigh => "Your blood pressure is pre-high",
        BPCategory.High => "Your blood pressure is high",
        _ => "Unable to determine blood pressure category"
    };
}

// Add this extension method for enum display names (if not already present)
public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttributes(
            typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
            .FirstOrDefault() as System.ComponentModel.DataAnnotations.DisplayAttribute;
        
        return attribute?.Name ?? value.ToString();
    }
}

// DTOs
public record BPRequest(int Systolic, int Diastolic);