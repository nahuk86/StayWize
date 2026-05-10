using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using StayWize.Services.Localization;

namespace StayWize.UnitTests.Services;

public class LocalizationServiceTests
{
    private ILocalizationService CreateService(string? acceptLanguage = null)
    {
        var httpContextMock = new Mock<HttpContext>();
        var requestMock = new Mock<HttpRequest>();
        var headersMock = new HeaderDictionary();

        if (acceptLanguage is not null)
            headersMock["Accept-Language"] = acceptLanguage;

        requestMock.Setup(r => r.Headers).Returns(headersMock);
        httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);

        return new LocalizationService(httpContextAccessorMock.Object);
    }

    [Fact]
    public void Get_NoAcceptLanguageHeader_ShouldReturnSpanish()
    {
        var service = CreateService();

        var result = service.Get("NotFound", "Propiedad");

        result.Should().Contain("no fue encontrado");
    }

    [Fact]
    public void Get_AcceptLanguageEs_ShouldReturnSpanish()
    {
        var service = CreateService("es");

        var result = service.Get("NotFound", "Propiedad");

        result.Should().Contain("no fue encontrado");
    }

    [Fact]
    public void Get_AcceptLanguageEn_ShouldReturnEnglish()
    {
        var service = CreateService("en");

        var result = service.Get("NotFound", "Property");

        result.Should().Contain("was not found");
    }

    [Fact]
    public void Get_UnknownKey_ShouldReturnKey()
    {
        var service = CreateService();

        var result = service.Get("UnknownKey");

        result.Should().Be("UnknownKey");
    }

    [Fact]
    public void Get_ConcurrencyError_EnglishShouldDifferFromSpanish()
    {
        var serviceEs = CreateService("es");
        var serviceEn = CreateService("en");

        var resultEs = serviceEs.Get("ConcurrencyError");
        var resultEn = serviceEn.Get("ConcurrencyError");

        resultEs.Should().NotBe(resultEn);
    }
}