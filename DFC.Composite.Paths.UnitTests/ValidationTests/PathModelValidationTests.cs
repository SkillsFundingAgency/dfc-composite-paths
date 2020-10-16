using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace DFC.Composite.Paths.UnitTests.ValidationTests
{
    [TestFixture]
    public class PathModelValidationTests
    {
        [TestCase("ab")]
        [TestCase("abc")]
        [TestCase("123")]
        [TestCase("a.b-c_d,e_f/g")]
        [TestCase("abc/def")]
        [TestCase("action-plans")]
        [TestCase("your-account")]
        [TestCase("job-profiles")]
        [TestCase("explore-careers")]
        [TestCase("skills-assessment")]
        [TestCase("find-a-course")]
        [TestCase("contact-us")]
        [TestCase("webchat")]
        [TestCase("about-us")]
        [TestCase("get-a-job")]
        [TestCase("help")]
        [TestCase("alerts")]
        [TestCase("discover-your-skills-and-careers")]
        [TestCase("matchskills")]
        [TestCase("pages")]
        [TestCase("explore-careers")]
        [TestCase("skills-assessment/skills-health-check")]
        [TestCase("skills-assessment/match-skills")]
        [TestCase("skills-assessment/discover-your-skills-and-careers")]
        public void PathModelValidationSuccessful(string pathValue)
        {
            // Arrange
            var model = CreateValidModel(pathValue);

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count == 0);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void PathModelValidationMandatoryFailure(string pathValue)
        {
            // Arrange
            var model = CreateValidModel(pathValue);

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count == 1);
            Assert.NotNull(vr.First(f => f.MemberNames.Any(a => a == nameof(model.Path))));
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "The {0} field is required.", nameof(model.Path)), vr.First(f => f.MemberNames.Any(a => a == nameof(model.Path))).ErrorMessage);
        }

        [TestCase("/")]
        [TestCase("/a")]
        [TestCase("a/")]
        [TestCase("a+b")]
        [TestCase("a(b")]
        [TestCase("a)b")]
        [TestCase("_a")]
        [TestCase("a_")]
        public void PathModelValidationFailure(string pathValue)
        {
            // Arrange
            var model = CreateValidModel(pathValue);

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count == 1);
            Assert.NotNull(vr.First(f => f.MemberNames.Any(a => a == nameof(model.Path))));
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "{0} is invalid", nameof(model.Path)), vr.First(f => f.MemberNames.Any(a => a == nameof(model.Path))).ErrorMessage);
        }

        private PathModel CreateValidModel(string pathValue)
        {
            return new PathModel
            {
                Path = pathValue,
                Layout = Layout.FullWidth,
            };
        }

        private List<ValidationResult> Validate(PathModel model)
        {
            var vr = new List<ValidationResult>();
            var vc = new ValidationContext(model);
            Validator.TryValidateObject(model, vc, vr, true);

            return vr;
        }
    }
}
