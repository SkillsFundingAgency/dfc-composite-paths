﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using DFC.Composite.Paths.Common;
using DFC.Swagger.Standard.Annotations;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace DFC.Composite.Paths.Models
{
    public class PathModel : IValidatableObject
    {

        [Display(Description = "Unique document identifier. This is auto generated")]
        [Example(Description = "b8592ff8-af97-49ad-9fb2-e5c3c717fd85")]
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [Display(Description = "The path of the application. This should match the url value immediately after the domain. i.e. https://nationalcareeers.service.gov.uk/explore-careers.")]
        [Example(Description = "explore-careers")]
        [Required]
        [MaxLength(100)]
        public string Path { get; set; }

        [Display(Description = "Text value that appears on the Top Navigation section of the National Careers Service website. If this value is NOT present no menu option will be displayed.")]
        [Example(Description = "Explore Careers")]
        public string TopNavigationText { get; set; }

        [Display(Description = "Number indicating the position in the top navigation bar.  This value will have to agreed with Product owners and Service designers before deployment. Its suggested that numbers are allocated in increments of 100 or so.")]
        [Example(Description = "200")]
        public int TopNavigationOrder { get; set; }

        [Display(Description = "Which page layout the application should us.")]
        [Example(Description = "FullWidth")]
        [Required]
        [EnumDataType(typeof(Layout))]
        public Layout Layout { get; set; }

        [Display(Description = "Indicator stating that the application is online and ready to use.")]
        [Example(Description = "true")]
        public bool IsOnline { get; set; }

        [Display(Description = "If the application is marked as offline (IsOnline = false) then this text is displayed on any application path (or child path).")]
        [Example(Description = "<strong>Sorry this application is offline</strong>")]
        public string OfflineHtml { get; set; }

        [Display(Description = "This property is used to store the HTML markup for the 'Phase Banner' if any")]
        [Example(Description = "<strong>Alpha/Beta - This is a new service</strong>")]
        public string PhaseBannerHtml { get; set; }

        [Display(Description = "Optional Url endpoint for the retrieval of an application sitemap.")]
        [Example(Description = "https://nationalcareeers.service.gov.uk/explore-careers/sitemap")]
        public string SitemapURL { get; set; }

        [Display(Description = "External Url endpoint.")]
        [Example(Description = "https://nationalcareeers.service.gov.uk/explore-careers")]
        public string ExternalURL { get; set; }

        [Display(Description = "Optional Url endpoint for the retrieval of an application Robots.txt file.")]
        [Example(Description = "https://nationalcareeers.service.gov.uk/explore-careers/robots.txt")]
        public string RobotsURL { get; set; }

        [Display(Description = "UTC date and time the application was registered. This is auto generated.")]
        public DateTime DateOfRegistration { get; set; }

        [Display(Description = "UTC date and time of when the application was last updated. This is auto generated.")]
        public DateTime LastModifiedDate { get; set; }

        public Dictionary<string, string> JavaScriptNames { get; set; }

        public Dictionary<string, string> CssScriptNames { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            var pathRegex = new Regex(RegularExpressions.Path);

            if (!pathRegex.IsMatch(Path))
            {
                result.Add(new ValidationResult(Message.PathIsInvalid, new string[] { nameof(Path) }));
            }

            if (!string.IsNullOrEmpty(OfflineHtml))
            {
                var htmlDoc = new HtmlDocument();

                htmlDoc.LoadHtml(OfflineHtml);

                if (htmlDoc.ParseErrors.Any())
                {
                    result.Add(new ValidationResult(string.Format(Message.MalformedHtml, nameof(OfflineHtml)), new string[] { nameof(OfflineHtml) }));
                }
            }

            if (!string.IsNullOrEmpty(PhaseBannerHtml))
            {
                var htmlDoc = new HtmlDocument();

                htmlDoc.LoadHtml(PhaseBannerHtml);

                if (htmlDoc.ParseErrors.Any())
                {
                    result.Add(new ValidationResult(string.Format(Message.MalformedHtml, nameof(PhaseBannerHtml))));
                }
            }

            return result;
        }
    }
}
