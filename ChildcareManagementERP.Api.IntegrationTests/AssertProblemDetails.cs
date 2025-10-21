using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ChildcareManagementERP.Api.IntegrationTests
{
    public class AssertProblemDetails
    {
        public static async Task<ProblemDetails> AssertProblem(HttpResponseMessage response, HttpStatusCode expectedCode)
        {
            response.StatusCode.ShouldBe(expectedCode);
            response.Content.Headers.ContentType!.MediaType.ShouldBe("application/problem+json");
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.ShouldNotBeNull();
            problem.Status.ShouldBe((int)expectedCode);

            return problem!;
        }
    }
}