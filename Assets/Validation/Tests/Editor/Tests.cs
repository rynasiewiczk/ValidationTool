namespace LazySloth.Validation
{
    using NUnit.Framework;

    public class Tests
    {
        [Test] public void TestValidationConfigExists()
        {
            var config = ValidationHelper.Config;
            Assert.NotNull(config);
        }
        
        [Test]
        public void TestProjectPath()
        {
            var config = ValidationHelper.Config;
            var pathString = config.ProjectMainFolderPath;
            Assert.That(pathString.Contains("Assets/"));
        }
    }
}