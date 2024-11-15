using System.Collections.Generic;
using HastaneTakipsistemi.Models;

public static class EnumHelper
{
    private static readonly Dictionary<TestType, string> TestTypeNames = new Dictionary<TestType, string>
    {
        { TestType.BloodTest, "Kan Tahlili" },
        { TestType.UrineTest, "İdrar Tahlili" },
        { TestType.XRay, "Röntgen" },
        { TestType.MRI, "MR" },
        { TestType.Ultrasound, "Ultrason" },
        { TestType.Other, "Diğer" }
    };

    private static readonly Dictionary<TestStatus, string> TestStatusNames = new Dictionary<TestStatus, string>
    {
        { TestStatus.Requested, "Beklemede" },
        { TestStatus.Completed, "Tamamlandı" },
        { TestStatus.Cancelled, "İptal Edildi" }
    };

    public static string GetTestTypeName(TestType testType)
    {
        return TestTypeNames[testType];
    }

    public static string GetTestStatusName(TestStatus testStatus)
    {
        return TestStatusNames[testStatus];
    }
}