using Microsoft.AspNetCore.Authorization;

namespace UnderTheHood.Authorization
{
    public class HRManagerProbationRequirement : IAuthorizationRequirement
    {
        public HRManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }
    }

    // Requirement Handler
    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        // custom policy based authorization
        // HR Manager is granted access after the probation period has passed (3 months)
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "EmploymentDate"))
                return Task.CompletedTask;

            if(DateTime.TryParse(context.User.FindFirst(x => x.Type == "EmploymentDate")?.Value, out DateTime employmentDate))
            {
                var period = DateTime.Now - employmentDate;
                if (period.Days > 30 * requirement.ProbationMonths)
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
