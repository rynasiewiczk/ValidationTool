namespace LazySloth.Validation
{
    using NUnit.Framework;

    public class Tests
    {
        [Test] public void TestValidationConfigExists()
        {
            var config = ValidationHelper.ValidationConfig;
            Assert.NotNull(config);
        }
        
        [Test]
        public void TestProjectPath()
        {
            var config = ValidationHelper.ValidationConfig;
            var pathString = config.ProjectMainFolderPath;
            Assert.That(pathString.Contains("Assets/"));
        }

        
    }
}