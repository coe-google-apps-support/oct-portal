using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using EnsureThat;
using System;
using System.Threading.Tasks;
using CoE.Ideas.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace CoE.Ideas.Server.Models
{
    public class InitiativeStepDetail
    {
        public int StepId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? CompletionDate { get; set; }

        internal static async Task<IEnumerable<InitiativeStepDetail>> FromInitiativeStatusHistoriesAsync(IEnumerable<InitiativeStatusHistory> steps,
            IPersonRepository personRepository,
            IStringTemplateService stringTemplateService)
        {
            if (steps == null || !steps.Any())
                return null;

            EnsureArg.IsNotNull(stringTemplateService);
            var lastStep = steps.OrderByDescending(x => x.StatusEntryDateUtc).First();

            var stack = new Stack<InitiativeStepDetail>();
            foreach (var step in steps)
            {
                var stepDto = new InitiativeStepDetail()
                {
                    StepId = (int)step.Status,
                    Title = GetTitle(step.Status),
                    Description = step.StatusDescriptionOverride,
                    StartDate = step.StatusEntryDateUtc.ToLocalTime()
                };
                if (stack.Count > 0)
                    stack.Peek().CompletionDate = stepDto.StartDate;

                // now fill in the text here
                if (string.IsNullOrWhiteSpace(stepDto.Description))
                {
                    var textTemplate = await stringTemplateService.GetStatusChangeTextAsync(step.Status, step != lastStep);
                    if (!string.IsNullOrWhiteSpace(textTemplate))
                    {
                        string expectedCompletionDateString = null;
						if (step.ExpectedExitDateUtc.HasValue)
						{
							TimeZoneInfo albertaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Edmonton");
							var expectedExitDateAlberta = TimeZoneInfo.ConvertTimeFromUtc(step.ExpectedExitDateUtc.Value, albertaTimeZone);
							var nowDateAlberta = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, albertaTimeZone);
							expectedCompletionDateString = expectedExitDateAlberta.ToStringRelativeToNow(nowDateAlberta);
							//expectedCompletionDateString = expectedExitDateAlberta.ToString();
						}
						Person assignee = null;
                        if (step.PersonId.HasValue)
                            assignee = await personRepository.GetPersonAsync(step.PersonId.Value);
                        stepDto.Description = string.Format(textTemplate, assignee?.Name, expectedCompletionDateString);
                    }
                }

                stack.Push(stepDto);
            }

            // now add missing statuses
            var allStatuses = new InitiativeStatus[]
            {
                InitiativeStatus.Submit,
                InitiativeStatus.Review,
                InitiativeStatus.Collaborate,
                InitiativeStatus.Deliver
            };

            var remaining = allStatuses.Where(x => !stack.Any(y => y.StepId == (int)x)).OrderBy(x => (int)x)
                .Select(x => new InitiativeStepDetail()
                {
                    StepId = (int)x,
                    Title = GetTitle(x)
                });
            return stack
                .Concat(remaining)
                .OrderBy(x => x.StepId);
        }

        private static string GetTitle(InitiativeStatus status)
        {
            switch (status)
            {
                case InitiativeStatus.Initiate:
                    return "Initiated";
                case InitiativeStatus.Submit:
                    return "Submitted";
                case InitiativeStatus.Review:
                    return "In Review";
                case InitiativeStatus.Collaborate:
                    return "In Collaboration";
                case InitiativeStatus.Deliver:
                    return "In Delivery";
                case InitiativeStatus.Cancelled:
                    return "Cancelled";
                default:
                    return status.ToString();
            }
        }
    }
}