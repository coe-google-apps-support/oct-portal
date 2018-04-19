using CoE.Ideas.Core.Data;
using CoE.Ideas.Core.Services;
using EnsureThat;
using System;
using System.Threading.Tasks;
using CoE.Ideas.Shared.Extensions;

namespace CoE.Ideas.Server.Models
{
    public class InitiativeStepDetail
    {
        public int StepId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? CompletionDate { get; set; }

        internal static async Task<InitiativeStepDetail> FromInitiativeStepAsync(InitiativeStep step,
            Person assignee,
            IStringTemplateService stringTemplateService,
            bool isCurrentStep)
        {
            if (step == null)
                return null;

            EnsureArg.IsNotNull(stringTemplateService);

            var returnValue = new InitiativeStepDetail()
            {
                StepId = (int)step.Status,
                Title = GetTitle(step.Status),
                StartDate = step.StartDate,
                CompletionDate = step.CompletionDate
            };

            // now fill in the text here
            var textTemplate = await stringTemplateService.GetStatusChangeTextAsync(step.Status, !isCurrentStep);
            if (!string.IsNullOrWhiteSpace(textTemplate))
            {
                string expectedCompletionDateString = null;
                if (step.ExpectedCompletionDate.HasValue)
                    expectedCompletionDateString = step.ExpectedCompletionDate.Value.LocalDateTime.ToStringRelativeToNow();

                returnValue.Description = string.Format(textTemplate, assignee?.Name, expectedCompletionDateString);
            }


            return returnValue;
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